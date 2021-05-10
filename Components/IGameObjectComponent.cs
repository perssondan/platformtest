using System;
using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public interface IGameObjectComponent : IDisposable
    {
        GameObject GameObject { get; set; }
    }

    public interface IGameObjectComponent<T> : IGameObjectComponent
    {
        void Update(T target, TimeSpan timeSpan);
    }
}