﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Multicast.AspectTests;

[assembly: AddTag( "Tagged" )]

// <target>

namespace Metalama.Extensions.Multicast.AspectTests.AddToAllFromCompilation
{
    public class AClass
    {
        public int Method( int p ) => 0;

        public int Property { get; private set; }

        public int Field;

        public event Action PublicEvent;
    }

    public class AStruct
    {
        private class Nested
        {
            public int Method( int p ) => 0;

            public int Property { get; private set; }

            public int Field;

            public event Action PublicEvent;
        }
    }

    // TODO: currently adding an attribute to an enum or delegate does nothing.

    public enum AnEnum { }

    public delegate void ADelegate();
}