// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Metalama.Extensions.Metrics
{
    public partial class StatementNumberMetricProvider
    {
        /// <summary>
        /// A visitor that counts the syntax nodes.
        /// </summary>
        private class Visitor : BaseVisitor
        {
            public override StatementNumber VisitIfStatement( IfStatementSyntax node )
            {
                var metric = this.Visit( node.Statement );

                metric.Value++;

                if ( node.Else != null )
                {
                    metric.Add( this.Visit( node.Else ) );
                }

                return metric;
            }

            public override StatementNumber VisitSwitchStatement( SwitchStatementSyntax node )
            {
                var metric = default(StatementNumber);

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

            public override StatementNumber VisitBlock( BlockSyntax node )
            {
                var metric = default(StatementNumber);

                foreach ( var statement in node.Statements )
                {
                    metric.Add( this.Visit( statement ) );
                }

                return metric;
            }

            public override StatementNumber VisitWhileStatement( WhileStatementSyntax node )
            {
                var metric = this.Visit( node.Statement );

                metric.Value++;

                return metric;
            }

            public override StatementNumber VisitDoStatement( DoStatementSyntax node )
            {
                var metric = this.Visit( node.Statement );

                metric.Value++;

                return metric;
            }

            public override StatementNumber VisitForStatement( ForStatementSyntax node )
            {
                // We intentionally ignore the assignment and increment statements because we don't want to count in trivial increments and initializations.

                var metric = this.Visit( node.Statement );

                metric.Value++;

                return metric;
            }

            public override StatementNumber VisitExpressionStatement( ExpressionStatementSyntax node ) => new() { Value = 1 };

            public override StatementNumber VisitVariableDeclarator( VariableDeclaratorSyntax node )
            {
                if ( node.Initializer != null )
                {
                    return new StatementNumber { Value = 1 };
                }
                else
                {
                    return default;
                }
            }

            public override StatementNumber VisitBreakStatement( BreakStatementSyntax node ) => new() { Value = 1 };

            public override StatementNumber VisitContinueStatement( ContinueStatementSyntax node ) => new() { Value = 1 };

            public override StatementNumber VisitGotoStatement( GotoStatementSyntax node ) => new() { Value = 1 };

            public override StatementNumber VisitThrowStatement( ThrowStatementSyntax node ) => new() { Value = 1 };

            public override StatementNumber VisitReturnStatement( ReturnStatementSyntax node ) => new() { Value = 1 };

            public override StatementNumber VisitLabeledStatement( LabeledStatementSyntax node ) => this.Visit( node.Statement );

            public override StatementNumber VisitLockStatement( LockStatementSyntax node )
            {
                var metric = this.Visit( node.Statement );

                metric.Value++;

                return metric;
            }

            public override StatementNumber VisitUnsafeStatement( UnsafeStatementSyntax node ) => this.Visit( node.Block );

            public override StatementNumber VisitTryStatement( TryStatementSyntax node )
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

            public override StatementNumber VisitUsingStatement( UsingStatementSyntax node )
            {
                var metric = this.Visit( node.Statement );

                metric.Value++;

                return metric;
            }

            public override StatementNumber VisitForEachStatement( ForEachStatementSyntax node )
            {
                var metric = this.Visit( node.Statement );

                metric.Value++;

                return metric;
            }

            public override StatementNumber VisitYieldStatement( YieldStatementSyntax node ) => new() { Value = 1 };
        }
    }
}