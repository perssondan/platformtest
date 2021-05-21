using System;
using uwpPlatformer.GameObjects;

namespace uwpPlatformer.Components
{
    public interface IComponent : IDisposable
    {
        GameObject GameObject { get; set; }
    }
}