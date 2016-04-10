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
   public static class ConstructorDeclarationExtension
   {
        public static bool IsStatic(this ConstructorDeclarationSyntax constructor)
        {
            return constructor.Modifiers.Any(x => x.IsKind(SyntaxKind.StaticKeyword));
        }
   }
}
