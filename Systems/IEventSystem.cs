using System;

namespace uwpPlatformer.Systems
{
    public interface IEventSystem
    {
        void Register<TEventArgument>(object receiver, Action<TEventArgument> onEventTriggered);

        void Unregister<TEventArgument>(object receiver);

        void Send<TEventArgument>(TEventArgument eventInstance);
    }
}
