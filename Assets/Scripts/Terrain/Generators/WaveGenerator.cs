using UnityEngine;
using System.Collections;
using System;

public class WaveGenerator : TerrainGenerator {

    public float amplitude = 1.0f;
    public float frequency = 2 * Mathf.PI;

    protected override void GenerateHeightmap() {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float xCoord = getCoordX(x);
                float yCoord = getCoordY(y);

                float v = Mathf.Sin(xCoord * frequency) * Mathf.Sin(yCoord * frequency);
                heightmap[y, x] = (v + 1.0f) / 2 * amplitude;
            }
        }
    }
}