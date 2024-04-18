// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Extensions.Architecture.Validators;
using Metalama.Framework.Aspects;

namespace Metalama.Extensions.Architecture.Aspects;

/// <summary>
/// Aspect that, when applied to a public type, reports a warning whenever any member of the target type is used from
/// from a different type than the ones specified by the <see cref="BaseUsageValidationAttribute.Types"/>, <see cref="BaseUsageValidationAttribute.Namespaces"/>,
/// <see cref="BaseUsageValidationAttribute.NamespaceOfTypes"/> or <see cref="BaseUsageValidationAttribute.CurrentNamespace"/> properties.
/// </summary>
[PublicAPI]
[RunTimeOrCompileTime]
public class InternalsCanOnlyBeUsedFromAttribute : InternalsUsageValidationAttribute
{
    protected override ReferencePredicateValidator CreateValidator(
        ReferencePredicate predicate,
        ReferencePredicate? exclusionPredicate )
        => new(
            predicate.Builder.HasFamilyAccess().Or( predicate ).Or( exclusionPredicate ),
            this.Description,
            this.ReferenceKinds );
}