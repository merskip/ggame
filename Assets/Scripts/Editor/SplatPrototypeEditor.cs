using UnityEngine;
using UnityEditor;
using System;

class SplatPrototypeEditor : EditorWindow {

    private SplatPrototypeData splat;

    public static void Show(SplatPrototypeData splat) {
        var window = CreateInstance<SplatPrototypeEditor>();
        window.splat = splat;

        window.titleContent = new GUIContent("Edit splat");
        window.maxSize = new Vector2(360f, 195f);
        window.minSize = window.maxSize;
        window.ShowAuxWindow();
    }

    void OnGUI() {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        {
            GUILayout.Label("Texture");
            splat.texture = (Texture2D)
                EditorGUILayout.ObjectField("", splat.texture, typeof(Texture2D), false, GUILayout.Width(64));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        {
            GUILayout.Label("Normal");
            splat.normalMap = (Texture2D)
                EditorGUILayout.ObjectField("", splat.normalMap, typeof(Texture2D), false, GUILayout.Width(64));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        {
            GUILayout.Label("Size");
            splat.tileSize = EditorGUILayout.Vector2Field("", splat.tileSize);

            GUILayout.Label("Offset");
            splat.tileOffset = EditorGUILayout.Vector2Field("", splat.tileOffset);


        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        GUILayout.Label("Only if not set material", EditorStyles.miniLabel);
        splat.metallic = EditorGUILayout.Slider("Metallic", splat.metallic, 0.0f, 1.0f);
        splat.smoothness = EditorGUILayout.Slider("Smoothness", splat.smoothness, 0.0f, 1.0f);

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Close", GUILayout.Width(100)))
            Close();
        GUILayout.EndHorizontal();
    }
}