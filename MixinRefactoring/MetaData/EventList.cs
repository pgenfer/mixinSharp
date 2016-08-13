using System.Collections;
using System.Collections.Generic;

namespace MixinRefactoring
{
    /// <summary>
    /// list of events within a class
    /// </summary>
    public class EventList : IEventList, IEnumerable<Event>
    {
        private readonly List<Event> _events = new List<Event>();
        public void AddEvent(Event newEvent) => _events.Add(newEvent);
        public IEnumerator<Event> GetEnumerator() => _events.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _events.GetEnumerator();
        public int Count => _events.Count;
        public Event this[int index] => _events[index];
    }
}