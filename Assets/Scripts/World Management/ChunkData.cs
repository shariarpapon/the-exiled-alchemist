using UnityEngine;

namespace Main.WorldManagement
{
    public class ChunkData
    {
        private int chunkSize;
        private int chunkVertSize;
        private NoiseSettings heightNoiseSettings;

        public readonly float[,] heightMap;
        public readonly Mesh chunkMesh;
        public readonly Material chunkMaterial;
        public readonly Vector2 relativePosition;
        public readonly Vector3 globalPosition;

        public ChunkData(int chunkSize, Material chunkMaterial, NoiseSettings heightNoiseSettings, Vector2 offset, Vector2 relativePos, Vector3 globalPos) 
        {
            this.chunkSize = chunkSize;
            this.chunkMaterial = chunkMaterial;
            this.heightNoiseSettings = heightNoiseSettings;
            this.relativePosition = relativePos;
            this.globalPosition = globalPos;
            chunkVertSize = chunkSize + 1;

            //Must pass in chunkVertSize (=chunkSize + 1) since the number of verticies is 1 greater than the actual length.
            heightMap = MapGenerator.GeneratePerlinNoiseMap(chunkVertSize, offset, heightNoiseSettings);
            chunkMesh = ChunkMeshGenerator.GenerateChunkMesh(chunkSize, heightMap);
        }

    }
}