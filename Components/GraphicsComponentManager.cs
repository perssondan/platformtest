namespace uwpKarate.Components
{
    public class GraphicsComponentManager : ComponentManager<GraphicsComponentBase>
    {
        protected GraphicsComponentManager()
        {
        }

        public static GraphicsComponentManager Instance { get; } = new GraphicsComponentManager();
    }
}