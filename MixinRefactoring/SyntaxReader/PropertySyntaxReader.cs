using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections;

namespace MixinRefactoring
{
    public class PropertySyntaxReader : SyntaxWalkerWithSemantic
    {
        private readonly IPropertyList _properties;
        /// <summary>
        /// Creates a new property syntax reader to
        /// read the properties of a provided class declaration.
        /// All properties will be stored in the given list of properties
        /// </summary>
        /// <param name="semantic">Semantic Model is used
        /// to resolve type syntax into TypeSymbols.</param>
        /// <param name="propertyList">list where the found
        /// properties should be stored in</param>
        public PropertySyntaxReader(IPropertyList propertyList,SemanticModel semantic):base(semantic)
        {
            _properties = propertyList;
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var hasGetter = node.ExpressionBody != null || // properties with expression bodies do not have an accessorlist
                node.AccessorList.Accessors.Any(x => x.Kind() == SyntaxKind.GetAccessorDeclaration);
            var hasSetter = node.AccessorList != null &&
                node.AccessorList.Accessors.Any(x => x.Kind() == SyntaxKind.SetAccessorDeclaration);

            var property = new Property(
                node.Identifier.ToString(),
                (ITypeSymbol)_semantic.GetSymbolInfo(node.Type).Symbol,
                hasGetter, hasSetter);
            _properties.AddProperty(property);            
        }
    }    
}
