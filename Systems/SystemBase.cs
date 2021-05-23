using System;

namespace uwpPlatformer.Systems
{
    public abstract class SystemBase<T> : ISystem<T>
    {
        protected SystemBase()
        {
        }

        public string Name => typeof(T).Name;

        public abstract void Update(TimeSpan deltaTime);
    }
}