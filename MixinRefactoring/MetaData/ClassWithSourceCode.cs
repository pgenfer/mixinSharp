using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring
{
    /// <summary>
    /// this class contains also its syntax node
    /// which was used to create it. It can be used
    /// to extend the syntax later with delegation methods
    /// </summary>
    public class ClassWithSourceCode : Class, IConstructorList
    {
        /// <summary>
        /// class also stores constructor so we can check if they have to be extended
        /// </summary>
        private readonly IConstructorList _constructors = new ConstructorList();

        public ClassDeclarationSyntax SourceCode
        {
            get;
        }

        /// <summary>
        /// This constructor is only for test cases, don't use it in production code
        /// </summary>
        public ClassWithSourceCode()
        { }

        public ClassWithSourceCode(ClassDeclarationSyntax classDeclaration)
        {
            SourceCode = classDeclaration;
        }
        
        public void AddConstructor(Constructor constructor) => _constructors.AddConstructor(constructor);
        public virtual bool HasConstructor => _constructors.HasConstructor;
        public virtual bool AllConstructorsHaveParameter(string parameterName) => _constructors.AllConstructorsHaveParameter(parameterName);
    }
}