using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public class ImplementedMethodService : MethodServiceBase, IImplementedMixinService
    {
        public ImplementedMethodService(
            MethodDeclarationSyntax method,
            SemanticModel semanticModel) :
            base(method.Identifier.ToString())
        {
            ReturnType = method.ReturnType.ToTypeSymbol(semanticModel);
            _parameters.AddRange(method.ParameterList.Parameters
                .Select(x =>
                    new Parameter(
                        x.Identifier.ToString(),
                        x.Type.ToTypeSymbol(semanticModel))));
        }

        public bool IsImplementationOf(IMixinService service) => service.IsImplementedBy(this);
    }
}
