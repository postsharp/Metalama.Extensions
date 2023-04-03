// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

internal class ReferencingAssemblyPredicate : BaseNamePredicate
{
    public ReferencingAssemblyPredicate( string name, ReferencePredicateBuilder? builder = null ) : base( name, builder ) { }

    public override bool IsMatch( in ReferenceValidationContext context )
    {
        return this.IsMatch( context.ReferencingType.DeclaringAssembly.Identity.Name );
    }
}