// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using System;

namespace Metalama.Extensions.DependencyInjection.Implementation;

public partial class LazyDependencyInjectionStrategy
{
    private class PullStrategy : DefaultPullStrategy
    {
        private readonly IField _funcField;
        private readonly INamedType _funcType;

        protected override IType ParameterType => this._funcType;

        public PullStrategy( DependencyContext context, IProperty mainProperty, IField funcField ) : base( context, mainProperty )
        {
            this._funcField = funcField;
            this._funcType = ((INamedType) TypeFactory.GetType( typeof(Func<>) )).WithTypeArguments( mainProperty.Type ).ConstructNullable();
        }

        protected override IFieldOrProperty AssignedFieldOrProperty => this._funcField;
    }
}