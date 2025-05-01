using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MatchMainCamera : MonoBehaviour
{
    void Start()
    {
        Camera mainCam = Camera.main;
        Camera overlayCam = GetComponent<Camera>();

        if (mainCam != null && overlayCam != null && overlayCam != mainCam)
        {
            // Match transform
            transform.position = mainCam.transform.position;
            transform.rotation = mainCam.transform.rotation;

            // Match camera settings
            overlayCam.fieldOfView = mainCam.fieldOfView;
            overlayCam.orthographic = mainCam.orthographic;
            overlayCam.orthographicSize = mainCam.orthographicSize;
            overlayCam.nearClipPlane = mainCam.nearClipPlane;
            overlayCam.farClipPlane = mainCam.farClipPlane;

            // Optional visual consistency
            overlayCam.clearFlags = CameraClearFlags.Depth;
            overlayCam.backgroundColor = mainCam.backgroundColor;
        }
    }
}
