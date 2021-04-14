using System;
using System.Drawing;
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
            Position += (float)timeSpan.TotalSeconds * Velocity;
            if (!world.WorldRect.Contains(Position.ToPoint()))
            {
                if (Position.Y < world.WorldRect.Top)//above
                {
                    // zero y
                    Position *= Vector2.UnitX;
                    Position += (float)world.WorldRect.Top * Vector2.UnitY;
                }
                if (Position.Y > world.WorldRect.Bottom)//below
                {
                    // zero y
                    Position *= Vector2.UnitX;
                    Position += (float)world.WorldRect.Top * Vector2.UnitY;
                }
                if (Position.X < world.WorldRect.Left)//left
                {
                    // zero x
                    Position *= Vector2.UnitY;
                    Position += (float)world.WorldRect.Left * Vector2.UnitX;
                }
                if (Position.X > world.WorldRect.Right)//right
                {
                    // zero x
                    Position *= Vector2.UnitY;
                    Position += (float)world.WorldRect.Right * Vector2.UnitX;
                }
            }
            // reset speed each update
            Velocity = Vector2.Zero;
        }
    }
}