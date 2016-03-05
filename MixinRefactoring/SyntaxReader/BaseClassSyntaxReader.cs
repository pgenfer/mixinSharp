using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring
{

    public class BaseClassSyntaxReader : SyntaxWalkerWithSemantic
    {
        private Class _subClass;
        
        public BaseClassSyntaxReader(Class subClass, SemanticModel semantic) : base(semantic)
        {
            _subClass = subClass;
        }

        public override void VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
            var baseClassType = node.Type;
            var baseClassSymbolInfo = _semantic.GetSymbolInfo(baseClassType);
            if (baseClassSymbolInfo.Symbol != null)
            {
                var baseClassTypeSymbol = (ITypeSymbol)baseClassSymbolInfo.Symbol;
                // skip interfaces as base types (since they do not have an implementation
                // their methods will always be overridden by the mixins)
                if (baseClassTypeSymbol.TypeKind == TypeKind.Interface)
                    return;
                var classFactory = new ClassFactory(_semantic);
                _subClass.BaseClass = classFactory.Create(baseClassTypeSymbol);
            }
        }
    }
}
