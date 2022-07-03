using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minecraft
{
    public class MCMeshGenerator : MonoBehaviour
    {
        public void Generate(int[,,] blocks)
        {
            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                for (int z = 0; z < blocks.GetLength(2); z++)
                {
                    for (int x = 0; x < blocks.GetLength(0); x++)
                    {
                        Vector3Int currPos = new Vector3Int(x, y, z);

                        Vector3Int[] directions = new Vector3Int[]
                        {
                            new Vector3Int(-1, 0, 0),
                            new Vector3Int(1, 0, 0),
                            new Vector3Int(0, -1, 0),
                            new Vector3Int(0, 1, 0),
                            new Vector3Int(1, 0, -1),
                            new Vector3Int(0, 0, 1),
                        };

                        for (int i = 0; i < directions.Length; i++)
                        {
                            Vector3Int posToLookAt = new Vector3Int(x, y, z) + directions[i];

                            // Out of bounds
                            // ! Blocks on chunk edge may not have faces.
                            if (!posToLookAt.Within(Vector3Int.zero, currPos)) continue;

                            // If air
                            if (blocks[posToLookAt.x, posToLookAt.y, posToLookAt.z] == 0)
                            {

                            }
                        }
                    }
                }
            }
        }
    }
}