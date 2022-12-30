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
        public WorldSettings worldSettings; 
        [SerializeField]
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
            if (worldSettings.heightNosieSettings.scale <= 0) worldSettings.heightNosieSettings.scale = 0.0001f;
            if (worldSettings.useRandomSeed) worldSettings.seed = DateTimeOffset.Now.ToUnixTimeMilliseconds().GetHashCode();

            //Chunk size must be greater than zero.
            if (worldSettings.chunkSize <= 0)
                worldSettings.chunkSize = 1;
        }

        private void ClearWorld()
        {
            world = null;

            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
        }

    }

}
