using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    /// <summary>
    /// index properties can also have a parameterlist, also
    /// their name is not relevant, so their meta name, which is "Item" is used
    /// </summary>
    public class IndexerProperty : Property, IParameterList
    {
        private ParameterList _parameters = new ParameterList();
        
        public IndexerProperty(
            ITypeSymbol type, 
            bool hasGetter, 
            bool hasSetter):
            base("Item",type,hasGetter, hasSetter)
        {
        }

        public int ParameterCount => _parameters.ParameterCount;

        public IEnumerable<Parameter> Parameters => _parameters.Parameters;
        public void Add(Parameter parameter) => _parameters.Add(parameter);
        public IEnumerator<Parameter> GetEnumerator() => _parameters.GetEnumerator();
        public Parameter GetParameter(int index) => _parameters.GetParameter(index);
        public override string ToString() => $"{Type.ToString()} this[{_parameters.ToString()}]";

        protected override Member CreateCopy()
        {
            var copy = new IndexerProperty(Type, HasGetter, HasSetter);
            foreach (var parameter in Parameters)
                copy.Add(new Parameter(parameter.Name, parameter.Type));
            return copy;                
        }
    }
}
