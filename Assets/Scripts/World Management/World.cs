using Everime.DebuggingUtility;
using System.Collections.Generic;
using UnityEngine;

namespace Everime.WorldManagement
{
    /// <summary>
    /// The world class generates the world map and holds all data of the particular world.
    /// </summary>
    public class World
    {
        public WorldSettings Settings { get; private set; }
        public Dictionary<Vector2, Chunk> Chunks { get; private set; }
        public float MinHeight { get; private set; }
        public float MaxHeight { get; private set; }

        public readonly int appendedSeed;
        public readonly System.Random prng;
        private readonly Vector2 worldOffset;

        private Transform worldHolder;

        public World(WorldSettings settings) 
        { 
            Settings = settings;
            MinHeight = float.MaxValue;
            MaxHeight = float.MinValue;
            appendedSeed = Settings.AppendedSeed("overworld");

            prng = new System.Random(appendedSeed);
            int noiseRange = 9_999_999;
            worldOffset = new Vector2((int)(prng.NextDouble() * 2 * noiseRange - noiseRange), (int)(prng.NextDouble() * 2 * noiseRange - noiseRange));
            Chunks = new Dictionary<Vector2, Chunk>();

            GenerateWorld();
        }

        //World generation entry point
        private void GenerateWorld() 
        {
            worldHolder = new GameObject($"World_{appendedSeed}").transform;
            worldHolder.position = Vector3.zero;

            for (int x = 0; x < Settings.worldSizeInChunks; x++)
                for (int y = 0; y < Settings.worldSizeInChunks; y++) 
                {
                    Vector2 relativePos = new Vector2(x, y);
                    Vector3 globalPos = RelativeToGlobalChunkPosition(relativePos);
                    Vector2 offset = worldOffset + new Vector2(globalPos.x, globalPos.z);
                    float[,] heightMap = MapGenerator.GenerateHeightMap(Settings.chunkSize + 1, offset, Settings.heightNoiseSettings);
                    ChunkData chunkData = new ChunkData(heightMap, Settings.chunkMaterial, relativePos, globalPos);
                    Chunk chunk = new Chunk(chunkData, !Settings.updateChunkVisibility);

                    chunk.chunkGameObject.transform.SetParent(worldHolder);
                    Chunks.Add(relativePos, chunk);
                }

            DebugUtils.LogConfirmation("World generated!");
        }

        /// <summary>
        /// Returns the relative position of the chunk that the given world position is occupying.
        /// </summary>
        public Vector2 GlobalToRelativeChunkPosition(Vector3 worldPos)
        {
            Vector2 chunkCoord = new Vector2(Mathf.RoundToInt((worldPos.x - Settings.worldOffset.x) / Settings.chunkSize), Mathf.RoundToInt((worldPos.z - Settings.worldOffset.z) / Settings.chunkSize));
            chunkCoord.x = Mathf.Clamp(chunkCoord.x, 0, Settings.worldSizeInChunks - 1);
            chunkCoord.y = Mathf.Clamp(chunkCoord.y, 0, Settings.worldSizeInChunks - 1);
            return chunkCoord;
        }

        /// <summary>
        /// Returns the global position of the chunk that the given relative position is occupying. 
        /// </summary>
        public Vector3 RelativeToGlobalChunkPosition(Vector2 relativePos) 
        {
            return new Vector3(relativePos.x, 0, relativePos.y) * Settings.chunkSize + Settings.worldOffset;
        }

        /// <summary>
        /// Comapres the passed in min/max height with the current min/max height and updates if a new best is found.
        /// </summary>
        public void CompareAndEvaluateHeights(float min, float max)
        {
            if (min < MinHeight) MinHeight = min;
            if (max > MaxHeight) MaxHeight = max;
        }

        /// <summary>
        /// Deletes the world with all of its data.
        /// </summary>
        public void Delete() 
        {
            GameObject.DestroyImmediate(worldHolder.gameObject);
        }
    }

}
