using System;
using System.Numerics;
using uwpKarate.Constants;
using static uwpKarate.Components.InputComponent;

namespace uwpKarate.GameObjects
{
    public class PlayerGameObject : GameObject
    {
        private Vector2 _normalGravity = new Vector2(0, PlayerConstants.Gravity);

        private readonly TimeSpan _jumpPressedRememberTime = TimeSpan.FromMilliseconds(150);
        private TimeSpan _jumpPressedAt;

        public PlayerGameObject()
        {
        }

        public override void OnBeforeUpdate(World world, TimeSpan timeSpan)
        {
            if (PhysicsComponent?.Gravity == Vector2.Zero)
            {
                PhysicsComponent.Gravity = _normalGravity;
            }

            _jumpPressedAt -= timeSpan;

            var userInputs = InputComponent.UserInputs;

            if ((userInputs & UserInput.Jump) == UserInput.Jump)
            {
                _jumpPressedAt = _jumpPressedRememberTime;
            }

            if ((userInputs & UserInput.Right) == UserInput.Right)
            {
                WalkOrientation = 1f;
            }
            else if ((userInputs & UserInput.Left) == UserInput.Left)
            {
                WalkOrientation = -1f;
            }
            else
            {
                WalkOrientation = 0f;
            }

            Walk(WalkOrientation);

            //Only jump when stationary
            if (!IsFalling && !IsJumping && _jumpPressedAt.TotalMilliseconds > 0f)
            {
                PhysicsComponent.Gravity = new Vector2(PhysicsComponent.Gravity.X, PlayerConstants.Gravity);
                _jumpPressedAt = TimeSpan.Zero;
                TransformComponent.Velocity += InitialJumpVelocity;
            }
        }

        private float WalkOrientation { get; set; }

        /// <summary>
        /// Initial jump velocity
        /// </summary>
        public Vector2 InitialJumpVelocity => new Vector2(0, PlayerConstants.InitialVerticalVelocity);

        private void Walk(float orientation)
        {
            if (orientation > 0f || orientation < 0f)
            {
                var horizontalVector = new Vector2(orientation * PlayerConstants.InitialHorizontalVelocity, 0f);
                TransformComponent.Velocity = TransformComponent.Velocity * Vector2.UnitY + horizontalVector;
            }
            else if ((TransformComponent.Velocity.X < 0f && PhysicsComponent.Gravity.X < 0)
                || (TransformComponent.Velocity.X > 0f && PhysicsComponent.Gravity.X > 0))
            {
                TransformComponent.Velocity *= Vector2.UnitY;
            }
            else
            {
                TransformComponent.Velocity *= Vector2.UnitY;
            }
        }

        private bool IsJumping => TransformComponent.Velocity.Y < 0f;
        private bool IsFalling => TransformComponent.Velocity.Y > 0f;

        public override void OnAfterUpdate(World world, TimeSpan timeSpan)
        {
            if (TransformComponent.Position.Y < world.WorldRect.Top)//above
            {
                // zero y
                TransformComponent.Velocity *= Vector2.UnitX;
                TransformComponent.Position = TransformComponent.Position * Vector2.UnitX + (float)world.WorldRect.Top * Vector2.UnitY;
            }
            if (TransformComponent.Position.Y > world.WorldRect.Bottom)//below
            {
                // zero y
                TransformComponent.Velocity *= Vector2.UnitX;
                TransformComponent.Position = TransformComponent.Position * Vector2.UnitX + (float)world.WorldRect.Top * Vector2.UnitY;
            }
            if (TransformComponent.Position.X < world.WorldRect.Left)//left
            {
                // zero x
                TransformComponent.Velocity *= Vector2.UnitY;
                TransformComponent.Position = TransformComponent.Position * Vector2.UnitY + (float)world.WorldRect.Left * Vector2.UnitX;
            }
            if (TransformComponent.Position.X + ColliderComponent.Size.X > world.WorldRect.Right)//right
            {
                // zero x velocity
                TransformComponent.Velocity *= Vector2.UnitY;
                // set right position to the right limit of the world minus the width. Sprite or collider width?
                TransformComponent.Position = TransformComponent.Position * Vector2.UnitY + (Vector2.UnitX * ((float)world.WorldRect.Right - ColliderComponent.Size.X));
            }
        }
    }
}