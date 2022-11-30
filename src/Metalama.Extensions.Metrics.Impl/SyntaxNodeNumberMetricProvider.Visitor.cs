// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Microsoft.CodeAnalysis;

namespace Metalama.Extensions.Metrics
{
    public partial class SyntaxNodeNumberMetricProvider
    {
        /// <summary>
        /// A visitor that counts the syntax nodes.
        /// </summary>
        private class Visitor : BaseVisitor
        {
            public override SyntaxNodeNumber DefaultVisit( SyntaxNode node )
            {
                var metric = new SyntaxNodeNumber { Value = 1 };

                foreach ( var child in node.ChildNodes() )
                {
                    metric.Add( this.Visit( child ) );
                }

                return metric;
            }
        }
    }
}