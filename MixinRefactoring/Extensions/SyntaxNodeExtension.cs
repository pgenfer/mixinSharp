using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public static class SyntaxNodeExtension
    {
        public static ClassDeclarationSyntax FindClassByName(this SyntaxNode node, string name)
        {
            return node.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault(x => x.Identifier.ToString() == name);
        }

        /// <summary>
        /// returns the first parent class where this syntax node is placed in
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static ClassDeclarationSyntax FindContainingClass(this SyntaxNode node)
        {
            if (node == null)
                return null;
            ClassDeclarationSyntax classDeclaration = node as ClassDeclarationSyntax;
            if (classDeclaration != null)
                return classDeclaration;
            return FindContainingClass(node.Parent);
        }

        public static FieldDeclarationSyntax FindMixinReference(this SyntaxNode node, string name)
        {
            return node.DescendantNodes()
                .OfType<FieldDeclarationSyntax>()
                .FirstOrDefault(x => x.Declaration.Variables.Any(y => y.Identifier.ToString() == name));
        }

        /// <summary>
        /// short cut for returning a string representation of the
        /// syntax node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string AsString(this SyntaxNode node)
        {
            return node != null ? node.GetText().ToString() : string.Empty;
        }
    }
}
