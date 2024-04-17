// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

internal class ReferencingNamespacePredicate : BaseNamePredicate
{
    public ReferencingNamespacePredicate( string name, ReferencePredicateBuilder? builder = null ) : base( name, builder ) { }

    public override bool IsMatch( ReferenceValidationContext context )
    {
        for ( var ns = context.Referencing.Namespace; ns != null; ns = ns.ParentNamespace )
        {
            if ( this.IsMatch( ns.FullName ) )
            {
                return true;
            }
        }

        return false;
    }

    public override ReferenceGranularity Granularity => ReferenceGranularity.Namespace;
}