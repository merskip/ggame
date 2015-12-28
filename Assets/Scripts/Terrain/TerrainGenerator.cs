using UnityEngine;
using System.Collections;
using System;

public abstract class TerrainGenerator : ScriptableObject {

    protected Chunk chunk;
    protected TerrainData data;

    protected float[,] heightmap;

    protected int width;
    protected int height;

    public void GenerateChunk(Chunk chunk) {
        this.chunk = chunk;
        data = chunk.terrain.terrainData;
        width = data.heightmapWidth;
        height = data.heightmapHeight;

        heightmap = new float[height, width];

        GenerateHeightmap();
        data.SetHeights(0, 0, heightmap);
    }

    protected abstract void GenerateHeightmap();

	protected float getCoordX(int x) {
        return chunk.coords.x + (float) x / (width - 1);
    }

    protected float getCoordY(int y) {
        return chunk.coords.y + (float) y / (height - 1);
    }

}

