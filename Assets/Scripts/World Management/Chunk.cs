using UnityEngine;

namespace Everime.WorldManagement
{
    /// <summary>
    /// This class contains all the data of a chunk.
    /// </summary>
    public class Chunk
    {
        public readonly ChunkData data;
        public GameObject chunkGameObject;
        private bool defaultVisibility;

        public Chunk(ChunkData data, bool defaultVisibility)
        {
            this.data = data;
            this.defaultVisibility = defaultVisibility;
        }

        public void InstantiateChunk()
        {
            chunkGameObject = new GameObject($"Chunk {data.relativePosition}");

            MeshFilter filter = chunkGameObject.AddComponent<MeshFilter>();
            filter.sharedMesh = data.chunkMesh;

            chunkGameObject.AddComponent<MeshRenderer>().material = data.chunkMaterial;

            MeshCollider collider = chunkGameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = data.chunkMesh;
            collider.isTrigger = false;
            collider.convex = false;

            chunkGameObject.transform.position = data.globalPosition;

            SetVisible(defaultVisibility);
        }

        public void SetVisible(bool visible) 
        {
            if(chunkGameObject != null)
                chunkGameObject.SetActive(visible);
        }

    }

}