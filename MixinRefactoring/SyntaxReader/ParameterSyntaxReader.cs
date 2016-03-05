using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace MixinRefactoring
{
    public class ParameterSyntaxReader : SyntaxWalkerWithSemantic
    {
        private readonly IParameterList _parameters;
        public ParameterSyntaxReader(IParameterList parameters, SemanticModel semantic) : base(semantic)
        {
            _parameters = parameters;
        }

        public override void VisitParameter(ParameterSyntax node)
        {
            // ignore parameters in lambdaexpressions
            if (node.Parent is SimpleLambdaExpressionSyntax)
                return;
            var parameter = new Parameter(
                node.Identifier.ToString(),
                (ITypeSymbol)_semantic.GetSymbolInfo(node.Type).Symbol);
            _parameters.Add(parameter);
        }
    }
}
