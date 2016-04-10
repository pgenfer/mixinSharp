using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring
{
    /// <summary>
    /// simply checks if a class declaration contains a constructor.
    /// </summary>
    public class CountConstructorReader : CSharpSyntaxWalker
    {
        /// <summary>
        /// true if the class has a constructor after traversing
        /// syntax declaration
        /// </summary>
        public bool HasConstructor { get; private set; }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            HasConstructor = true;
            base.VisitConstructorDeclaration(node);
        }
    }
}
