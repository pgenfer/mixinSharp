using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring
{
    /// <summary>
    /// extends a given class declaration by adding/extending constructors 
    /// with an injected mixin instance
    /// </summary>
    public class ConstructorInjectionSyntaxWriter : CSharpSyntaxRewriter
    {
        private readonly MixinReference _mixin;
        private readonly SemanticModel _semantic;
       
        private bool _SourceClassHasConstructor;
        private InjectConstructorImplementationStrategy _injectMixinIntoConstructor;


        public ConstructorInjectionSyntaxWriter(
            MixinReference mixin,
            SemanticModel semanticModel)
        {
            _mixin = mixin;
            _semantic = semanticModel;
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // needed to evaluate whether type names can be reduced (depends on the using statements in the file)
            var positionOfClassInSourceFile = node.GetLocation().SourceSpan.Start;
            // strategy to implement constructor injection
            _injectMixinIntoConstructor = new InjectConstructorImplementationStrategy(
                _mixin, _semantic, positionOfClassInSourceFile);

            // base handling will call other visitors
            var classDeclaration = (ClassDeclarationSyntax)base.VisitClassDeclaration(node);

            // create a new constructor and add it to the class declaration
            if (!_SourceClassHasConstructor)
                classDeclaration = classDeclaration.AddMembers(
                    _injectMixinIntoConstructor.CreateNewConstructor(classDeclaration.Identifier.Text));

            return classDeclaration;
        }

        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            node = (ConstructorDeclarationSyntax)base.VisitConstructorDeclaration(node);
            if(!node.IsStatic())  // ignore static constructors
                node = _injectMixinIntoConstructor.ExtendExistingConstructor(node);
            // remember that class already has a constructor,
            // so no need to create a new one
            _SourceClassHasConstructor = true;
            return node;
        }

        public override SyntaxNode VisitConstructorInitializer(ConstructorInitializerSyntax node)
        {
            node = (ConstructorInitializerSyntax)base.VisitConstructorInitializer(node);
            node = _injectMixinIntoConstructor.ExtendConstructorInitialization(node);
            return node;
        }
    }
}
