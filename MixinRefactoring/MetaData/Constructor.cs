using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{

    public class Constructor : IParameterList
    {
        private readonly IParameterList _parameters = new ParameterList();
        public int ParameterCount => _parameters.ParameterCount;
        public IEnumerable<Parameter> Parameters => _parameters.Parameters;
        public void Add(Parameter parameter) => _parameters.Add(parameter);
        public Parameter GetParameter(int index) => _parameters.GetParameter(index);
    }

    public interface IConstructorList
    {
        void AddConstructor(Constructor constructor);
        bool HasConstructor { get; }
        /// <summary>
        /// returns true if the given parameter is implemented as constructor argument
        /// in all constructors of this class
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        bool AllConstructorsHaveParameter(string parameterName);
    }

    public class ConstructorList : IConstructorList
    {
        private readonly List<Constructor> _constructors = new List<Constructor>(); 

        public void AddConstructor(Constructor constructor) =>_constructors.Add(constructor);
        public bool HasConstructor => _constructors.Any();
        public bool AllConstructorsHaveParameter(string parameterName)
            => _constructors.All(x => x.Parameters.Any(y => y.Name == parameterName));
    }
    
}
