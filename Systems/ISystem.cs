using GamesLibrary.Models;

namespace uwpPlatformer.Systems
{
    public interface ISystem
    {
        void Update(TimingInfo timingInfo);
        string Name { get; }
    }

    public interface ISystem<T> : ISystem
    {
    }
}
