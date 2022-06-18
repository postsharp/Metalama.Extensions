// Copyright (c) SharpCrafters s.r.o. All rights reserved. See LICENSE.md in the repository root for details.

namespace Metalama.Framework.DependencyInjection.DotNet.Tests.Aspect.EarlyRequired;

// <target>
public class TargetClass
{
    [Dependency]
    private readonly IFormatProvider _formatProvider;

    public TargetClass() { }

    public TargetClass( int x, IFormatProvider existingParameter ) { }
}