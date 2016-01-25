using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public class MixinReferenceFactory
    {
        private readonly SemanticModel _semantic;

        public MixinReferenceFactory(SemanticModel semantic)
        {
            _semantic = semantic;
        }

        public MixinReference Create(FieldDeclarationSyntax mixinFieldDeclaration)
        {
            var fieldSyntaxReader = new FieldSyntaxReader(_semantic);
            fieldSyntaxReader.Visit(mixinFieldDeclaration);
            return fieldSyntaxReader.MixinReference;
        }
    }
}
