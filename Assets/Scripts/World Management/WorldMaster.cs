using Main.DebuggingUtility;
using System;
using UnityEngine;

namespace Main.WorldManagement
{
    /// <summary>
    /// This class is used for creating a world and updating its chunks at runtime.
    /// </summary>
    [RequireComponent(typeof(ChunkVisibilityUpdateHandler))]
    public class WorldMaster : MonoBehaviour
    {
        private const int MINIMUM_CHUNK_SIZE = 16;

        public WorldSettings worldSettings;

        [SerializeField, Space]
        private bool createWorldOnStart = false;

        private ChunkVisibilityUpdateHandler chunkVisibilityUpdateHandler;
        private World world;

        private void Awake()
        {
            chunkVisibilityUpdateHandler = GetComponent<ChunkVisibilityUpdateHandler>();
        }

        private void Start()
        {
            if (createWorldOnStart) CreateWorld();
        }

        /// <summary>
        /// This method first clears any existing world and generates a new one with the assigned world settings.
        /// </summary>
        public void CreateWorld()
        {
            if (worldSettings == null)
            {
                DebugUtils.LogFailed("World creation failed! Make sure to assign the worldSettings");
                return;
            }

            ClearWorld();
            InitWorldData();

            world = new World(worldSettings);
            chunkVisibilityUpdateHandler.Init(world);
        }

        //Initializes world data and resolves any compatibility issues.
        private void InitWorldData()
        {
            if (worldSettings.chunkNoiseSettings.scale <= 0) worldSettings.chunkNoiseSettings.scale = 0.0001f;
            if (worldSettings.useRandomSeed) worldSettings.seed = DateTimeOffset.Now.ToUnixTimeMilliseconds().GetHashCode();

            //Minimum chunk size must be a multiple of 16 due to computer shader compatibility.
            worldSettings.chunkSize -= worldSettings.chunkSize % MINIMUM_CHUNK_SIZE;
            if (worldSettings.chunkSize < MINIMUM_CHUNK_SIZE)
                worldSettings.chunkSize = MINIMUM_CHUNK_SIZE;

            if (worldSettings.worldSize < worldSettings.chunkSize)
                worldSettings.worldSize = worldSettings.chunkSize;

            worldSettings.worldSize -= worldSettings.worldSize % worldSettings.chunkSize;
        }

        private void UpdateChunks()
        {
            if (chunkVisibilityUpdateHandler == null) return;
            //Update chunks
        }

        private void ClearWorld()
        {
            world = null;
            chunkVisibilityUpdateHandler = null;

            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
        }

    }

}
