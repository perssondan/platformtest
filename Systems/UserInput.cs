﻿using System;

namespace uwpKarate.Systems
{
    [Flags]
    public enum UserInput
    {
        None = 0,
        Left = 1,
        Right = 2,
        Jump = 4
    }
}