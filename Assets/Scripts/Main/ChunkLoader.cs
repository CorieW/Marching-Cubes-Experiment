using System.Collections.Generic;
using UnityEngine;

/// <summary>Used for loading chunks around the object it's attached to.</summary>
public class ChunkLoader : MonoBehaviour
{
    private Vector3Int _prevChunkPos = new Vector3Int(-1, -1, -1);
    private Dictionary<Vector3Int, float> _chunkPosToDetailDict = new Dictionary<Vector3Int, float>();

    private void Update()
    {
        Vector3Int currentChunkPos = GetCurrentChunkPos();
        if (_prevChunkPos != currentChunkPos) 
        {
            UnloadOldChunks(currentChunkPos);
            LoadNewlyVisibleChunk(currentChunkPos);
        }
    }

    // ! Has some issues with variable naming. Not clear of intention.
    private void UnloadOldChunks(Vector3Int currentChunkPos)
    {
        RangeDetails rangeDetails = GameController.Instance.GetSettings().GetRenderingSettings().rangeDetails;
        int maxRange = rangeDetails.GetMaxRange();
        
        for (int z = -maxRange; z <= maxRange; z++)
        {
            for (int x = -maxRange; x <= maxRange; x++)
            {
                Vector3Int totalPrevChunkPos = _prevChunkPos + new Vector3Int(x, 0, z);
                Vector3Int totalCurrentChunkPos = currentChunkPos + new Vector3Int(x, 0, z);
                
                // Get distance between current chunkloader chunk pos and the previous chunk position.
                float currRange = Vector3Int.Distance(currentChunkPos, totalPrevChunkPos);
                float currDetail = rangeDetails.GetDetailAtRange(currRange);

                float existingChunkDetail = 0;
                if (_chunkPosToDetailDict.TryGetValue(totalPrevChunkPos, out existingChunkDetail))
                {
                    // Chunk's detail doesn't need to change, so this chunk can stay loaded. Continue to next chunk.
                    if (existingChunkDetail == currDetail) continue;

                    // Unload chunk.
                    World.Instance.UnloadChunk(totalPrevChunkPos);
                    _chunkPosToDetailDict.Remove(totalPrevChunkPos);
                }
            }
        }
    }

    private void LoadNewlyVisibleChunk(Vector3Int currentChunkPos)
    {
        RangeDetails rangeDetails = GameController.Instance.GetSettings().GetRenderingSettings().rangeDetails;
        int maxRange = rangeDetails.GetMaxRange();
        
        for (int z = -maxRange; z <= maxRange; z++)
        {
            for (int x = -maxRange; x <= maxRange; x++)
            {
                Vector3Int totalChunkPos = currentChunkPos + new Vector3Int(x, 0, z);

                float currRange = Vector3Int.Distance(Vector3Int.zero, new Vector3Int(x, 0, z));
                float currDetail = rangeDetails.GetDetailAtRange(currRange);

                // If out of range, continue to next chunk position.
                if (currRange > maxRange) continue;

                float existingChunkDetail = 0;
                if (_chunkPosToDetailDict.TryGetValue(totalChunkPos, out existingChunkDetail))
                {
                    // Nothing needs to happen with this chunk, so just continue.
                    if (existingChunkDetail == currDetail) continue;
                }

                LoadNewChunk(totalChunkPos, currDetail);
            }
        }

        _prevChunkPos = currentChunkPos;
    }

    private void LoadNewChunk(Vector3Int chunkPos, float detail)
    {
        World.Instance.LoadChunk(chunkPos, detail);
        _chunkPosToDetailDict.Add(chunkPos, detail);
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
            Mathf.RoundToInt(nonWholeChunkPos.x),
            Mathf.RoundToInt(nonWholeChunkPos.y),
            Mathf.RoundToInt(nonWholeChunkPos.z)
        );
    }
}