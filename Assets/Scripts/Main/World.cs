using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance;

    [Tooltip("Size of the world in chunks.")]
    [SerializeField] private Vector3Int _worldSize;

    private Dictionary<Vector3Int, Chunk> _chunkPosToChunkDict = new Dictionary<Vector3Int, Chunk>();

    private void Awake()
    {
        Instance = this;
    }

    public void LoadChunk(Vector3Int chunkPos, float detail)
    {
        // If chunk position is outside of world bounds, continue.
        if (!chunkPos.Within(Vector3Int.zero, World.Instance.GetWorldSize())) return;

        bool loaded = false;

        if (loaded) {
            // Load chunk from files
        }
        else {
            // Generate chunk
            Chunk chunk = ChunkGenerator.Instance.GenerateChunk(chunkPos, detail);
            chunk.AssociatedMeshObject = MeshGenerator.Instance.GenerateNewChunkMesh(chunk);
            _chunkPosToChunkDict.Add(chunkPos, chunk);
        }
    }

    public void UnloadChunk(Vector3Int chunkPos)
    {
        Chunk chunk = null;
        if (_chunkPosToChunkDict.TryGetValue(chunkPos, out chunk))
        {
            // Save chunk
            // Destory chunk object
            Destroy(chunk.AssociatedMeshObject);
            // Remove from chunk grid
            _chunkPosToChunkDict.Remove(chunkPos);
        }
    }

    // Get chunk at vertex point positions
    public Chunk GetChunkAtWorldPosition(Vector3Int worldPos)
    {
        Vector3Int chunkPos = new Vector3Int(
            (int)(worldPos.x / Global.CHUNK_WIDTH),
            (int)(worldPos.y / Global.CHUNK_HEIGHT),
            (int)(worldPos.z / Global.CHUNK_LENGTH)
        );

        Chunk chunk = null;
        if (_chunkPosToChunkDict.TryGetValue(chunkPos, out chunk))
        {
            return chunk;
        }
        return null;
    }

    public TerrainVertexPoint GetVertexPointAtWorldPosition(Vector3Int worldPos)
    {
        Chunk chunk = GetChunkAtWorldPosition(worldPos);
        if (chunk == null) return new TerrainVertexPoint(0, 0);

        Vector3Int vertexPos = new Vector3Int(
            worldPos.x % Global.CHUNK_WIDTH,
            worldPos.y % Global.CHUNK_HEIGHT,
            worldPos.z % Global.CHUNK_LENGTH
        );
        return chunk.Grid[vertexPos.x, vertexPos.y, vertexPos.z];
    }

    public Vector3Int GetWorldSize() { return _worldSize; }
}