using UnityEngine;
using UnityEditor;
using System;

[Serializable]
[CustomEditor(typeof(TerrainCreator))]
class TerrainCreatorEditor : Editor {
    
    [SerializeField]
    TerrainCreator creator;

    [SerializeField]
    WorldEditor worldEditor = new WorldEditor();

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
        worldEditor.Draw(world);
    }    

}