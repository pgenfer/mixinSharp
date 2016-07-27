using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MixinRefactoring
{
    /// <summary>
    /// syntax writer that removes the whitespaces from a syntax node.
    /// Parts of this implementation are based on 
    /// https://github.com/JosefPihrt/Pihrtsoft.CodeAnalysis/blob/master/source/Pihrtsoft.CodeAnalysis.Common/CSharp/Removers/WhitespaceOrEndOfLineRemover.cs
    /// from Josef Pihrt, licensed under the Apache License, Version 2.0
    /// </summary>
    public class RemoveWhitespaceVisitor : CSharpSyntaxRewriter
    {
        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            if (trivia.Kind() == SyntaxKind.WhitespaceTrivia ||
                trivia.Kind() == SyntaxKind.EndOfLineTrivia)
                return SyntaxTrivia(SyntaxKind.WhitespaceTrivia, string.Empty);
            return base.VisitTrivia(trivia);
        }
    }
}
