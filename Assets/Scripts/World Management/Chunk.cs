using UnityEngine;

namespace Main.WorldManagement
{
    /// <summary>
    /// This class contains all the data of a chunk.
    /// </summary>
    public class Chunk
    {
        public readonly GameObject chunkGameObject;
        public readonly ChunkData data;

        public Chunk(ChunkData data, bool defaultVisibility)
        {
            this.data = data;

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
            chunkGameObject.SetActive(visible);
        }

    }

}