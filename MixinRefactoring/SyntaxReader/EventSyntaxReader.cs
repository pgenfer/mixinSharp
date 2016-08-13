using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MixinRefactoring
{
    /// <summary>
    /// reads event syntax from a child class and stores
    /// them in an event object list
    /// </summary>
    public class EventSyntaxReader : SyntaxWalkerWithSemantic
    {
        private readonly IEventList _events;

        public EventSyntaxReader(IEventList events, SemanticModel semantic) : base(semantic)
        {
            _events = events;
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            var eventDeclaration = node.Declaration;
            var @event = new Event(
                 (ITypeSymbol)_semantic.GetSymbolInfo(eventDeclaration.Type).Symbol,
                 eventDeclaration.Variables.ToString())
            {
                IsOverride = node.Modifiers.Any(x => x.IsKind(SyntaxKind.OverrideKeyword)),
                IsAbstract = node.Modifiers.Any(x => x.IsKind(SyntaxKind.AbstractKeyword))
            };
            _events.AddEvent(@event);
        }
    }
}
