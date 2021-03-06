﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    /// <summary>
    /// Composite command that includes a mixin into a child class.
    /// A reference to the mixin is already defined as a field in the child class.
    /// </summary>
    public class CreateMixinFromFieldDeclarationCommand : CompositeCommand
    {
        public CreateMixinFromFieldDeclarationCommand(
            FieldDeclarationSyntax mixinFieldDeclaration,
            SemanticModel semantic):
            this(new MixinReferenceFactory(semantic).Create(mixinFieldDeclaration))
        {
        }

        /// <summary>
        /// constructor that takes a mixin directly. Can be used for testing
        /// </summary>
        /// <param name="mixin"></param>
        public CreateMixinFromFieldDeclarationCommand(MixinReference mixin):base(mixin)
        { }

        public override string Title => $"Include mixin: '{Mixin.Name}'";

        protected override IMixinCommand[] CreateCommands(MixinReference mixin)
        {
            return new IMixinCommand[]
            {
                new IncludeMixinCommand(mixin),
                new AddMixinToBaseListCommand(mixin),
                new InjectMixinsIntoChildCommand(mixin),
            };
        }
    }
}
