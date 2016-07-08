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
    /// includes a mixin in a child class.
    /// The mixin is identified by a field declaration within 
    /// the child class
    /// </summary>
    public class CreateMixinFromFieldDeclarationCommand : MixinCommandBase
    {
        private Mixer _mixer = null;

        /// <summary>
        /// creates the command object
        /// </summary>
        /// <param name="model">Semantic model of the source code containing the
        /// child class and the mixin</param>
        /// <param name="mixinFieldDeclaration">
        /// Field Declaration of the mixin within the child class
        /// </param>
        public CreateMixinFromFieldDeclarationCommand(MixinReference mixin):base(mixin)
        {            
        }

        public override bool CanExecute(ClassWithSourceCode childClass, Settings settings = null)
        {
            if (_mixin == null || childClass == null)
                return false;
            // do the mixin operation the first time
            if (_mixer == null)
            {
                _mixer = new Mixer();
                _mixer.IncludeMixinInChild(_mixin, childClass);
            }

            // command can be executed if we either have to forward members or extend a constructor
            return _mixer.MembersToImplement.Any();
        }

        protected override ClassDeclarationSyntax InternalExecute(
            ClassWithSourceCode childClass, 
            SemanticModel semantic, 
            Settings settings = null)
        {
            var syntaxWriter = new IncludeMixinSyntaxWriter(
                _mixer.MembersToImplement,
                _mixin,
                semantic,
                settings);
            var classDeclaration = (ClassDeclarationSyntax)syntaxWriter.Visit(childClass.SourceCode);

            return classDeclaration;
        }
    }
}
