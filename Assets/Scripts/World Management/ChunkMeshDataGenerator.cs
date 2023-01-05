using UnityEngine;

namespace Everime.WorldManagement
{
    /// <summary>
    /// This class contains methods for chunk mesh generation.
    /// </summary>
    internal static class ChunkMeshDataGenerator
    {
        internal static Vector3[] GenerateMeshVertices(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, HeightCalculationMethod method)
        {
            int meshSize = heightMap.GetLength(0);
            Vector3[] chunkVerts = new Vector3[meshSize * meshSize];

            for (int x = 0; x < meshSize; x++)
                for (int y = 0; y < meshSize; y++)
                {
                    int index = x * meshSize + y;
                    float height = 0;
                    switch (method) 
                    {
                        case HeightCalculationMethod.Square:
                            height = heightMap[x, y] * heightMap[x, y] * heightMultiplier;
                            break;
                        case HeightCalculationMethod.Cube:
                            height = heightMap[x, y] * heightMap[x, y] * heightMap[x, y] * heightMultiplier;
                            break;
                        case HeightCalculationMethod.Curve:
                            height = heightMap[x, y] * heightMultiplier * heightCurve.Evaluate(Mathf.Abs(heightMap[x, y]));
                            break;
                        case HeightCalculationMethod.SquareCurve:
                            height = heightMap[x, y] * heightMap[x, y] * heightMultiplier * heightCurve.Evaluate(Mathf.Abs(heightMap[x, y]));
                            break;
                        case HeightCalculationMethod.CubeCurve:
                            height = heightMap[x, y] * heightMap[x, y] * heightMap[x, y] * heightMultiplier * heightCurve.Evaluate(Mathf.Abs(heightMap[x, y]));
                            break;
                    }
                    //float height = heightMap[x, y] * heightCurve.Evaluate(Mathf.Abs(heightMap[x, y])) * heightMultiplier;
                    chunkVerts[index] = new Vector3(x, height, y);
                }
            //mesh.normals = CalculateNormals(mesh);
            return chunkVerts;
        }

        #region Utils
        private static Vector3[] CalculateNormals(Mesh mesh) 
        {
            Vector3[] normals = new Vector3[mesh.vertices.Length];
            int triCount = mesh.triangles.Length / 3;
            for (int i = 0; i < triCount; i++) 
            {
                int triangleIndex = i * 3;
                int indexA = mesh.triangles[triangleIndex];
                int indexB= mesh.triangles[triangleIndex + 1];
                int indexC = mesh.triangles[triangleIndex + 2];

                Vector3 triangleNormal = SurfaceNormalFromIndices(mesh.vertices, indexA, indexB, indexC);
                normals[indexA] = triangleNormal;
                normals[indexB] = triangleNormal;
                normals[indexC] = triangleNormal;
            }

            for (int i = 0; i < normals.Length; i++) 
            {
                normals[i].Normalize();
            }

            return normals;
        }

        private static Vector3 SurfaceNormalFromIndices(Vector3[] vertices, int a, int b, int c) 
        {
            Vector3 pointA = vertices[a];
            Vector3 pointB = vertices[b];
            Vector3 pointC = vertices[c];

            Vector3 sideAB = pointB - pointA;
            Vector3 sideAC = pointC - pointB;
            return Vector3.Cross(sideAB, sideAC).normalized;
        }

        internal static int[] GenerateMeshTriangles(int chunkSize)
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
        #endregion

    }
}