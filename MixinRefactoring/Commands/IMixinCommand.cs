using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring
{
    /// <summary>
    /// base interface for commands
    /// </summary>
    public interface IMixinCommand
    {
        bool CanExecute(
            ClassWithSourceCode childClass,
            Settings settings = null);

        /// <summary>
        /// executes the command by changing the given class declaration
        /// </summary>
        /// <param name="childDeclaration">class declaration of child class</param>
        /// <param name="semantic"></param>
        /// <param name="settings"></param>
        /// <returns>changed class declaration or original declaration if command
        /// could not be executed</returns>
        ClassDeclarationSyntax Execute(
            ClassDeclarationSyntax childDeclaration,
            SemanticModel semantic,
            Settings settings = null);
    }
}