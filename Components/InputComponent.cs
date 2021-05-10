using System;
using uwpKarate.GameObjects;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace uwpKarate.Components
{
    public class InputComponent : GameObjectComponent, IGameObjectComponent
    {
        private readonly Window _window;
        private UserInput _userInputs;

        public InputComponent(GameObject gameObject, Window current)
            : base(gameObject)
        {
            _window = current;

            HookupKeyListener();
        }

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
                _userInputs |= userInput;
            }
            else
            {
                _userInputs &= ~userInput;
            }
        }

        public UserInput UserInputs => _userInputs;

        [Flags]
        public enum UserInput
        {
            None = 0,
            Left = 1,
            Right = 2,
            Jump = 4
        }
    }
}