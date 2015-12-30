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
    
    public SplatPrototypeData grassSplat = new SplatPrototypeData();
    public SplatPrototypeData snowSplat = new SplatPrototypeData();

    public AnimationCurve snowStrength = new AnimationCurve();

    public GameObject treePrefab;
    public AnimationCurve treesStrength = new AnimationCurve();
    public float treeSizeMin = 0.5f;
    public float treeSizeMax = 1.0f;

    private PinkNoise noise;

    private float[,,] alphamap;

    private System.Random r;
    private float xMaxMove;
    private float yMaxMove;

    protected override void OnBeforeGenerate() {
        noise = new PinkNoise(seed);
        noise.Frequency = frequency;
        noise.OctaveCount = octaveCount;
        noise.Persistence = persistence;
        noise.Lacunarity = lacunarity;

        SetupSplats();
        SetupTrees();
    }

    protected override void GenerateHeightmap() {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float h = GetHeightAt(x, y);
                heightmap[y, x] = h;
                
                PaintTexture(x, y, h);
                PlaceTree(x, y, h);
            }
        }
    }

    protected override void OnAfterGenerate() {
        if (alphamap != null)
            data.SetAlphamaps(0, 0, alphamap);
    }

    private float GetHeightAt(int x, int y) {
        float xCoord = getCoordX(x);
        float yCoord = getCoordY(y);
        float h = noise.GetValue(xCoord, yCoord, 0.0f);
        h = (h + 1.5f) / 3.0f * amplitude;
        h = Mathf.Clamp(h, 0.0f, 1.0f);
        return h;
    }

    private void SetupSplats() {
        if (grassSplat.texture == null)
            return;

        SplatPrototype[] splats = new SplatPrototype[snowSplat.texture != null ? 2 : 1];
        splats[0] = grassSplat.toSplatPrototype();

        if (snowSplat.texture != null)
            splats[1] = snowSplat.toSplatPrototype();

        data.splatPrototypes = splats;
        alphamap = data.GetAlphamaps(0, 0, width, height);
    }

    private void PaintTexture(int x, int y, float h) {
        if (data.splatPrototypes.Length < 2)
            return;

        float snow = snowStrength.Evaluate(h);

        alphamap[y, x, 0] = 1.0f - snow;
        alphamap[y, x, 1] = snow;
    }

    private void SetupTrees() {
        if (treePrefab == null)
            return;

        TreePrototype treePrototype = new TreePrototype();
        treePrototype.prefab = treePrefab;

        data.treePrototypes = new TreePrototype[] { treePrototype };
        xMaxMove = 1.0f / width;
        yMaxMove = 1.0f / height;
        r = new System.Random(seed);
    }

    private void PlaceTree(int x, int y, float h) {
        if (data.treePrototypes.Length == 0)
            return;

        float strengh = treesStrength.Evaluate(h);
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