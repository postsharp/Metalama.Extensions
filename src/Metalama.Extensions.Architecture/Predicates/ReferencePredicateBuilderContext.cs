// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Framework.Aspects;
using Metalama.Framework.Fabrics;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

[CompileTime]
public sealed class ReferencePredicateBuilderContext
{
    internal ReferencePredicateBuilderContext( ReferenceEndRole validatedRole, string? ns, string? assemblyName )
    {
        this.ValidatedRole = validatedRole;
        this.Namespace = ns;
        this.AssemblyName = assemblyName;
    }

    /// <summary>
    /// Gets the role of the <see cref="ReferenceEnd"/> validated by the built predicates.
    /// </summary>
    public ReferenceEndRole ValidatedRole { get; }

    /// <summary>
    /// Gets the namespace from which the current <see cref="ReferencePredicateBuilder"/> was instantiated, i.e. the namespace of the
    /// <see cref="NamespaceFabric"/> or the <see cref="TypeFabric"/>. Returns <c>null</c> if the <see cref="ReferencePredicateBuilder"/> was instantiated
    /// from a <see cref="ProjectFabric"/>. 
    /// </summary>
    public string? Namespace { get; }

    /// <summary>
    /// Gets assembly name the project that instantiated the current <see cref="IVerifier{T}"/>.
    /// </summary>
    public string? AssemblyName { get; }
}