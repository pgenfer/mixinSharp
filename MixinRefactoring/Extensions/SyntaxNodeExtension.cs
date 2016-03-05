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

        public static FieldDeclarationSyntax FindMixinReference(this SyntaxNode node, string name)
        {
            return node.DescendantNodes()
                .OfType<FieldDeclarationSyntax>()
                .FirstOrDefault(x => x.Declaration.Variables.Any(y => y.Identifier.ToString() == name));
        }
    }
}
