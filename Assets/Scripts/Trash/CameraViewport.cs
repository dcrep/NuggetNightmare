using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewport : MonoBehaviour
{
    void Awake()
    {
        // Set the camera to orthographic mode
        Camera camera = GetComponent<Camera>(); //Camera.main;
        if (camera != null)
        {
            //camera.orthographic = true;
            //camera.orthographicSize = 5; // Set the size of the orthographic camera
            
            //camera.rect = new Rect(0f, 0.1f, 0.5f, 0.5f);

            //Resolution currentRes = Screen.currentResolution;

            camera.pixelRect = new Rect(0f, 0f, Screen.width - 100f , Screen.height);
            Debug.Log("CAMERA: Pixel width :" + camera.pixelWidth + " Pixel height : " + camera.pixelHeight);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
