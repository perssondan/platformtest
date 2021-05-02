using System;
using System.Numerics;
using uwpKarate.GameObjects;
using uwpKarate.Utilities;

namespace uwpKarate.Components
{
    public class TransformComponent : GameObjectComponent, IGameObjectComponent<World>
    {
        private HistoryStack<Vector2> _historyStack = new HistoryStack<Vector2>(100);
        private Vector2 _position;

        public TransformComponent(GameObject gameObject) : base(gameObject)
        {
        }

        public Vector2 Velocity { get; set; }
        public Vector2[] PositionHistory => _historyStack.Items.ToArray();

        public Vector2 Position
        {
            get => _position;
            set
            {
                if (Velocity.LengthSquared() > 0f)
                {
                    _historyStack.Push(value);
                }

                _position = value;
            }
        }

        public void Update(World world, TimeSpan timeSpan)
        {
        }
    }
}