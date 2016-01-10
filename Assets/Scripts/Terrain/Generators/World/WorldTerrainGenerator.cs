using UnityEngine;
using System.Collections;
using System;

public abstract class WorldTerrainGenerator {

    protected int width, height;

    protected Chunk chunk;
    protected TerrainData data;
    protected float[,] heightmap;

    protected virtual void OnCreate() { }

    public float[,] GenerateTerrain(Chunk chunk, float[,] mask = null) {
        this.chunk = chunk;
        data = chunk.terrain.terrainData;
        width = data.heightmapWidth;
        height = data.heightmapHeight;
        heightmap = new float[height, width];

        OnGenerateTerrain();
        if (mask != null)
            ApplyMaskToHeightmap(mask);
        return heightmap;
    }

    protected abstract void OnGenerateTerrain();

    private void ApplyMaskToHeightmap(float[,] mask) {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                heightmap[y, x] *= mask[y, x];
            }
        }
    }

    public virtual void OnAfterGenerate() { }
}