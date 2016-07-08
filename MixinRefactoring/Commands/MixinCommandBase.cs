using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    /// <summary>
    /// base class for mixin operations
    /// </summary>
    public abstract class MixinCommandBase : IMixinCommand
    {
        /// <summary>
        /// a reference to the mixin that should be included in the child class
        /// </summary>
        protected readonly MixinReference _mixin;
       
        /// <summary>
        /// initializes the base fields of the command object
        /// </summary>
        /// <param name="mixinChild">child class where the mixin should be added</param>
        /// <param name="mixin">mixin that should be added to the child class</param>
        protected MixinCommandBase(MixinReference mixin)
        {
            _mixin = mixin;
        }

        /// <summary>
        /// the name of field that is used to generate the mixin
        /// </summary>
        public string MixinFieldName => _mixin?.Name;
        /// <summary>
        /// the mixin class that will be included in the child
        /// </summary>
        public MixinReference Mixin => _mixin;

        /// <summary>
        /// called to check if the command can be executed.
        /// </summary>
        /// <param name="settings">optional settings</param>
        /// <returns>true if the command can be executed, otherwise false</returns>
        public abstract bool CanExecute(
            ClassWithSourceCode childClass, 
            Settings settings = null);

        /// <summary>
        /// executes the command on the given child class.
        /// Logic of command execution must be implemented in derived classes.
        /// </summary>
        /// <param name="semantic">semantic model of the child's and mixin's source code</param>
        /// <param name="settings">optional settings object</param>
        /// <returns></returns>
        protected abstract ClassDeclarationSyntax InternalExecute(
            ClassWithSourceCode childClass,
            SemanticModel semantic, 
            Settings settings = null);

        /// <summary>
        /// Template method that checks if command can be executed and if yes,
        /// executes the command. Otherwise the original source code of the child is returned.
        /// </summary>
        /// <param name="semantic">semantic model of the child's and mixin's source code</param>
        /// <param name="settings">optional settings object</param>
        /// <returns>the modified class declaration of the child class after executing
        /// the command or the original child's source code if command could not be 
        /// executed</returns>
        public ClassDeclarationSyntax Execute(
            ClassDeclarationSyntax childDeclaration, 
            SemanticModel semantic, 
            Settings settings = null)
        {
            var childClass = new ClassFactory(semantic).Create(childDeclaration);
            if (CanExecute(childClass, settings))
                return InternalExecute(childClass, semantic, settings);
            return childClass.SourceCode;
        }
    }    
}
