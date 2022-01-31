namespace uwpPlatformer.Constants
{
    public class PlayerConstants
    {
        public const float MaxJumpHeight = 72f;
        public const float TimeToReachMaxJumpHeight = 0.44f;
        //public const float Gravity = 2f * MaxJumpHeight / (TimeToReachMaxJumpHeight * TimeToReachMaxJumpHeight);
        public const float MaxWalkWidth = 20f;
        public const float MaxWalkSpeed = 32f;
        public const float TimeToReachMaxWalkWidth = 0.44f;
        public const float Drag = -2f * MaxWalkWidth / (TimeToReachMaxWalkWidth * TimeToReachMaxWalkWidth);
        public const float InitialVerticalVelocity = -2f * MaxJumpHeight / TimeToReachMaxJumpHeight;
        public const float InitialHorizontalVelocity = 20f;//2f * MaxJumpHeight * MaxWalkSpeed / MaxWalkWidth;

        public const float Xh = 20f;
        public const float Vx = Xh / TimeToReachMaxJumpHeight;
        public const float Vy = -2f * MaxJumpHeight * Vx / Xh;
        public const float Gravity = 2f * MaxJumpHeight * Vx * Vx / (Xh * Xh);

        public const float VerticallyStationaryThreshold = 2.1f;
    }
}