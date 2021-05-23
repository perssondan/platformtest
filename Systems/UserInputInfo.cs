using Windows.System;

namespace uwpPlatformer.Systems
{
    public struct UserInputInfo
    {
        public UserInputInfo(VirtualKey virtualKey, bool isPressed)
        {
            VirtualKey = virtualKey;
            IsPressed = isPressed;
        }

        public readonly VirtualKey VirtualKey;
        public bool IsPressed;
    }
}
