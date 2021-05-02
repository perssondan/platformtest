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
        private bool _isJumpButtonPressed;

        public PlayerGameObject()
        {
        }

        /// <summary>
        /// Initial jump velocity
        /// </summary>
        public Vector2 InitialJumpVelocity => new Vector2(0, PlayerConstants.InitialVerticalVelocity);

        public override void OnBeforeUpdate(World world, TimeSpan timeSpan)
        {
            EnforceGravity();

            var userInputs = InputComponent.UserInputs;
            WalkHandler(userInputs);
            JumpHandler(userInputs, timeSpan);
        }

        public override void OnAfterUpdate(World world, TimeSpan timeSpan)
        {
            EnforceInsideWorld(world);
        }

        private void JumpHandler(UserInput userInputs, TimeSpan timeSpan)
        {
            _jumpPressedAt -= timeSpan;

            if ((userInputs & UserInput.Jump) == UserInput.Jump)
            {
                if (_isJumpButtonPressed == false)
                {
                    _isJumpButtonPressed = true;
                    _jumpPressedAt = _jumpPressedRememberTime;
                }
            }
            else
            {
                _isJumpButtonPressed = false;
                _jumpPressedAt = TimeSpan.Zero;
            }

            if (_jumpPressedAt.TotalMilliseconds <= 0f) return;

            //Only jump when grounded
            if (ColliderComponent.IsGrounded() == false) return;

            _jumpPressedAt = TimeSpan.Zero;

            Jump();
        }

        private void WalkHandler(UserInput userInputs)
        {
            var walkOrientation = GetWalkOrientationFromUserInput(userInputs);

            Walk(walkOrientation);
        }

        private float GetWalkOrientationFromUserInput(UserInput userInputs)
        {
            if ((userInputs & UserInput.Right) == UserInput.Right) return 1f;

            if ((userInputs & UserInput.Left) == UserInput.Left) return -1f;

            return 0f;
        }

        private void Jump()
        {
            TransformComponent.Velocity += InitialJumpVelocity;
        }

        private void EnforceGravity()
        {
            if (PhysicsComponent?.Gravity == Vector2.Zero)
            {
                PhysicsComponent.Gravity = _normalGravity;
            }
        }

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

        private void EnforceInsideWorld(World world)
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