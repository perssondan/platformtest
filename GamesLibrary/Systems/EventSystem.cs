using GamesLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GamesLibrary.Systems
{
    public class EventSystem : IEventSystem
    {
        private Dictionary<Type, List<ObserverInfo>> _observers = new Dictionary<Type, List<ObserverInfo>>();

        public static EventSystem Instance { get; } = new EventSystem();

        /// <inheritdoc />
        public void Subscribe<TEvent>(object observer, Action<object, TEvent> onEventTriggered)
        {
            if (!_observers.TryGetValue(typeof(TEvent), out var list))
            {
                list = new List<ObserverInfo>();
                _observers.Add(typeof(TEvent), list);
            }

            list.Add(new ObserverInfo<TEvent> { Observer = observer, ObserverEvent = onEventTriggered });
        }

        /// <inheritdoc />
        public void Send<TEvent>(object sender, TEvent eventInstance)
        {
            if (!_observers.TryGetValue(typeof(TEvent), out var list)) return;

            foreach (var observerInfo in list.OfType<ObserverInfo<TEvent>>())
            {
                observerInfo.ObserverEvent?.Invoke(sender, eventInstance);
            }
        }

        /// <inheritdoc />
        public void Unsubscribe<TEvent>(object observer)
        {
            if (!_observers.TryGetValue(typeof(TEvent), out var list)) return;

            var observersToRemove = list
                .Where(observerInfo => observerInfo.Observer == observer)
                .ToArray();

            observersToRemove.ForEach(observerToRemove => list.Remove(observerToRemove));
        }

        /// <inheritdoc />
        public void Unsubscribe(object observer)
        {
            _observers.ForEach(key =>
            {
                key.Value
                    .Where(observerInfo => observerInfo.Observer == observer)
                    .ToArray()
                    .ForEach(observerInfo => key.Value.Remove(observerInfo));
            });
        }

        protected class ObserverInfo
        {
            public object Observer { get; set; }
        }

        protected class ObserverInfo<TArgument> : ObserverInfo
        {
            public Action<object, TArgument> ObserverEvent { get; set; }
        }
    }
}
