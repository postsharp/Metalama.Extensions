// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using System.Linq;

namespace Metalama.Extensions.DependencyInjection.Implementation;

internal sealed class LoggerDependencyInjectionFramework : DefaultDependencyInjectionFramework
{
    public override bool CanHandleDependency( DependencyContext context )
        => context.FieldOrProperty.Type is INamedType { Name: "ILogger", FullName: "Microsoft.Extensions.Logging.ILogger" };

    protected override DefaultDependencyInjectionStrategy GetStrategy( DependencyContext context ) => new InjectionStrategy( context );

    // Our customized injection strategy. Decides how to create the field or property.
    // We actually have no customization except that we return a customized pull strategy instead of the default one.
    private class InjectionStrategy : DefaultDependencyInjectionStrategy
    {
        public InjectionStrategy( DependencyContext context ) : base( context ) { }

        protected override IPullStrategy GetPullStrategy( IFieldOrProperty introducedFieldOrProperty )
        {
            return new LoggerPullStrategy( this.Context, introducedFieldOrProperty );
        }
    }

    // Our customized pull strategy. Decides how to assign the field or property from the constructor.
    private class LoggerPullStrategy : DefaultPullStrategy
    {
        public LoggerPullStrategy( DependencyContext context, IFieldOrProperty introducedFieldOrProperty ) : base( context, introducedFieldOrProperty )
        {
            var loggerType = (INamedType) introducedFieldOrProperty.Type;
            var genericLoggerType = loggerType.Namespace.Types.OfName( "ILogger" ).Single( l => l.TypeParameters.Count == 1 );

            // Sets the type of the required or created constructor parameter. We return ILogger<T> where T is the declaring type.
            // (The default behavior would return just ILogger).
            this.ParameterType = genericLoggerType.WithTypeArguments( this.IntroducedFieldOrProperty.DeclaringType );
        }

        protected override IType ParameterType { get; }
    }
}