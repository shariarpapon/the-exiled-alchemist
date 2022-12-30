using System.Drawing;
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
        public int worldSize;
        public Vector3 worldOffset;

        [Header("Chunk Settings")]

        [Tooltip("Must be a multiple of 16.")]
        public int chunkSize;
        public Material chunkMaterial;
        public NoiseSettings chunkNoiseSettings;
    }
}
