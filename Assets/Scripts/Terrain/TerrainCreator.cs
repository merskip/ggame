﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

[RequireComponent (typeof(TerrainManager))]
public class TerrainCreator : MonoBehaviour {

    public Transform focus;
    public int createRange = 6;
    public int destoryRange = 8;

    public TerrainGenerator terrainGenerator;
    
    private TerrainManager manager;
    private Vector3 lastCreatedPosition;

    private Coroutine showChunksCoroutine;
    private List<Chunk.Coords> chunksToLoad;
    private int chunksLoadedCount;

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
        manager.generator = terrainGenerator;
    }

    void Start() {
        Chunk.Coords coords = GetCurrentCoords();
        OnChangeChunk(coords);
        lastCreatedPosition = focus.position;
    }

    void Update() {
        if (ShouldCreateChunks()) {
            Chunk.Coords coords = GetCurrentCoords();
            OnChangeChunk(coords);
            lastCreatedPosition = focus.position;
        }
    }
    
    private bool ShouldCreateChunks() {
        Vector3 delta = focus.position - lastCreatedPosition;
        return Mathf.Abs(delta.x) > manager.chunkSize.x
            || Mathf.Abs(delta.z) > manager.chunkSize.y;
    }

    private void OnChangeChunk(Chunk.Coords coords) {
        if (showChunksCoroutine != null)
            StopCoroutine(showChunksCoroutine);
        showChunksCoroutine = StartCoroutine(StartShowChunks(coords));

        RemoveChunksIfOutOfRange(coords);
    }

    public Chunk.Coords GetCurrentCoords() {
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

    private List<Chunk.Coords> GetChunkCoordsToLoad(Chunk.Coords coords) {
        List<Chunk.Coords> loadedChunk = manager.LoadedChunksCoords;
        List<Chunk.Coords> list = new List<Chunk.Coords>();
        int r2 = createRange * createRange;

        for (int x = -createRange; x <= createRange; x++) {
            for (int y = -createRange; y <= createRange; y++) {
                Chunk.Coords c = new Chunk.Coords(coords.x + x, coords.y + y);

                bool inRange = x * x + y * y < r2;
                bool isLoaded = loadedChunk.Contains(c);
                if (inRange && !isLoaded)
                    list.Add(c);
            }
        }

        return list;
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
