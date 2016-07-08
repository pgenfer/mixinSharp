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
    /// the mixin and its interfaces will be added to the base class list
    /// of the child class
    /// </summary>
    public class AddMixinToBaseListCommand : MixinCommandBase
    {
        public AddMixinToBaseListCommand(MixinReference mixin) : base(mixin)
        {
        }


        public override bool CanExecute(ClassWithSourceCode childClass, Settings settings = null)
        {
            return settings != null && settings.AddInterfacesToChild;
        }

        protected override ClassDeclarationSyntax InternalExecute(
            ClassWithSourceCode childClass, 
            SemanticModel semantic, 
            Settings settings = null)
        {
            var classDeclaration = childClass.SourceCode;
            var positionInSource = classDeclaration.GetLocation().SourceSpan.Start;

            var interfaceWriter = new AddInterfacesToChildSyntaxWriter(_mixin, semantic, positionInSource);
            classDeclaration = (ClassDeclarationSyntax)interfaceWriter.Visit(classDeclaration);
            return classDeclaration;
        }
    }
}
