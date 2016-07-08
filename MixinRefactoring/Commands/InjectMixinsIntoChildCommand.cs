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
    /// command that adds the constructor injection code
    /// of the mixins into the child class
    /// </summary>
    public class InjectMixinsIntoChildCommand : MixinCommandBase
    {
        public InjectMixinsIntoChildCommand(MixinReference mixin) : base(mixin)
        {
        }

        public override bool CanExecute(ClassWithSourceCode childClass, Settings settings = null)
        {
            if (settings == null || !settings.InjectMixins)
                return false;
            return !(childClass.AllConstructorsHaveParameter(_mixin.Name.ConvertFieldNameToParameterName()) &&
                  childClass.HasConstructor);
        }

        protected override ClassDeclarationSyntax InternalExecute(
            ClassWithSourceCode childClass, 
            SemanticModel semantic, 
            Settings settings = null)
        {
            var constructorSyntaxWriter = new ConstructorInjectionSyntaxWriter(_mixin, semantic);
            var classDeclaration = (ClassDeclarationSyntax)constructorSyntaxWriter.Visit(childClass.SourceCode);
            return classDeclaration;
        }
    }
}
