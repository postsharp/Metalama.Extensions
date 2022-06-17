// Copyright (c) SharpCrafters s.r.o.All rights reserved.
// This project is not open source.Please see the LICENSE.md file in the repository root for details.

namespace Metalama.Framework.DependencyInjection.Tests.Aspect.LazyOptional;

// <target>
public class TargetClass
{
    [Dependency( IsLazy = true, IsRequired = false )]
    private readonly IFormatProvider _formatProvider;

    public TargetClass() { }

    public TargetClass( int x, IFormatProvider existingParameter ) { }
}