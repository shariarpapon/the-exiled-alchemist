using UnityEngine;

namespace Everime.WorldManagement
{
    public class ChunkData
    {
        public float[,] heightMap;
        public readonly Vector2 relativePosition;
        public readonly Vector3 globalPosition;
        public readonly Material chunkMaterial;

        //Mesh data
        public Mesh chunkMesh;
        public Vector3[] vertices;
        public int[] triangles;

        public ChunkData(float[,] heightMap, Material chunkMaterial, Vector2 relativePosition, Vector3 globalPosition)
        {
            this.heightMap = heightMap;
            this.chunkMaterial = chunkMaterial;
            this.relativePosition = relativePosition;
            this.globalPosition = globalPosition;
        }

        public void GenerateChunkTerrainMeshData(float heightMultiplier, AnimationCurve heightCurve, HeightCalculationMethod method) 
        {
            vertices = ChunkMeshDataGenerator.GenerateMeshVertices(heightMap, heightMultiplier, heightCurve, method);
            triangles = ChunkMeshDataGenerator.GenerateMeshTriangles(heightMap.GetLength(0) - 1);
        }

        public void CreateMesh() 
        {
            chunkMesh = new Mesh();
            chunkMesh.vertices = vertices;
            chunkMesh.triangles = triangles;
            chunkMesh.RecalculateNormals();
        }
    }
}