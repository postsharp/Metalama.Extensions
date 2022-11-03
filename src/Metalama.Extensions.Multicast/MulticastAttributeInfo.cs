// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;
using System.Text.RegularExpressions;

namespace Metalama.Extensions.Multicast;

[CompileTime]
internal class MulticastAttributeInfo : IComparable<MulticastAttributeInfo>
{
    public IMulticastAttribute Attribute { get; }

    public MulticastAttributeInfo( IAspectInstance aspectInstance, IAspectBuilder aspectBuilder )
    {
        this.Attribute = (IMulticastAttribute) aspectInstance.Aspect;
        this.AttributeTargetTypesRegex = GetRegex( this.Attribute.AttributeTargetTypes, nameof(IMulticastAttribute.AttributeTargetTypes) );
        this.AttributeTargetMembersRegex = GetRegex( this.Attribute.AttributeTargetMembers, nameof(IMulticastAttribute.AttributeTargetMembers) );
        this.AttributeTargetParametersRegex = GetRegex( this.Attribute.AttributeTargetParameters, nameof(IMulticastAttribute.AttributeTargetParameters) );

        Regex? GetRegex( string? filter, string propertyName )
        {
            if ( filter == null )
            {
                return null;
            }
            else if ( filter.StartsWith( "regex:", StringComparison.OrdinalIgnoreCase ) )
            {
                try
                {
                    return new Regex( filter.Substring( 6 ) );
                }
                catch ( ArgumentException e )
                {
                    this.HasError = true;
                    var declaration = aspectInstance.TargetDeclaration.GetTarget( aspectBuilder.Target.Compilation );

                    aspectBuilder.Diagnostics.Report(
                        MulticastAspect.InvalidRegexError.WithArguments(
                            (propertyName, aspectInstance.AspectClass.ShortName, declaration.DeclarationKind, declaration, e.Message) ) );

                    return null;
                }
            }

#pragma warning disable CA1307
            return new Regex(
                "^" + Regex.Escape( filter ).Replace( "\\*", ".*" ).Replace( "<", "\\<" ).Replace( ">", "\\>" ) + "$",
                RegexOptions.IgnoreCase );
#pragma warning restore CA1307
        }
    }

    public bool HasError { get; private set; }

    public Regex? AttributeTargetParametersRegex { get; }

    public Regex? AttributeTargetMembersRegex { get; }

    public Regex? AttributeTargetTypesRegex { get; }

    public int CompareTo( MulticastAttributeInfo? other )
        => other == null ? 1 : this.Attribute.AttributePriority.CompareTo( other.Attribute.AttributePriority );

    private static bool DoesNameMatch( INamedDeclaration named, Regex? regex )
    {
        if ( regex == null )
        {
            return true;
        }
        else
        {
            return regex.IsMatch( named.Name );
        }
    }

    public bool IsMatch( IDeclaration declaration )
    {
        switch ( declaration.DeclarationKind )
        {
            case DeclarationKind.NamedType:
                return this.IsNamedTypeMatch( (INamedType) declaration );

            case DeclarationKind.Parameter:
                return this.IsParameterMatch( (IParameter) declaration );

            case DeclarationKind.Field:
                return this.IsFieldMatch( (IField) declaration );

            case DeclarationKind.Method:
                return this.IsMethodMatch( (IMethod) declaration );

            default:
                return this.IsMemberMatch( (IMember) declaration );
        }
    }

    private bool IsMemberMatch( IMember member )
        => DoMemberAttributesMatch( member, this.Attribute.AttributeTargetMemberAttributes ) && DoesNameMatch( member, this.AttributeTargetMembersRegex );

    private bool IsParameterMatch( IParameter parameter )
        => DoesParameterDirectionMatch( parameter, this.Attribute.AttributeTargetParameterAttributes )
           && DoesNameMatch( parameter, this.AttributeTargetParametersRegex );

    private bool IsNamedTypeMatch( INamedType type )
        => DoMemberOrNamedTypeAttributesMatch( type, this.Attribute.AttributeTargetTypeAttributes )
           && DoesFullNameMatch( type, this.AttributeTargetTypesRegex );

    private bool IsFieldMatch( IField field ) => this.IsMemberMatch( field ) && DoesLiteralMatch( field, this.Attribute.AttributeTargetMemberAttributes );

    private bool IsMethodMatch( IMethod method ) => this.IsMemberMatch( method ) && DoesManagedMatch( method, this.Attribute.AttributeTargetMemberAttributes );

    private static bool DoMemberOrNamedTypeAttributesMatch( IMemberOrNamedType member, MulticastAttributes attributes )
        => DoesAccessibilityMatch( member, attributes ) &&
           DoesAbstractionMatch( member, attributes ) &&
           DoesScopeMatch( member, attributes ) &&
           DoesCompilerGeneratedMatch( member, attributes );

