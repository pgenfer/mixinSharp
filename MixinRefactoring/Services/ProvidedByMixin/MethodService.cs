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
    public class MethodService : MethodServiceBase, IMixinService
    {
        public MethodService(IMethodSymbol methodSymbol) : base(methodSymbol.Name)
        {
            ReturnType = methodSymbol.ReturnType;
            _parameters.AddRange(methodSymbol.Parameters.Select(x => new Parameter(x.Name, x.Type)));
        }

        public bool IsImplementedBy(PropertyServiceBase implementation) => false;
        public bool IsImplementedBy(MethodServiceBase implementation) => IsEqual(implementation);

        public MemberDeclarationSyntax ToMemberDeclaration(string nameOfMixinVariable)
        {
            var parameters = _parameters.Select(x => SyntaxFactory.Parameter(
                new SyntaxList<AttributeListSyntax>(),
                SyntaxFactory.TokenList(),
                SyntaxFactory.ParseTypeName(x.Type.DisplayFormat()),
                SyntaxFactory.Identifier(x.Name),
                null));

            // method body should delegate call to mixin directly
            var mixinServiceInvocation =
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(nameOfMixinVariable),
                        SyntaxFactory.IdentifierName(Name)))
                .AddArgumentListArguments(_parameters
                    .Select(x => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(x.Name)))
                    .ToArray());
            // will be: void method(int parameter) => _mixin.method(parameter);
            var methodDeclaration =
                SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(ReturnType.DisplayFormat()), Name)
                .AddParameterListParameters(parameters.ToArray())
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(mixinServiceInvocation))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));

            return methodDeclaration;
        }
    }
}
