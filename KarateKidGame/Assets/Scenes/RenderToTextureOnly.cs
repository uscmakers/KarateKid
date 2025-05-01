using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RenderToTextureOnly : MonoBehaviour
{
    void Start()
    {
        Camera cam = GetComponent<Camera>();

        // Ensure we only render to texture
        cam.targetTexture = Resources.Load<RenderTexture>("ModuleDemoView");
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.clear;
        cam.depth = 100; // higher than default camera
        cam.cullingMask = ~0; // Everything

        // Most important: shrink viewport so nothing shows on screen
        cam.rect = new Rect(0, 0, 0, 0); // ← Hides it from Game view entirely
    }
}
