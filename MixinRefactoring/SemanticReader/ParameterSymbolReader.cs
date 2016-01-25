using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public class ParameterSymbolReader : SemanticTypeReaderBase
    {
        private readonly IParameterList _parameters;

        public ParameterSymbolReader(IParameterList parameters)
        {
            _parameters = parameters;
        }

        protected override void ReadSymbol(IParameterSymbol parameter)
        {
            _parameters.Add(new Parameter(parameter.Name, parameter.Type));
        }
    }
}
