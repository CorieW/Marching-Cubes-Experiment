using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public static MeshGenerator Instance;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GenerateNewChunkMesh(Chunk chunk)
    {
        GameObject newMeshObject = new GameObject($"Chunk ({chunk.ChunkPos.x}, {chunk.ChunkPos.y}, {chunk.ChunkPos.z})");
        newMeshObject.transform.position = chunk.ChunkPos * new Vector3Int(Global.CHUNK_WIDTH - 1, Global.CHUNK_HEIGHT - 1, Global.CHUNK_LENGTH - 1);
        MeshRenderer mr = newMeshObject.AddComponent<MeshRenderer>();
        MeshFilter mf = newMeshObject.AddComponent<MeshFilter>();

        List<Vector3> vertices = new List<Vector3>();
        Dictionary<string, int> posStringToVertexIndexDict = new Dictionary<string, int>();
        List<int> order = new List<int>();

        TerrainVertexPoint[,,] grid = chunk.Grid;

        for (int x = 0, i = 0; x < grid.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < grid.GetLength(1) - 1; y++)
            {
                for (int z = 0; z < grid.GetLength(2) - 1; z++, i++)
                {
                    Vector3[] corners = new Vector3[] {
                        new Vector3(0, 0, 0),
                        new Vector3(1, 0, 0),
                        new Vector3(1, 0, 1),
                        new Vector3(0, 0, 1),
                        new Vector3(0, 1, 0),
                        new Vector3(1, 1, 0),
                        new Vector3(1, 1, 1),
                        new Vector3(0, 1, 1)
                    };

                    double[] values = new double[] {
                        grid[x, y, z].EdgeValue,
                        grid[x + 1, y, z].EdgeValue,
                        grid[x + 1, y, z + 1].EdgeValue,
                        grid[x, y, z + 1].EdgeValue,
                        grid[x, y + 1, z].EdgeValue,
                        grid[x + 1, y + 1, z].EdgeValue,
                        grid[x + 1, y + 1, z + 1].EdgeValue,
                        grid[x, y + 1, z + 1].EdgeValue
                    };

                    int cubeIndex = CalcCubeIndex(values);
                    int[] triangulation = MarchTables.triangulation[cubeIndex];

                    for (int i2 = 0; triangulation[i2] != -1; i2 += 1)
                    {
                        int a0 = MarchTables.cornerIndexAFromEdge[triangulation[i2]];
                        int b0 = MarchTables.cornerIndexBFromEdge[triangulation[i2]];

                        Vector3 vec3 = InterpolateVerts(corners[a0], corners[b0], (float)values[a0], (float)values[b0]);
                        Vector3 totalPos = vec3 + new Vector3(x, y, z);

                        int vertexIndex = -1;
                        if (!posStringToVertexIndexDict.TryGetValue(totalPos.ToString(), out vertexIndex))
                        {
                            vertexIndex = vertices.Count;
                            posStringToVertexIndexDict.Add(totalPos.ToString(), vertexIndex);
                            vertices.Add(vec3 + new Vector3(x, y, z));
                        }
                        order.Add(vertexIndex);
                    }
                }
            }
        }

        int[] triangles = new int[order.Count];
        // Triangulate
        for (int i = 0; i < triangles.Length; i++)
        {
            triangles[i] = order[i];
        }

        Mesh mesh = mf.mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return newMeshObject;
    }

    /// <summary>
    /// The provided corners 3D array represents each corner of the cube and
    /// whether the cube has a corner at that location or not.
    /// Loops over all of the 8 corners of the cube and sets the corresponding
    /// bit to 1 if its value is below the surface level.
    /// This will result in a value between 0 and 255.
    /// </summary>
    private int CalcCubeIndex(double[] corners)
    {
        int cubeIndex = 0;
        for (int i = 0; i < corners.Length; i++)
        {
            if (corners[i] > 0) cubeIndex |= 1 << i;
        }
        return cubeIndex;
    }

    private Vector3 InterpolateVerts(Vector3 a, Vector3 b, float aV, float bV)
    {
        float t = (0 - aV) / (bV - aV);
        return a + t * (b - a);
    }

    private class Cube
    {
        public Vector3[] corners { get; }
        public bool[] values { get; }

        public Cube(Vector3[] corners, bool[] values)
        {
            this.corners = corners;
            this.values = values;
        }
    }
}