using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace MixinRefactoring
{
    public class PropertySymbolReader : SemanticTypeReaderBase
    {
        private readonly IPropertyList _properties;
        public PropertySymbolReader(IPropertyList properties)
        {
            _properties = properties;
        }

        protected override void ReadSymbol(IPropertySymbol propertySymbol)
        {
            // we don't need to know about static members
            // because they won't be delegated from child to mixin
            if (propertySymbol.IsStatic)
                return;

            // we ignore private and protected memebers
            var hasGetter = propertySymbol.GetMethod != null &&
                      !(propertySymbol.GetMethod.DeclaredAccessibility == Accessibility.Private ||
                        propertySymbol.GetMethod.DeclaredAccessibility == Accessibility.Protected);
            var hasSetter = propertySymbol.SetMethod != null &&
                      !(propertySymbol.SetMethod.DeclaredAccessibility == Accessibility.Private ||
                        propertySymbol.SetMethod.DeclaredAccessibility == Accessibility.Protected);

            // property has no accessors or accessors are not accessible => skip property
            if (!hasSetter && !hasGetter)
                return;

            Property property = null;

            if (propertySymbol.IsIndexer) // symbol is an indexer property
            {
                var indexerProperty = new IndexerProperty(
                    propertySymbol.Type,
                    hasGetter,
                    hasSetter);
                var parameterReader = new ParameterSymbolReader(indexerProperty);
                parameterReader.VisitSymbol(propertySymbol);
                property = indexerProperty;
            }
            else // symbol is a normal property
            {
                property = new Property(
                    propertySymbol.Name,
                    propertySymbol.Type,
                    hasGetter,
                    hasSetter);
            }
            property.IsAbstract = propertySymbol.IsAbstract;
            property.IsOverride = propertySymbol.IsOverride;

            // store information if accessors are internal,
            // we will need this for the generation later
            property.IsGetterInternal = hasGetter &&
                                        propertySymbol.GetMethod.DeclaredAccessibility == 
                                        Accessibility.Internal;
            property.IsSetterInternal = hasSetter &&
                                        propertySymbol.SetMethod.DeclaredAccessibility == 
                                        Accessibility.Internal;
                    
            property.Documentation = new DocumentationComment(propertySymbol.GetDocumentationCommentXml());

            _properties.AddProperty(property);
        }
    }
}
