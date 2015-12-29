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

        DrawDefaultSplat();

        if (GUI.changed) {
            EditorUtility.SetDirty(manager);
            serializedObject.ApplyModifiedProperties();
        }
    }
    
    private void DrawDefaultSplat() {
        GUILayout.Label("Default splat", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        {
            GUILayout.Label("Texture");
            manager.splatTexture = (Texture2D)
                EditorGUILayout.ObjectField("", manager.splatTexture, typeof(Texture2D), false, GUILayout.Width(64));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        {
            GUILayout.Label("Normal");
            manager.splatNormal = (Texture2D)
                EditorGUILayout.ObjectField("", manager.splatNormal, typeof(Texture2D), false, GUILayout.Width(64));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        {
            GUILayout.Label("Size");
            manager.splatSize = EditorGUILayout.Vector2Field("", manager.splatSize);

            GUILayout.Label("Ofsset");
            manager.splatOffset = EditorGUILayout.Vector2Field("", manager.splatOffset);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }
}

