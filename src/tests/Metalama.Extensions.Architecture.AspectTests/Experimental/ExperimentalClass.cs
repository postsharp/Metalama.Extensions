// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#pragma warning disable CS8618, CS0169

namespace Metalama.Extensions.Architecture.AspectTests.Experimental
{
    [Experimental]
    public class ExperimentalClass
    {
        // This is a legal use because this is inside the same type.
        public static ExperimentalClass Method() => null!;
    }

    [Experimental]
    public class OtherExperimentalClass
    {
        // This is legal use because this is inside another experimental class.
        private readonly ExperimentalClass _experimentalClass;
    }

    // Illegal inheritance.
    public class NonExperimentalClass : ExperimentalClass
    {
        // Illegal field.
        private readonly ExperimentalClass _field;

        public static ExperimentalClass Method2()
        {
            // Illegal use of constructor.
            _ = new ExperimentalClass();

            return Method();
        }
    }
}