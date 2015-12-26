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


public class TerrainManager : MonoBehaviour {

    public Terrain chunkPrefab;

    private float chunkWidth;
    private float chunkHeight;

    private Dictionary<Chunk.Coords, Chunk> chunkMap;
    public TerrainGenerator generator;

    void Start() {
        chunkMap = new Dictionary<Chunk.Coords, Chunk>();
        chunkWidth = chunkPrefab.terrainData.size.x;
        chunkHeight = chunkPrefab.terrainData.size.z;

        if (generator == null)
            generator = new WaveGenerator(0.1f, 10.0f);
    }

    public Chunk.Coords getChunkCoords(Vector3 position) {
        int x = (int) Mathf.Floor(position.x / chunkWidth);
        int y = (int) Mathf.Floor(position.z / chunkHeight);
        return new Chunk.Coords(x, y);
    }

    public Dictionary<Chunk.Coords, Chunk> getChunkMap() {
        return chunkMap;
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
        Terrain terrainChunk = CreateTerrainForChunk(coords);
        Chunk chunk = new Chunk();
        chunk.coords = coords;
        chunk.terrain = terrainChunk;
        generator.GenerateChunk(chunk);
        
        chunkMap.Add(chunk.coords, chunk);
    }

    private Terrain CreateTerrainForChunk(Chunk.Coords coords) {
        Vector3 pos = new Vector3(chunkWidth * coords.x, 0.0f, chunkHeight * coords.y);
        Terrain terrainChunk = (Terrain) Instantiate(chunkPrefab, pos, Quaternion.identity);
        terrainChunk.transform.parent = gameObject.transform;
        terrainChunk.transform.name = string.Format("Chunk ({0}, {1})", coords.x, coords.y);
        terrainChunk.terrainData = Instantiate(terrainChunk.terrainData);

        TerrainCollider collider = terrainChunk.GetComponent<TerrainCollider>();
        if (collider != null)
            collider.terrainData = terrainChunk.terrainData;

        return terrainChunk;
    }

    public void RemoveChunk(Chunk.Coords coords) {
        Chunk chunk;
        if (!chunkMap.TryGetValue(coords, out chunk))
            return;
        
        Destroy(chunk.terrain.gameObject);
        Destroy(chunk.terrain.terrainData);
        Destroy(chunk.terrain);
        chunkMap.Remove(coords);
    }
}
