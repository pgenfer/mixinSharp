using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace MixinRefactoring
{
    public class ParameterList : IParameterList, IEnumerable<Parameter>
    {
        private readonly List<Parameter> _parameters = new List<Parameter>();
        public void Add(Parameter parameter) => _parameters.Add(parameter);
        public IEnumerator<Parameter> GetEnumerator() => _parameters.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _parameters.GetEnumerator();
        public int ParameterCount => _parameters.Count;
        public Parameter GetParameter(int index) => _parameters[index];
        public override string ToString() => string.Join(",", _parameters.Select(x => $"{x.Type.ToString()} {x.Name}"));
        public IEnumerable<Parameter> Parameters => this;
    }
}