// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Extensions.DependencyInjection;
using Metalama.Extensions.DependencyInjection.AspectTests.Advice.Programmatic;
using Metalama.Framework.Code;

[assembly: AspectOrder( typeof(DependencyAttribute), typeof(MyAspect) )]

namespace Metalama.Extensions.DependencyInjection.AspectTests.Advice.Programmatic;

public class MyAspect : TypeAspect
{
    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        builder.TryIntroduceDependency( new DependencyProperties( builder.Target, typeof(IFormatProvider), "_formatProvider" ), out _ );
    }
}

// <target>
[MyAspect]
public class TargetClass
{
    public TargetClass() { }

    public TargetClass( int x, IFormatProvider existingParameter ) { }
}