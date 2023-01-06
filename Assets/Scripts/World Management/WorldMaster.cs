using Everime.DebuggingUtility;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Everime.WorldManagement
{
    /// <summary>
    /// This class is used for creating a world and updating its chunks at runtime.
    /// </summary>
    [RequireComponent(typeof(ChunkVisibilityUpdateHandler)), ExecuteAlways]
    public class WorldMaster : MonoBehaviour
    {
        public WorldSettings worldSettings; 
        [SerializeField]
        private bool createWorldOnStart = false;
        [SerializeField]
        private bool worldVisibleByDefault = false;

        private ChunkVisibilityUpdateHandler chunkVisibilityUpdater;
        private World world;

        //world generation
        [HideInInspector]
        public float minHeight;
        [HideInInspector]
        public float maxHeight;
        [HideInInspector]
        public int appendedSeed;
        public System.Random prng;
        private Vector2 worldOffset;
        private Transform worldHolder;

        //chunk generation thread
        public bool AwaitingThreadOperations { get; private set; }
        private Queue<Chunk> chunkQueue;
        private int threadReleasedChunkCount;

        private void Start()
        {
            if (createWorldOnStart) GenerateWorld();
        }

        private void Update()
        {
#if UNITY_EDITOR 
            if (world != null)
            {
                chunkVisibilityUpdater?.UpdateChunkVisibility();
            }
#endif
            UpdateThreadReleasedChunks();
        }
       
        #region World Generation

        /// <summary>
        /// First clears any existing world, then generates a new world.
        /// </summary>
        public void GenerateWorld()
        {
            if (worldSettings == null)
            {
                DebugUtils.LogFailed("World creation failed! Make sure to assign the worldSettings");
                return;
            }

            ClearExistingWorld();

            chunkVisibilityUpdater = GetComponent<ChunkVisibilityUpdateHandler>();

            if (worldSettings.heightNoiseSettings.scale <= 0) worldSettings.heightNoiseSettings.scale = 0.0001f;
            if (worldSettings.useRandomSeed) worldSettings.RandomizeSeed();

            if (worldSettings.chunkSize <= 0)
                worldSettings.chunkSize = 1;

            world = new World(worldSettings);
            chunkVisibilityUpdater.Init(world);

            minHeight = float.MaxValue;
            maxHeight = float.MinValue;
            appendedSeed = worldSettings.AppendedSeed("overworld");

            prng = new System.Random(appendedSeed);
            int noiseRange = 9_999_999;
            worldOffset = new Vector2((int)(prng.NextDouble() * 2 * noiseRange - noiseRange), (int)(prng.NextDouble() * 2 * noiseRange - noiseRange));

            worldHolder = new GameObject($"World_{appendedSeed}").transform;
            worldHolder.position = Vector3.zero;

            threadReleasedChunkCount = 0;
            chunkQueue = new Queue<Chunk>();
            ThreadStart threadStart = new ThreadStart(
                delegate { GenerateChunks(); });

            Thread thread = new Thread(threadStart);
            thread.Start();
            AwaitingThreadOperations = true;
        }

        private void GenerateChunks()
        {
            for (int x = 0; x < worldSettings.worldSizeInChunks; x++)
                for (int y = 0; y < worldSettings.worldSizeInChunks; y++)
                {
                    Vector2 relativePos = new Vector2(x, y);
                    Vector3 globalPos = world.RelativeToGlobalChunkPosition(relativePos);
                    Vector2 offset = worldOffset + new Vector2(globalPos.x, globalPos.z);
                    float[,] heightMap = MapGenerator.GenerateHeightMap(worldSettings.chunkSize + 1, offset, worldSettings.heightNoiseSettings, ref minHeight, ref maxHeight);
                    ChunkData chunkData = new ChunkData(heightMap, worldSettings.chunkMaterial, relativePos, globalPos);
                    world.chunks.Add(new Vector2(x, y), new Chunk(chunkData, worldVisibleByDefault));
                }

            //post full generation calculations ----> 
            for (int x = 0; x < worldSettings.worldSizeInChunks; x++)
                for (int y = 0; y < worldSettings.worldSizeInChunks; y++)
                {
                    Chunk chunk = world.chunks[new Vector2(x, y)];
                    chunk.chunkData.heightMap = MapGenerator.NormalizeHeightMap(chunk.chunkData.heightMap, minHeight, maxHeight, worldSettings.worldUnitSize, chunk.chunkData.globalPosition);
                    chunk.GenerateTerrainMeshData(worldSettings.heightMultiplier, worldSettings.heightCurve, worldSettings.vertexGradient, worldSettings.heightCalculationMethod);
                    lock (chunkQueue)
                    {
                        chunkQueue.Enqueue(chunk);
                    }
                }
        }

        /// <summary>
        /// Finalizes and updates any chunk that is released by the world generation thread.
        /// </summary>
        public void UpdateThreadReleasedChunks()
        {
            if (!AwaitingThreadOperations) return;

            while (chunkQueue.Count > 0)
            {
                Chunk chunk = chunkQueue.Dequeue();
                chunk.InstantiateChunk();
                chunk.chunkGameObject.transform.SetParent(worldHolder);
                threadReleasedChunkCount++;

                if (threadReleasedChunkCount >= worldSettings.worldSizeInChunks * worldSettings.worldSizeInChunks)
                    AwaitingThreadOperations = false;
            }
        }

        #endregion

        public void ClearExistingWorld()
        {
            if (worldHolder == null) return;
            DestroyImmediate(worldHolder.gameObject);
            world = null;
        }

    }

}
