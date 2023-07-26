// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;

namespace Metalama.Extensions.Architecture.Fabrics;

internal class CompilationSetVerifier : Verifier<ICompilation>, ITypeSetVerifier<ICompilation>
{
    public CompilationSetVerifier( IAspectReceiver<ICompilation> receiver, string assemblyName ) : base( receiver, assemblyName, null ) { }

    public IAspectReceiver<INamedType> TypeReceiver => this.Receiver.SelectMany( x => x.Types );

    public ITypeSetVerifier<INamedType> SelectTypesDerivedFrom( Type type, DerivedTypesOptions options = DerivedTypesOptions.Default )
        => new TypeSetVerifier<INamedType>( this.Receiver.SelectMany( c => c.GetDerivedTypes( type, options ) ), x => x, this.AssemblyName, null );
}