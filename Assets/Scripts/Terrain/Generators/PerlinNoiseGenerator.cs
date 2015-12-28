using UnityEngine;
using System.Collections;
using System;
using CoherentNoise.Generation.Fractal;

public class PerlinNoiseGenerator : TerrainGenerator {

    public int seed = 0;
    public float amplitude = 1.0f;
    public float frequency = 1.0f;

    public int octaveCount = 8;
    public float persistence = 0.45f;
    public float lacunarity = 2.0f;

    protected override void GenerateHeightmap() {
        PinkNoise noise = new PinkNoise(seed);
        noise.Frequency = frequency;
        noise.OctaveCount = octaveCount;
        noise.Persistence = persistence;
        noise.Lacunarity = lacunarity;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float xCoord = getCoordX(x);
                float yCoord = getCoordY(y);

                float v = noise.GetValue(xCoord, yCoord, 0.0f);
                heightmap[y, x] = Mathf.Clamp((v + 1.2f) / 2.4f * amplitude, 0.0f, 1.0f);
            }
        }
    }
}