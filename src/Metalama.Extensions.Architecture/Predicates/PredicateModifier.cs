// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Serialization;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

[RunTimeOrCompileTime]
internal abstract class PredicateModifier : ICompileTimeSerializable
{
    public abstract bool IsMatch( bool currentPredicateResult, ReferenceValidationContext context );
}