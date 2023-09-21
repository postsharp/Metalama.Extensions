// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Extensions.DependencyInjection.Implementation;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Options;
using Metalama.Framework.Project;
using Metalama.Framework.Serialization;
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Metalama.Extensions.DependencyInjection;

#pragma warning disable SA1623

/// <summary>
/// Options that influence the processing of <see cref="IntroduceDependencyAttribute"/>.
/// </summary>
[PublicAPI]
public sealed record DependencyInjectionOptions : IHierarchicalOptions<ICompilation>, IHierarchicalOptions<INamespace>, IHierarchicalOptions<INamedType>
{
#pragma warning disable CS0169 // False positive.
    [NonCompileTimeSerialized]
    private ImmutableArray<IDependencyInjectionFramework> _enabledFrameworks;
#pragma warning restore CS0169

    /// <summary>
    /// Gets or sets the list of frameworks that can be used to implement the <see cref="IntroduceDependencyAttribute"/> advice and <see cref="DependencyAttribute"/>
    /// aspect.
    /// </summary>
    public HierarchicalOptionItemCollection<DependencyInjectionFrameworkRegistration>? FrameworkRegistrations { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the default value for the <see cref="DependencyAttribute.IsRequired"/> property of <see cref="DependencyAttribute"/> and <see cref="IntroduceDependencyAttribute"/>.
    /// </summary>
    public bool? IsRequired { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the default value for the <see cref="DependencyAttribute.IsLazy"/> property of <see cref="DependencyAttribute"/> and <see cref="IntroduceDependencyAttribute"/>.
    /// </summary>
    public bool? IsLazy { get; init; }

    /// <summary>
    /// Gets or sets a delegate that is called when several dependency injection frameworks have been registered
    /// for the current project and many vote to handle a given dependency. The default implementation is to return
    /// the first framework in the array.
    /// </summary>
    public IDependencyInjectionFrameworkSelector? Selector { get; init; }

    internal bool TryGetFramework(
        DependencyProperties properties,
        in ScopedDiagnosticSink diagnostics,
        [NotNullWhen( true )] out IDependencyInjectionFramework? framework )
    {
        // Lazily instantiates the frameworks.
        if ( this._enabledFrameworks.IsDefault )
        {
            if ( this.FrameworkRegistrations == null )
            {
                this._enabledFrameworks = ImmutableArray<IDependencyInjectionFramework>.Empty;
            }
            else
            {
                this._enabledFrameworks = this.FrameworkRegistrations
                    .OrderBy( x => x.Priority ?? 0 )
                    .ThenBy( x => x.Type.FullName )
                    .Select( x => DependencyInjectionFrameworkFactory.GetInstance( x.Type ) )
                    .ToImmutableArray();
            }
        }

        // Get eligible frameworks.
        var d = diagnostics;
        var eligibleFrameworks = this._enabledFrameworks.Where( f => f.CanHandleDependency( properties, d ) ).ToImmutableArray();

        if ( eligibleFrameworks.IsEmpty )
        {
            var diagnostic = this._enabledFrameworks.IsEmpty
                ? DiagnosticDescriptors.NoDependencyInjectionFrameworkRegistered
                : DiagnosticDescriptors.NoSuitableDependencyInjectionFramework;

            diagnostics.Report( diagnostic.WithArguments( (properties.DependencyType, properties.TargetType) ) );

            framework = null;

            return false;
        }

        // Select the preferred framework.
        if ( eligibleFrameworks.Length == 1 || this.Selector == null )
        {
            framework = eligibleFrameworks[0];

            return true;
        }
        else
        {
            framework = this.Selector.SelectFramework( properties, eligibleFrameworks );

            if ( framework == null! )
            {
                throw new ArgumentNullException( $"{this.Selector.GetType().Name}.{nameof(this.Selector.SelectFramework)} returned null." );
            }

            return true;
        }
    }

    IHierarchicalOptions IHierarchicalOptions.GetDefaultOptions( IProject project )
        => new DependencyInjectionOptions
        {
            IsRequired = true,
            IsLazy = false,
            FrameworkRegistrations = new HierarchicalOptionItemCollection<DependencyInjectionFrameworkRegistration>(
                new DependencyInjectionFrameworkRegistration( typeof(LoggerDependencyInjectionFramework), 100 ),
                new DependencyInjectionFrameworkRegistration( typeof(DefaultDependencyInjectionFramework), 101 ) )
        };

    object IOverridable.OverrideWith( object options, in OverrideContext context )
    {
        var other = (DependencyInjectionOptions) options;

        return new DependencyInjectionOptions
        {
            IsRequired = other.IsRequired ?? this.IsRequired,
            IsLazy = other.IsLazy ?? this.IsLazy,
            Selector = other.Selector ?? this.Selector,
            FrameworkRegistrations = this.FrameworkRegistrations.OverrideWithSafe( other.FrameworkRegistrations, context )
        };
    }
}