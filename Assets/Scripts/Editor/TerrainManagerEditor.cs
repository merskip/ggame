using UnityEngine;
using UnityEditor;
using System;

[Serializable]
[CustomEditor(typeof(TerrainManager))]
class TerrainManagerEditor : Editor {

    [SerializeField]
    TerrainManager manager;

    void Awake() {
        manager = (TerrainManager) target;
    }


    public override void OnInspectorGUI() {
        serializedObject.Update();

        GUILayout.Label("Terrain options", EditorStyles.boldLabel);
        manager.chunkSize = EditorGUILayout.Vector3Field("Chunk size", manager.chunkSize);
        manager.resolution = EditorGUILayout.IntField("Resolution", manager.resolution);
        manager.pixelError = EditorGUILayout.IntSlider("Pixel Error", manager.pixelError, 1, 200);
        manager.setupsNeighbors = EditorGUILayout.Toggle("Setups Neighbors", manager.setupsNeighbors);

        manager.terrainMaterial = (Material)
            EditorGUILayout.ObjectField("Material", manager.terrainMaterial, typeof(Material), false);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Default texture");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent(" Edit splat...", manager.defaultSplat.texture), GUILayout.Height(32))) {
            SplatPrototypeEditor.Show(manager.defaultSplat);
        }
        GUILayout.EndHorizontal();

        if (GUI.changed) {
            EditorUtility.SetDirty(manager);
            serializedObject.ApplyModifiedProperties();
        }
    }

}
