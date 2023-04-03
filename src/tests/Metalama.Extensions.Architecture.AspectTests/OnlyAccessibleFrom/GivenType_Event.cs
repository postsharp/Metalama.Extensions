// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;
using System;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.GivenType_Event
{
    internal class ConstrainedClass
    {
        [CanOnlyBeUsedFrom( Types = new[] { typeof(AllowedClass) } )]
        public event EventHandler ConstrainedEvent;
    }

    internal class ForbiddenClass
    {
        private static void AddAndRemove()
        {
            var o = new ConstrainedClass();
            o.ConstrainedEvent += HandleEvent;
            o.ConstrainedEvent -= HandleEvent;
        }

        private static void HandleEvent( object? sender, EventArgs e ) { }
    }

    internal class AllowedClass
    {
        private static void AddAndRemove()
        {
            var o = new ConstrainedClass();
            o.ConstrainedEvent += HandleEvent;
            o.ConstrainedEvent -= HandleEvent;
        }

        private static void HandleEvent( object? sender, EventArgs e ) { }
    }
}