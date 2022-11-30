// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Compiler;
using Metalama.Framework.Engine.Metrics;

namespace Metalama.Extensions.Metrics
{
    /// <summary>
    /// A prototype implementation of <see cref="StatementNumber"/>.
    /// </summary>
    [MetalamaPlugIn]
    public partial class SyntaxNodeNumberMetricProvider : SyntaxMetricProvider<SyntaxNodeNumber>
    {
        public SyntaxNodeNumberMetricProvider() : base( new Visitor() ) { }

        protected override void Aggregate( ref SyntaxNodeNumber aggregate, in SyntaxNodeNumber newValue ) => aggregate.Add( newValue );
    }
}