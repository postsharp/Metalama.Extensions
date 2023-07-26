// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Aspects;
using Metalama.Framework.Diagnostics;
using System;

namespace Metalama.Extensions.Architecture.Aspects;

[CompileTime]
internal static class ExclusionPredicateTypeHelper
{
    public static bool ValidateExclusionPredicateType( Type? type, ScopedDiagnosticSink diagnostics )
    {
        if ( type != null )
        {
            if ( !typeof(ReferencePredicate).IsAssignableFrom( type ) )
            {
                diagnostics.Report( ArchitectureDiagnosticDefinitions.ExclusionTypePropertyMustBeOfTypeReferencePredicate );

                return false;
            }
            else if ( type.GetConstructor( Type.EmptyTypes ) == null )
            {
                diagnostics.Report( ArchitectureDiagnosticDefinitions.ExclusionTypePropertyMustHaveDefaultConstructor );

                return false;
            }
        }

        return true;
    }

    public static ReferencePredicate? GetExclusionPredicate( Type? type )
    {
        if ( type != null )
        {
            return (ReferencePredicate) Activator.CreateInstance( type );
        }
        else
        {
            return null;
        }
    }
}