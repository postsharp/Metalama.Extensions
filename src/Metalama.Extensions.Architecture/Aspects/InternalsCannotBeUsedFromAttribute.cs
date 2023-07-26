// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Extensions.Architecture.Validators;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using System.Linq;

namespace Metalama.Extensions.Architecture.Aspects;

/// <summary>
/// Aspect that, when applied to a public type, reports a warning whenever any member of the target type is used from
/// from any type specified by the <see cref="BaseUsageValidationAttribute.Types"/>, <see cref="BaseUsageValidationAttribute.Namespaces"/>,
/// <see cref="BaseUsageValidationAttribute.NamespaceOfTypes"/> or <see cref="BaseUsageValidationAttribute.CurrentNamespace"/> properties.
/// </summary>
[PublicAPI]
public class InternalsCannotBeUsedFromAttribute : InternalsUsageValidationAttribute
{
    protected override ReferencePredicateValidator CreateValidator( ReferencePredicate predicate )
    {
        return new ReferencePredicateValidator(
            new OrPredicate( new HasFamilyAccessPredicate(), predicate.Not() ),
            this.Description,
            this.ReferenceKinds );
    }
}