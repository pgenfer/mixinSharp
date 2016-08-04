using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public static class ParameterSyntaxExpression
    {
        /// <summary>
        /// checks if the given parameter syntax is part of a lambda expression
        /// (either a single one or a lambda parameter list).
        /// </summary>
        /// <param name="parameterSyntax"></param>
        /// <returns>
        /// true if parameter is within a lambda statement,
        /// otherwise false</returns>
        public static bool IsWithinLambda(this ParameterSyntax parameterSyntax)
        {
            if (parameterSyntax.Parent is SimpleLambdaExpressionSyntax)
                return true;
            var parentList = parameterSyntax.Parent as ParameterListSyntax;
            if (parentList != null)
                return parentList.Parent is ParenthesizedLambdaExpressionSyntax;
            return false;
        }
    }
}
