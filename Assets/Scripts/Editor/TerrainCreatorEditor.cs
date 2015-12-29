using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

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

        string[] customFields = new string[] { "m_Script", "generatorType", "createRange", "destoryRange" };

        GUILayout.Label("Creator options", EditorStyles.boldLabel);
        DrawPropertiesExcluding(serializedObject, customFields);

        creator.createRange = EditorGUILayout.IntSlider("Create Range", creator.createRange, 0, 32);
        if (creator.createRange >= creator.destoryRange)
            creator.destoryRange = creator.createRange;
        creator.destoryRange = EditorGUILayout.IntSlider("Destroy Range", creator.destoryRange, 0, 32);


        GUILayout.Label("Generator options", EditorStyles.boldLabel);
        string[] types = Enum.GetNames(typeof(TerrainCreator.GeneratorType));
        int newType = GUILayout.Toolbar((int) creator.generatorType, types);

        if (newType != (int) creator.generatorType) {
            if (ShowConfimChangeTypeGenerator())
                creator.generatorType = (TerrainCreator.GeneratorType) newType;
        }

        switch (creator.generatorType) {
            case TerrainCreator.GeneratorType.Plain:
                DrawPlainEditor();
                break;
            case TerrainCreator.GeneratorType.Wave:
                DrawWaveEditor();
                break;
            case TerrainCreator.GeneratorType.PerlinNoise:
                DrawPerlinNoiseEditor();
                break;
        }
        
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

    private void DrawPlainEditor() {
        PlainGenerator plane = GetIntanceOrCreateGenerator<PlainGenerator>();

        plane.baseHeight = EditorGUILayout.Slider("Base height", plane.baseHeight, 0.0f, 1.0f);
    }

    private void DrawWaveEditor() {
        WaveGenerator wave = GetIntanceOrCreateGenerator<WaveGenerator>();

        wave.amplitude = EditorGUILayout.Slider("Amplitude", wave.amplitude, 0.0f, 1.0f);
        wave.frequency = EditorGUILayout.FloatField("Frequency", wave.frequency);
    }

    private void DrawPerlinNoiseEditor() {
        PerlinNoiseGenerator noise = GetIntanceOrCreateGenerator<PerlinNoiseGenerator>();

        noise.seed = EditorGUILayout.IntField("Seed", noise.seed);
        noise.amplitude = EditorGUILayout.Slider("Amplitude", noise.amplitude, 0.0f, 1.0f);
        noise.frequency = EditorGUILayout.FloatField("Frequency", noise.frequency);
        noise.octaveCount = EditorGUILayout.IntField("Octave count", noise.octaveCount);
        noise.persistence = EditorGUILayout.FloatField("Persistence", noise.persistence);
        noise.lacunarity = EditorGUILayout.FloatField("Lacunarity", noise.lacunarity);
        
        GUILayout.Label("Textures", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        
        GUILayout.BeginVertical();
        GUILayout.Label("Grass");
        if (GUILayout.Button(noise.grassSplat.texture, GUILayout.Height(64), GUILayout.Width(64)))
            SplatPrototypeEditor.Show(noise.grassSplat);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("Snow");
        if (GUILayout.Button(noise.snowSplat.texture, GUILayout.Height(64), GUILayout.Width(64)))
            SplatPrototypeEditor.Show(noise.snowSplat);
        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        noise.snowStrength = EditorGUILayout.CurveField("Snow Strenght", noise.snowStrength);
    }

    private T GetIntanceOrCreateGenerator<T>() where T : TerrainGenerator {
        if (creator.TerrainGenerator is T == false)
            creator.TerrainGenerator = (T) CreateInstance(typeof(T));
        
        return (T) creator.TerrainGenerator;
    }
}