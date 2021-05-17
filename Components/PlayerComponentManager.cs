namespace uwpKarate.Components
{
    public class PlayerComponentManager : ComponentManager<PlayerComponent>
    {
        protected PlayerComponentManager()
        {
        }

        public static PlayerComponentManager Instance { get; } = new PlayerComponentManager();
    }
}
