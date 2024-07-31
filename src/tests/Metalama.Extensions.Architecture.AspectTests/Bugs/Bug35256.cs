// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Extensions.Architecture.Predicates;

namespace Metalama.Extensions.Architecture.AspectTests.Bugs.Bug32851.Bug35256;

public class SingletonAttribute : TypeAspect
{
    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        builder.Outbound
            .SelectMany( t => t.Constructors )
            .CanOnlyBeUsedFrom(
                scope => scope.Type( typeof(Startup) ).Or().Namespace( "**.Tests.**" ),
                description: "The class is a [Singleton]." );
    }
}

[Singleton]
internal class C;

internal class Startup
{
    private C c = new();
}

internal class Rogue
{
    private C c = new();
}