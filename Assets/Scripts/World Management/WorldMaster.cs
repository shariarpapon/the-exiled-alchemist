using Everime.DebuggingUtility;
using System;
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

        private ChunkVisibilityUpdateHandler chunkVisibilityUpdater;
        private World world;

        private void Start()
        {
            if (createWorldOnStart) CreateWorld();
        }

        private void Update()
        {

//Only for editor purposes
#if UNITY_EDITOR 
            if (world != null)
            {
                chunkVisibilityUpdater?.UpdateChunkVisibility();
            }
#endif

        }

        /// <summary>
        /// This method first clears any existing world and generates a new one with the assigned world settings.
        /// </summary>
        public World CreateWorld()
        {
            if (worldSettings == null)
            {
                DebugUtils.LogFailed("World creation failed! Make sure to assign the worldSettings");
                return null;
            }

            chunkVisibilityUpdater = GetComponent<ChunkVisibilityUpdateHandler>();

            ClearExistingWorld();
            InitWorldData();

            world = new World(worldSettings);
            chunkVisibilityUpdater.Init(world);
            return world;
        }

        //Initializes world data and resolves any compatibility issues.
        private void InitWorldData()
        {
            if (worldSettings.heightNoiseSettings.scale <= 0) worldSettings.heightNoiseSettings.scale = 0.0001f;
            if (worldSettings.useRandomSeed) worldSettings.RandomizeSeed();

            //Chunk size must be greater than zero.
            if (worldSettings.chunkSize <= 0)
                worldSettings.chunkSize = 1;
        }

        public void ClearExistingWorld()
        {
            world?.Delete();
            world = null;
        }

    }

}
