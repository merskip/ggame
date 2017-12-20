using UnityEngine;
using UnityEditor;
using System;

[Serializable]
[CustomEditor(typeof(TerrainCreator))]
class TerrainCreatorEditor : Editor {
    
    [SerializeField]
    TerrainCreator creator;
    
    void Awake() {
        creator = (TerrainCreator) target;
    }
    
    public override void OnInspectorGUI() {
        serializedObject.Update();

        string[] hideFields = new string[] { "m_Script", "activeGenerator", "createRange", "destoryRange" };

        GUILayout.Label("Creator options", EditorStyles.boldLabel);
        DrawPropertiesExcluding(serializedObject, hideFields);

        creator.createRange = EditorGUILayout.IntSlider("Create Range", creator.createRange, 0, 32);
        if (creator.createRange >= creator.destoryRange)
            creator.destoryRange = creator.createRange;
        creator.destoryRange = EditorGUILayout.IntSlider("Destroy Range", creator.destoryRange, 0, 32);
        
        if (GUI.changed) {
            EditorUtility.SetDirty(creator);
            serializedObject.ApplyModifiedProperties();
        }
    }
  
}