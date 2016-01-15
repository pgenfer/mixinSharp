using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    /// <summary>
    /// Stores information about a property which a mixin
    /// provides as a service.
    /// Current information is not yet supported or must be evaluated:
    /// 1. Generic properties (in case the mixin is a generic type)
    /// 2. The generated delegation is always public, because at
    ///     the moment, there is no way of setting the property access within the mixin child
    /// </summary>
    public class PropertyService : PropertyServiceBase, IMixinService
    {
        public PropertyService(IPropertySymbol propertySymbol) : base(propertySymbol)
        {           
        }

        public bool IsImplementedBy(MethodServiceBase implementation) => false;
        public bool IsImplementedBy(PropertyServiceBase implementation) => IsEqual(implementation);

        public MemberDeclarationSyntax ToMemberDeclaration(string nameOfMixinVariable)
        {
            // access to member (like: "_mixin.Property")
            var memberAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(nameOfMixinVariable),
                SyntaxFactory.IdentifierName(Name));

            // in case only getter => use expression body
            if (HasGetter && !HasSetter)
            {
                return
                    SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(Type.DisplayFormat()), Name)
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(memberAccess))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));
            }
            // otherwise create getter and/or setter bodies
            var getStatement = HasGetter ?
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                 .AddBodyStatements(SyntaxFactory.ReturnStatement(memberAccess)) :
                null;
            var setStatement = HasSetter ?
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .AddBodyStatements(SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        memberAccess,
                        SyntaxFactory.IdentifierName("value")))) :
                null;

            var accessorList = new SyntaxList<AccessorDeclarationSyntax>();
            // store statements in accessorlist 
            if (getStatement != null) accessorList = accessorList.Add(getStatement);
            if (setStatement != null) accessorList = accessorList.Add(setStatement);

            var propertyDeclaration =
                SyntaxFactory.PropertyDeclaration(
                SyntaxFactory.ParseTypeName(Type.DisplayFormat()), Name)
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithAccessorList(SyntaxFactory.AccessorList(accessorList));
            return propertyDeclaration;
        }
    }
}
