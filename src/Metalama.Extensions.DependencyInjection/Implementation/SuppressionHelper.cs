// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.Collections.Generic;
using System.Linq;

namespace Metalama.Extensions.DependencyInjection.Implementation;

[CompileTime]
public static class SuppressionHelper
{
    /// <summary>
    /// Suppresses the warning CS8618 ("Non-nullable variable must contain a non-null value when exiting constructor.") on selected constructors for a member that is being introduced,
    /// if necessary. This is useful for design-time diagnostics.
    /// </summary>
    public static void SuppressNonNullableFieldMustContainValue( IAspectBuilder builder, IFieldOrProperty introducedMember, IEnumerable<IConstructor> constructors )
    {
        if ( introducedMember.Type.IsNullable != true )
        {
            foreach ( var constructor in constructors )
            {
                builder.Diagnostics.Suppress(
                    DiagnosticDescriptors.NonNullableFieldMustContainValue.WithFilter( diag => diag.Arguments.Any( arg => arg is string s && s == introducedMember.Name ) ),
                    constructor );
            }
        }
    }

    /// <summary>
    /// Suppress the warning CS0169 ("The private field is never used.") on a member that is being introduced.
    /// This is primarily useful for design-time.
    /// </summary>
    public static void SuppressUnusedWarnings( IAspectBuilder builder, IFieldOrProperty introducedMember )
    {
        builder.Diagnostics.Suppress( DiagnosticDescriptors.FieldIsNeverUsed, introducedMember );
    }
}