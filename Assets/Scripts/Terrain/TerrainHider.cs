using UnityEngine;
using System.Collections;

[RequireComponent (typeof(TerrainManager))]
public class TerrainHider : MonoBehaviour {

    public Camera playerCamera;

    public bool hideTreesAndFoliage = true;
    public bool hideHeightmap = true;

    private TerrainManager manager;

    private Terrain terrain;
    private Vector3 leftBottom, leftTop, rightTop, rightBottom;
    private Vector3 lb, lt, rt, rb;

    public void Awake() {
        manager = GetComponent<TerrainManager>();
        if (playerCamera == null)
            playerCamera = Camera.main;
    }
    
    public void Update() {
        Vector3 cameraPosition = Camera.main.transform.position;
        Quaternion cameraRotation = Camera.main.transform.rotation;
        playerCamera.transform.position = new Vector3(cameraPosition.x, 0.0f, cameraPosition.z);
        playerCamera.transform.rotation = new Quaternion(0.0f, cameraRotation.y, 0.0f, cameraRotation.w);

        Vector3 chunkSize = manager.chunkSize;
        var loadedChuks = manager.LoadedChunksCoords;
        foreach (Chunk.Coords coords in loadedChuks) {
            Chunk chunk = manager.TryGetChunk(coords).Value;
            Vector3 pos = chunk.terrain.GetPosition();

            terrain = chunk.terrain;
            leftBottom = pos + new Vector3(0, 0, 0);
            leftTop = pos + new Vector3(0, 0, chunkSize.z);
            rightTop = pos + new Vector3(chunkSize.x, 0, chunkSize.z);
            rightBottom = pos + new Vector3(chunkSize.x, 0, 0);

            lb = playerCamera.WorldToViewportPoint(leftBottom);
            lt = playerCamera.WorldToViewportPoint(leftTop);
            rt = playerCamera.WorldToViewportPoint(rightTop);
            rb = playerCamera.WorldToViewportPoint(rightBottom);

            if (IsVisibleAnyVertice() || IsVisibleAnyEdge())
                Show();
            else
                Hide();
        }

        playerCamera.transform.position = cameraPosition;
        playerCamera.transform.rotation = cameraRotation;
    }

    private bool IsVisibleAnyVertice() {
        return IsVisible(lb) || IsVisible(lt) || IsVisible(rt) || IsVisible(rb);
    }

    private bool IsVisible(Vector3 v) {
        bool isOutOfViewport = v.x < 0.0f || v.x > 1.0f || v.y < 0.0f || v.y > 1.00f || v.z < 0.0f;
        return !isOutOfViewport;
    }

    private bool IsVisibleAnyEdge() {
        if (lb.z < 0.0f && lt.z < 0.0f && rt.z < 0.0f && rb.z < 0.0f)
            return false;

        return
            (lb.x * lt.x * lb.z * lt.z <= 0.0f) ||
            (lt.x * rt.x * lt.z * rt.z <= 0.0f) ||
            (rt.x * rb.x * rt.z * rb.z <= 0.0f) ||
            (rb.x * lb.x * rb.z * lb.z <= 0.0f);
    }

    private void Hide() {
        if (hideTreesAndFoliage)
            terrain.drawTreesAndFoliage = false;
        if (hideHeightmap)
            terrain.drawHeightmap = false;
    }
    
    private void Show() {
        if (hideTreesAndFoliage)
            terrain.drawTreesAndFoliage = true;
        if (hideHeightmap)
            terrain.drawHeightmap = true;
    }
}
