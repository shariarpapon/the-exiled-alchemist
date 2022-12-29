using Main.DebuggingUtility;
using System.Collections.Generic;
using UnityEngine;

namespace Main.WorldManagement
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
        public int ChunksPerAxis { get; private set; }

        public World(WorldSettings settings) 
        {
            Settings = settings;
            MinHeight = float.MaxValue;
            MaxHeight = float.MinValue;

            ChunksPerAxis = settings.worldSize / settings.chunkSize;

            GenerateWorld();
        }

        //World generation entry point
        private void GenerateWorld() 
        {
            DebugUtils.LogConfirmation("World generated!");
        }

        /// <summary>
        /// Returns coordinates of the chunk the given worldPosition is occupying.
        /// </summary>
        public Vector2 GetChunkLocalPosition(Vector3 worldPos)
        {
            Vector2 chunkCoord = new Vector2(Mathf.RoundToInt(worldPos.x / Settings.chunkSize), Mathf.RoundToInt(worldPos.z / Settings.chunkSize));
            chunkCoord.x = Mathf.Clamp(chunkCoord.x, 0, ChunksPerAxis - 1);
            chunkCoord.y = Mathf.Clamp(chunkCoord.y, 0, ChunksPerAxis - 1);
            return chunkCoord;
        }

        /// <summary>
        /// Comapres the passed in min/max height with the current min/max height and updates if a new best is found.
        /// </summary>
        public void CompareAndEvaluateHeights(float min, float max)
        {
            if (min < MinHeight) MinHeight = min;
            if (max > MaxHeight) MaxHeight = max;
        }
    }

}
