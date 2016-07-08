using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring
{
    public class CompositeCommand : IMixinCommand
    {
        IMixinCommand[] _commands;
      
        public CompositeCommand(MixinReference mixin)
        {
            _commands = new IMixinCommand[]
            {
                new CreateMixinFromFieldDeclarationCommand(mixin),
                new AddMixinToBaseListCommand(mixin),
                new InjectMixinsIntoChildCommand(mixin),
            };
        }

        public bool CanExecute(ClassWithSourceCode childClass, Settings settings = null)
        {
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
            var childClassName = childDeclaration.Identifier.Text.ToString();
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
                childDeclaration = semantic.SyntaxTree.GetRoot().FindClassByName(childClassName);
            }
            return childDeclaration;
        }
    }
}
