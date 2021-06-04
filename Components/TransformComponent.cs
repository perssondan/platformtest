using System.Numerics;
using uwpPlatformer.GameObjects;
using uwpPlatformer.Utilities;

namespace uwpPlatformer.Components
{
    public class TransformComponent : GameObjectComponent, IComponent
    {
        private HistoryStack<Vector2> _historyStack = new HistoryStack<Vector2>(100);
        private Vector2 _position;

        public TransformComponent(GameObject gameObject) : base(gameObject)
        {
            TransformComponentManager.Instance.AddComponent(this);
        }

        public Vector2 Velocity { get; set; }

        public Vector2 Position
        {
            get => _position;
            set
            {
                if (_position == value) return;

                _historyStack.Push(value);

                PreviousPosition = _position;
                _position = value;
            }
        }

        public Vector2 PreviousPosition { get; private set; }

        public Vector2[] PositionHistory => _historyStack.Items.ToArray();

        protected override void OnDispose()
        {
            TransformComponentManager.Instance.RemoveComponent(this);
        }
    }
}