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
    /// A composite command that includes a mixin to a child class by generating
    /// the mixin from an interface of the child class.
    /// A reference field to the mixin instance will also be created in the child class.
    /// </summary>
    public class CreateMixinFromBaseClass : CompositeCommand
    {
        public CreateMixinFromBaseClass(
            SimpleBaseTypeSyntax baseType,
            SemanticModel semantic):
            base(new MixinReferenceFactory(semantic).Create(baseType))
        {
        }

        protected override IMixinCommand[] CreateCommands(MixinReference mixin)
        {
            return new IMixinCommand[]
            {
                new AddFieldDeclarationForMixinCommand(mixin),
                new IncludeMixinCommand(mixin),
                new AddMixinToBaseListCommand(mixin),
                new InjectMixinsIntoChildCommand(mixin),
            };
        }
    }
}
