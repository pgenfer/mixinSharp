using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring
{
    /// <summary>
    /// base class for commands composed of other commands.
    /// A composite command will execute all its commands
    /// sequentially and will create 
    /// </summary>
    public abstract class CompositeCommand : IMixinCommand
    {
        private readonly IMixinCommand[] _commands;

        /// <summary>
        /// reference to the mixin that is used by the command
        /// </summary>
        public MixinReference Mixin { get; }

        /// <summary>
        /// name of the command is used in the code refactoring context menu
        /// </summary>
        public abstract string Title { get; }
        
              
        protected CompositeCommand(MixinReference mixin)
        {            
            _commands = CreateCommands(mixin);
            Mixin = mixin;
        }

        /// <summary>
        /// must be implemented by derived classes. Should
        /// return all commands that are part of this composition.
        /// </summary>
        /// <returns>List of commands that are part of this composition in their correct order.</returns>
        protected abstract IMixinCommand[] CreateCommands(MixinReference mixin);
        

        public virtual bool CanExecute(ClassWithSourceCode childClass, Settings settings = null)
        {
            if (Mixin == null)
                return false;
            var canExecute = false;
            // check that at least one command is executable
            foreach (var command in _commands)
                canExecute = canExecute || command.CanExecute(childClass, settings);
            return canExecute;
        }

        public ClassDeclarationSyntax Execute(
            ClassDeclarationSyntax childDeclaration, 
            SemanticModel semantic, 
            Settings settings = null)
        {
            // we need the class name of the child to find it in the syntax tree
            // after it was changed
            var childClassName = childDeclaration.Identifier.Text;
            foreach (var command in _commands)
            {
                var oldSyntaxTree = childDeclaration.SyntaxTree;
                var newChildDeclaration = command.Execute(childDeclaration, semantic, settings);
                // replace the old class declaration with the new one in the syntax tree
                var newRoot = semantic.SyntaxTree.GetRoot().ReplaceNode(childDeclaration, newChildDeclaration);
                var newSyntaxTree = newRoot.SyntaxTree;
                var compilation = semantic.Compilation.ReplaceSyntaxTree(oldSyntaxTree,newSyntaxTree);
                // after creating the new syntax tree, get a new semantic model
                semantic = compilation.GetSemanticModel(newSyntaxTree);
                // also retrieve the class declaration from the new syntax tree
                childDeclaration = newRoot.FindClassByName(childClassName);
            }
            return childDeclaration;
        }
    }
}
