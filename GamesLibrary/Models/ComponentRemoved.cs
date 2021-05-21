using GamesLibrary.Components;

namespace GamesLibrary.Models
{
    public struct ComponentRemoved
    {
        public ComponentRemoved(IComponent component)
        {
            Component = component;
        }

        public readonly IComponent Component;
    }
}
