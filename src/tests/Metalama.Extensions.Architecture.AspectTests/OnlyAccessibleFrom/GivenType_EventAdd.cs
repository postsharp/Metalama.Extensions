// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;
using System;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.GivenType_EventAdd
{
    // TODO: Event add and event remove are not distinguished. See AspectReceiver<T>.SelectReferenceValidatorInstances method.

    internal class ConstrainedClass
    {
        private event EventHandler PrivateEvent;

        public event EventHandler ConstrainedEvent
        {
            [CanOnlyBeUsedFrom( Types = new[] { typeof( AllowedClass ) } )]
            add => this.PrivateEvent += value;

            remove => this.PrivateEvent -= value;
        }
    }

    internal class ForbiddenClass
    {
        private static void AddAndRemove()
        {
            var o = new ConstrainedClass();
            o.ConstrainedEvent += HandleEvent;
            o.ConstrainedEvent -= HandleEvent;
        }

        private static void HandleEvent( object? sender, EventArgs e )
        {
        }
    }

    internal class AllowedClass
    {
        private static void AddAndRemove()
        {
            var o = new ConstrainedClass();
            o.ConstrainedEvent += HandleEvent;
            o.ConstrainedEvent -= HandleEvent;
        }

        private static void HandleEvent( object? sender, EventArgs e )
        {
        }
    }
}