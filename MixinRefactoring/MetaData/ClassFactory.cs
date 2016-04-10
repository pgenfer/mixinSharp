using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring
{
    /// <summary>
    /// can create class objects either from plain source code
    /// or from symbol information. 
    /// Even when source code is read, symbol information is needed
    /// to resolve the base class.
    /// Furhter base classes will be resolved recursivly.
    /// </summary>
    public class ClassFactory
    {
        private readonly SemanticModel _semantic;

        public ClassFactory(SemanticModel semantic)
        {
            _semantic = semantic;
        }

        public ClassWithSourceCode Create(ClassDeclarationSyntax classDeclaration)
        {
            var @class = new ClassWithSourceCode(classDeclaration);
            @class.Name = classDeclaration.Identifier.ToString();
            var propertyReader = new PropertySyntaxReader(@class, _semantic);
            propertyReader.Visit(classDeclaration);
            var methodReader = new MethodSyntaxReader(@class, _semantic);
            methodReader.Visit(classDeclaration);
            var baseClassReader = new BaseClassSyntaxReader(@class, _semantic);
            baseClassReader.Visit(classDeclaration);
            // we could skip this if inject mixin option is not set
            var constructorCountReader = new CountConstructorReader();
            constructorCountReader.Visit(classDeclaration);
            @class.HasConstructor = constructorCountReader.HasConstructor;

            return @class;
        }

        public ClassWithTypeSymbol Create(ITypeSymbol classSymbol)
        {
            var @class = new ClassWithTypeSymbol(classSymbol);
            @class.Name = classSymbol.Name;
            var propertyReader = new PropertySymbolReader(@class);
            propertyReader.VisitSymbol(classSymbol);
            var methodReader = new MethodSymbolReader(@class);
            methodReader.VisitSymbol(classSymbol);
            // search base classes until we reach System.Object where we stop
            if (classSymbol.BaseType != null && 
                classSymbol.BaseType.SpecialType != SpecialType.System_Object)
                @class.BaseClass = Create(classSymbol.BaseType);
            return @class;
        }
    }
}
