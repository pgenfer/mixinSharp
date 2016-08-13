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
            var @class = new ClassWithSourceCode(classDeclaration)
            {
                Name = classDeclaration.Identifier.ToString()
            };

            // create readers for reading syntax members
            var syntaxReaders = new SyntaxWalkerWithSemantic[]
            {
                new PropertySyntaxReader(@class, _semantic),
                new MethodSyntaxReader(@class, _semantic),
                new EventSyntaxReader(@class, _semantic),
                new BaseClassSyntaxReader(@class, _semantic),
                new InterfaceSyntaxReader(@class.Interfaces, _semantic),
                new ConstructorSyntaxReader(@class, _semantic)
            };

            foreach (var syntaxReader in syntaxReaders)
                syntaxReader.Visit(classDeclaration);

            return @class;
        }

        public ClassWithTypeSymbol Create(ITypeSymbol classSymbol)
        {
            var @class = new ClassWithTypeSymbol(classSymbol) {Name = classSymbol.Name};

            // create readers for reading symbol members
            var symbolReaders = new SemanticTypeReaderBase[]
            {
                new PropertySymbolReader(@class),
                new MethodSymbolReader(@class),
                new EventSymbolReader(@class)
            };

            foreach (var symbolReader in symbolReaders)
                symbolReader.VisitSymbol(classSymbol);

            // search base classes until we reach System.Object where we stop
            if (classSymbol.BaseType != null && 
                classSymbol.BaseType.SpecialType != SpecialType.System_Object)
                @class.BaseClass = Create(classSymbol.BaseType);
            // add interfaces to interface list
            foreach (var @interface in classSymbol.Interfaces)
                @class.Interfaces.AddInterface(new Interface(@interface));
            return @class;
        }
    }
}
