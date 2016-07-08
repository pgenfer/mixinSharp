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
        /// <summary>
        /// list of all interfaces from which the mixin class
        /// derives. These interfaces will be added to the base list
        /// of the child class
        /// </summary>
        private readonly InterfaceList _mixinInterfaces;
        /// <summary>
        /// this flag stores if the child class already
        /// has a base class list. If yes, the new interfaces will just
        /// be added to this list, otherwise the complete base list
        /// will be created
        /// </summary>
        private bool _hasInterfaceList = false;
        private readonly SemanticModel _semantic;
        private readonly int _positionOfChildClassInCode;

        public AddInterfacesToChildSyntaxWriter(
            MixinReference mixin,
            SemanticModel model,
            // needed to reduce type names depending on namespaces
            int positionOfChildClassInCode)
        {
            _mixinInterfaces = new InterfaceList();
            if (mixin.Class.IsInterface)
                _mixinInterfaces.AddInterface(mixin.Class.AsInterface());
            else
                foreach (var @interface in mixin.Class.Interfaces)
                    _mixinInterfaces.AddInterface(@interface);
            _semantic = model;
            _positionOfChildClassInCode = positionOfChildClassInCode;
        }

        protected string ReduceQualifiedTypeName(ITypeSymbol type)
        {
            return type.ReduceQualifiedTypeName(_semantic, _positionOfChildClassInCode);
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var classDeclaration = (ClassDeclarationSyntax)base.VisitClassDeclaration(node);

            // child class either has a base list already (it will be visited separatly)
            // or there are no interfaces to add, so nothing to do
            if (_hasInterfaceList || _mixinInterfaces.IsEmpty)
                return classDeclaration;

            classDeclaration = classDeclaration
                .AddBaseListTypes(_mixinInterfaces
                .Select(x => SimpleBaseType(IdentifierName(
                    x.GetReducedTypeName(_semantic, _positionOfChildClassInCode))))
                .ToArray());
            // there is always a trailing newline
            // when we add the base, so skip it by recreating the identifer
            // of the class
            classDeclaration = classDeclaration
                .WithIdentifier(Identifier(classDeclaration.Identifier.Text));
            var text = classDeclaration.GetText().ToString();

            return classDeclaration;
        }

        public override SyntaxNode VisitBaseList(BaseListSyntax node)
        {
            _hasInterfaceList = true;
            var remainingMixinInterfaces = _mixinInterfaces
                .Select(x => x.GetReducedTypeName(_semantic, _positionOfChildClassInCode))
                .Except(node.Types.Select(x => x.TypeName()))
                .Select(x => SimpleBaseType(IdentifierName(x)))
                .ToArray();
            var result = node.AddTypes(remainingMixinInterfaces);
            return result;
        }
    }
}
