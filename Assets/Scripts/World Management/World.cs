using Everime.DebuggingUtility;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace Everime.WorldManagement
{
    /// <summary>
    /// The world class generates the world map and holds all data of the particular world.
    /// </summary>
    public class World
    {
        public WorldSettings Settings { get; private set; }
        public Dictionary<Vector2, Chunk> Chunks { get; private set; }
        public float minHeight;
        public float maxHeight;

        public readonly int appendedSeed;
        public readonly System.Random prng;
        private readonly Vector2 worldOffset;

        private Transform worldHolder;

        #region Seperate thread variables
        public bool AwaitingThreadOperations { get; private set; }
        private Queue<Chunk> chunkQueue;
        private int chunkCount;
        #endregion

        public World(WorldSettings settings) 
        { 
            Settings = settings;
            minHeight = float.MaxValue;
            maxHeight = float.MinValue;
            appendedSeed = Settings.AppendedSeed("overworld");

            prng = new System.Random(appendedSeed);
            int noiseRange = 9_999_999;
            worldOffset = new Vector2((int)(prng.NextDouble() * 2 * noiseRange - noiseRange), (int)(prng.NextDouble() * 2 * noiseRange - noiseRange));
            Chunks = new Dictionary<Vector2, Chunk>();

            worldHolder = new GameObject($"World_{appendedSeed}").transform;
            worldHolder.position = Vector3.zero;


            if (settings.worldGenerationMode == WorldGenerationMode.MainThread)
            {
                GenerateWorldOnMainThread();
            }
            else
            {
                chunkCount = 0;
                chunkQueue = new Queue<Chunk>();
                ThreadStart threadStart = new ThreadStart(
                    delegate { GenerateWorldOnSeperateThread(); });

                Thread thread = new Thread(threadStart);
                thread.Start();
                AwaitingThreadOperations = true;
            }
        }

        /// <summary>
        /// Generates the world on the main thread all at once.
        /// </summary>
        private void GenerateWorldOnMainThread()
        {
            for (int x = 0; x < Settings.worldSizeInChunks; x++)
                for (int y = 0; y < Settings.worldSizeInChunks; y++)
                {
                    Vector2 relativePos = new Vector2(x, y);
                    Vector3 globalPos = RelativeToGlobalChunkPosition(relativePos);
                    Vector2 offset = worldOffset + new Vector2(globalPos.x, globalPos.z);
                    float[,] heightMap = MapGenerator.GenerateHeightMap(Settings.chunkSize + 1, offset, Settings.heightNoiseSettings, ref minHeight, ref maxHeight);
                    ChunkData chunkData = new ChunkData(heightMap, Settings.chunkMaterial, relativePos, globalPos);
                    Chunk chunk = new Chunk(chunkData, !Settings.updateChunkVisibility);
                    Chunks.Add(relativePos, chunk);
                }

            for (int x = 0; x < Settings.worldSizeInChunks; x++)
                for (int y = 0; y < Settings.worldSizeInChunks; y++)
                {
                    Chunk chunk = Chunks[new Vector2(x, y)];
                    chunk.data.heightMap = MapGenerator.NormalizeHeightMap(chunk.data.heightMap, minHeight, maxHeight);

                    //Post height map generation
                    chunk.data.GenerateChunkTerrainMeshData(Settings.heightMultiplier, Settings.heightCurve, Settings.heightCalculationMethod);
                    chunk.data.CreateMesh();
                    chunk.InstantiateChunk();
                    chunk.chunkGameObject.transform.SetParent(worldHolder);
                }

            DebugUtils.LogConfirmation("World generated!");
        }

        /// <summary>
        /// Generates the world on a seperate thread.
        /// </summary>
        private void GenerateWorldOnSeperateThread() 
        {
            for (int x = 0; x < Settings.worldSizeInChunks; x++)
                for (int y = 0; y < Settings.worldSizeInChunks; y++)
                {
                    Vector2 relativePos = new Vector2(x, y);
                    Vector3 globalPos = RelativeToGlobalChunkPosition(relativePos);
                    Vector2 offset = worldOffset + new Vector2(globalPos.x, globalPos.z);
                    float[,] heightMap = MapGenerator.GenerateHeightMap(Settings.chunkSize + 1, offset, Settings.heightNoiseSettings, ref minHeight, ref maxHeight);
                    ChunkData chunkData = new ChunkData(heightMap, Settings.chunkMaterial, relativePos, globalPos);
                    Chunks.Add(new Vector2(x, y), new Chunk(chunkData, !Settings.updateChunkVisibility));
                }

            //post full generation calculations ----> 
            for (int x = 0; x < Settings.worldSizeInChunks; x++)
                for (int y = 0; y < Settings.worldSizeInChunks; y++)
                {
                    Chunk chunk = Chunks[new Vector2(x, y)];
                    chunk.data.heightMap = MapGenerator.NormalizeHeightMap(chunk.data.heightMap, minHeight, maxHeight);
                    chunk.data.GenerateChunkTerrainMeshData(Settings.heightMultiplier, Settings.heightCurve, Settings.heightCalculationMethod);
                    lock (chunkQueue)
                    {
                        chunkQueue.Enqueue(chunk);
                    }
                }
        }

        public void UpdateThreadChunkQueue() 
        {
            if (!AwaitingThreadOperations) return;

            while (chunkQueue.Count > 0)
            {
                Chunk chunk = chunkQueue.Dequeue();
                chunk.data.CreateMesh();
                chunk.InstantiateChunk();
                chunk.chunkGameObject.transform.SetParent(worldHolder);
                chunkCount++;

                if (chunkCount >= Settings.worldSizeInChunks * Settings.worldSizeInChunks)
                    AwaitingThreadOperations = false;
            }
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
        /// Deletes the world with all of its data.
        /// </summary>
        public void Delete() 
        {
            if (worldHolder == null) return;
            GameObject.DestroyImmediate(worldHolder.gameObject);
        }
    }

}
