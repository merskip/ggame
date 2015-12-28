using UnityEngine;
using System.Collections;
using System;

public class PlainGenerator : TerrainGenerator {

    public float baseHeight = 0.0f;

    protected override void GenerateHeightmap() {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                heightmap[y, x] = baseHeight;
            }
        }
    }
}