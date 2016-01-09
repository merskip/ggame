using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class LoadingScreen : MonoBehaviour {

    public bool onlyFirstLoading = true;
    public Behaviour[] forFreezingComponent;

    public float fadeInDuration = 1.0f;
    public float fadeOutDuration = 1.0f;
    
    public Canvas loadingCanvas;
    public Text counter;
    public RectTransform progressBar;
    public Quotes quotes;

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

        if (isFirstLoading) {
            quotes.ShowNext();
            StartCoroutine(StartFadeIn(fadeInDuration));
        }
    }

    private IEnumerator StartFadeIn(float duration, float delay = 0.05f) {
        var list = loadingCanvas.GetComponentsInChildren<CanvasRenderer>();
        float step = 1.0f / (duration / delay);
        for (float i = 0.0f; i <= 1.0f; i += step) {
            foreach (var c in list) {
                if (c.gameObject != loadingCanvas.gameObject)
                    c.SetAlpha(i);
            }

            yield return new WaitForSeconds(delay);
        }

        foreach (var c in list)
            c.SetAlpha(1.0f);
    }

    private void EndLoadingScreen() {
        EnableCamera();
        foreach (var c in forFreezingComponent)
            c.enabled = true;

        
            StartCoroutine(StartFadeOut(fadeOutDuration));

        if (isFirstLoading)
            quotes.gameObject.SetActive(false);
    }

    private IEnumerator StartFadeOut(float duration, float delay = 0.05f) {
        var c = loadingCanvas.GetComponent<CanvasGroup>();
        float step = 1.0f / (duration / delay);
        for (float i = 1.0f; i >= 0.0f; i -= step) {
            c.alpha = i;
            yield return new WaitForSeconds(delay);
        }

        loadingCanvas.enabled = false;
        c.alpha = 1.0f;
    }

    private void DisableCamera() {
        mainCamera.cullingMask = 0;
    }

    private void EnableCamera() {
        mainCamera.cullingMask = savedCullingMask;
    }
}
