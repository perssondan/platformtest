namespace GamesLibrary.Components
{
    public struct GroupComponent : IComponent
    {
        public GroupComponent(string tag)
        {
            Group = tag;
        }

        public string Group { get; set; }
    }
}
