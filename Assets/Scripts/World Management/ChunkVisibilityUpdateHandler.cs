using Main.DebuggingUtility;
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
        [SerializeField]
        private Transform viewer;
        [SerializeField]
        private ComputeShader visibilityComputer;
        [SerializeField]
        private ChunkUpdateMethod updateMethod;
      
        [SerializeField, Tooltip("This will auto round to the nearest multiple of 16 that is greater than or equal to 16 due to the thread compatibility")]
        private int visibleChunksPerAxis;
        [SerializeField]
        private float maxViewDistance = 100;

        private World world;
        private Dictionary<Vector2, Chunk> chunks;
        private ChunkUpdateData[] ChunkDataBuffer;
        private int maxVisibleChunks;
        private int chunksVisibleRadially;

        private const int BUFFER_BYTESIZE = sizeof(float) * 2 + sizeof(int);
        private const int CHUNK_UPDATE_BUFFER_THREAD_COUNT = 16;
        private int chunkUpdateThreadGroups;

        public void Init(World world) 
        {
            this.world = world;
            this.chunks = world.Chunks;

            ValidateChunksPerAxisWithThreadCount();

            chunksVisibleRadially = visibleChunksPerAxis / 2;
            maxVisibleChunks = visibleChunksPerAxis * visibleChunksPerAxis;
            chunkUpdateThreadGroups = visibleChunksPerAxis / CHUNK_UPDATE_BUFFER_THREAD_COUNT;

            visibilityComputer.SetFloat("maxViewDistance", maxViewDistance);
            visibilityComputer.SetFloat("chunkSize", world.Settings.chunkSize);
            visibilityComputer.SetFloat("chunkExtent", world.Settings.chunkSize / 2.0f);
            visibilityComputer.SetInt("chunksVisibleRadially", chunksVisibleRadially);
            visibilityComputer.SetInt("visibleChunksPerAxis", visibleChunksPerAxis);
            ChunkDataBuffer = new ChunkUpdateData[maxVisibleChunks];
        }

        #region Update Methods
        private void Update()
        {
            if (updateMethod == ChunkUpdateMethod.Update)
                UpdateChunkVisibility();
        }

        private void FixedUpdate()
        {
            if (updateMethod == ChunkUpdateMethod.FixedUpdate)
                UpdateChunkVisibility();
        }

        private void LateUpdate()
        {
            if (updateMethod == ChunkUpdateMethod.LateUpdate)
                UpdateChunkVisibility();
        }
        #endregion

        /// <summary>
        /// Updates the visibility of the chunks based on its distance from the viewer.
        /// </summary>
        public void UpdateChunkVisibility()
        {
            if (world == null) return;

            ComputeBuffer dataBuffer = new ComputeBuffer(maxVisibleChunks, BUFFER_BYTESIZE);
            dataBuffer.SetData(ChunkDataBuffer);

            visibilityComputer.SetBuffer(0, "dataBuffer", dataBuffer);
            visibilityComputer.SetVector("viewerPosition", viewer.position);

            //Check the previously visible chunks with new viewer position
            visibilityComputer.Dispatch(0, chunkUpdateThreadGroups, chunkUpdateThreadGroups, 1);
            dataBuffer.GetData(ChunkDataBuffer);
            CheckVisibility();

            //Check visible chunks with updated viewer position
            Vector2 viewerChunkCoord = world.GlobalToLocalChunkPosition(viewer.position);
            visibilityComputer.SetInt("viewerChunkCoordX", (int)viewerChunkCoord.x);
            visibilityComputer.SetInt("viewerChunkCoordY", (int)viewerChunkCoord.y);

            visibilityComputer.Dispatch(0, chunkUpdateThreadGroups, chunkUpdateThreadGroups, 1);
            dataBuffer.GetData(ChunkDataBuffer);
            dataBuffer.Dispose();

            CheckVisibility();
        }

        private void CheckVisibility()
        {
            foreach (ChunkUpdateData data in ChunkDataBuffer)
            {
                if (chunks.ContainsKey(data.coord))
                {
                    if (data.setActive == 1) world.Chunks[data.coord].SetVisible(true);
                    else world.Chunks[data.coord].SetVisible(false);
                }
            }
        }

        private void OnValidate()
        {
            ValidateChunksPerAxisWithThreadCount();
        }

        //NOTE: The number of visible chunks per axis must be greater than and a multiple of CHUNK_UPDATE_BUFFER_THREAD_COUNT.
        private void ValidateChunksPerAxisWithThreadCount() 
        {
            visibleChunksPerAxis -= visibleChunksPerAxis % CHUNK_UPDATE_BUFFER_THREAD_COUNT;
            if (visibleChunksPerAxis < CHUNK_UPDATE_BUFFER_THREAD_COUNT) visibleChunksPerAxis = CHUNK_UPDATE_BUFFER_THREAD_COUNT;
        }

        [Serializable]
        public enum ChunkUpdateMethod
        {
            Update,
            FixedUpdate,
            LateUpdate
        }

        [System.Serializable]
        private struct ChunkUpdateData 
        {
            public Vector2 coord;
            public int setActive;
        }

    }
}
