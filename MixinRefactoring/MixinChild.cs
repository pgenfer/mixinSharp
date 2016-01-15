using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    /// <summary>
    /// from definition, a mixin child is the class
    /// that contains other mixins
    /// </summary>
    public class MixinChild
    {
        private List<IImplementedMixinService> _implementedServices = new List<IImplementedMixinService>();
        private readonly string _name;
        private ClassDeclarationSyntax _classDeclarationNode;
    
        /// <summary>
        /// Create the mixin child by retrieving
        /// its type
        /// </summary>
        /// <param name="classDeclaration"></param>
        public MixinChild(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel)
        {
             _classDeclarationNode = classDeclaration;
            _name = classDeclaration.Identifier.ToString();

            SetServicesFromDeclaration(classDeclaration,semanticModel);
        }

        private void SetServicesFromDeclaration(
            ClassDeclarationSyntax classDeclaration,
            SemanticModel semanticModel)
        {
            _implementedServices.Clear();
            _implementedServices.AddRange(classDeclaration.Members
                .OfType<PropertyDeclarationSyntax>()
                .Select(x => new ImplementedPropertyService(x, semanticModel)));
            _implementedServices.AddRange(classDeclaration.Members
                .OfType<MethodDeclarationSyntax>()
                .Select(x => new ImplementedMethodService(x, semanticModel)));

            if (classDeclaration.BaseList != null)
            {
                // search for base class with implementation
                var baseClassSymbols =
                    from baseType in classDeclaration.BaseList.Types
                    select baseType.Type.ToTypeSymbol(semanticModel) into baseTypeSymbol
                    where baseTypeSymbol.TypeKind != TypeKind.Interface
                    select baseTypeSymbol;
                // add the properties from every base class 
                // (in fact there should only be one base class since we have single inheritance)
                foreach (var baseClass in baseClassSymbols)
                    SetServicesFromClassSymbol(baseClass, semanticModel);
            }
        }

        private void SetServicesFromClassSymbol(ITypeSymbol classType, SemanticModel semanticModel)
        {
            var baseClass = classType.BaseType;
            // first handle the base classes (skip all special types like object, array etc...)
            if (baseClass != null && baseClass.SpecialType == SpecialType.None)
                SetServicesFromClassSymbol(baseClass, semanticModel);
            // then add the properties of this class
            // in that way, base properties will be handled first
            var propertiesToImplement =
                from property in classType.GetMembers().OfType<IPropertySymbol>()
                select new ImplementedPropertyService(property) into propertyService
                where !IsImplemented(propertyService)
                select propertyService;
            _implementedServices.AddRange(propertiesToImplement);
        }

        /// <summary>
        /// checks if the given service is already implemented by this mixin child.
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        private bool IsImplemented(IMixinService service)
        {
            return _implementedServices.Any(x => x.IsImplementationOf(service));
        }

        private bool IsImplemented(PropertyServiceBase property)
        {
            return _implementedServices
                .OfType<PropertyServiceBase>()
                .Any(x => x.IsEqual(property));
        }

        private MemberDeclarationSyntax Implement(IMixinService service, string mixinName)
        {
            return service.ToMemberDeclaration(mixinName);
        }

        /// <summary>
        /// the process of adding a mixin to a child class.
        /// All services of the mixin will be implemented 
        /// in the child class by delegating them to the mixin
        /// </summary>
        /// <param name="mixin"></param>
        public ClassDeclarationSyntax Include(Mixin mixin)
        {
            var newMembers = mixin.Services
                .Where(x => !IsImplemented(x))
                .Select(x => Implement(x, mixin.Name))
                .ToArray();
            // create a new class declaration where the new members
            // were added
            var newClassDeclaration = _classDeclarationNode.AddMembers(newMembers);

            // this class declaration does not have a root,
            // but we need one otherwise we cannot create a
            // compilation unit (which we need for the semantic model
            // to resolve all type symbols correctly)
            // so we must get a copy of the original syntax tree where
            // the class declaration is replaced with the new one

            // create a new root with the replaced class declaration
            var newClassWithRoot = AddRootToNewClassDeclaration(newClassDeclaration);
            var syntaxTree = newClassWithRoot.SyntaxTree;
            // compile the syntax tree so we can access semantic information
            var compilationUnit = CSharpCompilation.Create("tmp",syntaxTrees:new[] { syntaxTree });

            SetServicesFromDeclaration(newClassWithRoot, compilationUnit.GetSemanticModel(syntaxTree));
            return newClassDeclaration;
        }

        /// <summary>
        /// takes the syntax tree of the inital class declaration and creates a new syntax tree
        /// where the old class declaration is replaced with the new one
        /// </summary>
        /// <param name="newClassDeclaration"></param>
        /// <returns></returns>
        private ClassDeclarationSyntax AddRootToNewClassDeclaration(ClassDeclarationSyntax newClassDeclaration)
        {
            var newRoot = _classDeclarationNode
                .SyntaxTree.GetRoot()
                .ReplaceNode(_classDeclarationNode, newClassDeclaration);
            var newClassDeclarationWithRoot = newRoot.FindClassByName(_name);
            return newClassDeclarationWithRoot;
        }

        public override string ToString() => string.Format("{0} (mixin child)", _name);
        public IEnumerable<IImplementedMixinService> Members => _implementedServices;
    }
}
