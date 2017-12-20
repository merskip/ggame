using UnityEngine;
using UnityEditor;
using System;
using System.Text;

[Serializable]
[CustomEditor(typeof(WorldGenerator))]
class WorldGeneratorEditor : Editor {

    public enum WorldTab {
        Mountains
    };

    public enum MoutainsTab {
        Heightmap,
        Textures,
        Trees
    };

    public WorldTab worldTab = WorldTab.Mountains;
    public MoutainsTab mountainsTab = MoutainsTab.Heightmap;

    [SerializeField]
    private WorldGenerator world;

    void Awake() {
        world = (WorldGenerator)target;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        string[] tabs = Enum.GetNames(typeof(WorldTab));
        worldTab = (WorldTab)GUILayout.Toolbar((int)worldTab, tabs);
        switch (worldTab) {
            case WorldTab.Mountains:
                DrawMountainsEditor(world.mountainsGenerator);
                break;
        }

        if (GUI.changed) {
            EditorUtility.SetDirty(world);
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawMountainsEditor(MountainsGenerator mountains) {
        string[] tabs = Enum.GetNames(typeof(MoutainsTab));
        mountainsTab = (MoutainsTab) GUILayout.Toolbar((int) mountainsTab, tabs);

        switch (mountainsTab) {
            case MoutainsTab.Heightmap:
                TiledNoiseEditor.Draw(mountains.heightmapNoise);
                break;
            case MoutainsTab.Textures:
                DrawTextures(mountains);
                break;
            case MoutainsTab.Trees:
                DrawTress(mountains);
                break;
        }
    }

    private void DrawTextures(MountainsGenerator mountains) {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Label("Grass");
        if (GUILayout.Button(mountains.grassSplat.texture, GUILayout.Height(64), GUILayout.Width(64)))
            SplatPrototypeEditor.Show(mountains.grassSplat);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("Snow");
        if (GUILayout.Button(mountains.snowSplat.texture, GUILayout.Height(64), GUILayout.Width(64)))
            SplatPrototypeEditor.Show(mountains.snowSplat);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("Rock");
        if (GUILayout.Button(mountains.rockSplat.texture, GUILayout.Height(64), GUILayout.Width(64)))
            SplatPrototypeEditor.Show(mountains.rockSplat);
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        mountains.snowStrength = EditorGUILayout.CurveField("Snow Strenght", mountains.snowStrength);
        mountains.rockSteepness = EditorGUILayout.CurveField("Rock Steepness", mountains.rockSteepness);
    }

    private void DrawTress(MountainsGenerator mountains) {
        mountains.treesSeed = EditorGUILayout.IntField("Seed", mountains.treesSeed);
        mountains.treePrefab = (GameObject)
            EditorGUILayout.ObjectField("Prefab", mountains.treePrefab, typeof(GameObject), false);
        mountains.treesStrength = EditorGUILayout.CurveField("Strenght", mountains.treesStrength);

        EditorGUILayout.MinMaxSlider(new GUIContent("Size"), ref mountains.treeSizeMin, ref mountains.treeSizeMax, 0.0f, 1.0f);
    }

}
