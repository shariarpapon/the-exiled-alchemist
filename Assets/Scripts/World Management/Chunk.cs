using Main.DebuggingUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.WorldManagement
{
    /// <summary>
    /// This class contains all the data of a chunk.
    /// </summary>
    public class Chunk
    {
        public GameObject chunkGameObject;

        private Vector2 localPosition;
        private Vector2 globalPosition;

        //TODO: Instead of taking the data directly, first generate ChunkData then pass that in.
        public Chunk(Mesh mesh, Material chunkMaterial, Vector2 localPosition, Vector3 globalPosition) 
        {
            this.localPosition = localPosition;
            this.globalPosition = globalPosition;

            chunkGameObject = new GameObject($"Chunk {localPosition}");

            MeshFilter filter = chunkGameObject.AddComponent<MeshFilter>();
            filter.sharedMesh = mesh;

            chunkGameObject.AddComponent<MeshRenderer>().material = chunkMaterial;

            MeshCollider collider = chunkGameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;
            collider.isTrigger = false;
            collider.convex = false;

            chunkGameObject.transform.position = globalPosition;

            SetVisible(false);
        }

        public void SetVisible(bool visible) 
        {
            chunkGameObject.SetActive(visible);
        }

    }

}