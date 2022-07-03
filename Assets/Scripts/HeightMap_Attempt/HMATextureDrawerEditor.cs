using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HeightMap_Attempt
{
    [CustomEditor(typeof(HMATextureDrawer))]
    public class HMATextureDrawerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            HMATextureDrawer textureDrawer = (HMATextureDrawer)target;

            if (GUILayout.Button("Generate Texture"))
            {
                textureDrawer.Generate();
            }
        }
    }
}