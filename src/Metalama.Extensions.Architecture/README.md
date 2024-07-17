![Metalama Logo](https://raw.githubusercontent.com/postsharp/Metalama/master/images/metalama-by-postsharp.svg)

The `Metalama.Extensions.Architecture` package is an official and open-source Metalama extension that allows you to validate the code against the architecture.

There are two ways to use this package:

1. By adding to your code one of the following architecture custom attributes from the `Metalama.Extensions.Architecture.Aspects` namespace:
    * `[CannotBeUsedFrom]`
    * `[CanOnlyBeUsedFrom]`
    * `[InternalsCannotBeUsedFrom]`
    * `[InternalsCanOnlyBeUsedFrom]`
    * `[Experimental]`
    * `[InternalImplementOnly]`
    * `[DerivedTypesMustRespectNamingConvention]`
    * `[DerivedTypesMustRespectRegexNamingConvention]`

2. By using fabrics, for instance:


    ```cs
        namespace SomeNamespace;

        using Metalama.Extensions.Architecture.Fabrics;
        using Metalama.Framework.Fabrics;

        public class Fabric : NamespaceFabric
        {
            public override void AmendNamespace( INamespaceAmender amender )
            {
                amender.Verify().InternalsCanOnlyBeUsedFrom( UsageRule.OwnNamespace );
            }
        }
    ```