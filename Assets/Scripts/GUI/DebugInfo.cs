using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Text))]
public class DebugInfo : MonoBehaviour {

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
        var coords = terrainCreator.CurrentCoords;
        var chunks = terrainCreator.TerrainManager.getChunkMap().Count;
        text.text = string.Format(format, coords.x, coords.y, chunks);
	}
}
