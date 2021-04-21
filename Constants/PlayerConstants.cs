namespace uwpKarate.Constants
{
    public class PlayerConstants
    {
        public const float MaxJumpHeight = 66;
        public const float TimeToReachMaxJumpHeight = 0.44f;
        public const float Gravity = 2 * MaxJumpHeight / (TimeToReachMaxJumpHeight * TimeToReachMaxJumpHeight);
        public const float V_0 = -2f * MaxJumpHeight / TimeToReachMaxJumpHeight;

        public const float _jumpLength = 72;
        public const float V_X = X_H * V_0 / (2 * TimeToReachMaxJumpHeight);
        public const float X_H = _jumpLength / 2f;
        public const float gravity_x = -2 * MaxJumpHeight * V_X * V_X / (X_H * X_H);
    }
}