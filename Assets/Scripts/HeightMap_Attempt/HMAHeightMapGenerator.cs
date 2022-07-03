using System;
using UnityEngine;

namespace HeightMap_Attempt
{
    public class HMAHeightMapGenerator : MonoBehaviour
    {
        [Header("Noise Settings")]
        [SerializeField] private int _seed;
        [SerializeField] private float _scale = 200;
        [SerializeField] private int _octaves = 4;
        [SerializeField] private bool _fallOffEnabled = true;
        [Range(0f, 1f)]
        [SerializeField] private float _fallOffSpeed = 0.1f;
        [SerializeField] private float _fallOffMultiplier = 1;
        [Range(0, 1)]
        [SerializeField] private float _maxFallOff = 1;
        [SerializeField] private float _multiplier = 1;

        /// <summary>
        /// Generates a height map.
        /// <param name="totalSize">The total size of the generation with all chunks.</param>
        /// <param name="stitch">Used to stitch together chunk meshes.</param>
        /// </summary>
        public float[,] GenerateChunk(Vector3Int totalSize, Vector2Int chunkSize, Vector2Int chunkOffset, int stitch = 0)
        {
            float[,] heightMap = PerlinNoise.GeneratePerlinNoise(chunkSize.x + stitch, chunkSize.y + stitch, _seed, _scale, _octaves, (chunkSize * chunkOffset), 0.5f, 2f, 0f, PerlinNoise.NormalizeMode.Global);

            int halfTotalWidth = totalSize.x / 2;
            int halfTotalHeight = totalSize.z / 2;
            Vector2Int center = new Vector2Int(halfTotalWidth, halfTotalHeight);
            float maxDist = Mathf.Sqrt((halfTotalWidth * halfTotalWidth) + (halfTotalHeight * halfTotalHeight));

            for (int y = 0; y < heightMap.GetLength(1); y++)
            {
                for (int x = 0; x < heightMap.GetLength(0); x++)
                {
                    // Distance away from center of the world.
                    float dist = Vector2Int.Distance((chunkSize * chunkOffset) + new Vector2Int(x, y), center);
                    // Distance from 0 (0 being center) to 1 (1 being edge).
                    float invLerpedDist = _fallOffEnabled ? Mathf.InverseLerp(maxDist * (1 - _fallOffSpeed), 0, dist) : 1;

                    // Calculate the height falloff.
                    float fallOff = Mathf.Clamp01(Mathf.Max(invLerpedDist / _fallOffMultiplier, _maxFallOff));

                    heightMap[x, y] = Mathf.Clamp((heightMap[x, y] * fallOff) * _multiplier, 0, totalSize.y);
                }
            }

            return heightMap;
        }
    }
}