using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public const float NOISE_SCALE = 350;

    public static ChunkGenerator Instance;

    private Noise _noise;

    private void Awake()
    {
        Instance = this;
        _noise = new Noise();
    }

    public Chunk GenerateChunk(Vector3Int chunkPos, float detail)
    {
        // A mesh always needs to have +1 vertices to match the same intended size.
        TerrainVertexPoint[,,] grid = new TerrainVertexPoint[
            (int)(Global.CHUNK_WIDTH * detail) + 1 + Global.STITCH_VERTICES, 
            (int)(Global.CHUNK_HEIGHT * detail) + 1 + Global.STITCH_VERTICES,
            (int)(Global.CHUNK_LENGTH * detail) + 1 + Global.STITCH_VERTICES
        ];
        // double[,] heightMap = GenerateHeightMap(chunkPos, detail);

        for (int z = 0; z < grid.GetLength(2); z++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                int dX = (int)(x / detail);
                int dZ = (int)(z / detail);

                double noise = _noise.Evaluate((double)((chunkPos.x * Global.CHUNK_WIDTH) + dX) / NOISE_SCALE, (double)((chunkPos.z * Global.CHUNK_LENGTH) + dZ) / NOISE_SCALE);
                double heightValue = (noise * 100) + 101;
                int y = (int)heightValue;

                for (int n = -1; n <= 1; n++)
                {
                    // Coordinates including detail calculations.
                    // int dY = (int)(y / detail);

                    // Vector3Int currPos = (chunkPos * new Vector3Int(Global.CHUNK_WIDTH, Global.CHUNK_HEIGHT, Global.CHUNK_LENGTH)) + new Vector3Int(dX, dY, dZ);
                    // Calculate the slope
                    double edgeValue = Mathf.Clamp((float)heightValue - (y + n), -1, 1);
                    grid[x, (int)((y + n) * detail), z] = new TerrainVertexPoint(edgeValue, 1);

                    // if (currPos.y > heightValue) grid[x, y, z] = new TerrainVertexPoint(-1, 0);
                    // else if (currPos.y <= Mathf.FloorToInt((float)heightValue)) grid[x, y, z] = new TerrainVertexPoint(heightValue - currPos.y, 1);
                    // if (Mathf.Floor((float)heightValue) - currPos.y == 0) Debug.Log(heightValue - currPos.y);
                    // double edgeVal = _noise.Evaluate(((((double)chunkPos.x * (Global.CHUNK_WIDTH)) + dX)) / 5, ((((double)chunkPos.y * (Global.CHUNK_HEIGHT)) + dY)) / 5, ((((double)chunkPos.z * (Global.CHUNK_LENGTH)) + dZ)) / 5);
                    // grid[x, y, z] = new TerrainVertexPoint(edgeVal, 0);
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
        int totalWidth = Global.CHUNK_WIDTH + 1 + Global.STITCH_VERTICES;
        int totalLength = Global.CHUNK_LENGTH + 1 + Global.STITCH_VERTICES;

        Vector3Int verticesPer = new Vector3Int(
            (int)(totalWidth / ((float)totalWidth * detail)),
            0,
            (int)(totalLength / ((float)totalLength * detail))
        );
        double[,] heightMap = new double[(int)(totalWidth / verticesPer.x), (int)(totalLength / verticesPer.z)];

        for (int z = 0; z < heightMap.GetLength(1); z++)
        {
            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                heightMap[x, z] = _noise.Evaluate((chunkPos.x + x / NOISE_SCALE), (chunkPos.z + z) / NOISE_SCALE);
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