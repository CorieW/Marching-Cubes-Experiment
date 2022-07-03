using System;
using UnityEngine;

namespace Test
{
    public static class Global
    {
        public const int CHUNK_WIDTH = 32;
        public const int CHUNK_HEIGHT = 32;
        public const int CHUNK_LENGTH = 32;
    }

    [System.Serializable]
    public class Chunk
    {
        public Vector3Int ChunkPos { get; }
        public TerrainVertexPoint[,,] Grid { get; }

        public Chunk(Vector3Int chunkPos, TerrainVertexPoint[,,] grid)
        {
            this.ChunkPos = chunkPos;
        }

        public void Save()
        {

        }
    }

    [System.Serializable]
    public class TerrainVertexPoint
    {
        public double Value { get; set; }
        public int MaterialID { get; set; }
    }

    public class WorldGrid
    {
        private Chunk[,,] chunkGrid;

        public void Load(Vector3Int chunkPos)
        {
            bool loaded = false;

            if (loaded) {
                // Load chunk from files
            }
            else {
                // Generate chunk
            }
        }

        public void Unload(Vector3Int chunkPos)
        {
            // Save chunk
            // Destory chunk object
            // Remove from chunk grid
        }

        // Get chunk at vertex point positions
        public Chunk GetChunkAtWorldPosition(Vector3Int worldPos)
        {
            Vector3Int chunkPos = new Vector3Int(
                (int)(worldPos.x / Global.CHUNK_WIDTH),
                (int)(worldPos.y / Global.CHUNK_HEIGHT),
                (int)(worldPos.z / Global.CHUNK_LENGTH)
            );
            return chunkGrid[chunkPos.x, chunkPos.y, chunkPos.z];
        }

        public TerrainVertexPoint GetVertexPointAtWorldPosition(Vector3Int worldPos)
        {
            Chunk chunk = GetChunkAtWorldPosition(worldPos);
            Vector3Int vertexPos = new Vector3Int(
                worldPos.x % Global.CHUNK_WIDTH,
                worldPos.y % Global.CHUNK_HEIGHT,
                worldPos.z % Global.CHUNK_LENGTH
            );
            return chunk.Grid[vertexPos.x, vertexPos.y, vertexPos.z];
        }
    }

    public class ChunkLoader
    {

    }
}