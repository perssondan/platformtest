using GamesLibrary.Entities;

namespace GamesLibrary.Components
{
    public struct TagComponent : IComponent
    {
        public TagComponent(string tag)
        {
            Tag = tag;
        }

        public string Tag { get; set; }
    }
}
