using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public static class TypeSymbolExtension
    {
        public static string ReduceQualifiedTypeName(
            this ITypeSymbol type,SemanticModel semantic,int classDeclarationPosition)
        {
            return type.ToMinimalDisplayString(semantic, classDeclarationPosition);
        }
    }
}
