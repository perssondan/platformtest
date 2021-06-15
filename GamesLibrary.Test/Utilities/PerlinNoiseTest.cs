using GamesLibrary.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace GamesLibrary.Test.Utilities
{
    public class PerlinNoiseTest
    {
        [Test]
        public void FirstTest()
        {
            var perlinNoise = new PerlinNoise();

            var rand = new Random();
            var perlinTable = new StringBuilder();

            var perlinInputx = 0f;
            var perlinInputy = 1000f;

            for (var counter =0;counter <10; counter++)
            {
                var x = perlinNoise.Noise(new Vector3(perlinInputx, perlinInputx, perlinInputx));
                perlinInputx += 0.1f;
                var y = perlinNoise.Noise(new Vector3(perlinInputy, perlinInputy, perlinInputy));
                perlinInputy += 0.1f;

                perlinTable.AppendLine($"({GameMath.Map((float)x, 0f, 1f, 0, 320)},{GameMath.Map((float)y, 0f, 1f, 0, 320)})");
            }

            Debug.WriteLine(perlinTable.ToString());
        }

        [Test]
        public void SecondTest()
        {
            var perlinNoise = new PerlinNoise();

            var perlinTable = new StringBuilder();

            var perlinInputx = 0f;
            var perlinInputy = 1000f;

            for (var counter = 0; counter < 100; counter++)
            {
                var x = perlinNoise.Noise(new Vector3(perlinInputx, perlinInputy, 0f));
                perlinInputx += 0.01f;
                var y = perlinNoise.Noise(new Vector3(perlinInputx, perlinInputy, 0f));
                perlinInputy += 0.01f;

                perlinTable.AppendLine($"({x*100},{y*100})");
                //perlinTable.AppendLine($"({GameMath.Map((float)x, 0f, 1f, 0, 320)},{GameMath.Map((float)y, 0f, 1f, 0, 320)})");
            }

            Debug.WriteLine(perlinTable.ToString());
        }
    }
}
