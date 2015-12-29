using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

[RequireComponent (typeof(TerrainManager))]
public class TerrainCreator : MonoBehaviour {

    public enum GeneratorType {
        Plain,
        Wave,
        PerlinNoise
    }

    public Transform focus;
    public int createRange = 6;
    public int destoryRange = 8;
    
    public GeneratorType generatorType;
    
    public TerrainGenerator TerrainGenerator {
        set {
            manager.generator = value;
        }
        get {
            manager = GetComponent<TerrainManager>();
            return manager.generator;
        }
    }

    private TerrainManager manager;
    private Chunk.Coords lastCoords;

    private Coroutine showChunksCoroutine;
    private List<Chunk.Coords> chunksToLoad;
    private int chunksLoadedCount;

    public Chunk.Coords CurrentCoords {
        get { return lastCoords; }
    }

    public TerrainManager TerrainManager {
        get { return manager; }
    }

    public bool IsLoadingChunks {
        get { return chunksToLoad != null; }
    }

    public int ChunksToLoadCount {
        get { return chunksToLoad != null ? chunksToLoad.Count : 0; }
    }

    public int ChunksLoadedCount {
        get { return chunksLoadedCount; }
    }

    void Awake() {
        if (focus == null)
            focus = Camera.main.transform;

        manager = GetComponent<TerrainManager>();
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
        return manager.ToChunkCoords(focus.position);
    }

    private IEnumerator StartShowChunks(Chunk.Coords cuurentCoords) {
        chunksToLoad = GetChunkCoordsToLoad(cuurentCoords);
        chunksLoadedCount = 0;

        foreach (var coords in chunksToLoad) {
            manager.ShowChunk(coords);
            chunksLoadedCount++;
            yield return null;
        }
        chunksToLoad = null;
        chunksLoadedCount = 0;
    }

    private List<Chunk.Coords> GetChunkCoordsToLoad(Chunk.Coords currentCoords) {
        List<Chunk.Coords> list = new List<Chunk.Coords>();
        for (int x = 0; x < createRange; x++) {
            for (int y = 0; y < createRange - x; y++) {
                AddUniquteCoords(list, currentCoords.x + x, currentCoords.y + y);
                AddUniquteCoords(list, currentCoords.x - x, currentCoords.y + y);
                AddUniquteCoords(list, currentCoords.x + x, currentCoords.y - y);
                AddUniquteCoords(list, currentCoords.x - x, currentCoords.y - y);
            }
        }

        return list;
    }

    private void AddUniquteCoords(List<Chunk.Coords> list, int x, int y) {
        var coords = new Chunk.Coords(x, y);
        if (!list.Contains(coords)) {
            if (!manager.LoadedChunksCoords.Contains(coords))
                list.Add(coords);
        }
    }

    private void RemoveChunksIfOutOfRange(Chunk.Coords currentCoords) {
        var chunkCoords = manager.LoadedChunksCoords;
        foreach (var coords in chunkCoords) {
            if (IsOutOfRange(currentCoords, coords))
                manager.RemoveChunk(coords);
        }
    }

    private bool IsOutOfRange(Chunk.Coords currentCoords, Chunk.Coords otherCoords) {
        float distanceX = Mathf.Abs(currentCoords.x - otherCoords.x);
        float distanceY = Mathf.Abs(currentCoords.y - otherCoords.y);

        return distanceX > destoryRange || distanceY > destoryRange;
    }
}
