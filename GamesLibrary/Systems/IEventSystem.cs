using System;

namespace GamesLibrary.Systems
{
    public interface IEventSystem
    {
        /// <summary>
        /// Subscribe to events of type <typeparamref name="TArgument"/>
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="observer"></param>
        /// <param name="onEventTriggered"></param>
        void Subscribe<TArgument>(object observer, Action<object, TArgument> onEventTriggered);

        /// <summary>
        /// Unsubscribe from a specific event, <typeparamref name="TArgument"/>
        /// </summary>
        /// <param name="observer"></param>
        void Unsubscribe<TArgument>(object observer);

        /// <summary>
        /// Unsubscrive from all events
        /// </summary>
        /// <param name="observer"></param>
        void Unsubscribe(object observer);

        /// <summary>
        /// Sends an event of type <typeparamref name="TArgument"/> to all subscribers
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="sender"></param>
        /// <param name="argument"></param>
        void Send<TArgument>(object sender, TArgument argument);
    }
}
