// Copyright (c) SharpCrafters s.r.o.All rights reserved.
// This project is not open source.Please see the LICENSE.md file in the repository root for details.

namespace Metalama.Framework.DependencyInjection.DotNet.Tests.Aspect.EarlyOptional;

// <target>
public class TargetClass
{
    [Dependency( IsRequired = false )]
    private readonly IFormatProvider? _formatProvider;

    public TargetClass() { }

    public TargetClass( int x, IFormatProvider existingParameter ) { }
}