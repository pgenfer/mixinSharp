using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public static class BaseTypeSyntaxExtension
    {
        /// <summary>
        /// returns the name of the given type
        /// without any leading or trailing whitespaces
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string TypeName(this BaseTypeSyntax node)
        {
            return node.Type.GetText().ToString().Trim();
        }
    }
}
