using GamesLibrary.Components;

namespace GamesLibrary.Models
{
    public struct ComponentAdded
    {
        public ComponentAdded(IComponent component)
        {
            Component = component;
        }

        public readonly IComponent Component;
    }
}
