namespace uwpPlatformer.Constants
{
    public class PlayerConstants
    {
        public const float MaxJumpHeight = 72f;
        public const float TimeToReachMaxJumpHeight = 0.44f;
        public const float Gravity = 2f * MaxJumpHeight / (TimeToReachMaxJumpHeight * TimeToReachMaxJumpHeight);
        public const float MaxWalkWidth = 20f;
        public const float MaxWalkSpeed = 32f;
        public const float TimeToReachMaxWalkWidth = 0.44f;
        public const float Drag = -2f * MaxWalkWidth / (TimeToReachMaxWalkWidth * TimeToReachMaxWalkWidth);
        public const float InitialVerticalVelocity = -2f * MaxJumpHeight / TimeToReachMaxJumpHeight;
        public const float InitialHorizontalVelocity = 20f;//2f * MaxJumpHeight * MaxWalkSpeed / MaxWalkWidth;

        public const float _jumpLength = 72f;
        public const float V_X = X_H * InitialVerticalVelocity / (2 * TimeToReachMaxJumpHeight);
        public const float X_H = _jumpLength / 2f;
        public const float gravity_x = -2 * MaxJumpHeight * V_X * V_X / (X_H * X_H);
    }
}