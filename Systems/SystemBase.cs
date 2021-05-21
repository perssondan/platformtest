using System;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Systems
{
    public abstract class SystemBase<T> : ISystem<T>
        where T : new()
    {
        protected SystemBase()
        {
            Initialize();
        }

        public string Name => typeof(T).Name;

        public static T Instance { get; } = new T();

        public abstract void Update(TimeSpan deltaTime);

        protected virtual void Initialize()
        {
        }
    }
}