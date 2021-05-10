using System;
using uwpKarate.GameObjects;

namespace uwpKarate.Systems
{
    public interface ISystem
    {
        void Update(World world, TimeSpan deltaTime);
    }

    public interface ISystem<T> : ISystem
    {
        string Name { get; }
    }
}