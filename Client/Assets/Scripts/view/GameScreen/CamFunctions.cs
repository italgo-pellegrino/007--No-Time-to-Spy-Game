using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

/**
 * Kamera Funktionen im Game Screen
 * MouseWheel zum Zoomen
 * Right Click zur Bewegung
 */
public class CamFunctions : MonoBehaviour
{
    public Camera main_cam;
    private Vector3 dragOrigin;
    private static readonly float dragSpeed = 10f;
    private static readonly float cam_zoom_scale = -0.5f;  // coefficient for camera zoom on wheel movement
    // Start is called before the first frame update
    void Start()
    {
        main_cam = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        camZoom();
    }

    private void LateUpdate()
    {
        mouseMove();
    }

    /**
     * If RClick held down and mouse moved
     * make camera go that way
     */
    private void mouseMove() {
        
        if (Input.GetMouseButtonDown(1)) //RClick press origin set
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(1)) //RClick held
        {
            if (!Input.mousePosition.Equals(dragOrigin)) // Mouse moved
            {
                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
                Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

                transform.position += move;
                dragOrigin = Input.mousePosition;
            }
        }
    }

    /**
     * Diese Methode zoomt die Kamera rein und raus
     * orthographic size attribute of camera is responsible for the size of viewport of the cam
     */
    private void camZoom()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            main_cam.GetComponent<Camera>().orthographicSize += Input.mouseScrollDelta.y * cam_zoom_scale;
            // preventing cam to zoom too much
            if (main_cam.GetComponent<Camera>().orthographicSize < 2)
            {
                main_cam.GetComponent<Camera>().orthographicSize = 2;
            }
        }
    }
}
