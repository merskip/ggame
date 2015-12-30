using UnityEngine;
using System.Collections;

public class StartupPosition : MonoBehaviour {

    public bool startOnSurface = true;
    public bool startOnCenter = true;

    void Start() {
        TerrainManager manager = FindObjectOfType<TerrainManager>();
        if (manager == null || !manager.enabled) {
            Debug.LogError("Not found TerrainManager or is enabled on scene!");
            enabled = false;
            return;
        }

        Vector3 chunkSize = manager.chunkSize;
        Chunk.Coords startCoords = manager.ToChunkCoords(transform.position);

        Vector3 newPos = transform.position;
        if (startOnCenter) {
            newPos.x = startCoords.x * chunkSize.x + chunkSize.x / 2.0f;
            newPos.z = startCoords.y * chunkSize.z + chunkSize.z / 2.0f;
        }
        if (startOnSurface) {
            Chunk chunk = manager.GetChunk(startCoords);
            newPos.y = chunk.terrain.SampleHeight(newPos);
        }
        
        transform.position = newPos;
    }
	
}
