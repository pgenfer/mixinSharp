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
        private IPropertyList _properties;
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

            Property property = null;

            if (propertySymbol.IsIndexer) // symbol is an indexer property
            {
                var indexerProperty = new IndexerProperty(
                    propertySymbol.Type,
                    propertySymbol.GetMethod != null,
                    propertySymbol.SetMethod != null);
                var parameterReader = new ParameterSymbolReader(indexerProperty);
                parameterReader.VisitSymbol(propertySymbol);
                property = indexerProperty;                    
            }
            else // symbol is a normal property
            {
                property = new Property(
                    propertySymbol.Name,
                    propertySymbol.Type,
                    propertySymbol.GetMethod != null,
                    propertySymbol.SetMethod != null);
            }
                        
            _properties.AddProperty(property);
        }
    }
}
