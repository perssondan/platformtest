using System;
using System.Numerics;
using uwpKarate.Constants;
using static uwpKarate.Components.InputComponent;

namespace uwpKarate.GameObjects
{
    public class PlayerGameObject : GameObject
    {
        private Vector2 _normalGravity = new Vector2(0, PlayerConstants.Gravity);
        private Vector2 _walkingSpeed = new Vector2(100f, 0f);

        private readonly TimeSpan _jumpPressedRememberTime = TimeSpan.FromSeconds(0.4f);
        private TimeSpan _jumpPressedAt;
        private readonly TimeSpan _walkPressedRememberTime = TimeSpan.FromSeconds(0.0004f);
        private TimeSpan _walkPressedAt;

        public PlayerGameObject()
        {
        }

        public override void OnBeforeUpdate(World world, TimeSpan timeSpan)
        {
            _jumpPressedAt -= timeSpan;
            _walkPressedAt -= timeSpan;

            var userInputs = InputComponent.UserInputs;

            if ((userInputs & UserInput.Jump) == UserInput.Jump)
            {
                _jumpPressedAt = _jumpPressedRememberTime;
            }

            if ((userInputs & UserInput.Right) == UserInput.Right || _walkPressedAt.TotalMilliseconds > 0f)
            {
                WalkOrientation = 1f;
            }
            else if ((userInputs & UserInput.Left) == UserInput.Left || _walkPressedAt.TotalMilliseconds > 0f)
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

        private void GoRight()
        {
            Walk(1f);
        }

        private void GoLeft()
        {
            Walk(-1f);
        }

        private void Walk(float orientation)
        {
            if (orientation > 0f || orientation < 0f)
            {
                var horizontalVector = new Vector2(orientation * PlayerConstants.InitialHorizontalVelocity, 0f);
                TransformComponent.Velocity = TransformComponent.Velocity * Vector2.UnitY + horizontalVector;
                var gravity = new Vector2(orientation * PlayerConstants.Drag, PlayerConstants.Gravity);
                PhysicsComponent.Gravity = gravity;
            }
            else if ((TransformComponent.Velocity.X < 0f && PhysicsComponent.Gravity.X < 0)
                || (TransformComponent.Velocity.X > 0f && PhysicsComponent.Gravity.X > 0))
            {
                TransformComponent.Velocity *= Vector2.UnitY;
                PhysicsComponent.Gravity *= Vector2.UnitY;
            }
        }

        private bool IsJumping => TransformComponent.Velocity.Y < 0f;
        private bool IsFalling => TransformComponent.Velocity.Y > 0f;
        private bool IsWalking => TransformComponent.Velocity.X != 0;

        public override void OnAfterUpdate(World world, TimeSpan timeSpan)
        {
        }
    }
}