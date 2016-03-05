using System.Collections.Generic;

namespace MixinRefactoring
{
    public interface IParameterList
    {
        void Add(Parameter parameter);
        int ParameterCount
        {
            get;
        }

        IEnumerable<Parameter> Parameters { get; }

        Parameter GetParameter(int index);
    }
}