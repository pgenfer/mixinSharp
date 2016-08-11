using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MixinRefactoring
{
    /// <summary>
    /// creates the syntactial representation of a method forwarding
    /// </summary>
    public class ImplementMethodForwarding : ImplementMemberForwardingBase<Method>
    {
        public ImplementMethodForwarding(MixinReference mixin, SemanticModel semanticModel, Settings settings): base (mixin, semanticModel, settings)
        {
        }

        /// <summary>
        /// special handling for modifiers in case 
        /// the method overrides a member from Object and needs an "override" keyword
        /// </summary>
        /// <param name = "member"></param>
        /// <returns></returns>
        protected override SyntaxTokenList CreateModifiers(Method member)
        {
            var modifiers = TokenList(Token(SyntaxKind.PublicKeyword));
            modifiers = member.IsOverrideFromObject || member.IsOverride ? 
                modifiers.Add(Token(SyntaxKind.OverrideKeyword)) : 
                modifiers;
            return modifiers;
        }

        protected override MemberDeclarationSyntax ImplementMember(Method member)
        {
            var parameters = ImplementParameters(member.Parameters);
            // method body should delegate call to mixin directly
            var mixinServiceInvocation = 
                InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression, 
                        IdentifierName(Name), 
                        IdentifierName(member.Name)))
                .AddArgumentListArguments(member.Parameters.Select(x => Argument(IdentifierName(x.Name))).ToArray());
            // will be: void method(int parameter) => _mixin.method(parameter);
            var methodDeclaration = 
                MethodDeclaration(
                    ParseTypeName(ReduceQualifiedTypeName(member.ReturnType)), 
                    member.Name)
                .AddParameterListParameters(parameters.ToArray())
                .WithExpressionBody(ArrowExpressionClause(mixinServiceInvocation))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                .WithModifiers(CreateModifiers(member))
                .WithLeadingTrivia(CreateComment(member.Documentation));
            return methodDeclaration;
        }
    }
}