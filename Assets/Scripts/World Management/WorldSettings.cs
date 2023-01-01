using UnityEngine;

namespace Main.WorldManagement
{        
    /// <summary>
    /// This scriptable object contains the settings for world generation.
    /// </summary>
    [CreateAssetMenu(fileName = "New World Settings", menuName = "World Generation/World Settings" )]
    public class WorldSettings : ScriptableObject
    {
        [Header("Global Settings")]
        public bool useRandomSeed;

        [SerializeField]
        private string seed;
        public int Seed { get { return seed.GetHashCode(); } }

        [Tooltip("The number of chunks per axis (X, Z) of the world")]
        public int worldSizeInChunks;
        public int WorldSize { get { return worldSizeInChunks * chunkSize; } }

        public Vector3 worldOffset;
        
        [Header("Chunk Settings")]
        public int chunkSize;
        public Material chunkMaterial;
        public NoiseSettings heightNoiseSettings;
        public bool updateChunkVisibility = true;

        /// <summary>
        /// Returns a seed that is generated from appending the passed in seed with the gloabl seed.
        /// </summary>
        public int AppendedSeed(string appendSeed) 
        {
            return (seed + appendSeed).GetHashCode();
        }

        /// <summary>
        /// Randomizes the global seed.
        /// </summary>
        public void RandomizeSeed() 
        {
            seed = System.IO.Path.GetRandomFileName();
        }
    }
}
