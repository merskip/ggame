using UnityEngine;
using System.Collections;
using System;
using CoherentNoise.Generation.Fractal;

public class WorldGenerator : TerrainGenerator {

    public MountainsGenerator mountainsGenerator = new MountainsGenerator();

    protected override void GenerateHeightmap() {
        heightmap = mountainsGenerator.GenerateTerrain(chunk);
    }

    protected override void OnAfterGenerate() {
        mountainsGenerator.OnAfterGenerate();
    }

    private float[,] GetDefaultMask() {
        int width = data.heightmapWidth;
        int height = data.heightmapHeight;
        float[,] mask = new float[height, width];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                mask[y, x] = 1.0f;
            }
        }
        return mask;
    }

}