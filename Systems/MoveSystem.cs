using System;
using System.Numerics;
using uwpKarate.Components;
using uwpKarate.GameObjects;

namespace uwpKarate.Systems
{
    public class MoveSystem : SystemBase<MoveSystem>
    {
        public override void Update(World world, TimeSpan deltaTimeSpan)
        {
            float deltaTime = (float)deltaTimeSpan.TotalSeconds;
            foreach (var transform in TransformComponentManager.Instance.Components)
            {
                if (transform.Velocity.LengthSquared() == 0f) continue;

                if (transform.Velocity.Y < 0.5f && transform.Velocity.Y > -0.5f)
                {
                    transform.Velocity *= Vector2.UnitX;
                }

                if (transform.Velocity.X < 0.5f && transform.Velocity.X > -0.5f)
                {
                    transform.Velocity *= Vector2.UnitY;
                }

                var position = transform.Velocity * deltaTime;
                if (transform.Position == position) continue;

                transform.Position += position;
            }
        }
    }
}