using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    /// <summary>
    /// Converts a type from a syntax node into a type symbol
    /// from the semantic model.
    /// </summary>
    public static class TypeSyntaxExtension
    {
        public static ITypeSymbol ToTypeSymbol(this TypeSyntax typeSyntax,SemanticModel model )
        {
            return (ITypeSymbol)model.GetSymbolInfo(typeSyntax).Symbol;
        }
    }
}
