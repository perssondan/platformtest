using GamesLibrary.Models;
using GamesLibrary.Systems;
using System.Linq;
using uwpPlatformer.Components;
using uwpPlatformer.Extensions;
using uwpPlatformer.GameObjects;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace uwpPlatformer.Systems
{
    public class InputSystem : ISystem
    {
        private Window _window;
        private UserInput _userInputs;
        private readonly IEventSystem _eventSystem;
        private readonly IGameObjectManager _gameObjectManager;

        public InputSystem(IEventSystem eventSystem, IGameObjectManager gameObjectManager)
        {
            _eventSystem = eventSystem;
            _gameObjectManager = gameObjectManager;
        }

        public string Name => nameof(InputSystem);

        public Window Current
        {
            get => _window;
            set
            {
                if (_window == value) return;

                HookdownKeyListener(_window);
                _window = value;
                HookupKeyListener(_window);
            }
        }

        public void Update(TimingInfo timingInfo)
        {
            _gameObjectManager.GameObjects
                .Select(gameObject => (gameObject, inputComponent: gameObject.GetComponent<InputComponent>()))
                .Where(result => result != default && result.inputComponent != default)
                .ToArray() // clone
                .ForEach(result => result.inputComponent.UserInputs = UserInputs);
        }

        private void HookdownKeyListener(Window current)
        {
            if (current == null) return;

            if (!current.Dispatcher.HasThreadAccess)
            {
                _ = current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HookdownKeyListener(current));
                return;
            }

            var coreWindow = current?.CoreWindow;
            if (coreWindow == null) return;

            coreWindow.KeyDown -= KeyDown;
            coreWindow.KeyUp -= KeyUp;
        }

        private void HookupKeyListener(Window current)
        {
            if (!current.Dispatcher.HasThreadAccess)
            {
                _ = _window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HookupKeyListener(current));
                return;
            }
            var coreWindow = current?.CoreWindow;
            if (coreWindow == null) return;

            coreWindow.KeyDown += KeyDown;
            coreWindow.KeyUp += KeyUp;
        }

        private void KeyDown(object _, KeyEventArgs keyEventArgs)
        {
            UpdateUserInput(keyEventArgs.VirtualKey, true);
        }

        private void KeyUp(object _, KeyEventArgs keyEventArgs)
        {
            UpdateUserInput(keyEventArgs.VirtualKey, false);
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

            _eventSystem.Send(this, new UserInputInfo(virtualKey, keyDown));
        }

        public UserInput UserInputs => _userInputs;
    }
}
