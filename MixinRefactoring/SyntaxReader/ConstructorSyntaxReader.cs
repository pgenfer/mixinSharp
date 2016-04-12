using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring
{
    /// <summary>
    /// simply checks if a class declaration contains a constructor.
    /// </summary>
    public class ConstructorSyntaxReader : CSharpSyntaxWalker
    {
        private readonly IConstructorList _constructors;
        private readonly SemanticModel _semantic;

        public ConstructorSyntaxReader(IConstructorList constructors, SemanticModel semantic)
        {
            _constructors = constructors;
            _semantic = semantic;
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            // ignore static constructors
            if (node.IsStatic())
                return;

            var constructor = new Constructor();
            // read parameters
            var parameterSyntaxReader = new ParameterSyntaxReader(constructor, _semantic);
            parameterSyntaxReader.Visit(node);
            _constructors.AddConstructor(constructor);
            base.VisitConstructorDeclaration(node);
        }
    }
}
