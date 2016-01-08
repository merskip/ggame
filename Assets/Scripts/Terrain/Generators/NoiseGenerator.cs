using UnityEngine;
using System.Collections;
using System;
using CoherentNoise.Generation.Fractal;

public class NoiseGenerator : TerrainGenerator {

    public PinkTiledNoise noise = new PinkTiledNoise();

    protected override void GenerateHeightmap() {
        Chunk.Coords c = chunk.coords;
        heightmap = noise.GetTiledMap(c.x, c.y, width, height);
    }
}