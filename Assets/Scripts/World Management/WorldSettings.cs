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
        public int seed;

        [Tooltip("The number of chunks per axis (X, Z) of the world")]
        public int worldSizeInChunks;
        public int WorldSize { get { return worldSizeInChunks * chunkSize; } }

        public Vector3 worldOffset;
        
        [Header("Chunk Settings")]
        public int chunkSize;
        public Material chunkMaterial;
        public NoiseSettings heightNosieSettings;
    }
}
