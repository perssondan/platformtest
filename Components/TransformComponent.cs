using System;
using System.Numerics;
using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public class TransformComponent
    {
        private Vector2 _velocity;
        private Vector2 _maxVelocity = new Vector2(200, 200);

        /// <summary>
        /// px/sec
        /// </summary>
        public Vector2 Velocity
        {
            get => _velocity;
            set
            {
                if (value.LengthSquared() > _maxVelocity.LengthSquared())
                    return;

                _velocity = value;
            }
        }

        public Vector2 Position { get; set; }

        public void Update(World world, TimeSpan timeSpan)
        {
            var oldPosition = Position;
            Position += (float)timeSpan.TotalSeconds * Velocity;
            if (!world.WorldRect.Contains(Position.ToPoint()))
            {
                Position = Vector2.Zero;
            }
        }
    }
}