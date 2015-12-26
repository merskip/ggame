using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class ChunksLoading : MonoBehaviour {

    private TerrainCreator terrainCreator;

    private string format;
    private Text text;

    private void Awake() {
        text = GetComponent<Text>();
        format = text.text;

        terrainCreator = FindObjectOfType<TerrainCreator>();
        if (terrainCreator == null || !terrainCreator.enabled) {
            Debug.LogError("Not found TerrainCreator or is enabled on scene!");
            enabled = false;
        }
    }

    void LateUpdate() {
        bool isLoading = terrainCreator.IsLoadingChunks;
        if (isLoading) {
            var toLoadCount = terrainCreator.ChunksToLoadCount;
            var loadedCount = terrainCreator.ChunksLoadedCount;
            text.text = string.Format(format, loadedCount, toLoadCount);
            text.enabled = true;
        } else {
            text.enabled = false;
        }
    }
}
