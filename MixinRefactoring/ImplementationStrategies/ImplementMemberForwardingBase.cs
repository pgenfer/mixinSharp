using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MixinSharp.XmlSyntaxFactory;
using System;

namespace MixinRefactoring
{
    /// <summary>
    /// base class for creating the syntax nodes needed
    /// to implement the member forwarding from child to mixin.
    /// </summary>
    /// <typeparam name = "T">type of the member that should be forwarded</typeparam>
    public abstract class ImplementMemberForwardingBase<T> : IImplementMemberForwarding where T : Member
    {
        private readonly SemanticModel _semantic;
        // we need the position of the class in source file to reduce qualified names
        private int _classDeclarationPosition;
        protected readonly Settings _settings;
        public string Name
        {
            get;
        }

        protected ImplementMemberForwardingBase(
            string mixinReferenceName, 
            SemanticModel semanticModel, 
            Settings settings)
        {
            Name = mixinReferenceName;
            _semantic = semanticModel;
            _settings = settings;
        }

        /// <summary>
        /// casts the input parameter to the type T, will only work 
        /// if member is of type T.
        /// Enables a dispatch so that correct method of derived class can be called
        /// </summary>
        /// <param name = "member"></param>
        /// <returns></returns>
        public MemberDeclarationSyntax ImplementMember(Member member, int positionOfClassInSourceFile)
        {
            _classDeclarationPosition = positionOfClassInSourceFile;
            return ImplementMember((T)member);
        }

        /// <summary>
        /// Implementation must be provided by derived classes
        /// </summary>
        /// <param name = "member"></param>
        /// <returns></returns>
        protected abstract MemberDeclarationSyntax ImplementMember(T member);
        /// <summary>
        /// reduces the qualification of a type name if possible.
        /// Depends on the position of the mixin class within the source code
        /// because it has to be checked whether the types namespace
        /// is already included by a using statement.
        /// </summary>
        /// <param name = "type"></param>
        /// <returns></returns>
        protected string ReduceQualifiedTypeName(ITypeSymbol type)
        {
            return type.ReduceQualifiedTypeName(_semantic, _classDeclarationPosition);
        }

        /// <summary>
        /// generates the documentation node for this members documentation
        /// </summary>
        /// <param name = "documentation">documentation of the original member</param>
        /// <returns>syntax nodes containing the documentation for the newly generated
        /// member</returns>
        protected SyntaxTriviaList CreateComment(DocumentationComment documentation)
        {
            if (_settings.IncludeDocumentation && documentation.HasSummary)
            {
                // create an array with every line in the documentation
                var commentLines = documentation.Elements.Select(x => MultiLineElement(x.Tag, x.Content, x.Attributes)).ToArray();
                // add an additional new line before the comment
                var documentationNode = TriviaList(EndOfLine(Environment.NewLine), Trivia(DocumentationComment(commentLines)));
                return documentationNode;
            }

            return TriviaList();
        }

        /// <summary>
        /// creates the modifiers for this member
        /// </summary>
        /// <param name = "member"></param>
        /// <returns></returns>
        protected virtual SyntaxTokenList CreateModifiers(T member)
        {
            var modifiers = TokenList(Token(SyntaxKind.PublicKeyword));
            modifiers = member.IsOverride ? modifiers.Add(Token(SyntaxKind.OverrideKeyword)) : modifiers;
            return modifiers;
        }

        /// <summary>
        /// creates the list of parameters for a method or an indexer
        /// </summary>
        /// <param name = "parameterList"></param>
        /// <returns></returns>
        protected IEnumerable<ParameterSyntax> ImplementParameters(IEnumerable<Parameter> parameterList)
        {
            var parameters = parameterList.Select(x => Parameter(new SyntaxList<AttributeListSyntax>(), TokenList(), ParseTypeName(ReduceQualifiedTypeName(x.Type)), Identifier(x.Name), null));
            return parameters;
        }
    }
}