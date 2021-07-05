using System.Numerics;
using uwpPlatformer.GameObjects;
using uwpPlatformer.Utilities;

namespace uwpPlatformer.Components
{
    public class TransformComponent : ComponentBase, IComponent
    {
        private HistoryStack<Vector2> _historyStack = new HistoryStack<Vector2>(100);
        private Vector2 _position;

        public TransformComponent(GameObject gameObject) : base(gameObject)
        {
        }

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

        public Vector2 Scale { get; set; } = Vector2.One;

        public float Rotation { get; set; }

        public Vector2 PreviousPosition { get; private set; }

        /// <summary>
        /// Gets a number of historical position values, for debugging purpose
        /// </summary>
        public Vector2[] PositionHistory => _historyStack.Items.ToArray();
    }
}