namespace uwpKarate.Components
{
    public class PhysicsComponentManager : ComponentManager<PhysicsComponent>
    {
        protected PhysicsComponentManager()
        {
        }

        public static PhysicsComponentManager Instance { get; } = new PhysicsComponentManager();
    }
}