// Copyright (c) SharpCrafters s.r.o. All rights reserved. See LICENSE.md in the repository root for details.

using Metalama.Framework.DependencyInjection.Implementation;
using Metalama.Framework.Project;
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Metalama.Framework.DependencyInjection;

/// <summary>
/// Options that influence the processing of <see cref="IntroduceDependencyAttribute"/>.
/// </summary>
public sealed class DependencyInjectionOptions : ProjectExtension
{
    private Func<DependencyContext, ImmutableArray<IDependencyInjectionFramework>, IDependencyInjectionFramework?> _selector = ( context, frameworks )
        => frameworks[0];

    private ImmutableArray<IDependencyInjectionFramework> _registeredFrameworks =
        ImmutableArray.Create<IDependencyInjectionFramework>( new DefaultDependencyInjectionFramework() );

    /// <summary>
    /// Gets or sets the list of frameworks that can be used to implement the <see cref="IntroduceDependencyAttribute"/> advice and <see cref="DependencyAttribute"/>
    /// aspect.
    /// </summary>
    public ImmutableArray<IDependencyInjectionFramework> RegisteredFrameworks
    {
        get => this._registeredFrameworks;
        set
        {
            if ( this.IsReadOnly )
            {
                throw new InvalidOperationException();
            }

            this._registeredFrameworks = value.IsDefault ? ImmutableArray<IDependencyInjectionFramework>.Empty : value;
        }
    }

#pragma warning disable SA1623
    /// <summary>
    /// Gets or sets the default value for the <see cref="DependencyAttribute.IsRequired"/> property of <see cref="DependencyAttribute"/> and <see cref="IntroduceDependencyAttribute"/>.
    /// </summary>
    public bool IsRequiredByDefault { get; set; } = true;

    /// <summary>
    /// Gets or sets the default value for the <see cref="DependencyAttribute.IsLazy"/> property of <see cref="DependencyAttribute"/> and <see cref="IntroduceDependencyAttribute"/>.
    /// </summary>
    public bool IsLazyByDefault { get; set; }
#pragma warning restore SA1623

    /// <summary>
    /// Registers an implementation of the <see cref="IDependencyInjectionFramework"/> interface with highest priority.
    /// The registration is ignored if another framework of the same was already registered. This method is typically called from the transitive project fabric
    /// of libraries that implement a specific dependency injection framework so that they are registered first by default. In a user project, you should
    /// rather set the <see cref="RegisteredFrameworks"/> property.
    /// </summary>
    /// <returns><c>true</c> if <paramref name="framework"/> was registered, or <c>false</c> if a framework of the same type was already registered.</returns>
    public bool RegisterFramework( IDependencyInjectionFramework framework )
    {
        if ( this.IsReadOnly )
        {
            throw new InvalidOperationException();
        }

        if ( this.RegisteredFrameworks.Any( f => f.GetType() == framework.GetType() ) )
        {
            return false;
        }

        this.RegisteredFrameworks = this.RegisteredFrameworks.Insert( 0, framework );

        return true;
    }

    /// <summary>
    /// Gets or sets a delegate that is called when several dependency injection frameworks have been registered
    /// for the current project and many vote to handle a given dependency. The default implementation is to return
    /// the first framework in the array.
    /// </summary>
    public Func<DependencyContext, ImmutableArray<IDependencyInjectionFramework>, IDependencyInjectionFramework?> Selector
    {
        get => this._selector;
        set
        {
            if ( this.IsReadOnly )
            {
                throw new InvalidOperationException();
            }

            this._selector = value;
        }
    }

    internal bool TryGetFramework( DependencyContext context, [NotNullWhen( true )] out IDependencyInjectionFramework? framework )
    {
        var eligibleFrameworks = this.RegisteredFrameworks.Where( f => f.CanHandleDependency( context ) ).ToImmutableArray();

        if ( eligibleFrameworks.IsEmpty )
        {
            if ( this.RegisteredFrameworks.IsDefaultOrEmpty )
            {
                context.Diagnostics.Report(
                    DiagnosticDescriptors.NoDependencyInjectionFrameworkRegistered.WithArguments( (context.FieldOrProperty, context.TargetType) ) );
            }
            else
            {
                context.Diagnostics.Report(
                    DiagnosticDescriptors.NoSuitableDependencyInjectionFramework.WithArguments( (context.FieldOrProperty, context.TargetType) ) );
            }

            framework = null;

            return false;
        }

        if ( eligibleFrameworks.Length == 1 )
        {
            framework = eligibleFrameworks[0];
        }
        else
        {
            framework = this.Selector.Invoke( context, eligibleFrameworks );

            if ( framework == null )
            {
                context.Diagnostics.Report(
                    DiagnosticDescriptors.MoreThanOneSuitableDependencyInjectionFramework.WithArguments( (context.FieldOrProperty, context.TargetType) ) );

                return false;
            }
        }

        return true;
    }
}