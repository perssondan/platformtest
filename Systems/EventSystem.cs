using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uwpPlatformer.Extensions;

namespace uwpPlatformer.Systems
{
    public class EventSystem : IEventSystem
    {
        private Dictionary<Type, List<EventRegistration>> _eventRegistrations = new Dictionary<Type, List<EventRegistration>>();

        protected class EventRegistration
        {
            public object Receiver { get; set; }
        }

        protected class EventRegistration<T> : EventRegistration
        {
            public Action<T> ActionEvent { get; set; }
        }

        public static EventSystem Instance { get; } = new EventSystem();

        public void Register<TEvent>(object receiver, Action<TEvent> onEventTriggered)
        {
            if (!_eventRegistrations.TryGetValue(typeof(TEvent), out var list))
            {
                list = new List<EventRegistration>();
                _eventRegistrations.Add(typeof(TEvent), list);
            }

            list.Add(new EventRegistration<TEvent> { Receiver = receiver, ActionEvent = onEventTriggered });
        }

        public void Send<TEvent>(TEvent eventInstance)
        {
            if (!_eventRegistrations.TryGetValue(typeof(TEvent), out var list)) return;

            foreach (var eventregistration in list.OfType<EventRegistration<TEvent>>())
            {
                eventregistration.ActionEvent?.Invoke(eventInstance);
            }
        }

        public void Unregister<TEvent>(object receiver)
        {
            if (!_eventRegistrations.TryGetValue(typeof(TEvent), out var list)) return;

            var itemsToRemove = list
                .Where(eventRegistration => eventRegistration.Receiver == receiver)
                .ToArray();

            itemsToRemove.ForEach(itemToRemove => list.Remove(itemToRemove));
        }
    }
}
