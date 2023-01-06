using UnityEngine;

namespace Everime.WorldManagement
{
    /// <summary>
    /// This class contains all the data of a chunk.
    /// </summary>
    public class Chunk
    {
        public readonly ChunkData chunkData;
        public MeshData meshData;

        public GameObject chunkGameObject;

        private bool defaultVisibility;

        public Chunk(ChunkData data, bool defaultVisibility)
        {
            this.chunkData = data;
            this.defaultVisibility = defaultVisibility;
        }

        public void InstantiateChunk()
        {
            chunkGameObject = new GameObject($"Chunk {chunkData.relativePosition}");

            MeshFilter filter = chunkGameObject.AddComponent<MeshFilter>();
            filter.sharedMesh = meshData.CreateMeshFromData();

            chunkGameObject.AddComponent<MeshRenderer>().material = chunkData.chunkMaterial;

            MeshCollider collider = chunkGameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = filter.sharedMesh;
            collider.isTrigger = false;
            collider.convex = false;

            chunkGameObject.transform.position = chunkData.globalPosition;

            SetVisible(defaultVisibility);
        }

        public void GenerateTerrainMeshData(float heightMultiplier, AnimationCurve heightCurve, Gradient gradient,  HeightCalculationMethod method) 
        {
            meshData = ChunkMeshDataGenerator.GenerateMeshData(chunkData.heightMap, heightMultiplier, heightCurve, gradient, method);
        }

        public void SetVisible(bool visible) 
        {
            if(chunkGameObject != null)
                chunkGameObject.SetActive(visible);
        }

    }

}