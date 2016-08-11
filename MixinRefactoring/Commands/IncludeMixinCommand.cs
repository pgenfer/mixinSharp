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
    /// </summary>
    public class IncludeMixinCommand : MixinCommandBase
    {
        private Mixer _mixer = null;

        public IncludeMixinCommand(MixinReference mixin):base(mixin)
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
