namespace uwpPlatformer.Components
{
    public class ShapeGraphicsComponentManager : ComponentManager<ShapeGraphicsComponent>
    {
        protected ShapeGraphicsComponentManager()
        {
        }

        public static ShapeGraphicsComponentManager Instance { get; } = new ShapeGraphicsComponentManager();
    }
}
