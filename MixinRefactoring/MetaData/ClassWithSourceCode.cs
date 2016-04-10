using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring
{
    /// <summary>
    /// this class contains also its syntax node
    /// which was used to create it. It can be used
    /// to extend the syntax later with delegation methods
    /// </summary>
    public class ClassWithSourceCode : Class
    {
        public ClassDeclarationSyntax SourceCode
        {
            get;
        }

        public ClassWithSourceCode(ClassDeclarationSyntax classDeclaration)
        {
            SourceCode = classDeclaration;
        }

        /// <summary>
        /// flag that is set when the class has a constructor declaration,
        /// otherwise false
        /// </summary>
        public bool HasConstructor { get; set; }
    }
}