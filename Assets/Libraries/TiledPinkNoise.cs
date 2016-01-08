using UnityEngine;
using System.Collections;
using CoherentNoise.Generation.Fractal;
using System;
using CoherentNoise;

[Serializable]
public class PinkTiledNoise : TiledNoise {

    public int seed = 0;
    public float frequency = 1.0f;
    public int octaveCount = 8;
    public float persistence = 0.45f;
    public float lacunarity = 2.0f;

    protected override Generator OnCreateGenerator() {
        PinkNoise n = new PinkNoise(seed);
        n.Frequency = frequency;
        n.OctaveCount = octaveCount;
        n.Persistence = persistence;
        n.Lacunarity = lacunarity;
        return n;
    }
}
