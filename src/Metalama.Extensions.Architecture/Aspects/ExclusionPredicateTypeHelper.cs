// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Aspects;
using Metalama.Framework.Diagnostics;
using System;

namespace Metalama.Extensions.Architecture.Aspects;

[CompileTime]
internal static class ExclusionPredicateTypeHelper
{
    public static bool TryCreateExclusionPredicate(
        Type? type,
        ScopedDiagnosticSink diagnostics,
        ReferencePredicateBuilder predicateBuilder,
        out ReferencePredicate? predicate )
    {
        if ( type != null )
        {
            if ( !typeof(ReferencePredicate).IsAssignableFrom( type ) )
            {
                diagnostics.Report( ArchitectureDiagnosticDefinitions.ExclusionTypePropertyMustBeOfTypeReferencePredicate );
                predicate = null;

                return false;
            }
            else
            {
                var constructor = type.GetConstructor( [typeof(ReferencePredicateBuilder)] );

                if ( constructor == null )
                {
                    diagnostics.Report( ArchitectureDiagnosticDefinitions.ExclusionTypePropertyMustHaveDefaultConstructor );
                    predicate = null;

                    return false;
                }

                predicate = (ReferencePredicate) constructor.Invoke( [predicateBuilder] );

                return true;
            }
        }

        predicate = null;

        return true;
    }
}