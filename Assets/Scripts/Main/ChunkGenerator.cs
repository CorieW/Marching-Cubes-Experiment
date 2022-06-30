using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public const float SCALE = 100;

    public static ChunkGenerator Instance;

    [Tooltip("Size of the world in chunks.")]
    [SerializeField] private Vector3Int _worldSize;

    private Noise _noise;

    private void Awake()
    {
        Instance = this;
        _noise = new Noise();
    }

    public Chunk GenerateChunk(Vector3Int chunkPos, float detail)
    {
        // If outside of bounds, don't generate a chunk.
        if (!chunkPos.Within(Vector3Int.zero, _worldSize)) return null;

        TerrainVertexPoint[,,] grid = new TerrainVertexPoint[Global.CHUNK_WIDTH, Global.CHUNK_HEIGHT, Global.CHUNK_LENGTH];
        double[,] heightMap = GenerateHeightMap(chunkPos, detail);

        for (int y = 0; y < Global.CHUNK_HEIGHT; y++)
        {
            for (int z = 0; z < Global.CHUNK_LENGTH; z++)
            {
                for (int x = 0; x < Global.CHUNK_WIDTH; x++)
                {
                    // double heightValue = heightMap[x, z] * SCALE;
                    // if (heightValue > 0) grid[x, y, z] = new TerrainVertexPoint(1, 1);
                    // else grid[x, y, z] = new TerrainVertexPoint(-1, 0);
                    grid[x, y, z] = new TerrainVertexPoint(_noise.Evaluate((((double)chunkPos.x * ((Global.CHUNK_WIDTH - 1)) + x)) / 5, (((double)chunkPos.y * ((Global.CHUNK_HEIGHT - 1)) + y)) / 5, (((double)chunkPos.z * ((Global.CHUNK_LENGTH - 1)) + z)) / 5), 0);
                }
            }
        }

        Chunk chunk = new Chunk();
        chunk.ChunkPos = chunkPos;
        chunk.Grid = grid;
        chunk.Detail = detail;
        return chunk;
    }

    /// <summary>Generates a 2D array of doubles between -1 and 1.</summary>
    private double[,] GenerateHeightMap(Vector3Int chunkPos, float detail)
    {
        Vector3Int verticesPer = new Vector3Int(
            (int)(Global.CHUNK_WIDTH / ((float)Global.CHUNK_WIDTH * detail)),
            0,
            (int)(Global.CHUNK_LENGTH / ((float)Global.CHUNK_LENGTH * detail))
        );
        double[,] heightMap = new double[(int)(Global.CHUNK_WIDTH / verticesPer.x), (int)(Global.CHUNK_LENGTH / verticesPer.z)];

        for (int z = 0; z < Global.CHUNK_LENGTH; z += verticesPer.z)
        {
            for (int x = 0; x < Global.CHUNK_WIDTH; x += verticesPer.x)
            {
                heightMap[x, z] = _noise.Evaluate(chunkPos.x + x, chunkPos.z + z);
            }
        }
        return heightMap;
    }
}
public class Chunk
{
    public Vector3Int ChunkPos { get; set; }
    public TerrainVertexPoint[,,] Grid { get; set; }
    public GameObject AssociatedMeshObject { get; set; }

    private float _detail;
    /// <summary>
    /// Identifies how detailed the chunk mesh should be when rendering it.
    /// <para>Contain a value between or equal to 0 and 1.</para>
    /// </summary>
    public float Detail { 
        get
        {
            return this._detail;
        } 
        set
        {
            float val = Mathf.Clamp01(value);
            _detail = val;
        } 
    }
}
public struct TerrainVertexPoint
{
    public double EdgeValue { get; set; }
    public int MaterialID { get; set; }

    public TerrainVertexPoint(double edgeValue, int materialID)
    {
        EdgeValue = edgeValue;
        MaterialID = materialID;
    }
}