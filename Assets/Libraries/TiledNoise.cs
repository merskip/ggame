using UnityEngine;
using System.Collections;
using System;
using CoherentNoise;


[Serializable]
public abstract class TiledNoise {

    public float amplitude = 1.0f;

    private Generator gen;

    protected abstract Generator OnCreateGenerator();

    public float[,] GetTiledMap(int tileX, int tileY, int width, int height) {
        if (gen == null)
            gen = OnCreateGenerator();

        float[,] map = new float[height, width];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float xCoords = GetCoords(tileX, width, x);
                float yCoords = GetCoords(tileY, height, y);

                map[y, x] = GetNormalizedValueAt(xCoords, yCoords);
            }
        }

        return map;
    }

    public float GetNormalizedValueAt(float xCoords, float yCoords) {
        float v = gen.GetValue(xCoords, yCoords, 0.0f);
        v = (v + 1.5f) / 3.0f * amplitude;
        return v;
    }

    public static float GetCoords(int tile, int tileSize, int i) {
        return tile + (float) i / (tileSize - 1);
    }
}
