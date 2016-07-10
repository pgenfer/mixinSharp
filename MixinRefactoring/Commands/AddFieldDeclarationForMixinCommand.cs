using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MixinRefactoring
{
    /// <summary>
    /// generates a field reference in the child class
    /// that points to the mixin
    /// </summary>
    public class AddFieldDeclarationForMixinCommand : MixinCommandBase
    {
        public AddFieldDeclarationForMixinCommand(MixinReference mixin) : base(mixin)
        {
        }

        public override bool CanExecute(
            ClassWithSourceCode childClass, 
            Settings settings = null)
        {
            // ensure that the child class does not already have a 
            // reference to the mixin with the given name
            var mixinField = childClass.SourceCode.FindMixinReference(_mixin.Name);
            return mixinField == null;
        }

        protected override ClassDeclarationSyntax InternalExecute(
            ClassWithSourceCode childClass, 
            SemanticModel semantic, 
            Settings settings = null)
        {
            // create a field declaration and add it to the child class
            // check the following cases:
            // 1. if mixin type is a concrete type that has a parameterless constructor
            //    if injection setting is not set => init field with new instance of type
            // 2. if mixin is interface or does not have a parameterless constructor
            //    do nothing
            var positionOfClassInSourceFile = childClass.SourceCode.GetLocation().SourceSpan.Start;
            var typeSyntax = ParseTypeName(
                _mixin.Class.TypeSymbol.ReduceQualifiedTypeName(semantic,positionOfClassInSourceFile));
            var newFieldDeclaration =
                FieldDeclaration(
                    VariableDeclaration(typeSyntax)
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(_mixin.Name))))
                .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)));

            var newClassDeclaration = childClass.SourceCode.AddMembers(newFieldDeclaration);

            return newClassDeclaration;
        }
    }
}