    private static bool DoMemberAttributesMatch( IMember member, MulticastAttributes attributes )
        => DoMemberOrNamedTypeAttributesMatch( member, attributes ) &&
           DoesVirtualityMatch( member, attributes );

    private static bool DoesFullNameMatch( INamedType named, Regex? regex )
    {
        if ( regex == null )
        {
            return true;
        }
        else
        {
            // TODO: Not sure if PostSharp has the same name rendering in case of generics, nested types, and so on.
            return regex.IsMatch( named.FullName );
        }
    }

    private static bool DoesAccessibilityMatch( IMemberOrNamedType member, MulticastAttributes attributes )
    {
        if ( (attributes & MulticastAttributes.AnyVisibility) != 0 )
        {
            switch ( member.Accessibility )
            {
                case Accessibility.Private:
                    return attributes.HasFlag( MulticastAttributes.Private );

                case Accessibility.PrivateProtected:
                    return attributes.HasFlag( MulticastAttributes.InternalAndProtected );

                case Accessibility.Protected:
                    return attributes.HasFlag( MulticastAttributes.Protected );

                case Accessibility.Internal:
                    return attributes.HasFlag( MulticastAttributes.Internal );

                case Accessibility.ProtectedInternal:
                    return attributes.HasFlag( MulticastAttributes.InternalOrProtected );

                case Accessibility.Public:
                    return attributes.HasFlag( MulticastAttributes.Public );

                default:
                    throw new ArgumentOutOfRangeException( $"Unexpected accessibility: {member.Accessibility}." );
            }
        }
        else
        {
            return true;
        }
    }

    private static bool DoesLiteralMatch( IField member, MulticastAttributes attributes )
    {
        if ( (attributes & MulticastAttributes.AnyLiterality) != 0 )
        {
            if ( member.Writeability == Writeability.None )
            {
                return attributes.HasFlagFast( MulticastAttributes.Literal );
            }
            else
            {
                return attributes.HasFlagFast( MulticastAttributes.NonLiteral );
            }
        }
        else
        {
            return true;
        }
    }

    private static bool DoesManagedMatch( IMethod member, MulticastAttributes attributes )
    {
        if ( (attributes & MulticastAttributes.AnyImplementation) != 0 )
        {
            if ( member.IsExtern )
            {
                return attributes.HasFlagFast( MulticastAttributes.NonManaged );
            }
            else
            {
                return attributes.HasFlagFast( MulticastAttributes.Managed );
            }
        }
        else
        {
            return true;
        }
    }

    private static bool DoesCompilerGeneratedMatch( IDeclaration member, MulticastAttributes attributes )
    {
        if ( (attributes & MulticastAttributes.AnyGeneration) != 0 )
        {
            if ( member.Origin.IsCompilerGenerated )
            {
                return attributes.HasFlagFast( MulticastAttributes.CompilerGenerated );
            }
            else
            {
                return attributes.HasFlagFast( MulticastAttributes.UserGenerated );
            }
        }
        else
        {
            return true;
        }
    }

    private static bool DoesScopeMatch( IMemberOrNamedType member, MulticastAttributes attributes )
    {
        if ( (attributes & MulticastAttributes.AnyScope) != 0 )
        {
            if ( member.IsStatic )
            {
                return attributes.HasFlag( MulticastAttributes.Static );
            }
            else
            {
                return attributes.HasFlag( MulticastAttributes.Instance );
            }
        }
        else
        {
            return true;
        }
    }

    private static bool DoesAbstractionMatch( IMemberOrNamedType member, MulticastAttributes attributes )
    {
        if ( (attributes & MulticastAttributes.AnyAbstraction) != 0 )
        {
            if ( member.IsAbstract )
            {
                return attributes.HasFlag( MulticastAttributes.Abstract );
            }
            else
            {
                return attributes.HasFlag( MulticastAttributes.NonAbstract );
            }
        }
        else
        {
            return true;
        }
    }

    private static bool DoesVirtualityMatch( IMember member, MulticastAttributes attributes )
    {
        if ( (attributes & MulticastAttributes.AnyVirtuality) != 0 )
        {
            if ( member.IsVirtual )
            {
                return attributes.HasFlag( MulticastAttributes.Virtual );
            }
            else
            {
                return attributes.HasFlag( MulticastAttributes.NonVirtual );
            }
        }
        else
        {
            return true;
        }
    }

    private static bool DoesParameterDirectionMatch( IParameter parameter, MulticastAttributes attributes )
    {
        if ( (attributes & MulticastAttributes.AnyParameter) != 0 )
        {
            return parameter.RefKind switch
            {
                RefKind.Out => attributes.HasFlag( MulticastAttributes.OutParameter ),
                RefKind.Ref => attributes.HasFlag( MulticastAttributes.RefParameter ),
                _ => attributes.HasFlag( MulticastAttributes.InParameter )
            };
        }
        else
        {
            return true;
        }
    }
}