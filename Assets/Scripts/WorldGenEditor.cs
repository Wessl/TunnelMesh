#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (WorldGenerator))]
public class PathGeneratorEditor : Editor {

    public override void OnInspectorGUI() {
        WorldGenerator worldGen = (WorldGenerator)target;

        DrawDefaultInspector();

        if (GUILayout.Button ("Generate")) {
            worldGen.GenerateWorld();
        }
    }
}
#endif