using Microsoft.CodeAnalysis;

namespace MixinRefactoring
{
    public class ClassWithTypeSymbol : Class
    {
        public ITypeSymbol TypeSymbol { get; }

        public virtual bool IsInterface => TypeSymbol.TypeKind == TypeKind.Interface;

        public virtual Interface AsInterface() => new Interface(TypeSymbol);

        /// <summary>
        /// only for test cases, should not be used during production code
        /// </summary>
        public ClassWithTypeSymbol() { }

        public ClassWithTypeSymbol(ITypeSymbol typeSymbol)
        {
            TypeSymbol = typeSymbol;
        }
    }
}