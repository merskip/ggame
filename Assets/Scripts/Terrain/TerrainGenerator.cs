using UnityEngine;
using System.Collections;
using System;
using CoherentNoise.Generation.Fractal;

public abstract class TerrainGenerator {

    protected Chunk chunk;
    protected TerrainData data;

    protected int width;
    protected int height;

    public void GenerateChunk(Chunk chunk) {
        this.chunk = chunk;
        data = chunk.terrain.terrainData;
        width = data.heightmapWidth;
        height = data.heightmapHeight;
        GenerateHeightmap();
    }

    protected abstract void GenerateHeightmap();

	protected float getCoordX(int x) {
        return chunk.coords.x + (float) x / (width - 1);
    }

    protected float getCoordY(int y) {
        return chunk.coords.y + (float) y / (height - 1);
    }

    protected void SetHeightmap(float[,] heightmap) {
        data.SetHeights(0, 0, heightmap);
    }

}

public class PlainGenerator : TerrainGenerator {

    public float baseHeight;

    public PlainGenerator(float baseHeight) {
        this.baseHeight = baseHeight;
    }

    protected override void GenerateHeightmap() {
        float[,] heightmap = new float[height, width];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                heightmap[y, x] = baseHeight;
            }
        }
        SetHeightmap(heightmap);
    }
}

public class WaveGenerator : TerrainGenerator {

    public float amplitude;
    public float frequency;

    public WaveGenerator(float amplitude, float frequency) {
        this.amplitude = amplitude;
        this.frequency = frequency;
    }

    protected override void GenerateHeightmap() {
        float[,] heightmap = new float[height, width];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float xCoord = getCoordX(x);
                float yCoord = getCoordY(y);

                float v = Mathf.Sin(xCoord * frequency) * Mathf.Sin(yCoord * frequency);
                heightmap[y, x] = (v + 1.0f) / 2 * amplitude;
            }
        }
        SetHeightmap(heightmap);
    }
}

public class PerlinNoiseGenerator : TerrainGenerator {

    public int seed = 0;
    public float amplitude;
    public float frequency;

    public int octaveCount = 8;
    public float persistence = 0.2f;
    public float lacunarity = 2.0f;

    public PerlinNoiseGenerator(float amplitude, float frequency) {
        this.amplitude = amplitude;
        this.frequency = frequency;
    }

    protected override void GenerateHeightmap() {
        PinkNoise noise = new PinkNoise(seed);
        noise.Frequency = frequency;
        noise.OctaveCount = octaveCount;
        noise.Persistence = persistence;
        noise.Lacunarity = lacunarity;

        float[,] heightmap = new float[height, width];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float xCoord = getCoordX(x);
                float yCoord = getCoordY(y);

                float v = noise.GetValue(xCoord, yCoord, 0.0f);
                
                heightmap[y, x] = Mathf.Clamp((v + 1.2f) / 2.4f * amplitude, 0.0f, 1.0f) ;
            }
        }
        SetHeightmap(heightmap);
    }
}