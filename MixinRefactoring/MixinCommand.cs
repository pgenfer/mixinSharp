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
        private readonly ClassWithSourceCode _mixinChild;
        private readonly MixinReference _mixin;
        private Mixer _mixer = null;
        public MixinCommand(SemanticModel model, FieldDeclarationSyntax mixinFieldDeclaration)
            :this(
                 new ClassFactory(model).Create((ClassDeclarationSyntax)mixinFieldDeclaration.Parent), 
                 new MixinReferenceFactory(model).Create(mixinFieldDeclaration))
        {
            // code is now in constructor initialization
            //var classDeclaration = mixinFieldDeclaration.Parent as ClassDeclarationSyntax;
            //_mixin = new MixinReferenceFactory(model).Create(mixinFieldDeclaration);
            //_mixinChild = new ClassFactory(model).Create(classDeclaration);
        }

        public MixinCommand(ClassWithSourceCode mixinChild, MixinReference mixin)
        {
            _mixinChild = mixinChild;
            _mixin = mixin;
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
        public bool CanExecute(Settings settings = null)
        {
            if (_mixin == null || _mixinChild == null)
                return false;
            // do the mixin operation the first time
            if (_mixer == null)
            {
                _mixer = new Mixer();
                _mixer.IncludeMixinInChild(_mixin, _mixinChild);
            }

            // command can be executed if we either have to forward members or extend a constructor
            return _mixer.MembersToImplement.Any() || NeedsConstructorExtension(settings);
        }

        /// <summary>
        /// checks if a constructor must be extended or created for the
        /// mixin child (in case the InjectMixin option is set)
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public bool NeedsConstructorExtension(Settings settings)
        {
            if (settings == null || !settings.InjectMixins)
                return false;
            return !(_mixinChild.AllConstructorsHaveParameter(_mixin.Name.ConvertFieldNameToParameterName()) &&
                  _mixinChild.HasConstructor);
        }

        /// <summary>
        /// creates the new syntax node with the added members.
        /// Will check before if the mixin can be executed,
        /// if no execution is possible, the original
        /// class declaration will be returned.
        /// </summary>
        /// <returns></returns>
        public ClassDeclarationSyntax Execute(SemanticModel semantic, Settings settings = null)
        {
            if (CanExecute(settings))
            {
                var classDeclaration = _mixinChild.SourceCode;
                var positionInSource = classDeclaration.GetLocation().SourceSpan.Start;
                // create constructors if necessary
                if (settings != null && settings.InjectMixins)
                {
                    var constructorSyntaxWriter = new ConstructorInjectionSyntaxWriter(_mixin, semantic);
                    classDeclaration = (ClassDeclarationSyntax)constructorSyntaxWriter.Visit(classDeclaration);
                }

                var syntaxWriter = new IncludeMixinSyntaxWriter(
                    _mixer.MembersToImplement, 
                    _mixin,
                    semantic,
                    settings);
                classDeclaration = (ClassDeclarationSyntax)syntaxWriter.Visit(classDeclaration);

                if(settings != null && settings.AddInterfacesToChild)
                {

                    var interfaceWriter = new AddInterfacesToChildSyntaxWriter(_mixin, semantic, positionInSource);
                    classDeclaration = (ClassDeclarationSyntax)interfaceWriter.Visit(classDeclaration);
                }
                
                return classDeclaration;
            }

            return _mixinChild.SourceCode;
        }
    }
}