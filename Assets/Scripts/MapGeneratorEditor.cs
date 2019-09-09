#if UNITY_EDITOR
using Assets.Scripts;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var myScript = (MapGenerator)target;
        if (GUILayout.Button("Generate"))
        {
            myScript.Generate();
        }
    }
}
#endif