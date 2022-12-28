using Main.DebuggingUtility;
using System;
using UnityEngine;

namespace Main.WorldManagement
{
    /// <summary>
    /// This class is used for creating a world and updating its chunks at runtime.
    /// </summary>
    public class WorldMaster : MonoBehaviour
    {
        private const int MINIMUM_CHUNK_SIZE = 16;

        public WorldSettings worldSettings;

        [SerializeField]
        private ChunkVisibilityUpdateHandler chunkUpdateHandler;
        [SerializeField, Space]
        private bool createWorldOnStart = false;

        private World world;

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
            chunkUpdateHandler.Init(world);
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
            if (chunkUpdateHandler == null) return;
            //Update chunks
        }

        private void ClearWorld()
        {
            world = null;
            chunkUpdateHandler = null;

            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
        }

    }

}
