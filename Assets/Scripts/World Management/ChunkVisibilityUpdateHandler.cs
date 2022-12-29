using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main.WorldManagement
{
    /// <summary>
    /// This class handles chunk updates.
    /// </summary>
    public class ChunkVisibilityUpdateHandler : MonoBehaviour
    {
        private const int BUFFER_BYTESIZE = sizeof(float) * 2 + sizeof(int);

        [SerializeField]
        private ComputeShader visibilityComputer;
        [SerializeField]
        private ChunkUpdateMethod chunkUpdateMethod;
        [SerializeField]
        private Transform viewer;
        [SerializeField]
        private int maxChunksVisibleRadially;
        [SerializeField]
        private float maxViewDistance = 100;

        private World world;
        private Dictionary<Vector2, Chunk> chunks;
        private ChunkUpdateData[] ChunkDataBuffer;
        private int maxVisibleChunkCount = 0;

        public void Init(World world) 
        {
            this.world = world;
            this.chunks = world.Chunks;
            maxVisibleChunkCount = maxChunksVisibleRadially * 2 * maxChunksVisibleRadially * 2;

            visibilityComputer.SetFloat("maxChunkViewDist", maxViewDistance);
            visibilityComputer.SetFloat("chunkSize", world.Settings.chunkSize);
            visibilityComputer.SetFloat("chunkExtent", world.Settings.chunkSize / 2.0f);
            visibilityComputer.SetInt("chunksVisibleRadially", maxChunksVisibleRadially);
            visibilityComputer.SetInt("visibleChunkPerAxis", maxChunksVisibleRadially * 2);
            ChunkDataBuffer = new ChunkUpdateData[maxVisibleChunkCount];
        }

        #region Update Methods
        private void Update()
        {
            if (chunkUpdateMethod == ChunkUpdateMethod.Update)
                UpdateChunkVisibility();
        }

        private void FixedUpdate()
        {
            if (chunkUpdateMethod == ChunkUpdateMethod.FixedUpdate)
                UpdateChunkVisibility();
        }

        private void LateUpdate()
        {
            if (chunkUpdateMethod == ChunkUpdateMethod.LateUpdate)
                UpdateChunkVisibility();
        }
        #endregion

        /// <summary>
        /// Updates the visibility of the chunks based on its distance from the viewer.
        /// </summary>
        public void UpdateChunkVisibility()
        {
            if (world == null) return;

            ComputeBuffer dataBuffer = new ComputeBuffer(maxVisibleChunkCount, BUFFER_BYTESIZE);
            dataBuffer.SetData(ChunkDataBuffer);

            visibilityComputer.SetBuffer(0, "dataBuffer", dataBuffer);
            visibilityComputer.SetVector("viewerPosition", viewer.position);

            //Check the previously visible chunks with new viewer position
            visibilityComputer.Dispatch(0, world.Settings.worldSize , world.Settings.worldSize, 1);
            dataBuffer.GetData(ChunkDataBuffer);
            CheckVisibility();

            //Check visible chunks with updated viewer position
            Vector2 viewerChunkCoord = world.GetChunkLocalPosition(viewer.position);
            visibilityComputer.SetInt("viewerChunkCoordX", (int)viewerChunkCoord.x);
            visibilityComputer.SetInt("viewerChunkCoordY", (int)viewerChunkCoord.y);

            visibilityComputer.Dispatch(0, world.Settings.worldSize, world.Settings.worldSize, 1);
            dataBuffer.GetData(ChunkDataBuffer);
            dataBuffer.Dispose();

            CheckVisibility();
        }

        private void CheckVisibility()
        {
            foreach (ChunkUpdateData data in ChunkDataBuffer)
            {
                if (world.Chunks.ContainsKey(data.coord))
                {
                    if (data.setActive == 1) world.Chunks[data.coord].SetVisible(true);
                    else world.Chunks[data.coord].SetVisible(false);
                }
            }
        }

        [Serializable]
        public enum ChunkUpdateMethod
        {
            Update,
            FixedUpdate,
            LateUpdate
        }

        private struct ChunkUpdateData 
        {
            public Vector2 coord;
            public int setActive;
        }

    }
}
