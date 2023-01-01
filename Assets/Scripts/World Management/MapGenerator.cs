using JetBrains.Annotations;
using System.Collections.Generic;
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
        internal static float[,] GenerateHeightMap(int size, Vector2 offset, NoiseSettings settings)
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

        /// <summary>
        /// Generates a noise map according to the given size and settings and stores the global min and max value of the complete map.
        /// </summary>
        internal static float[,] GenerateHeightMap(int size, Vector2 offset, NoiseSettings settings, ref float minHeight, ref float maxHeight)
        {
            float[,] map = new float[size, size];

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                {
                    Vector2 sample = new Vector2(x + offset.x, y + offset.y) * settings.frequency / settings.scale;
                    float height = Mathf.PerlinNoise(sample.x, sample.y) * settings.amplitude;
                    map[x, y] = height;

                    if (height < minHeight) minHeight = height;
                    if (height > maxHeight) maxHeight = height;
                }

            return map;
        }

        /// <summary>
        /// Normalizes the given height map between 0 and 1 using the given min/max height.
        /// </summary>
        internal static float[,] NormalizeHeightMap(float[,] heightMap, float minHeight, float maxHeight)
        {
            int size = heightMap.GetLength(0);
            float[,] normalizedMap = new float[size, size];

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                {
                    normalizedMap[x, y] = Mathf.InverseLerp(minHeight, maxHeight, heightMap[x, y]);
                }
            return normalizedMap;
        }
    }


}
