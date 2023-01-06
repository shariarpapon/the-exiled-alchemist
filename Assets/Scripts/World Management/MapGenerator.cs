using System.IO.MemoryMappedFiles;
using UnityEngine;

namespace Everime.WorldManagement
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
                    float ampltiude = 1;
                    float frequency = 1;
                    float heightValue = 0;
                    for (int i = 0; i < settings.octaves; i++)
                    {
                        Vector2 sample = (new Vector2(x, y) + offset) / settings.scale * frequency;
                        float perlinValue = Mathf.PerlinNoise(sample.x, sample.y) * 2 - 1;
                        heightValue += perlinValue * ampltiude;

                        ampltiude *= settings.persistence;
                        frequency *= settings.lacunarity;
                    }

                    map[x, y] = heightValue;

                    if (heightValue < minHeight) minHeight = heightValue;
                    if (heightValue > maxHeight) maxHeight = heightValue;
                }

            return map;
        }

        /// <summary>
        /// Normalizes the given height map between 0 and 1 using the given min/max height.
        /// </summary>
        internal static float[,] NormalizeHeightMap(float[,] heightMap, float minHeight, float maxHeight, float maxWorldSize, Vector3 worldMapPosition)
        {
            int size = heightMap.GetLength(0);
            float[,] normalizedMap = new float[size, size];

            float mapExtent = (size - 1) / 2f;
            float maxDistToCenter = maxWorldSize / 2f;
            Vector2 center = new Vector2(maxDistToCenter, maxDistToCenter);

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                {
                    float heightValue = Mathf.InverseLerp(minHeight, maxHeight, heightMap[x, y]);

                    float worldX = (x - mapExtent) + worldMapPosition.x;
                    float worldZ = (y - mapExtent) + worldMapPosition.z;

                    Vector2 vertexPosition = new Vector2(worldX, worldZ);
                    float falloff = 1 - maxDistToCenter / Vector2.Distance(vertexPosition, center);

                    normalizedMap[x, y] = Mathf.Clamp01(heightValue * (1 - falloff));
                }
            return normalizedMap;
        }

    }


}
