using System.Collections.Generic;
using UnityEngine;

namespace HeightMap_Attempt
{
    public class HMAMeshGenerator : MonoBehaviour
    {
        public static HMAMeshGenerator Instance;

        [SerializeField] private Material _terrainMaterial;

        private void Awake()
        {
            Instance = this;
        }

        public GameObject GenerateNewChunkMesh(Vector2Int chunkPos, float[,] heightMap)
        {
            int xSize = heightMap.GetLength(0) - 1;
            int zSize = heightMap.GetLength(1) - 1;

            GameObject newMeshObject = new GameObject($"Chunk ({chunkPos.x}, {chunkPos.y})");
            Vector2Int totalPos = chunkPos * new Vector2Int(Global.CHUNK_WIDTH, Global.CHUNK_LENGTH);
            newMeshObject.transform.position = new Vector3Int(totalPos.x, 0, totalPos.y);
            MeshRenderer mr = newMeshObject.AddComponent<MeshRenderer>();
            mr.material = _terrainMaterial;
            MeshFilter mf = newMeshObject.AddComponent<MeshFilter>();

            Vector3[] vertices = new Vector3[(xSize + 1) * (zSize + 1)];
            Vector2[] uv = new Vector2[vertices.Length];
            for (int i = 0, z = 0; z <= zSize; z++) {
                for (int x = 0; x <= xSize; x++, i++) {
                    vertices[i] = new Vector3(x, heightMap[x, z] * 50, z);                
                    uv[i] = new Vector2((float)x / xSize, (float)z / zSize);
                }
            }

            int[] triangles = new int[xSize * zSize * 6];
            for (int ti = 0, vi = 0, z = 0; z < zSize; z++, vi++) {
                for (int x = 0; x < xSize; x++, ti += 6, vi++) {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                    triangles[ti + 5] = vi + xSize + 2;
                }
            }

            Mesh mesh = mf.mesh = new Mesh();
            // mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.RecalculateNormals();
            
            return newMeshObject;
        }

        public GameObject GenerateNewChunkMesh2(Vector2Int chunkPos, float[,] heightMap, float detail)
        {
            int xSize = heightMap.GetLength(0);
            int zSize = heightMap.GetLength(1);

            GameObject newMeshObject = new GameObject($"Chunk ({chunkPos.x}, {chunkPos.y})");
            Vector2Int totalPos = chunkPos * new Vector2Int(Global.CHUNK_WIDTH, Global.CHUNK_LENGTH);
            newMeshObject.transform.position = new Vector3Int(totalPos.x, 0, totalPos.y);
            MeshRenderer mr = newMeshObject.AddComponent<MeshRenderer>();
            mr.material = _terrainMaterial;
            MeshFilter mf = newMeshObject.AddComponent<MeshFilter>();

            Vector3[] vertices = new Vector3[(xSize + 1) * (zSize + 1)];
            Dictionary<Vector2, int> posToVertexIndexDict = new Dictionary<Vector2, int>();
            Vector2[] uv = new Vector2[vertices.Length];
            // Plot vertices
            for (int z = 0, i = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++, i++)
                {
                    Vector3 currPos = new Vector3(x, heightMap[x, z] * 50, z);
                    vertices[i] = currPos;
                    posToVertexIndexDict.Add(new Vector2(x, z), i);
                    uv[i] = new Vector2((float)x / xSize, (float)z / zSize);
                }
            }

            List<int> order = new List<int>();
            for (int z = 0; z < zSize - 1; z++)
            {
                for (int x = 0; x < xSize - 1; x++)
                {
                    Vector2 currPos = new Vector2(x, z);
                    order.Add(posToVertexIndexDict[currPos]);
                    // Debug.Log(currPos + new Vector2(0, 1));
                    order.Add(posToVertexIndexDict[currPos + new Vector2(0, 1)]);
                    order.Add(posToVertexIndexDict[currPos + new Vector2(1, 0)]);
                    order.Add(posToVertexIndexDict[currPos + new Vector2(0, 1)]);
                    order.Add(posToVertexIndexDict[currPos + new Vector2(1, 1)]);
                    order.Add(posToVertexIndexDict[currPos + new Vector2(1, 0)]);
                }
            }

            int[] triangles = new int[order.Count];
            // Triangulate
            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i] = order[i];
            }

            Mesh mesh = mf.mesh = new Mesh();
            // mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            // mesh.uv = uv;
            mesh.RecalculateNormals();
            
            return newMeshObject;
        }
    }
}