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
            Chunks = new Dictionary<Vector2, Chunk>();

            GenerateWorld();
        }

        //World generation entry point
        private void GenerateWorld() 
        {
            Transform chunkHolder = new GameObject("Chunk Holder").transform;
            chunkHolder.position = Vector3.zero;

            for (int x = 0; x < ChunksPerAxis; x++)
                for (int y = 0; y < ChunksPerAxis; y++) 
                {
                    Vector2 localPos = new Vector2(x, y);
                    Vector3 globalPos = LocalToGlobalChunkPosition(localPos);

                    Mesh mesh = ChunkMeshGenerator.CreateChunkMesh(Settings.chunkSize);
                    Chunk chunk = new Chunk(mesh, Settings.chunkMaterial, localPos, globalPos);
                    chunk.chunkGameObject.transform.SetParent(chunkHolder);
                    Chunks.Add(localPos, chunk);
                }

            DebugUtils.LogConfirmation("World generated!");
        }

        /// <summary>
        /// Returns coordinates of the chunk the given worldPosition is occupying.
        /// </summary>
        public Vector2 GlobalToLocalChunkPosition(Vector3 worldPos)
        {
            Vector2 chunkCoord = new Vector2(Mathf.RoundToInt((worldPos.x - Settings.worldOffset.x) / Settings.chunkSize), Mathf.RoundToInt((worldPos.z - Settings.worldOffset.z) / Settings.chunkSize));
            chunkCoord.x = Mathf.Clamp(chunkCoord.x, 0, ChunksPerAxis - 1);
            chunkCoord.y = Mathf.Clamp(chunkCoord.y, 0, ChunksPerAxis - 1);
            return chunkCoord;
        }

        public Vector3 LocalToGlobalChunkPosition(Vector2 localPosition) 
        {
            return new Vector3(localPosition.x, 0, localPosition.y) * Settings.chunkSize + Settings.worldOffset;
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
