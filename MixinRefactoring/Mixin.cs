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
    /// Mixin is the class that contains methods and properties
    /// that can be "mixed" into other classes. These members
    /// that can be used by child classes are called "Services".
    /// Also the process of adding a mixin to a child class is
    /// called "Including"
    /// </summary>
    public class Mixin
    {
        private readonly List<IMixinService> _services = new List<IMixinService>();
        // the mixin must know its name (the name of the field that defines the mixin)
        // because we must reference it during code generation
        private readonly string _mixinName;
        public Mixin(string mixinName, ITypeSymbol mixinType)
        {
            _mixinName = mixinName;

            _services.AddRange(mixinType.GetMembers()
                .OfType<IPropertySymbol>()
                .Select(x => new PropertyService(x)));
            _services.AddRange(mixinType.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(x => x.MethodKind == MethodKind.Ordinary)
                .Select(x => new MethodService(x)));
        }
        
        public IEnumerable<IMixinService> Services { get { return _services; } }

        public override string ToString()
        {
            return string.Format("{0} (mixin)", _mixinName);
        }

        public string Name => _mixinName;
    }

    /// <summary>
    /// Factory used to create mixins from a field declaration in code.
    /// Example:
    /// private Engine _engine; // => a field definition of engine
    /// </summary>
    public class MixinFactory
    {
        private readonly SemanticModel _semanticModel;

        public MixinFactory(SemanticModel semanticCodeModel)
        {
            _semanticModel = semanticCodeModel;
        }

        public Mixin FromFieldDeclaration(FieldDeclarationSyntax fieldDeclarationNode)
        {
            try
            {
                foreach (var variable in fieldDeclarationNode.Declaration.Variables)
                {
                    var field = _semanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;
                    // TODO: add additional check here 
                    // to avoid mixins of standard data types etc...
                    if (field != null)
                    {
                        // a declaration could have more variables,
                        // but they would all be of the same type,
                        // therefore it does not make sense to create more than one mixin
                        return new Mixin(variable.Identifier.ToString(), field.Type);
                    }
                }
            }
            catch(Exception)
            {
                // TODO: think about correct error handling here
                return null;
            }
            // TODO: think about correct error handling here
            return null;
        }
    }
}
