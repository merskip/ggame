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

    private void DrawNoiseEdtior(NoiseGenerator noise = null) {
        if (noise == null) 
            noise = creator.GetGenerator<NoiseGenerator>();

        noise.seed = EditorGUILayout.IntField("Seed", noise.seed);
        noise.amplitude = EditorGUILayout.Slider("Amplitude", noise.amplitude, 0.0f, 1.0f);
        noise.frequency = EditorGUILayout.FloatField("Frequency", noise.frequency);
        noise.octaveCount = EditorGUILayout.IntField("Octave count", noise.octaveCount);
        noise.persistence = EditorGUILayout.FloatField("Persistence", noise.persistence);
        noise.lacunarity = EditorGUILayout.FloatField("Lacunarity", noise.lacunarity);
    }

    private void DrawWorldEditor() {
        WorldGenerator world = creator.GetGenerator<WorldGenerator>();
        
        world.seed = EditorGUILayout.IntField("Seed", world.seed);
        world.amplitude = EditorGUILayout.Slider("Amplitude", world.amplitude, 0.0f, 1.0f);
        world.frequency = EditorGUILayout.FloatField("Frequency", world.frequency);
        world.octaveCount = EditorGUILayout.IntField("Octave count", world.octaveCount);
        world.persistence = EditorGUILayout.FloatField("Persistence", world.persistence);
        world.lacunarity = EditorGUILayout.FloatField("Lacunarity", world.lacunarity);
        
        GUILayout.Label("Textures", EditorStyles.boldLabel);
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

        world.snowStrength = EditorGUILayout.CurveField("Snow Strenght", world.snowStrength);
        world.rockSteepness = EditorGUILayout.CurveField("Rock Steepness", world.rockSteepness);

        GUILayout.Label("Trees", EditorStyles.boldLabel);
        world.treePrefab = (GameObject)
            EditorGUILayout.ObjectField("Tree prefab", world.treePrefab, typeof(GameObject), false);
        world.treesStrength = EditorGUILayout.CurveField("Trees Strenght", world.treesStrength);

        EditorGUILayout.MinMaxSlider(new GUIContent("Trees size"), ref world.treeSizeMin, ref world.treeSizeMax, 0.0f, 1.0f);

    }
}