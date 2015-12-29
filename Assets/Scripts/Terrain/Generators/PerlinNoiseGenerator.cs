﻿using UnityEngine;
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

    public AnimationCurve snowStrength = new AnimationCurve();
    
    public SplatPrototypeData grassSplat = new SplatPrototypeData();
    public SplatPrototypeData snowSplat = new SplatPrototypeData();

    public override void GenerateChunk(Chunk chunk) {
        base.GenerateChunk(chunk);

        SetupSplats();
        PaintTexture();
    }

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
                v = (v + 1.5f) / 3.0f * amplitude;
                heightmap[y, x] = Mathf.Clamp(v, 0.0f, 1.0f);
            }
        }
    }

    protected void SetupSplats() {
        if (grassSplat.texture == null)
            return;

        SplatPrototype[] splats = new SplatPrototype[snowSplat.texture != null ? 2 : 1];
        splats[0] = grassSplat.toSplatPrototype();

        if (snowSplat.texture != null)
            splats[1] = snowSplat.toSplatPrototype();

        data.splatPrototypes = splats;
    }

    protected void PaintTexture() {
        if (data.splatPrototypes.Length < 2)
            return;

        float[,,] alphamap = data.GetAlphamaps(0, 0, width, height);
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {

                float h = heightmap[x, y];
                float snow = snowStrength.Evaluate(h);

                alphamap[x, y, 0] = 1.0f - snow;
                alphamap[x, y, 1] = snow;
            }
        }
        data.SetAlphamaps(0, 0, alphamap);
    }
}