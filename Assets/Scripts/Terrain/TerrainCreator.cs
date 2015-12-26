using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

[RequireComponent (typeof(TerrainManager))]
public class TerrainCreator : MonoBehaviour {

    public bool usePerlinNoiseGenerator = false;

    public Transform focus;
    public int createRange = 1;
    public int destoryRange = 5;

    private TerrainManager manager;
    private Chunk.Coords lastCoords;

    private Coroutine showChunksCoroutine;

    void Awake() {
        if (focus == null)
            focus = Camera.main.transform;

        manager = GetComponent<TerrainManager>();

        if (usePerlinNoiseGenerator) {
            PerlinNoiseGenerator generator = new PerlinNoiseGenerator(0.8f, 1.0f);
            generator.persistence = 0.3f;
            generator.lacunarity = 2.5f;
            manager.generator = generator;
        }
    }

	void Start() {
	    
    }

    void OnGUI() {
        GUI.Label(new Rect(10, 10, 100, 20),
            string.Format("Coords: ({0}, {1})", lastCoords.x, lastCoords.y));
        GUI.Label(new Rect(10, 30, 100, 20),
            string.Format("Chunks: {0}", manager.getChunkMap().Count));

        GUI.Label(new Rect(10, 50, 100, 20),
            string.Format("Rotate: {0:0.0}", focus.rotation.eulerAngles.y));
    }

    void Update() {
        Chunk.Coords coords = getCurrentChunkCoords();
        if (!coords.Equals(lastCoords)) {
            OnChangeChunk(coords);
            lastCoords = coords;
        }
    }

    private void OnChangeChunk(Chunk.Coords coords) {
        if (showChunksCoroutine != null)
            StopCoroutine(showChunksCoroutine);
        showChunksCoroutine = StartCoroutine(StartShowChunks(coords));

        RemoveChunksIfOutOfRange(coords);
    }

    private Chunk.Coords getCurrentChunkCoords() {
        return manager.getChunkCoords(focus.position);
    }

    private IEnumerator StartShowChunks(Chunk.Coords coords) {
        for (int x = 0; x < createRange; x++) {
            for (int y = 0; y < createRange - x; y++) {
                if (manager.ShowChunk(coords.x + x, coords.y + y))
                    yield return null;
                if (manager.ShowChunk(coords.x - x, coords.y + y))
                    yield return null;
                if (manager.ShowChunk(coords.x + x, coords.y - y))
                    yield return null;
                if (manager.ShowChunk(coords.x - x, coords.y - y))
                    yield return null;
            }
        }

    }

    private void RemoveChunksIfOutOfRange(Chunk.Coords currentCoords) {
        var chunkMap = manager.getChunkMap();
        List<Chunk.Coords> chunkCoords = new List<Chunk.Coords>(chunkMap.Keys);

        foreach (var coords in chunkCoords) {
            if (chunkIsOutOfRange(currentCoords, coords))
                manager.RemoveChunk(coords);
        }
    }

    private bool chunkIsOutOfRange(Chunk.Coords currentCoords, Chunk.Coords otherCoords) {
        float distanceX = Mathf.Abs(currentCoords.x - otherCoords.x);
        float distanceY = Mathf.Abs(currentCoords.y - otherCoords.y);

        return distanceX > destoryRange || distanceY > destoryRange;
    }
}
