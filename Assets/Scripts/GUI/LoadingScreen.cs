using UnityEngine;
using System.Collections;


[RequireComponent (typeof(Camera))]
public class LoadingScreen : MonoBehaviour {

    public bool onlyFirstLoading = true;

    public Behaviour[] forFreezingComponent;

    public Color backgroundColor = Color.black;

    public Vector2 barSize = new Vector2(250, 5);

    public Color barForeground = Color.blue;
    public Color barBackground = Color.grey;

    private Texture2D texBarFb;
    private Texture2D texBarBg;

    private bool isFirstLoading = true;
    private bool lastShowScreen = false;

    private TerrainCreator creator;

    private Camera mainCamera;
    private CameraClearFlags savedClearFlags;
    private int savedCullingMask;
    private Color savedBackground;


    void Awake() {
        creator = FindObjectOfType<TerrainCreator>();
        if (creator == null || !creator.enabled) {
            Debug.LogError("Not found TerrainCreator or is enabled on scene!");
            enabled = false;
        }

        mainCamera = Camera.main;
        savedClearFlags = mainCamera.clearFlags;
        savedCullingMask = mainCamera.cullingMask;
        savedBackground = mainCamera.backgroundColor;

        texBarFb = new Texture2D(1, 1);
        texBarFb.SetPixel(0, 0, barForeground);
        texBarFb.Apply();

        texBarBg = new Texture2D(1, 1);
        texBarBg.SetPixel(0, 0, barBackground);
        texBarBg.Apply();
    }

    void Start() {
        StartLoadingScreen();
    }
	
	void OnGUI() {
        if (IsShowLoadingScreen()) {
            if (!lastShowScreen)
                StartLoadingScreen();

            float loading = (float) creator.ChunksLoadedCount / creator.ChunksToLoadCount;
            DrawLoadingBarWithLabel(loading);
            
            lastShowScreen = true;
        } else {
            if (lastShowScreen) {
                EndLoadingScreen();
                isFirstLoading = false;
                lastShowScreen = false;
            }
        }
    }

    private void DrawLoadingBarWithLabel(float loading) {
        Vector2 barPos = new Vector2();
        barPos.x = Screen.width / 2.0f - barSize.x / 2.0f;
        barPos.y = Screen.height / 2.0f - barSize.y / 2.0f;

        GUI.DrawTexture(new Rect(barPos.x, barPos.y, barSize.x, barSize.y), texBarBg);
        GUI.DrawTexture(new Rect(barPos.x, barPos.y, barSize.x * loading, barSize.y), texBarFb);

        GUI.Label(new Rect(barPos.x, barPos.y - 24, barSize.x, 20), "Generate world...");
    }

    private bool IsShowLoadingScreen() {
        return creator.IsLoadingChunks && (isFirstLoading || !onlyFirstLoading);
    }

    private void StartLoadingScreen() {
        DisableCamera();
        foreach (var c in forFreezingComponent)
            c.enabled = false;
    }

    private void EndLoadingScreen() {
        EnableCamera();
        foreach (var c in forFreezingComponent)
            c.enabled = true;
    }

    private void DisableCamera() {
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.cullingMask = 0;
        mainCamera.backgroundColor = backgroundColor;
    }

    private void EnableCamera() {
        mainCamera.clearFlags = savedClearFlags;
        mainCamera.cullingMask = savedCullingMask;
        mainCamera.backgroundColor = savedBackground;
    }
}
