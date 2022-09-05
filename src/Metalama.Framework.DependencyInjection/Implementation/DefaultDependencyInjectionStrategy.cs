// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Metalama.Framework.DependencyInjection.Implementation;

/// <summary>
/// The default implementation of the <see cref="IDependencyInjectionFramework.IntroduceDependency"/> interface method. It is designed
/// to be easily extended and overwritten.
/// </summary>
[CompileTime]
public class DefaultDependencyInjectionStrategy
{
    /// <summary>
    /// Gets the <see cref="IntroduceDependencyContext"/> for which the current object was created.
    /// </summary>
    public DependencyContext Context { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDependencyInjectionStrategy"/> class.
    /// </summary>
    public DefaultDependencyInjectionStrategy( DependencyContext context )
    {
        this.Context = context;
    }

    /// <summary>
    /// Introduces the field or property into the target class.
    /// </summary>
    /// <param name="builder">An <see cref="IAspectBuilder{TAspectTarget}"/> for the target class.</param>
    /// <param name="introducedFieldOrProperty">At output, the created field or property.</param>
    /// <returns><c>true</c> if the dependency was introduced, <c>false</c> if the dependency was ignored (for instance because it already existed) or in case or error.</returns>
    protected virtual bool TryIntroduceFieldOrProperty(
        IAspectBuilder<INamedType> builder,
        [NotNullWhen( true )] out IFieldOrProperty? introducedFieldOrProperty )
    {
        var adviceResult =
            this.Context.FieldOrProperty switch
            {
                IField field =>
                    (IIntroductionAdviceResult<IFieldOrProperty>) builder.Advice.IntroduceField(
                        builder.Target,
                        field.Name,
                        field.Type,
                        IntroductionScope.Instance,
                        OverrideStrategy.Ignore ),

                IProperty property =>
                    builder.Advice.IntroduceAutomaticProperty(
                        builder.Target,
                        property.Name,
                        property.Type,
                        IntroductionScope.Instance,
                        OverrideStrategy.Ignore ),
                _ => throw new InvalidOperationException()
            };

        if ( adviceResult.Outcome is AdviceOutcome.Default )
        {
            introducedFieldOrProperty = adviceResult.Declaration;

            return true;
        }
        else
        {
            introducedFieldOrProperty = null;

            return false;
        }
    }

    /// <summary>
    /// The entry point of the <see cref="DefaultDependencyInjectionStrategy"/>. Orchestrates all steps: first calls <see cref="TryIntroduceFieldOrProperty"/>,
    /// then <see cref="GetPullStrategy"/>, then <see cref="PullDependency(Metalama.Framework.Aspects.IAspectBuilder{Metalama.Framework.Code.INamedType},Metalama.Framework.DependencyInjection.Implementation.IPullStrategy)"/>.
    /// </summary>
    /// <param name="builder"></param>
    public virtual void IntroduceDependency( IAspectBuilder<INamedType> builder )
    {
        if ( !this.TryIntroduceFieldOrProperty( builder, out var fieldOrProperty ) )
        {
            return;
        }

        var pullStrategy = this.GetPullStrategy( fieldOrProperty );

        this.PullDependency( builder, pullStrategy );
    }

    public virtual void ImplementDependency( IAspectBuilder<IFieldOrProperty> builder )
    {
        var pullStrategy = this.GetPullStrategy( builder.Target );

        this.PullDependency( builder.WithTarget( builder.Target.DeclaringType ), pullStrategy );
    }

    /// <summary>
    /// Gets the constructors that are modified by <see cref="PullDependency(Metalama.Framework.Aspects.IAspectBuilder{Metalama.Framework.Code.INamedType},Metalama.Framework.DependencyInjection.Implementation.IPullStrategy)"/>.
    /// </summary>
    /// <param name="type">The type in which the dependency is being injected.</param>
    /// <returns></returns>
    protected virtual IEnumerable<IConstructor> GetConstructors( INamedType type )
        => type.Constructors.Where( c => c.InitializerKind != ConstructorInitializerKind.This );

    /// <summary>
    /// Pulls the dependency from all constructors, i.e. introduce a parameter to these constructors (according to an <see cref="IPullStrategy"/>), and
    /// assigns its value to the dependency property.
    /// </summary>
    /// <param name="aspectBuilder">An <see cref="IAspectBuilder{TAspectTarget}"/> for the target type.</param>
    /// <param name="pullStrategy">A pull strategy (typically the one returned by <see cref="GetPullStrategy"/>).</param>
    protected virtual void PullDependency( IAspectBuilder<INamedType> aspectBuilder, IPullStrategy pullStrategy )
    {
        foreach ( var constructor in this.GetConstructors( aspectBuilder.Target ) )
        {
            if ( constructor.InitializerKind != ConstructorInitializerKind.This )
            {
                this.PullDependency( aspectBuilder, pullStrategy, constructor );
            }
        }
    }

    /// <summary>
    /// Pulls the dependency from a given constructor.
    /// </summary>
    protected virtual void PullDependency( IAspectBuilder<INamedType> aspectBuilder, IPullStrategy pullStrategy, IConstructor constructor )
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
    }

    /// <summary>
    /// Gets an <see cref="IPullStrategy"/>, i.e. a strategy to pull a dependency field or property from constructors.
    /// </summary>
    /// <param name="introducedFieldOrProperty">The value returned by <see cref="TryIntroduceFieldOrProperty"/>.</param>
    /// <returns>The <see cref="IPullStrategy"/>.</returns>
    protected virtual IPullStrategy GetPullStrategy( IFieldOrProperty introducedFieldOrProperty )
        => new DefaultPullStrategy( this.Context, introducedFieldOrProperty );
}