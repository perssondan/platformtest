using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public class PhysicsComponent
    {
        private readonly GameObject _gameObject;

        public PhysicsComponent(GameObject gameObject)
        {
            _gameObject = gameObject;
        }

        public void Update(World world, TimeSpan timeSpan)
        {
        }
    }
}