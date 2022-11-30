The `Metalama.Extensions.DependencyInjection` package implements support for dependency injection in Metalama.

It has two major features:

* `[IntroduceDependency]` is an advice attribute that makes it easier for aspect to pull dependencies without knowing which dependency framework is used by the consuming project.
* `[Dependency]` is an aspect that turns a field or property into a dependency, and pulls it according to the rules of the dependency framework used by the consuming project.

## See also

* [Conceptual Documentation](https://doc.metalama.net/aspects/dependency-injection)
* [API Documentation](https://doc.metalama.net/api/metalama_extensions_dependencyinjection)

## Backward Compatibility Promise

All public APIs are guaranteed to be backward compatible within the current major version.

The promise does not apply to any preview build of this package.

