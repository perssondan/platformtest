using System;

namespace uwpKarate.Components
{
    public interface IGameObjectComponent
    {
    }

    public interface IGameObjectComponent<T> : IGameObjectComponent
    {
        void Update(T target, TimeSpan timeSpan);
    }
}