// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Compiler;
using Metalama.Framework.Engine.Metrics;

namespace Metalama.Extensions.Metrics
{
    /// <summary>
    /// A prototype implementation of <see cref="StatementNumber"/>.
    /// </summary>
    [MetalamaPlugIn]
    public partial class StatementNumberMetricProvider : SyntaxMetricProvider<StatementNumber>
    {
        public StatementNumberMetricProvider() : base( new Visitor() ) { }

        protected override void Aggregate( ref StatementNumber aggregate, in StatementNumber newValue ) => aggregate.Add( newValue );
    }
}