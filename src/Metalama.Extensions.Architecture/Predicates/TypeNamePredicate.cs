// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

internal class TypeNamePredicate : BaseNamePredicate
{
    public TypeNamePredicate( string name, ReferencePredicateBuilder builder ) : base( name, builder ) { }

    public override bool IsMatch( in ReferenceEnd referenceEnd ) => this.IsMatch( referenceEnd.Type.FullName );

    public override ReferenceGranularity Granularity => ReferenceGranularity.Type;
}