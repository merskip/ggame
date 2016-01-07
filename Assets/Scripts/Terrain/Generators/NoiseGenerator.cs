﻿using UnityEngine;
using System.Collections;
using System;
using CoherentNoise.Generation.Fractal;

public class NoiseGenerator : TerrainGenerator {

    public int seed = 0;
    public float amplitude = 1.0f;
    public float frequency = 1.0f;
    public int octaveCount = 8;
    public float persistence = 0.45f;
    public float lacunarity = 2.0f;
    
    private PinkNoise noise;

    protected override void OnBeforeGenerate() {
        noise = new PinkNoise(seed);
        noise.Frequency = frequency;
        noise.OctaveCount = octaveCount;
        noise.Persistence = persistence;
        noise.Lacunarity = lacunarity;
    }

    protected override void GenerateHeightmap() {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float h = GetHeightAt(x, y);
                heightmap[y, x] = h;
            }
        }
    }

    private float GetHeightAt(int x, int y) {
        float xCoord = getCoordX(x);
        float yCoord = getCoordY(y);
        float h = noise.GetValue(xCoord, yCoord, 0.0f);
        h = (h + 1.5f) / 3.0f * amplitude;
        h = Mathf.Clamp(h, 0.0f, 1.0f);
        return h;
    }
}