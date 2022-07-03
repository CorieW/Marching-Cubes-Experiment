using System.Collections.Generic;
using UnityEngine;

namespace HeightMap_Attempt
{
    public class HMATextureDrawer : MonoBehaviour
    {
        public static HMATextureDrawer Instance;

        [Header("References")]
        [SerializeField] private SpriteRenderer _sr;
        [SerializeField] private HMAHeightMapGenerator _heightMapGenerator;

        private void Awake()
        {
            Instance = this;
            
            _sr = GetComponent<SpriteRenderer>();
        }

        public void Generate()
        {
            int worldSize = 128;
            int chunkSize = 8;

            float[,] heightMap = new float[worldSize * chunkSize, worldSize * chunkSize];
            for (int z = 0; z < worldSize; z++)
            {
                for (int x = 0; x < worldSize; x++)
                {
                    float[,] newHeightMap = _heightMapGenerator.GenerateChunk(new Vector3Int(worldSize * chunkSize, 256, worldSize * chunkSize), new Vector2Int(chunkSize, chunkSize), new Vector2Int(x, z));
                    for (int z1 = 0; z1 < chunkSize; z1++)
                    {
                        for (int x1 = 0; x1 < chunkSize; x1++)
                        {
                            heightMap[(x * chunkSize) + x1, (z * chunkSize) + z1] = newHeightMap[x1, z1];
                        }
                    }
                }
            }
            Generate(heightMap);
            // float[,] heightMap = _heightMapGenerator.GenerateChunk(new Vector3Int(worldSize, 256, worldSize), new Vector2Int(256, 256), new Vector2Int(1, 1));
            // Generate(heightMap);
        }

        public void Generate(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.zero, 1);
            _sr.sprite = sprite;

            Color[] colors = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colors[(y * width) + x] = new Color(heightMap[x, y], heightMap[x, y], heightMap[x, y]);
                }
            }

            texture.SetPixels(colors);
            texture.Apply();
        }
    }
}