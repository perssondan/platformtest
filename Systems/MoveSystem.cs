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
                // TODO: can we add do this with colliders added by the world instead?
                EnforceInsideWorld(world, transform);
            }
        }

        private void EnforceInsideWorld(World world, TransformComponent transformComponent)
        {
            if (transformComponent.Position.Y < world.WorldRect.Top)//above
            {
                // zero y
                transformComponent.Velocity *= Vector2.UnitX;
                transformComponent.Position = transformComponent.Position * Vector2.UnitX + (float)world.WorldRect.Top * Vector2.UnitY;
            }
            if (transformComponent.Position.Y > world.WorldRect.Bottom)//below
            {
                // zero y
                transformComponent.Velocity *= Vector2.UnitX;
                transformComponent.Position = transformComponent.Position * Vector2.UnitX + (float)world.WorldRect.Top * Vector2.UnitY;
            }
            if (transformComponent.Position.X < world.WorldRect.Left)//left
            {
                // zero x
                transformComponent.Velocity *= Vector2.UnitY;
                transformComponent.Position = transformComponent.Position * Vector2.UnitY + (float)world.WorldRect.Left * Vector2.UnitX;
            }
            if (transformComponent.Position.X + transformComponent.GameObject.ColliderComponent.Size.X > world.WorldRect.Right)//right
            {
                // zero x velocity
                transformComponent.Velocity *= Vector2.UnitY;
                // set right position to the right limit of the world minus the width. Sprite or collider width?
                transformComponent.Position = transformComponent.Position * Vector2.UnitY + (Vector2.UnitX * ((float)world.WorldRect.Right - transformComponent.GameObject.ColliderComponent.Size.X));
            }
        }
    }
}