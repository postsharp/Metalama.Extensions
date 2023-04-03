// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.CurrentAssembly;

[CanOnlyBeUsedFrom( CurrentAssembly = true, ReferenceKinds = ReferenceKinds.BaseType, Description = "Can only be implemented in the current assembly." )]
public class RestrictedClass { }