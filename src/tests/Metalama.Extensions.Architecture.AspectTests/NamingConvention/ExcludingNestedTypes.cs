// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Code;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.AspectTests.NamingConvention.ExcludingNestedTypes
{
    [DerivedTypesMustRespectNamingConvention( "*Factory", ExclusionPredicateType = typeof(ExcludeNestedTypesPredicate) )]
    internal class BaseClass { }

    internal class CorrectNameFactory : BaseClass
    {
        private class IgnoredBecauseNestedClass : BaseClass { }
    }

    internal class IncorrectName : BaseClass { }

    internal class ExcludeNestedTypesPredicate : ReferencePredicate
    {
        public override bool IsMatch( in ReferenceValidationContext context ) => context.ReferencingDeclaration is INamedType { DeclaringType: not null };
    }
}