using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections;

namespace MixinRefactoring
{
     public class MethodSyntaxReader : SyntaxWalkerWithSemantic
    {
        private readonly IMethodList _methods;

        public MethodSyntaxReader(IMethodList methods,SemanticModel semantic) :base(semantic)
        {
            _methods = methods;
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var method = new Method(
                node.Identifier.ToString(),
                (ITypeSymbol)_semantic.GetSymbolInfo(node.ReturnType).Symbol);
            var parameterSyntaxReader = new ParameterSyntaxReader(method, _semantic);
            parameterSyntaxReader.Visit(node);
            _methods.AddMethod(method);
        }
    }   
}
