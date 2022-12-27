using System.Collections;
using System.Collections.Generic;
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
        public int seed;
        public int worldSize;
        public Vector2 worldOffset;

        [Header("Chunk Settings")]
        public int chunkSize;
        public int maxChunkVisibleDistance = 100;
        public NoiseSettings chunkNoiseSettings;
    }
}
