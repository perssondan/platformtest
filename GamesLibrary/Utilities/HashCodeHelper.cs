namespace GamesLibrary.Utilities
{
    public static class HashCodeHelper
    {
        /// <summary>
        /// Combines two hash codes
        /// </summary>
        public static int CombineHashCodes(this int h1, int h2)
        {
            return (h1 << 5) + h1 ^ h2;
        }
    }
}