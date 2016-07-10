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
            // one problem is that the type could be from a different semantic model
            // (one that was created before) so we search the type in the
            // new model and if we find it use the new one instead
            var typeInSemanticModel = semantic.LookupSymbols(0, name: type.Name)
                .OfType<ITypeSymbol>()
                .FirstOrDefault();
            if (typeInSemanticModel != null)
                type = typeInSemanticModel;
            return type.ToMinimalDisplayString(semantic, classDeclarationPosition);
        }
    }
}
