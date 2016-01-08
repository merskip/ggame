using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class LoadingScreen : MonoBehaviour {

    public bool onlyFirstLoading = true;
    public Behaviour[] forFreezingComponent;
    
    public Canvas loadingCanvas;
    public Text counter;
    public RectTransform progressBar;

    private string counterFormat;
    
    private TerrainCreator creator;

    private Camera mainCamera;
    private int savedCullingMask;

    private bool isFirstLoading = true;
    private bool lastShowScreen = false;

    void Awake() {
        creator = FindObjectOfType<TerrainCreator>();
        if (creator == null || !creator.enabled) {
            Debug.LogError("Not found TerrainCreator or is enabled on scene!");
            enabled = false;
        }

        counterFormat = counter.text;

        mainCamera = Camera.main;
        savedCullingMask = mainCamera.cullingMask;
    }

    void Start() {
        StartLoadingScreen();
    }
	
	void OnGUI() {
        if (IsShowLoadingScreen()) {
            if (!lastShowScreen)
                StartLoadingScreen();

            float loading = (float) creator.ChunksLoadedCount / creator.ChunksToLoadCount;
            UpdateProgress(loading);
            
            lastShowScreen = true;
        } else {
            if (lastShowScreen) {
                EndLoadingScreen();
                isFirstLoading = false;
                lastShowScreen = false;
            }
        }
    }

    private void UpdateProgress(float loading) {
        Vector3 barScale = progressBar.localScale;
        barScale.x = loading;
        progressBar.localScale = barScale;

        int loaded = creator.ChunksLoadedCount;
        int toLoad = creator.ChunksToLoadCount;
        counter.text = string.Format(counterFormat, loaded, toLoad);
    }

    private bool IsShowLoadingScreen() {
        return creator.IsLoadingChunks && (isFirstLoading || !onlyFirstLoading);
    }

    private void StartLoadingScreen() {
        DisableCamera();
        foreach (var c in forFreezingComponent)
            c.enabled = false;
        loadingCanvas.enabled = true;
    }

    private void EndLoadingScreen() {
        EnableCamera();
        foreach (var c in forFreezingComponent)
            c.enabled = true;
        loadingCanvas.enabled = false;
    }

    private void DisableCamera() {
        mainCamera.cullingMask = 0;
    }

    private void EnableCamera() {
        mainCamera.cullingMask = savedCullingMask;
    }
}
