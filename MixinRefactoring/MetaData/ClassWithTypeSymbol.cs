using Microsoft.CodeAnalysis;

namespace MixinRefactoring
{
    public class ClassWithTypeSymbol : Class
    {
        public ITypeSymbol TypeSymbol { get; }

        public ClassWithTypeSymbol(ITypeSymbol typeSymbol)
        {
            TypeSymbol = typeSymbol;
        }
    }
}