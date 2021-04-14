using System;
using System.Numerics;
using uwpKarate.GameObjects;

namespace uwpKarate.Components
{
    public class TransformComponent
    {
        /// <summary>
        /// px/sec
        /// </summary>
        public Vector2 Velocity { get; set; }

        public Vector2 Position { get; set; }

        public void Update(World world, TimeSpan timeSpan)
        {
            var oldPosition = Position;
            Position += (float)timeSpan.TotalSeconds * Velocity;
            if (!world.WorldRect.Contains(Position.ToPoint()))
            {
                Position = Vector2.Zero;
            }
            Velocity = Vector2.Zero;
        }
    }
}