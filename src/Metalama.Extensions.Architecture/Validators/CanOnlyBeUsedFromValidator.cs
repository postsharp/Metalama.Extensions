// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;
using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Framework.Aspects;

namespace Metalama.Extensions.Architecture.Validators;

[CompileTime]
internal class CanOnlyBeUsedFromValidator : BaseUsageValidator
{
    public CanOnlyBeUsedFromValidator( MatchingRule rule, string? currentNamespace ) : base(
        rule,
        currentNamespace ) { }

    public override string ConstraintName => "CanOnlyBeUsedFrom";

    protected override MatchBehavior OnMatch => MatchBehavior.Allow;
}