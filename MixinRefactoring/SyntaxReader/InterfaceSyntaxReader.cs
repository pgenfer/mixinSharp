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
    public class InterfaceSyntaxReader : SyntaxWalkerWithSemantic
    {
        private readonly InterfaceList _interfaces;

        public InterfaceSyntaxReader(InterfaceList interfaces, SemanticModel semantic) : base(semantic)
        {
            _interfaces = interfaces;
        }

        public override void VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
            var baseClassType = node.Type;
            var baseClassSymbolInfo = _semantic.GetSymbolInfo(baseClassType);
            if (baseClassSymbolInfo.Symbol != null)
            {
                var baseClassTypeSymbol = (ITypeSymbol)baseClassSymbolInfo.Symbol;
                // this time, only check for interfaces
                if (baseClassTypeSymbol.TypeKind == TypeKind.Interface)
                    _interfaces.AddInterface(
                        new Interface(baseClassTypeSymbol.Name));
            }
        }
    }
}
