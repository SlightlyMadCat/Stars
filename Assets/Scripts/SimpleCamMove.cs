using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleCamMove : MonoBehaviour
{
    /*public GameObject tapPanel;
    public Image fillPanel;
    public GameObject fillTapZone;
    */
    private static readonly float PanSpeed = 10f;
    private static readonly float ZoomSpeedTouch = 0.3f;
    private static readonly float ZoomSpeedMouse = 0.2f;

    public float[] BoundsX = new float[] { -43f, -17f };
    public float[] BoundsZ = new float[] { 25f, 55f };
    //private static readonly float[] ZoomBounds = new float[] { 15f, 85f };

    private Camera cam;

    private Vector3 lastPanPosition;
    private int panFingerId; // Touch mode only

    private bool wasZoomingLastFrame; // Touch mode only
    private Vector2[] lastZoomPositions; // Touch mode only

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            HandleTouch();
        }
        else
        {
            HandleMouse();
        }
    }

    private void FixedUpdate()
    {
        //print(transform.position.z);
        /*if (transform.position.z < 40 && !tapPanel.activeSelf)
        {
            tapPanel.SetActive(true);
            fillTapZone.SetActive(true);
        } else if (transform.position.z >= 40 && tapPanel.activeSelf)
        {
            tapPanel.SetActive(false);
            fillTapZone.SetActive(false);
        }*/
    }

    void HandleTouch()
    {
        Touch touch = Input.GetTouch(0);
        int id = touch.fingerId;
        if (EventSystem.current.IsPointerOverGameObject(id))                //блок при нажатии на ui
        {
            //print("ui");
            return;
            // ui touched
        }

        switch (Input.touchCount)
        {

            case 1: // Panning
                wasZoomingLastFrame = false;

                // If the touch began, capture its position and its finger ID.
                // Otherwise, if the finger ID of the touch doesn't match, skip it.
                /*Touch touch = Input.GetTouch(0);

                int id = touch.fingerId;
                if (EventSystem.current.IsPointerOverGameObject(id))                //блок при нажатии на ui
                {
                    //print("ui");
                    return;
                    // ui touched
                }*/

                RaycastHit hitInfo;
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.GetTouch(0).position), out hitInfo, Mathf.Infinity);

                if (hitInfo.transform.name == "TapZone" || hitInfo.transform.tag == "TouchBlock")                //если это тап зона, не вдигать камеру
                {
                    return;
                }

                /*if (hitInfo.transform.tag == "TapField")
                {
                    return;
                }*/

                if (touch.phase == TouchPhase.Began)
                {
                    lastPanPosition = touch.position;
                    panFingerId = touch.fingerId;
                }
                else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved)
                {
                    PanCamera(touch.position);
                }
                break;

            case 2: // Zooming
                Vector2[] newPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };
                if (!wasZoomingLastFrame)
                {
                    lastZoomPositions = newPositions;
                    wasZoomingLastFrame = true;
                }
                else
                {
                    // Zoom based on the distance between the new positions compared to the 
                    // distance between the previous positions.
                    float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                    float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                    float offset = newDistance - oldDistance;

                    ZoomCamera(offset, ZoomSpeedTouch);

                    lastZoomPositions = newPositions;
                }
                break;

            default:
                wasZoomingLastFrame = false;
                break;
        }
    }

    void HandleMouse()
    {
        // On mouse down, capture it's position.
        // Otherwise, if the mouse is still down, pan the camera.

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            PanCamera(Input.mousePosition);
        }

        // Check for scrolling to zoom the camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll, ZoomSpeedMouse);
    }

    void PanCamera(Vector3 newPanPosition)
    {
        if (Vector3.Distance(lastPanPosition, newPanPosition) < 6)
            return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            int id = touch.fingerId;
            if (EventSystem.current.IsPointerOverGameObject(id))                //блок при нажатии на ui
            {
                //print("ui");
                return;
                // ui touched
            }
        }

        // Determine how much to move the camera
        Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
        //print(Vector3.Distance(lastPanPosition, newPanPosition));
        Vector3 move = new Vector3(offset.x * PanSpeed * 1f, 0, offset.y * PanSpeed * 1.5f);
        move = Quaternion.Euler(0, -40, 0) * move;              //разворот вектора движения камеры на угол 30

        // Perform the movement
        transform.Translate(-move, Space.World);

        // Ensure the camera remains within bounds.
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, BoundsX[0], BoundsX[1]);
        pos.z = Mathf.Clamp(transform.position.z, BoundsZ[0], BoundsZ[1]);
        transform.position = pos;

        // Cache the position
        lastPanPosition = newPanPosition;
    }

    void ZoomCamera(float offset, float speed)
    {
        /*if (offset == 0)
        {
            return;
        }

        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - (offset * speed), ZoomBounds[0], ZoomBounds[1]);*/
    }
}