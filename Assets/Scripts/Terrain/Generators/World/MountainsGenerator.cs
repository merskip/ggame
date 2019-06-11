using UnityEngine;
using System.Collections;
using System;
using CoherentNoise.Generation.Fractal;

[Serializable]
public class MountainsGenerator : WorldTerrainGenerator {
    
    public PinkTiledNoise heightmapNoise = new PinkTiledNoise();
    
    public SplatPrototypeData grassSplat = new SplatPrototypeData();
    public SplatPrototypeData snowSplat = new SplatPrototypeData();
    public SplatPrototypeData rockSplat = new SplatPrototypeData();

    public AnimationCurve snowStrength = new AnimationCurve();
    public AnimationCurve rockSteepness = new AnimationCurve();

    public int treesSeed = 0;
    public GameObject treePrefab;
    public AnimationCurve treesStrength = new AnimationCurve();
    public float treeSizeMin = 0.5f;
    public float treeSizeMax = 1.0f;

    private float[,,] alphamap;

    private System.Random r;
    private float xMaxMove;
    private float yMaxMove;

    protected override void OnGenerateTerrain() {
        Chunk.Coords c = chunk.coords;
        heightmap = heightmapNoise.GetTiledMap(c.x, c.y, width, height);
        alphamap = data.GetAlphamaps(0, 0, width, height);

        SetupSplats();
        SetupTrees();

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float h = heightmap[y, x];
                PaintTexture(x, y, h);
                PlaceTree(x, y, h);
            }
        }
    }

    public override void OnAfterGenerate() {
        PaintRockOnSlope();
        data.SetAlphamaps(0, 0, alphamap);
    }

    private void SetupSplats() {
        data.splatPrototypes = new SplatPrototype[] {
            grassSplat.ToSplatPrototype(),
            snowSplat.ToSplatPrototype(),
            rockSplat.ToSplatPrototype()
        };
        alphamap = data.GetAlphamaps(0, 0, width, height);
    }

    private void PaintTexture(int x, int y, float h) {
        if (data.splatPrototypes.Length < 2)
            return;

        float snow = snowStrength.Evaluate(h);

        alphamap[y, x, 0] = 1.0f - snow;
        alphamap[y, x, 1] = snow;
        alphamap[y, x, 2] = 0.0f;
    }

    private void PaintRockOnSlope() {
        if (data.splatPrototypes.Length < 3)
            return;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float xCoord = (float) x / (width - 1);
                float yCoord = (float) y / (height - 1);

                float steepness = data.GetSteepness(xCoord, yCoord) / 90.0f;
                float rock = rockSteepness.Evaluate(steepness);

                float grass = alphamap[y, x, 0];
                float snow = alphamap[y, x, 1];
                float sum = grass + snow + rock;

                alphamap[y, x, 0] = grass / sum;
                alphamap[y, x, 1] = snow / sum;
                alphamap[y, x, 2] = rock / sum;
            }
        }
    }

    private void SetupTrees() {
        if (treePrefab == null)
            return;

        TreePrototype treePrototype = new TreePrototype();
        treePrototype.prefab = treePrefab;

        data.treePrototypes = new TreePrototype[] { treePrototype };
        xMaxMove = 1.0f / width;
        yMaxMove = 1.0f / height;
        r = new System.Random(treesSeed);
    }

    private void PlaceTree(int x, int y, float h) {
        if (data.treePrototypes.Length == 0)
            return;

        float strengh = treesStrength.Evaluate(h) / data.heightmapResolution;
        bool placeTree = r.NextDouble() <= strengh;
        if (placeTree) {
            float xCoord = (float) x / width;
            float yCoord = (float) y / height;

            float xMove = (float) r.NextDouble() % xMaxMove;
            float yMove = (float) r.NextDouble() % yMaxMove;

            float size = (float) r.NextDouble() * (treeSizeMax - treeSizeMin) + treeSizeMin;

            TreeInstance tree = new TreeInstance();
            tree.prototypeIndex = 0;
            tree.heightScale = size;
            tree.widthScale = size;
            tree.rotation = (float) r.NextDouble() * 2 * Mathf.PI;
            tree.color = Color.white;
            tree.position = new Vector3(xCoord + xMove, 0.0f, yCoord + yMove);
            chunk.terrain.AddTreeInstance(tree);
        }
    }
}