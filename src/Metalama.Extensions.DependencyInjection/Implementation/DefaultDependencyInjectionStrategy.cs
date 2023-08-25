// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Metalama.Extensions.DependencyInjection.Implementation;

/// <summary>
/// The default implementation of the <see cref="IDependencyInjectionFramework.TryIntroduceDependency"/> interface method. It is designed
/// to be easily extended and overwritten.
/// </summary>
[CompileTime]
public class DefaultDependencyInjectionStrategy
{
    /// <summary>
    /// Gets the <see cref="DependencyProperties"/> for which the current object was created.
    /// </summary>
    protected DependencyProperties Properties { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDependencyInjectionStrategy"/> class.
    /// </summary>
    public DefaultDependencyInjectionStrategy( DependencyProperties properties )
    {
        this.Properties = properties;
    }

    /// <summary>
    /// Introduces the field or property into the target class.
    /// </summary>
    /// <param name="builder">An <see cref="IAspectBuilder{TAspectTarget}"/> for the target class.</param>
    /// <param name="introducedFieldOrProperty">At output, the created field or property.</param>
    /// <returns><c>true</c> if the dependency was introduced, <c>false</c> if the dependency was ignored (for instance because it already existed) or in case or error.</returns>
    private bool TryIntroduceFieldOrProperty(
        IAspectBuilder<INamedType> builder,
        [NotNullWhen( true )] out IFieldOrProperty? introducedFieldOrProperty,
        out bool didExist )
    {
        var adviceResult =
            this.Properties.Kind switch
            {
                DeclarationKind.Field =>
                    (IIntroductionAdviceResult<IFieldOrProperty>) builder.Advice.IntroduceField(
                        builder.Target,
                        this.Properties.Name,
                        this.Properties.DependencyType,
                        IntroductionScope.Instance,
                        OverrideStrategy.Ignore ),

                DeclarationKind.Property =>
                    builder.Advice.IntroduceAutomaticProperty(
                        builder.Target,
                        this.Properties.Name,
                        this.Properties.DependencyType,
                        IntroductionScope.Instance,
                        OverrideStrategy.Ignore ),
                _ => throw new InvalidOperationException()
            };

        introducedFieldOrProperty = adviceResult.Declaration;
        didExist = adviceResult.Outcome == AdviceOutcome.Ignore;

        return true;
    }

    /// <summary>
    /// The entry point of the <see cref="DefaultDependencyInjectionStrategy"/>. Orchestrates all steps: first calls <see cref="TryIntroduceFieldOrProperty"/>,
    /// then <see cref="GetPullStrategy"/>, then <see cref="TryPullDependency(Metalama.Framework.Aspects.IAspectBuilder{Metalama.Framework.Code.INamedType},Metalama.Framework.Code.IFieldOrProperty,Metalama.Extensions.DependencyInjection.Implementation.IPullStrategy)"/>.
    /// </summary>
    /// <param name="builder"></param>
    public virtual bool TryIntroduceDependency( IAspectBuilder<INamedType> builder, [NotNullWhen( true )] out IFieldOrProperty? fieldOrProperty )
    {
        if ( !this.TryIntroduceFieldOrProperty( builder, out fieldOrProperty, out var didExist ) )
        {
            return false;
        }

        if ( didExist )
        {
            return true;
        }

        var pullStrategy = this.GetPullStrategy( fieldOrProperty );

        return this.TryPullDependency( builder, fieldOrProperty, pullStrategy );
    }

    public virtual bool TryImplementDependency( IAspectBuilder<IFieldOrProperty> builder )
    {
        var pullStrategy = this.GetPullStrategy( builder.Target );

        this.TryPullDependency( builder.WithTarget( builder.Target.DeclaringType ), builder.Target, pullStrategy );

        return true;
    }

    /// <summary>
    /// Gets the constructors that are modified by <see cref="TryPullDependency(Metalama.Framework.Aspects.IAspectBuilder{Metalama.Framework.Code.INamedType},Metalama.Framework.Code.IFieldOrProperty,Metalama.Extensions.DependencyInjection.Implementation.IPullStrategy)"/>.
    /// </summary>
    /// <param name="type">The type in which the dependency is being injected.</param>
    /// <returns></returns>
    private static IEnumerable<IConstructor> GetConstructors( INamedType type )
        => type.Constructors.Where( c => c.InitializerKind != ConstructorInitializerKind.This );

    /// <summary>
    /// Pulls the dependency from all constructors, i.e. introduce a parameter to these constructors (according to an <see cref="IPullStrategy"/>), and
    /// assigns its value to the dependency property.
    /// </summary>
    /// <param name="aspectBuilder">An <see cref="IAspectBuilder{TAspectTarget}"/> for the target type.</param>
    /// <param name="dependencyFieldOrProperty">The field or property that exposed the dependency.</param>
    /// <param name="pullStrategy">A pull strategy (typically the one returned by <see cref="GetPullStrategy"/>).</param>
    protected bool TryPullDependency( IAspectBuilder<INamedType> aspectBuilder, IFieldOrProperty dependencyFieldOrProperty, IPullStrategy pullStrategy )
    {
        var success = true;

        foreach ( var constructor in GetConstructors( aspectBuilder.Target ) )
        {
            if ( constructor.InitializerKind != ConstructorInitializerKind.This )
            {
                if ( !this.TryPullDependency( aspectBuilder, dependencyFieldOrProperty, pullStrategy, constructor ) )
                {
                    success = false;
                }
            }
        }

        return success;
    }

    /// <summary>
    /// Pulls the dependency from a given constructor.
    /// </summary>
    protected virtual bool TryPullDependency(
        IAspectBuilder<INamedType> aspectBuilder,
        IFieldOrProperty dependencyFieldOrProperty,
        IPullStrategy pullStrategy,
        IConstructor constructor )
    {
        // Find a compatible type in the constructor.
        var existingParameter = pullStrategy.GetExistingParameter( constructor );

        // If there is no compatible parameter, create one.
        if ( existingParameter == null )
        {
            var newParameter = pullStrategy.GetNewParameter( constructor );

            existingParameter = aspectBuilder.Advice.IntroduceParameter(
                    constructor,
                    newParameter.Name,
                    newParameter.Type,
                    TypedConstant.Default( newParameter.Type ),
                    pullStrategy.PullParameter,
                    newParameter.Attributes )
                .Declaration;
        }

        var assignment = pullStrategy.GetAssignmentStatement( existingParameter );
        aspectBuilder.Advice.AddInitializer( constructor, assignment );

        return true;
    }

    /// <summary>
    /// Gets an <see cref="IPullStrategy"/>, i.e. a strategy to pull a dependency field or property from constructors.
    /// </summary>
    /// <param name="introducedFieldOrProperty">The value returned by <see cref="TryIntroduceFieldOrProperty"/>.</param>
    /// <returns>The <see cref="IPullStrategy"/>.</returns>
    protected virtual IPullStrategy GetPullStrategy( IFieldOrProperty introducedFieldOrProperty )
        => new DefaultPullStrategy( this.Properties, introducedFieldOrProperty );
}