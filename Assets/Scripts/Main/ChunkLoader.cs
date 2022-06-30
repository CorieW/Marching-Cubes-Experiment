using System.Collections.Generic;
using UnityEngine;

/// <summary>Used for loading chunks around the object it's attached to.</summary>
public class ChunkLoader : MonoBehaviour
{
    [SerializeField] private RangeDetails _rangeDetails;

    private Vector3Int _prevChunkPos = new Vector3Int(-1, -1, -1);
    private List<Chunk> _loadedChunks = new List<Chunk>();
    // TODO: Implement
    private Dictionary<Vector3Int, float> _chunkPosToDetailDict = new Dictionary<Vector3Int, float>();

    private void Update()
    {
        Vector3Int currentChunkPos = GetCurrentChunkPos();
        if (_prevChunkPos != currentChunkPos) 
        {
            UnloadOldChunks();
            LoadNewlyVisibleChunk();
        }
    }

    private void LoadNewlyVisibleChunk()
    {
        Vector3Int currentChunkPos = GetCurrentChunkPos();
        int maxRange = _rangeDetails.GetMaxRange();
        
        for (int y = -maxRange; y <= maxRange; y++)
        {
            for (int z = -maxRange; z <= maxRange; z++)
            {
                for (int x = -maxRange; x <= maxRange; x++)
                {
                    float currRange = Vector3Int.Distance(Vector3Int.zero, new Vector3Int(x, y, z));
                    float currDetail = _rangeDetails.GetDetailAtRange(currRange);

                    // If out of range, continue to next chunk position.
                    if (currRange >= maxRange) continue;

                    Vector3Int chunkPos = currentChunkPos + new Vector3Int(x, y, z);
                    LoadNewChunk(chunkPos, currDetail);
                }
            }
        }

        _prevChunkPos = currentChunkPos;
    }

    private void LoadNewChunk(Vector3Int chunkPos, float detail)
    {
        Chunk chunk = ChunkGenerator.Instance.GenerateChunk(chunkPos, detail);
        if (chunk == null) return;

        chunk.AssociatedMeshObject = MeshGenerator.Instance.GenerateNewChunkMesh(chunk);
        _loadedChunks.Add(chunk);
    }

    private void UnloadOldChunks()
    {
        Vector3Int currentChunkPos = GetCurrentChunkPos();
        int maxRange = _rangeDetails.GetMaxRange();

        for (int i = 0; i < _loadedChunks.Count; i++)
        {
            Chunk loadedChunk = _loadedChunks[i];

            if (Vector3Int.Distance(currentChunkPos, loadedChunk.ChunkPos) >= maxRange)
            {
                Destroy(loadedChunk.AssociatedMeshObject);
                _loadedChunks.RemoveAt(i);
                i -= 1;
            }
        }
    }

    private Vector3Int GetCurrentChunkPos()
    {
        Vector3 currPos = transform.position;
        Vector3 nonWholeChunkPos = new Vector3(
            currPos.x / Global.CHUNK_WIDTH,
            currPos.y / Global.CHUNK_HEIGHT,
            currPos.z / Global.CHUNK_LENGTH
        );

        // * Apparently casting to int is faster than Mathf.FloorToInt
        return new Vector3Int(
            (int)nonWholeChunkPos.x,
            (int)nonWholeChunkPos.y,
            (int)nonWholeChunkPos.z
        );
    }
}