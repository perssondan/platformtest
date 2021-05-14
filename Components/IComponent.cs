using System;
using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public interface IComponent : IDisposable
    {
        GameObject GameObject { get; set; }
    }
}