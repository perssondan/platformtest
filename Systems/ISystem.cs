using System;

namespace uwpPlatformer.Systems
{
    public interface ISystem
    {
        void Update(TimeSpan deltaTime);
    }

    public interface ISystem<T> : ISystem
    {
        string Name { get; }
    }
}