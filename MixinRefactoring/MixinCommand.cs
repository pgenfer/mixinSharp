using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace MixinRefactoring
{
    /// <summary>
    ///  Encapsulates the mixin process.
    /// </summary>
    public class MixinCommand
    {
        private readonly SemanticModel _semanticModel;
        private readonly ClassWithSourceCode _mixinChild;
        private readonly MixinReference _mixin;
        private Mixer _mixer = null;
        public MixinCommand(SemanticModel model, FieldDeclarationSyntax mixinFieldDeclaration)
        {
            _semanticModel = model;
            var classDeclaration = mixinFieldDeclaration.Parent as ClassDeclarationSyntax;
            _mixin = new MixinReferenceFactory(model).Create(mixinFieldDeclaration);
            _mixinChild = new ClassFactory(model).Create(classDeclaration);
        }

        /// <summary>
        /// the name of field that is used to generate the mixin
        /// </summary>
        public string MixinFieldName => _mixin?.Name;
        /// <summary>
        /// the source class declaration syntax of the child class
        /// </summary>
        public ClassDeclarationSyntax OriginalChildSource => _mixinChild?.SourceCode;
        /// <summary>
        /// the mixin class that will be included in the child
        /// </summary>
        public MixinReference Mixin => _mixin;
        /// <summary>
        /// Checks whether a mixin can be executed.
        /// All parameters for the mixin must be valid and
        /// the mixin must have any members that could be added to the child
        /// </summary>
        /// <returns></returns>
        public bool CanExecute()
        {
            if (_mixin == null || _mixinChild == null)
                return false;
            // do the mixin operation the first time
            if (_mixer == null)
            {
                _mixer = new Mixer();
                _mixer.IncludeMixinInChild(_mixin, _mixinChild);
            }

            return _mixer.MembersToImplement.Any();
        }

        /// <summary>
        /// creates the new syntax node with the added members.
        /// Will check before if the mixin can be executed,
        /// if no execution is possible, the original
        /// class declaration will be returned.
        /// </summary>
        /// <returns></returns>
        public ClassDeclarationSyntax Execute(Settings settings = null)
        {
            if (CanExecute())
            {
                var syntaxWriter = new IncludeMixinSyntaxWriter(
                    _mixer.MembersToImplement, 
                    _mixin.Name, 
                    _semanticModel,
                    settings);
                var newClassDeclaration = (ClassDeclarationSyntax)syntaxWriter.Visit(_mixinChild.SourceCode);
                return newClassDeclaration;
            }

            return _mixinChild.SourceCode;
        }
    }
}