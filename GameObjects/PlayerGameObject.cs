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

            PhysicsComponent.Gravity = _normalGravity;

            var _userInputs = InputComponent.UserInputs;

            if ((_userInputs & UserInput.Jump) == UserInput.Jump)
            {
                _jumpPressedAt = _jumpPressedRememberTime;
            }

            if ((_userInputs & UserInput.Right) == UserInput.Right || _walkPressedAt.TotalMilliseconds > 0f)
            {
                GoRight();
            }
            else if ((_userInputs & UserInput.Left) == UserInput.Left || _walkPressedAt.TotalMilliseconds > 0f)
            {
                GoLeft();
            }
            else
            {
                StopWalk();
            }

            //Only jump when stationary
            if (!IsFalling && !IsJumping && _jumpPressedAt.TotalMilliseconds > 0f)
            {
                _jumpPressedAt = TimeSpan.Zero;
                TransformComponent.Velocity += InitialJumpVelocity;
            }
        }

            /// <summary>
            /// Initial jump velocity
            /// </summary>
        public Vector2 InitialJumpVelocity => new Vector2(0, PlayerConstants.V_0);

        private void GoRight()
        {
            _walkPressedAt = _walkPressedRememberTime;
            var walkVector = _walkingSpeed;
            Walk(walkVector);
        }

        private void GoLeft()
        {
            _walkPressedAt = _walkPressedRememberTime;
            var walkVector = -_walkingSpeed;
            Walk(walkVector);
        }

        private void Walk(Vector2 walkVector)
        {
            if (IsFalling) // Or increase gravity, this can be done on short jumps too
            {
                walkVector /= 3f;
            }

            if (TransformComponent.Velocity.X != walkVector.X)
            {
                TransformComponent.Velocity = TransformComponent.Velocity * Vector2.UnitY + walkVector;
            }
        }

        private void StopWalk()
        {
            TransformComponent.Velocity *= Vector2.UnitY;
        }

        private bool IsJumping => TransformComponent.Velocity.Y < 0f;
        private bool IsFalling => TransformComponent.Velocity.Y > 0f;
        private bool IsWalking => TransformComponent.Velocity.X != 0;

        public override void OnAfterUpdate(World world, TimeSpan timeSpan)
        {
        }
    }
}