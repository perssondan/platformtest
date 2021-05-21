namespace uwpPlatformer.Components
{
    public class GraphicsComponentManager : ComponentManager<AnimatedGraphicsComponent>
    {
        protected GraphicsComponentManager()
        {
        }

        public static GraphicsComponentManager Instance { get; } = new GraphicsComponentManager();
    }
}