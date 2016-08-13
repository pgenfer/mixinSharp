using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace MixinRefactoring
{
    /// <summary>
    /// representation of an event
    /// </summary>
    public class Event : Member
    {
        /// <summary>
        /// type of the event handler
        /// </summary>
        public ITypeSymbol EventType { get; }

        public Event(ITypeSymbol eventType, string eventName)
        {
            EventType = eventType;
            Name = eventName;
        }

        protected override Member CreateCopy() => new Event(EventType, Name);
    }
}
