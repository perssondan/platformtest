using System;
using System.Numerics;
using uwpKarate.GameObjects;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace uwpKarate.Components
{
    public class InputComponent : IGameObjectComponent<World>
    {
        private readonly GameObject _gameObject;
        private readonly Window _window;
        private Vector2 _walkingSpeed = new Vector2(100f, 0f);
        private Vector2 _initialJumpSpeed = new Vector2(0f, -85f);
        private UserInput _userInput;
        private readonly TimeSpan _jumpPressedRememberTime = TimeSpan.FromSeconds(0.4f);
        private TimeSpan _jumpPressedAt;
        private readonly TimeSpan _walkPressedRememberTime = TimeSpan.FromSeconds(1.7f);
        private TimeSpan _walkPressedAt;
        private TimeSpan _timeToApex = TimeSpan.Zero;

        public InputComponent(GameObject gameObject, Window current)
        {
            _gameObject = gameObject;
            _window = current;

            HookupKeyListener();
        }

        public void Update(World world, TimeSpan timeSpan)
        {
            _jumpPressedAt -= timeSpan;
            _walkPressedAt -= timeSpan;
            _timeToApex -= timeSpan;

            var walkVector = Vector2.Zero;
            if ((_userInput & UserInput.Jump) == UserInput.Jump)
            {
                _jumpPressedAt = _jumpPressedRememberTime;
                if (_timeToApex <= TimeSpan.Zero)
                {
                    var timeToApex = _initialJumpSpeed.LengthSquared() / _gameObject.PhysicsComponent.Gravity.LengthSquared();
                    //_timeToApex = TimeSpan.FromSeconds((_initialJumpSpeed / _gameObject.PhysicsComponent.Gravity).Length());
                }
            }
            if ((_userInput & UserInput.Right) == UserInput.Right)
            {
                _walkPressedAt = _walkPressedRememberTime;
                walkVector = _walkingSpeed;
            }
            else if ((_userInput & UserInput.Left) == UserInput.Left)
            {
                _walkPressedAt = _walkPressedRememberTime;
                walkVector = -_walkingSpeed;
            }

            if (_walkPressedAt.TotalMilliseconds > 0f)
            {
                _walkPressedAt = TimeSpan.Zero;
                _gameObject.TransformComponent.Velocity += walkVector;
            }

            //Only jump when stationary
            if (!IsJumping && !IsFalling && _jumpPressedAt.TotalMilliseconds > 0f)
            {
                _jumpPressedAt = TimeSpan.Zero;
                _gameObject.TransformComponent.Velocity += _initialJumpSpeed;
            }
        }

        private bool IsJumping => _gameObject.TransformComponent.Velocity.Y < 0f;
        private bool IsFalling => _gameObject.TransformComponent.Velocity.Y > 0f;

        private void HookupKeyListener()
        {
            if (!_window.Dispatcher.HasThreadAccess)
            {
                _ = _window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HookupKeyListener());
                return;
            }
            var coreWindow = _window?.CoreWindow;
            if (coreWindow == null) return;

            coreWindow.KeyDown += (sender, args) => UpdateUserInput(args.VirtualKey, true);
            coreWindow.KeyUp += (sender, args) => UpdateUserInput(args.VirtualKey, false);
        }

        private void UpdateUserInput(VirtualKey virtualKey, bool keyDown)
        {
            var userInput = UserInput.None;
            switch (virtualKey)
            {
                case VirtualKey.W:
                case VirtualKey.GamepadA:
                    userInput = UserInput.Jump;
                    break;

                case VirtualKey.D:
                case VirtualKey.GamepadDPadRight:
                    userInput = UserInput.Right;
                    break;

                case VirtualKey.A:
                case VirtualKey.GamepadDPadLeft:
                    userInput = UserInput.Left;
                    break;
            }

            if (keyDown)
            {
                _userInput |= userInput;
            }
            else
            {
                _userInput &= ~userInput;
            }
        }

        private enum UserInput
        {
            None = 0,
            Left = 1,
            Right = 2,
            Jump = 4
        }
    }
}