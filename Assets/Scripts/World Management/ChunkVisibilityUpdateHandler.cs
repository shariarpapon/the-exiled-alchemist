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
        private int maxChunkCount = 0;

        public void Init(World world) 
        {
            this.world = world;
            this.chunks = world.Chunks;
            maxChunkCount = maxChunksVisibleRadially * 2 * maxChunksVisibleRadially * 2;

            visibilityComputer.SetFloat("maxChunkViewDist", maxViewDistance);
            visibilityComputer.SetFloat("chunkSize", world.Settings.chunkSize);
            visibilityComputer.SetFloat("chunkExtent", world.Settings.chunkSize / 2.0f);
            visibilityComputer.SetInt("chunksVisibleRadially", maxChunksVisibleRadially);
            visibilityComputer.SetInt("visibleChunkPerAxis", maxChunksVisibleRadially * 2);
            ChunkDataBuffer = new ChunkUpdateData[maxChunkCount];
        }

        #region Update Methods
        private void Update()
        {
            if (chunkUpdateMethod == ChunkUpdateMethod.Update)
                UpdateChunks();
        }

        private void FixedUpdate()
        {
            if (chunkUpdateMethod == ChunkUpdateMethod.FixedUpdate)
                UpdateChunks();
        }

        private void LateUpdate()
        {
            if (chunkUpdateMethod == ChunkUpdateMethod.LateUpdate)
                UpdateChunks();
        }
        #endregion

        public void UpdateChunks()
        {
            if (world == null) return;

            ComputeBuffer dataBuffer = new ComputeBuffer(maxChunkCount, BUFFER_BYTESIZE);
            dataBuffer.SetData(ChunkDataBuffer);

            visibilityComputer.SetBuffer(0, "dataBuffer", dataBuffer);
            visibilityComputer.SetVector("viewerPosition", viewer.position);

            //Check the previously visible chunks with new viewPoint
            visibilityComputer.Dispatch(0, world.Settings.worldSize , world.Settings.worldSize, 1);
            dataBuffer.GetData(ChunkDataBuffer);
            CheckVisibility();

            //Check visible chunks with updated chunk coords
            Vector2 viewerChunkCoord = world.GetChunkCoordinate(viewer.position);
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
