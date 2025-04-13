using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class CameraMovement : MonoBehaviour
{
    bool draggingMap = false;
    Vector3 dragStart = Vector3.zero;

    float originalZoom = 6f;

    bool zoomOnUpdate = false;
    float zoomAmount = 0f;
    float zoomOutMax = 6f;
    const float MAX_ZOOM_IN = 1f;

    private GridLayout tileGrid;

    // At zoom level 1, tile bounds seem to be 4x3
    Rect screenBounds = new Rect(-2, -2, 4, 4);
    BoundsInt tileGridBounds;

    public void CameraZoomOnUpdate(float zoomAdjust)
    {
        zoomOnUpdate = true;
        zoomAmount = zoomAdjust;
        Debug.Log("Zoom: " + Camera.main.orthographicSize);
    }

    public void CameraDragStart()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        dragStart = Camera.main.ScreenToWorldPoint(mousePosition);
        draggingMap = true;
    }
    public void CameraDragEnd()
    {
        draggingMap = false;
    }    

    // Start is called before the first frame update
    void Start()
    {
        tileGrid = FindObjectOfType<GridLayout>();
        if (tileGrid == null)
        {
            Debug.LogError("GridLayout not found in the scene.");
            return;
        }
        Tilemap tileMap = null;
        foreach (Transform child in tileGrid.transform)
        {
            tileMap = child.GetComponent<Tilemap>();
            if (tileMap != null)
                break;
        }
        if (tileMap == null)
        {
            Debug.LogError("Tilemap not found on GridLayout.");            
        }
        else
        {
            //Debug.Log("GridLayout found: " + tileGrid.name + " with cell size: " + tileGrid.cellSize);

            tileGridBounds = tileMap.cellBounds;
            //Debug.Log("Tilemap bounds: " + tileGridBounds);
            screenBounds = new Rect(
            tileGridBounds.xMin * tileGrid.cellSize.x,
            tileGridBounds.yMin * tileGrid.cellSize.y,
            tileGridBounds.size.x * tileGrid.cellSize.x,
            tileGridBounds.size.y * tileGrid.cellSize.y
            );
            Debug.Log("Screen bounds set to: " + screenBounds);
        }
        originalZoom = Camera.main.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        if (zoomOnUpdate)
        {
            Camera.main.orthographicSize -= zoomAmount;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, MAX_ZOOM_IN, zoomOutMax);            
            zoomOnUpdate = false;
            //Debug.Log("Zoom: " + Camera.main.orthographicSize);
        }
        if (draggingMap)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            //float moveSpeed = 0.5f; // Adjust this value to control the speed of the camera movement
            // Get mouse movement
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 difference = dragStart - mouseWorldPos;
            Vector3 newPos = Camera.main.transform.position + difference;
            // Clamp the camera position to the screen bounds
            newPos.x = Mathf.Clamp(newPos.x, screenBounds.xMin, screenBounds.xMax);
            newPos.y = Mathf.Clamp(newPos.y, screenBounds.yMin, screenBounds.yMax);
            Camera.main.transform.position = newPos;
        }
    }
}
