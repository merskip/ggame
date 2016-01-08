using UnityEngine;
using UnityEditor;
using System;

[Serializable]
[CustomEditor(typeof(TerrainCreator))]
class TerrainCreatorEditor : Editor {
    
    [SerializeField]
    TerrainCreator creator;

    private enum WorldTab {
        Heightmap,
        Textures,
        Trees
    };
    private WorldTab worldTab = WorldTab.Heightmap;

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


        GUILayout.Label("Generator options", EditorStyles.boldLabel);
        string[] types = Enum.GetNames(typeof(GeneratorType));
        creator.activeGenerator = (GeneratorType) GUILayout.Toolbar((int) creator.activeGenerator, types);

        DrawGeneratorEditor();
        
        if (GUI.changed) {
            EditorUtility.SetDirty(creator);
            serializedObject.ApplyModifiedProperties();
        }
        
    }

    private static bool ShowConfimChangeTypeGenerator() {
        return EditorUtility.DisplayDialog("Change generator?",
            "Are you sure you want change type of generator?\nYou will lose previous options.",
            "Yes", "Cancel");
    }

    private void DrawGeneratorEditor() {
        switch (creator.activeGenerator) {
            case GeneratorType.Plain:
                DrawPlainEditor();
                break;
            case GeneratorType.Wave:
                DrawWaveEditor();
                break;
            case GeneratorType.Noise:
                DrawNoiseEdtior();
                break;
            case GeneratorType.World:
                DrawWorldEditor();
                break;
        }
    }

    private void DrawPlainEditor() {
        PlainGenerator plane = creator.GetGenerator<PlainGenerator>();

        plane.baseHeight = EditorGUILayout.Slider("Base height", plane.baseHeight, 0.0f, 1.0f);
    }

    private void DrawWaveEditor() {
        WaveGenerator wave = creator.GetGenerator<WaveGenerator>();

        wave.amplitude = EditorGUILayout.Slider("Amplitude", wave.amplitude, 0.0f, 1.0f);
        wave.frequency = EditorGUILayout.FloatField("Frequency", wave.frequency);
    }

    private void DrawNoiseEdtior() {
        NoiseGenerator noise = creator.GetGenerator<NoiseGenerator>();
        
        TiledNoiseEditor.Draw(noise.noise);
    }

    private void DrawWorldEditor() {
        WorldGenerator world = creator.GetGenerator<WorldGenerator>();

        string[] tabs = Enum.GetNames(typeof(WorldTab));
        worldTab = (WorldTab) GUILayout.Toolbar((int) worldTab, tabs);
        GUILayout.Space(5);

        switch (worldTab) {
            case WorldTab.Heightmap:
                TiledNoiseEditor.Draw(world.heightmapNoise);
                break;
            case WorldTab.Textures:
                DrawWorldTextures(world);
                break;
            case WorldTab.Trees:
                DrawWorldTress(world);
                break;
        }
    }
    
    private void DrawWorldTextures(WorldGenerator world) {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Label("Grass");
        if (GUILayout.Button(world.grassSplat.texture, GUILayout.Height(64), GUILayout.Width(64)))
            SplatPrototypeEditor.Show(world.grassSplat);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("Snow");
        if (GUILayout.Button(world.snowSplat.texture, GUILayout.Height(64), GUILayout.Width(64)))
            SplatPrototypeEditor.Show(world.snowSplat);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("Rock");
        if (GUILayout.Button(world.rockSplat.texture, GUILayout.Height(64), GUILayout.Width(64)))
            SplatPrototypeEditor.Show(world.rockSplat);
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        world.snowStrength = EditorGUILayout.CurveField("Snow Strenght", world.snowStrength);
        world.rockSteepness = EditorGUILayout.CurveField("Rock Steepness", world.rockSteepness);
    }

    private void DrawWorldTress(WorldGenerator world) {
        world.treesSeed = EditorGUILayout.IntField("Seed", world.treesSeed);
        world.treePrefab = (GameObject)
            EditorGUILayout.ObjectField("Prefab", world.treePrefab, typeof(GameObject), false);
        world.treesStrength = EditorGUILayout.CurveField("Strenght", world.treesStrength);

        EditorGUILayout.MinMaxSlider(new GUIContent("Size"), ref world.treeSizeMin, ref world.treeSizeMax, 0.0f, 1.0f);
    }
}