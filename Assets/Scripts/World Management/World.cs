using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.DebuggingUtility;

namespace Main.WorldManagement
{
    /// <summary>
    /// The world class generates the world map and holds all data of the particular world.
    /// </summary>
    public class World
    {
        public Chunk[,] chunks;
        public WorldSettings worldSettings;

        public float minHeight;
        public float maxHeight;

        public World(WorldSettings worldSettings) 
        {
            this.worldSettings = worldSettings;
            GenerateWorld();
        }

        private void GenerateWorld() 
        {
            DebugUtils.LogConfirmation("World generated!");
        }

        /// <summary>
        /// Comapres the passed in min/max height with the current min/max height and updates if a new best is found.
        /// </summary>
        public void CompareAndEvaluateHeights(float min, float max)
        {
            if (min < minHeight) minHeight = min;
            if (max > maxHeight) maxHeight = max;
        }
    }

}
