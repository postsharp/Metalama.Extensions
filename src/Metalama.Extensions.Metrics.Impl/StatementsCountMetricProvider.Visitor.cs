// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Metalama.Extensions.Metrics
{
    public partial class StatementsCountMetricProvider
    {
        /// <summary>
        /// A visitor that counts the syntax nodes.
        /// </summary>
        private class Visitor : BaseVisitor
        {
            public override StatementsCount VisitIfStatement( IfStatementSyntax node )
            {
                var metric = this.Visit( node.Statement );

                metric.Value++;

                if ( node.Else != null )
                {
                    metric.Add( this.Visit( node.Else ) );
                }

                return metric;
            }

            public override StatementsCount VisitSwitchStatement( SwitchStatementSyntax node )
            {
                var metric = default(StatementsCount);

                metric.Value++;

                foreach ( var section in node.Sections )
                {
                    foreach ( var statement in section.Statements )
                    {
                        metric.Add( this.Visit( statement ) );
                    }
                }

                return metric;
            }

            public override StatementsCount VisitBlock( BlockSyntax node )
            {
                var metric = default(StatementsCount);

                foreach ( var statement in node.Statements )
                {
                    metric.Add( this.Visit( statement ) );
                }

                return metric;
            }

            public override StatementsCount VisitWhileStatement( WhileStatementSyntax node )
            {
                var metric = this.Visit( node.Statement );

                metric.Value++;

                return metric;
            }

            public override StatementsCount VisitDoStatement( DoStatementSyntax node )
            {
                var metric = this.Visit( node.Statement );

                metric.Value++;

                return metric;
            }

            public override StatementsCount VisitForStatement( ForStatementSyntax node )
            {
                // We intentionally ignore the assignment and increment statements because we don't want to count in trivial increments and initializations.

                var metric = this.Visit( node.Statement );

                metric.Value++;

                return metric;
            }

            public override StatementsCount VisitExpressionStatement( ExpressionStatementSyntax node ) => new() { Value = 1 };

            public override StatementsCount VisitVariableDeclarator( VariableDeclaratorSyntax node )
            {
                if ( node.Initializer != null )
                {
                    return new StatementsCount { Value = 1 };
                }
                else
                {
                    return default;
                }
            }

            public override StatementsCount VisitBreakStatement( BreakStatementSyntax node ) => new() { Value = 1 };

            public override StatementsCount VisitContinueStatement( ContinueStatementSyntax node ) => new() { Value = 1 };

            public override StatementsCount VisitGotoStatement( GotoStatementSyntax node ) => new() { Value = 1 };

            public override StatementsCount VisitThrowStatement( ThrowStatementSyntax node ) => new() { Value = 1 };

            public override StatementsCount VisitReturnStatement( ReturnStatementSyntax node ) => new() { Value = 1 };

            public override StatementsCount VisitLabeledStatement( LabeledStatementSyntax node ) => this.Visit( node.Statement );

            public override StatementsCount VisitLockStatement( LockStatementSyntax node )
            {
                var metric = this.Visit( node.Statement );

                metric.Value++;

                return metric;
            }

            public override StatementsCount VisitUnsafeStatement( UnsafeStatementSyntax node ) => this.Visit( node.Block );

            public override StatementsCount VisitTryStatement( TryStatementSyntax node )
            {
                var metric = this.Visit( node.Block );

                foreach ( var block in node.Catches )
                {
                    metric.Add( this.Visit( block.Block ) );
                }

                if ( node.Finally != null )
                {
                    metric.Add( this.Visit( node.Finally ) );
                }

                return metric;
            }

            public override StatementsCount VisitUsingStatement( UsingStatementSyntax node )
            {
                var metric = this.Visit( node.Statement );

                metric.Value++;

                return metric;
            }

            public override StatementsCount VisitForEachStatement( ForEachStatementSyntax node )
            {
                var metric = this.Visit( node.Statement );

                metric.Value++;

                return metric;
            }

            public override StatementsCount VisitYieldStatement( YieldStatementSyntax node ) => new() { Value = 1 };
        }
    }
}