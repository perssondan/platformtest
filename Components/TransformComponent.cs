using System;
using System.Collections.Generic;
using System.Numerics;
using uwpKarate.GameObjects;
using uwpKarate.Utilities;

namespace uwpKarate.Components
{
    public class TransformComponent : IGameObjectComponent<World>
    {
        private HistoryStack<Vector2> _historyStack = new HistoryStack<Vector2>(100);
        private Vector2 _position;

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