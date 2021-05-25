using GamesLibrary.Models;

namespace uwpPlatformer.Systems
{
    public abstract class SystemBase<T> : ISystem<T>
    {
        protected SystemBase()
        {
        }

        public string Name => typeof(T).Name;

        public abstract void Update(TimingInfo timingInfo);
    }
}
