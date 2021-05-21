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
        public void Subscribe<TArgument>(object observer, Action<object, TArgument> onEventTriggered)
        {
            if (!_observers.TryGetValue(typeof(TArgument), out var list))
            {
                list = new List<ObserverInfo>();
                _observers.Add(typeof(TArgument), list);
            }

            list.Add(new ObserverInfo<TArgument> { Observer = observer, ObserverEvent = onEventTriggered });
        }

        /// <inheritdoc />
        public void Send<TArgument>(object sender, TArgument eventInstance)
        {
            if (!_observers.TryGetValue(typeof(TArgument), out var list)) return;

            foreach (var observerInfo in list.OfType<ObserverInfo<TArgument>>())
            {
                observerInfo.ObserverEvent?.Invoke(sender, eventInstance);
            }
        }

        /// <inheritdoc />
        public void Unsubscribe<TArgument>(object observer, TArgument type = default)
        {
            if (!_observers.TryGetValue(typeof(TArgument), out var list)) return;

            var observersToRemove = list
                .Where(observerInfo => observerInfo.Observer == observer)
                .ToArray();

            observersToRemove.ForEach(observerToRemove => list.Remove(observerToRemove));
        }

        /// <inheritdoc />
        public void Unsubscribe(object observer)
        {
            _observers.Keys.ForEach(key => Unsubscribe(observer, key));
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
