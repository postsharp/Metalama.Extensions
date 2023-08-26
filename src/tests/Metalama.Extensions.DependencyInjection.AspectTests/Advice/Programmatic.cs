// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Extensions.DependencyInjection;
using Metalama.Extensions.DependencyInjection.Tests.Advice.Programmatic;
using Metalama.Framework.Code;
using Metalama.Extensions.DependencyInjection.Implementation;

[assembly: AspectOrder( typeof(DependencyAttribute), typeof(MyAspect) )]

namespace Metalama.Extensions.DependencyInjection.Tests.Advice.Programmatic;

public class MyAspect : TypeAspect

{
    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        builder.TryIntroduceDependency( new DependencyProperties( builder.Target, typeof( IFormatProvider ), "_formatProvider", false ), out _ );
    }
}

// <target>
[MyAspect]
public class TargetClass
{
    public TargetClass() { }

    public TargetClass( int x, IFormatProvider existingParameter ) { }
}