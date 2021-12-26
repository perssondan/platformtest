using System;

namespace uwpPlatformer.Systems
{
    [Flags]
    public enum UserInput
    {
        None = 0,
        Left = 1,
        Right = 2,
        Jump = 4,
        Up = 8,
        Down = 16,
    }
}