using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeightMap_Attempt
{
    public class HMAWorld : MonoBehaviour
    {
        [Header("Dimensions")]
        [SerializeField] private int _width = 1024;
        [SerializeField] private int _length = 1024;

        [Header("Generation")]
        [SerializeField] private HMAHeightMapGenerator _heightMapGenerator;

        private void Start()
        {
            GenerateChunks();
        }

        public void GenerateChunks()
        {
            Vector3Int worldSize = new Vector3Int(_width, Global.CHUNK_HEIGHT, _length);

            for (int z = 0; z < _length / Global.CHUNK_LENGTH; z++)
            {
                for (int x = 0; x < _width / Global.CHUNK_WIDTH; x++)
                {
                    Vector2Int globalChunkPos = new Vector2Int(x * Global.CHUNK_WIDTH, z * Global.CHUNK_LENGTH);
                    float[,] heightMap = _heightMapGenerator.GenerateChunk(worldSize, new Vector2Int(Global.CHUNK_WIDTH + 1, Global.CHUNK_LENGTH + 1), new Vector2Int(x, z), 1);
                    GameObject meshObj = HMAMeshGenerator.Instance.GenerateNewChunkMesh2(new Vector2Int(x, z), heightMap, 1);
                }
            }
        }
    }
}