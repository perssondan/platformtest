using System;
using System.Numerics;
using uwpKarate.GameObjects;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace uwpKarate.Components
{
    public class InputComponent
    {
        private readonly GameObject _gameObject;
        private readonly Window _window;
        private float _walkingForce = 1f;
        private Vector2 _jumpingForce = new Vector2(0f, -140f);
        private VirtualKey _virtualKey;
        private VirtualKey _previousVirtualKey;

        public InputComponent(GameObject gameObject, Window current)
        {
            _gameObject = gameObject;
            _window = current;

            HookupKeyListener();
        }

        public void Update(TimeSpan timeSpan)
        {
            var force = default(Vector2);
            switch (_virtualKey)
            {
                //case VirtualKey.S:
                //case VirtualKey.GamepadDPadDown:
                //    force.Y = 10;
                //    break;
                //case VirtualKey.W:
                case VirtualKey.GamepadDPadUp:
                    // Only jump when stationary
                    if (!IsJumping && !IsFalling)
                    {
                        // Accelaration
                        //_gameObject.TransformComponent.Velocity += (float)timeSpan.TotalSeconds * _jumpingForce;
                        // Impulse
                        _gameObject.TransformComponent.Velocity += _jumpingForce;
                    }
                    
                    break;
                case VirtualKey.D:
                case VirtualKey.GamepadDPadRight:
                    force.X = _walkingForce;
                    _gameObject.TransformComponent.Position += force;
                    break;
                case VirtualKey.A:
                case VirtualKey.GamepadDPadLeft:
                    force.X = -_walkingForce;
                    _gameObject.TransformComponent.Position += force;
                    break;
            }

            
            _previousVirtualKey = _virtualKey;
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

            coreWindow.KeyDown += OnKeyDown;
            coreWindow.KeyUp += (a, b) => _virtualKey = VirtualKey.None;
        }

        private void OnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            _virtualKey = args.VirtualKey;
        }
    }
}