using GamesLibrary.Utilities;
using NUnit.Framework;

namespace GamesLibrary.Test.Utilities
{
    public class GameMathTest
    {
        [TestCase(.5f, 0f, 1f, 0f, 200f, 100f)]
        [TestCase(.75f, 0f, 1f, 0f, 480f, 360f)]
        [TestCase(1f, 0f, 1f, 0f, 480f, 480f)]
        [TestCase(0f, 0f, 1f, 0f, 480f, 0f)]
        [TestCase(.25f, 0f, 1f, 0f, 100f, 25f)]
        public void Map(float value, float inMin, float inMax, float outMin, float outMax, float expected)
        {
            Assert.That(GameMath.Map(value, inMin, inMax, outMin, outMax), Is.EqualTo(expected));
        }
    }
}
