using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public struct Chunk {
    public Coords coords;
    public Terrain terrain;

    public class Coords {
        public int x { get; set; }
        public int y { get; set; }

        public Coords(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj) {
            Coords other = obj as Coords;
            if (obj == null) return false;

            return x == other.x && y == other.y;
        }

        public override int GetHashCode() {
            int somePrimeNumer = 5171;
            return x.GetHashCode() * somePrimeNumer + y.GetHashCode();
        }
    }
}

[Serializable]
public class SplatPrototypeData {

    public Texture2D texture;
    public Texture2D normalMap;
    public Vector2 tileSize = new Vector2(15.0f, 15.0f);
    public Vector2 tileOffset;
    public float metallic = 0.0f;
    public float smoothness = 0.0f;

    public SplatPrototype toSplatPrototype() {
        var splat = new SplatPrototype();
        splat.texture = texture;
        splat.normalMap = normalMap;
        splat.tileSize = tileSize;
        splat.tileOffset = tileOffset;
        splat.metallic = metallic;
        splat.smoothness = smoothness;
        return splat;
    }
}

public class TerrainManager : MonoBehaviour {

    public Vector3 chunkSize;
    [SerializeField]
    private int _resoltion;
    public int pixelError = 8;
    public bool setupsNeighbors = true;

    public Material terrainMaterial;
    public SplatPrototypeData defaultSplat = new SplatPrototypeData();

    public TerrainGenerator generator;

    public int resolution {
        get { return _resoltion; }
        set {
            float log2 = Mathf.Log(value - 1, 2);
            if (log2 % 1.0f != 0) {
                int c = Mathf.CeilToInt(log2);
                value = (1 << c) + 1;
            }
            _resoltion = Mathf.Max(value, 33);
        }
    }

    private Dictionary<Chunk.Coords, Chunk> chunkMap;

    public int ChunksCount {
        get { return chunkMap.Count; }
    }

    public List<Chunk.Coords> LoadedChunksCoords {
        get { return new List<Chunk.Coords>(chunkMap.Keys); }
    }

    void Awake() {
        chunkMap = new Dictionary<Chunk.Coords, Chunk>();

        if (generator == null)
            generator = (TerrainGenerator) ScriptableObject.CreateInstance(typeof(PlainGenerator));
    }

    public Chunk.Coords ToChunkCoords(Vector3 position) {
        int x = (int) Mathf.Floor(position.x / chunkSize.x);
        int y = (int) Mathf.Floor(position.z / chunkSize.z);
        return new Chunk.Coords(x, y);
    }

    public Chunk GetChunk(int x, int y) {
        return GetChunk(new Chunk.Coords(x, y));
    }

    public Chunk GetChunk(Chunk.Coords coords) {
        Chunk? chunkOrNull = TryGetChunk(coords);
        if (chunkOrNull != null)
            return (Chunk) chunkOrNull;

        ShowChunk(coords);
        return (Chunk) TryGetChunk(coords);
    }

    public bool ShowChunk(int x, int y) {
        return ShowChunk(new Chunk.Coords(x, y));
    }

    public bool ShowChunk(Chunk.Coords coords) {
        if (!chunkMap.ContainsKey(coords)) {
            CreateChunk(coords);
            return true;
        }
        return false;
    }
    
    private void CreateChunk(Chunk.Coords coords) {
        Terrain terrain = CreateTerrainForChunk(coords);
        Chunk chunk = new Chunk();
        chunk.coords = coords;
        chunk.terrain = terrain;
        generator.GenerateChunk(chunk);
        chunk.terrain.Flush();

        chunkMap.Add(chunk.coords, chunk);

        if (setupsNeighbors) {
            SetupNeighbors(chunk.coords.x, chunk.coords.y);
            SetupNeighbors(chunk.coords.x + 1, chunk.coords.y);
            SetupNeighbors(chunk.coords.x, chunk.coords.y + 1);
            SetupNeighbors(chunk.coords.x - 1, chunk.coords.y);
            SetupNeighbors(chunk.coords.x, chunk.coords.y - 1);
        }
    }

    private void SetupNeighbors(int x, int y) {
        Chunk.Coords coords = new Chunk.Coords(x, y);
        Chunk? chunk = TryGetChunk(x, y);
        if (chunk == null)
            return;

        Terrain left = GetTerrainOrNull(coords.x - 1, coords.y);
        Terrain top = GetTerrainOrNull(coords.x, coords.y + 1);
        Terrain right = GetTerrainOrNull(coords.x + 1, coords.y);
        Terrain bottom = GetTerrainOrNull(coords.x, coords.y - 1);

        chunk.Value.terrain.SetNeighbors(left, top, right, bottom);
    }

    public Terrain GetTerrainOrNull(int x, int y) {
        Chunk? chunk = TryGetChunk(x, y);
        if (chunk != null)
            return chunk.Value.terrain;
        return null;
    }

    private Terrain CreateTerrainForChunk(Chunk.Coords coords) {
        TerrainData terrainData = CreateAndSetupTerrainData();
        
        GameObject terrainObject = Terrain.CreateTerrainGameObject(terrainData);
        terrainObject.transform.position = new Vector3(chunkSize.x * coords.x, 0.0f, chunkSize.z * coords.y);
        terrainObject.transform.parent = gameObject.transform;
        terrainObject.transform.name = string.Format("Chunk ({0}, {1})", coords.x, coords.y);

        return GetAndSetupTerrain(terrainObject);
    }

    private TerrainData CreateAndSetupTerrainData() {
        TerrainData data = new TerrainData();
        data.heightmapResolution = resolution;
        data.size = new Vector3(chunkSize.x, chunkSize.y, chunkSize.z);

        if (this.defaultSplat.texture != null) {
            SplatPrototype defaultSplat = this.defaultSplat.toSplatPrototype();
            data.splatPrototypes = new SplatPrototype[] { defaultSplat };
        }

        data.alphamapResolution = resolution;

        return data;
    }

    private Terrain GetAndSetupTerrain(GameObject terrainObject) {
        Terrain terrain = terrainObject.GetComponent<Terrain>();
        terrain.heightmapPixelError = pixelError;
        if (terrainMaterial != null) {
            terrain.materialType = Terrain.MaterialType.Custom;
            terrain.materialTemplate = terrainMaterial;
        }

        return terrain;
    }

    public void RemoveChunk(Chunk.Coords coords) {
        Chunk? chunkOrNull = TryGetChunk(coords);
        if (chunkOrNull != null) {
            Chunk chunk = (Chunk) chunkOrNull;
            Destroy(chunk.terrain.gameObject);
            Destroy(chunk.terrain.terrainData);
            Destroy(chunk.terrain);
            chunkMap.Remove(coords);
        }
    }

    public Chunk? TryGetChunk(int x, int y) {
        return TryGetChunk(new Chunk.Coords(x, y));
    }

    public Chunk? TryGetChunk(Chunk.Coords coords) {
        Chunk chunk;
        if (chunkMap.TryGetValue(coords, out chunk))
            return chunk;
        return null;
    }
}
