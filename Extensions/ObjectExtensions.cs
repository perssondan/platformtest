namespace uwpKarate.Extensions
{
    public static class ObjectExtensions
    {
        public static T Swap<T>(this T x, ref T y)
        {
            T temp = y;
            y = x;
            return temp;
        }

        public static void Swap<T>(ref T first, ref T second)
        {
            var temp = first;
            first = second;
            second = temp;
        }
    }
}