using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minecraft
{
    public static class Vector3IntExtension
    {
        public static bool Within(this Vector3Int pos, Vector3Int a, Vector3Int b)
        {
            if (pos.x < a.x || pos.y < a.y || pos.z < a.z) return false;
            if (pos.x > b.x || pos.y > b.y || pos.z > b.z) return false;

            return true;
        }
    }
}