using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public class InputComponent
    {
        private readonly GameObject _gameObject;

        public InputComponent(GameObject gameObject)
        {
            _gameObject = gameObject;
        }

        public void Update()
        {
        }
    }
}