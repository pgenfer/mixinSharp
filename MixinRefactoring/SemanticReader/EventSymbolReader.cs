using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace MixinRefactoring
{
    public class EventSymbolReader : SemanticTypeReaderBase
    {
        private readonly IEventList _events;
        public EventSymbolReader(IEventList events)
        {
            _events = events;
        }

        protected override void ReadSymbol(IEventSymbol eventSymbol)
        {
            var @event = new Event(eventSymbol.Type, eventSymbol.Name)
            {
                IsAbstract = eventSymbol.IsAbstract,
                IsOverride = eventSymbol.IsOverride,
                IsInternal = eventSymbol.DeclaredAccessibility.HasFlag(Accessibility.Internal),
                Documentation = new DocumentationComment(eventSymbol.GetDocumentationCommentXml())
            };
            _events.AddEvent(@event);
        }
    }
}
