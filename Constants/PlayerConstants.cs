namespace uwpPlatformer.Constants
{
    public class PlayerConstants
    {
        // Jump and walk constants
        public const float MaxJumpHeight = 72f;
        public const float TimeToReachMaxJumpHeight = 0.44f; // seconds

        public const float Xh = 20f; // Horizontal distance to travel before reaching max jump height (parabola peak)
        public const float V0x = Xh / TimeToReachMaxJumpHeight; // Max horizontal velocity
        public const float V0y = -2f * MaxJumpHeight * V0x / Xh; // Initial vertical velocity
        public const float PlayerGravity = 2f * MaxJumpHeight * V0x * V0x / (Xh * Xh); // Player gravity

        public const float VerticallyStationaryThreshold = 2.1f;
    }
}