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
        manager.terrainMaterial = (Material)
            EditorGUILayout.ObjectField("Material", manager.terrainMaterial, typeof(Material), false);

        if (GUI.changed) {
            EditorUtility.SetDirty(manager);
            serializedObject.ApplyModifiedProperties();
        }
    }
}