using UnityEngine;

namespace Main.WorldManagement
{
    /// <summary>
    /// This class contains methods for noise map generation.
    /// </summary>
    internal static class MapGenerator
    {
        /// <summary>
        /// Generates a noise map according to the given size and settings.
        /// </summary>
        internal static float[,] GeneratePerlinNoiseMap(int size, Vector2 offset, NoiseSettings settings)
        {
            float[,] map = new float[size, size];

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                { 
                    Vector2 sample = new Vector2(x + offset.x, y + offset.y) * settings.frequency / settings.scale;
                    float perlinValue = Mathf.PerlinNoise(sample.x, sample.y) * settings.amplitude;
                    map[x, y] = perlinValue;
                }

            return map;
        }

    }


}
