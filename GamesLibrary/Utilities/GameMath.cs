namespace GamesLibrary.Utilities
{
    public static class GameMath
    {
        public static float Lerp(float startValue, float endValue, float by)
        {
            return startValue * (1 - by) + endValue * by;
        }

        public static float Map(float value, float inMin, float inMax, float outMin, float outMax)
        {
            var k = (outMax - outMin) / (inMax - inMin);
            return value * k;
        }
    }
}
