using System;
using System.Numerics;
using uwpKarate.Constants;
using uwpKarate.Systems;
using Windows.Foundation;
using static uwpKarate.Components.InputComponent;

namespace uwpKarate.GameObjects
{
    public class PlayerGameObject : GameObject
    {
        // TODO: fix these static sprites
        private static readonly Rect[] _walkSourceRects = new Rect[]
            {
                new Rect(0, 32f, 32f, 32f),
                new Rect(32f, 32f, 32f, 32f),
                new Rect(64f, 32f, 32f, 32f),
                new Rect(96f, 32f, 32f, 32f),
            };

        private static readonly Rect[] _staticSourceRects = new Rect[]
            {
                new Rect(0f, 96f, 32f, 32f)
            };

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
            var userInputs = InputComponent.UserInputs;
            WalkHandler(userInputs);
            JumpHandler(userInputs, timeSpan);
        }

        public override void OnAfterUpdate(World world, TimeSpan timeSpan)
        {
            //EnforceInsideWorld(world);
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
            if (ColliderComponent.IsColliding == false) return;

            _jumpPressedAt = TimeSpan.Zero;

            Jump();
        }

        private void WalkHandler(UserInput userInputs)
        {
            var walkOrientation = GetWalkOrientationFromUserInput(userInputs);
            UpdateWalkAnimation(walkOrientation);
            Walk(walkOrientation);
        }

        private void UpdateWalkAnimation(float walkOrientation)
        {
            if (walkOrientation < 0f)
            {
                GraphicsComponent.InvertTile = true;
                GraphicsComponent.SourceRects = _walkSourceRects;
            }
            else if (walkOrientation > 0f)
            {
                GraphicsComponent.InvertTile = false;
                GraphicsComponent.SourceRects = _walkSourceRects;
            }
            else
            {
                GraphicsComponent.InvertTile = false;
                GraphicsComponent.SourceRects = _staticSourceRects;
            }
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
    }
}