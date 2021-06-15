using System;
using System.Numerics;

namespace GamesLibrary.Utilities
{
    public class PerlinNoise
    {
        private static readonly Random _random = new Random();
        private readonly int[] _permutations;

        private readonly int[] _dynamicHashTable;

        public PerlinNoise()
        {
            _dynamicHashTable = new int[256];

            InitializeHashTable(_dynamicHashTable);
            RandomSwapValues(_dynamicHashTable);

            var size = _dynamicHashTable.Length * 2;
            _permutations = new int[size];
            var length = _dynamicHashTable.Length;
            for (var x = 0; x < size; x++)
            {
                _permutations[x] = _dynamicHashTable[x % length];
            }
        }

        public float Noise(Vector3 position) => Noise(position, out var _);

        public float Noise(Vector3 position, out Vector3 derivative)
        {
            var tableSizeMask = _dynamicHashTable.Length - 1;

            var flooredX = (int)FastFloor(position.X);
            var flooredY = (int)FastFloor(position.Y);
            var flooredZ = (int)FastFloor(position.Z);

            int xi0 = flooredX & tableSizeMask;
            int yi0 = flooredY & tableSizeMask;
            int zi0 = flooredZ & tableSizeMask;

            int xi1 = (xi0 + 1) & tableSizeMask;
            int yi1 = (yi0 + 1) & tableSizeMask;
            int zi1 = (zi0 + 1) & tableSizeMask;

            var tx = position.X - flooredX;
            var ty = position.Y - flooredY;
            var tz = position.Z - flooredZ;

            var u = Quintic(tx);
            var v = Quintic(ty);
            var w = Quintic(tz);

            float x0 = tx, x1 = tx - 1;
            float y0 = ty, y1 = ty - 1;
            float z0 = tz, z1 = tz - 1;

            var a = GradientDotVector(Hash(xi0, yi0, zi0), x0, y0, z0);
            var b = GradientDotVector(Hash(xi1, yi0, zi0), x1, y0, z0);
            var c = GradientDotVector(Hash(xi0, yi1, zi0), x0, y1, z0);
            var d = GradientDotVector(Hash(xi1, yi1, zi0), x1, y1, z0);
            var e = GradientDotVector(Hash(xi0, yi0, zi1), x0, y0, z1);
            var f = GradientDotVector(Hash(xi1, yi0, zi1), x1, y0, z1);
            var g = GradientDotVector(Hash(xi0, yi1, zi1), x0, y1, z1);
            var h = GradientDotVector(Hash(xi1, yi1, zi1), x1, y1, z1);

            var du = QuinticDerivative(tx);
            var dv = QuinticDerivative(ty);
            var dw = QuinticDerivative(tz);

            float k0 = a;
            float k1 = (b - a);
            float k2 = (c - a);
            float k3 = (e - a);
            float k4 = (a + d - b - c);
            float k5 = (a + f - b - e);
            float k6 = (a + g - c - e);
            float k7 = (b + c + e + h - a - d - f - g);

            derivative.X = du * (k1 + k4 * v + k5 * w + k7 * v * w);
            derivative.Y = dv * (k2 + k4 * u + k6 * w + k7 * v * w);
            derivative.Z = dw * (k3 + k5 * u + k6 * v + k7 * v * w);

            return k0 + k1 * u + k2 * v + k3 * w + k4 * u * v + k5 * u * w + k6 * v * w + k7 * u * v * w;
        }

        public float OctavePerlin(Vector3 position, int octaves, float persistence)
        {
            var total = 0f;
            var frequency = 1f;
            var amplitude = 1f;

            for (var i = 0; i < octaves; i++)
            {
                total += Noise(position * frequency, out var _) * amplitude;

                amplitude *= persistence;
                frequency *= 2;
            }

            return total;
        }

        private void InitializeHashTable(int[] hashTable)
        {
            var value = hashTable.Length - 1;
            while (value >= 0)
            {
                hashTable[hashTable.Length - 1 - value] = value;
                value--;
            }
        }

        private void RandomSwapValues(int[] hashTable)
        {
            var value = hashTable.Length - 1;
            while (value >= 0)
            {
                var firstIndex = _random.Next(0, hashTable.Length);
                var temp = hashTable[firstIndex];
                var secondIndex = _random.Next(0, hashTable.Length);
                hashTable[firstIndex] = hashTable[secondIndex];
                hashTable[secondIndex] = temp;
                value--;
            }
        }

        private int FastFloor(float x)
        {
            return x > 0 ? (int)x : (int)x - 1;
        }

        private int Hash(int x, int y, int z)
        {
            return _permutations[_permutations[_permutations[x] + y] + z];
        }

        private float Quintic(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private float QuinticDerivative(float t)
        {
            return 30 * t * t * (t * (t - 2) + 1);
        }

        private float GradientDotVector(int perm, float x, float y, float z)
        {
            switch (perm & 15)
            {
                case 0: return x + y; // (1,1,0)
                case 1: return -x + y; // (-1,1,0)
                case 2: return x - y; // (1,-1,0)
                case 3: return -x - y; // (-1,-1,0)
                case 4: return x + z; // (1,0,1)
                case 5: return -x + z; // (-1,0,1)
                case 6: return x - z; // (1,0,-1)
                case 7: return -x - z; // (-1,0,-1)
                case 8: return y + z; // (0,1,1),
                case 9: return -y + z; // (0,-1,1),
                case 10: return y - z; // (0,1,-1),
                case 11: return -y - z; // (0,-1,-1)
                case 12: return y + x; // (1,1,0)
                case 13: return -x + y; // (-1,1,0)
                case 14: return -y + z; // (0,-1,1)
                case 15: return -y - z; // (0,-1,-1)
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
