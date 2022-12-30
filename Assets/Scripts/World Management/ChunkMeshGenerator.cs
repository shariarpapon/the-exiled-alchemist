using UnityEngine;

namespace Main.WorldManagement
{
    internal static class ChunkMeshGenerator
    {
        internal static Mesh GenerateChunkMesh(int chunkSize, float[,] heightMap)
        {
            int vertCount = chunkSize + 1;
            Vector3[] verticies = new Vector3[vertCount * vertCount];

            for (int x = 0; x < vertCount; x++)
                for (int y = 0; y < vertCount; y++)
                {
                    int index = x * vertCount + y;
                    float height = heightMap[x, y];
                    verticies[index] = new Vector3(x, height, y);
                }

            Mesh mesh = new()
            {
                vertices = verticies,
                triangles = GetChunkMeshTriangles(chunkSize)
            };
            return mesh;
        }

        private static int[] GetChunkMeshTriangles(int chunkSize)
        {
            int[] tris = new int[chunkSize * chunkSize * 6];
            int t = 0, v = 0;
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    tris[t] = v + 1;
                    tris[t + 1] = v + chunkSize + 1;
                    tris[t + 2] = v + 0;
                    tris[t + 3] = v + chunkSize + 2;
                    tris[t + 4] = v + chunkSize + 1;
                    tris[t + 5] = v + 1;
                    t += 6;
                    v++;
                }
                v++;
            }
            return tris;
        }

    }
}