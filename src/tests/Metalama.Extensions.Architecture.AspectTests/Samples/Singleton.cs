// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Extensions.Architecture.AspectTests.Samples
{

    public class SingletonAttribute : TypeAspect
    {
        public override void BuildAspect( IAspectBuilder<INamedType> builder )
        {
            builder.Outbound.SelectMany( t => t.Constructors )
                .CanOnlyBeUsedFrom(
                    scope => scope.Namespace( "**.Tests" )
                        .Or()
                        .Type( typeof(Startup) ),
                    "The class is a [Singleton]." );
        }
    }

    [Singleton]
    class MySingleton
    {
        
    }

    class Startup
    {
        public void Start()
        {
            // Allowed
            new MySingleton();
        }
    }

    class Forbidden
    {
        public void Start()
        {
            // Not Allowed
            new MySingleton();
        }
    }

    namespace Tests
    {
        class SomeTest
        {
            public void Start()
            {
                // Allowed
                new MySingleton();
            }
        }
    }

}