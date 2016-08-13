using System.Collections.Generic;
using System.Collections;

namespace MixinRefactoring
{
    public class PropertyList : IPropertyList, IEnumerable<Property>
    {
        private readonly List<Property> _properties = new List<Property>();
        public void AddProperty(Property newProperty) => _properties.Add(newProperty);
        public void AddProperties(IEnumerable<Property> properties) => _properties.AddRange(properties);
        public IEnumerator<Property> GetEnumerator() => _properties.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _properties.GetEnumerator();
        public int Count => _properties.Count;
        public Property this[int index] => _properties[index];
    }
}