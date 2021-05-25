using GamesLibrary.Models;

namespace uwpPlatformer.Systems
{
    public interface ISystem
    {
        void Update(TimingInfo timingInfo);
    }

    public interface ISystem<T> : ISystem
    {
        string Name { get; }
    }
}
