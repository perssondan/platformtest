using GamesLibrary.Models;

namespace uwpPlatformer.Systems
{
    public interface ISystem
    {
        /// <summary>
        /// Gets the name of the system
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="timingInfo"></param>
        void Update(TimingInfo timingInfo);

        /// <summary>
        /// Init method.
        /// </summary>
        void Init();
    }

    public interface ISystem<T> : ISystem
    {
    }
}
