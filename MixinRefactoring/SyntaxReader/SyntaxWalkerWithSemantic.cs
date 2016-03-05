using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public abstract class SyntaxWalkerWithSemantic : CSharpSyntaxWalker
    {
        protected readonly SemanticModel _semantic;
        protected SyntaxWalkerWithSemantic(SemanticModel semantic)
        {
            _semantic = semantic;
        }
    }
}
