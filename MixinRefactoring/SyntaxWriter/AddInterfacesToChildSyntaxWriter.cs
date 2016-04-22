using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MixinRefactoring
{
    public class AddInterfacesToChildSyntaxWriter : CSharpSyntaxRewriter
    {
        private readonly MixinReference _mixin;
        private bool _hasInterfaceList = false;

        public AddInterfacesToChildSyntaxWriter(MixinReference mixin)
        {
            _mixin = mixin;
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var classDeclaration = (ClassDeclarationSyntax)base.VisitClassDeclaration(node);
            if (!_hasInterfaceList)
                classDeclaration = classDeclaration
                    .AddBaseListTypes(_mixin.Class.Interfaces
                    .Select(x => SimpleBaseType(IdentifierName(x.Name)))
                    .ToArray());
            return classDeclaration;
        }

        public override SyntaxNode VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
            foreach (var @interface in _mixin.Class.Interfaces)
            {
                if (node.Type.GetText().ToString() == @interface.Name)
                {
                    _hasInterfaceList = true;
                }
            }
            return node;
        }

        public override SyntaxNode VisitBaseList(BaseListSyntax node)
        {
            _hasInterfaceList = true;
            BaseListSyntax result = node;
            foreach (var @interface in _mixin.Class.Interfaces)
            {
                if (!node.Types.Any(x => x.Type.GetText().ToString() == @interface.Name))
                {
                    result = result.AddTypes(SimpleBaseType(IdentifierName(@interface.Name)));
                    
                }
            }
            return result;
        }
    }
}
