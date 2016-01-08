using UnityEngine;
using UnityEditor;
using System;

class TiledNoiseEditor : EditorWindow {

    private TiledNoise noise;

    public static void Show(TiledNoise noise) {
        var window = CreateInstance<TiledNoiseEditor>();
        window.noise = noise;

        window.titleContent = new GUIContent("Edit noise");
        window.maxSize = new Vector2(360f, 155f);
        window.minSize = window.maxSize;
        window.ShowAuxWindow();
    }

    void OnGUI() {
        Draw(noise);

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Close", GUILayout.Width(100)))
            Close();
        GUILayout.EndHorizontal();
    }

    public static void Draw(TiledNoise noise) {
        if (noise is PinkTiledNoise)
            DrawPinkNoise(noise as PinkTiledNoise);
        else
            throw new Exception("Unknown type of TiledNoise: " + noise.GetType());
    }

    private static void DrawPinkNoise(PinkTiledNoise noise) {
        noise.seed = EditorGUILayout.IntField("Seed", noise.seed);
        noise.amplitude = EditorGUILayout.Slider("Amplitude", noise.amplitude, 0.0f, 1.0f);
        noise.frequency = EditorGUILayout.FloatField("Frequency", noise.frequency);
        noise.octaveCount = EditorGUILayout.IntField("Octave count", noise.octaveCount);
        noise.persistence = EditorGUILayout.FloatField("Persistence", noise.persistence);
        noise.lacunarity = EditorGUILayout.FloatField("Lacunarity", noise.lacunarity);
    }
}