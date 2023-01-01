using UnityEngine;

namespace Main.WorldManagement
{
    public class ChunkData
    {
        public float[,] heightMap;

        public readonly Mesh chunkMesh;
        public readonly Material chunkMaterial;
        public readonly Vector2 relativePosition;
        public readonly Vector3 globalPosition;

        public ChunkData(float[,] heightMap, Material chunkMaterial, Vector2 relativePosition, Vector3 globalPosition)
        {
            this.chunkMaterial = chunkMaterial;
            this.relativePosition = relativePosition;
            this.globalPosition = globalPosition;
            chunkMesh = ChunkMeshGenerator.GenerateMesh(heightMap);
        }

    }
}