using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOverlayController : MonoBehaviour
{
    [Header("Overlay Settings")]
    public string overlaySceneName = "ModuleDemo";
    public Vector2 overlaySize = new Vector2(0.25f, 0.25f); // Width/Height of the overlay
    public Vector2 overlayPosition = new Vector2(0f, 0f);   // Position on screen (0,0 = bottom-left)

    void Start()
    {
        // Load ModuleDemo additively
        SceneManager.LoadScene(overlaySceneName, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnOverlaySceneLoaded;
    }

    void OnOverlaySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != overlaySceneName) return;

        // Find any camera in the overlay scene (ideally "OverlayCamera")
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Camera cam = root.GetComponentInChildren<Camera>();
            if (cam != null && cam != Camera.main)
            {
                SetupOverlayCamera(cam);
                break;
            }
        }

        // Detach handler so it doesn't fire on future scenes
        SceneManager.sceneLoaded -= OnOverlaySceneLoaded;
    }

    void SetupOverlayCamera(Camera overlayCam)
    {
        // Remove RenderTexture (we're drawing directly)
        overlayCam.targetTexture = null;

        // Shrink camera output to a screen corner
        overlayCam.rect = new Rect(overlayPosition.x, overlayPosition.y, overlaySize.x, overlaySize.y);

        // Make sure it renders after the main scene
        overlayCam.depth = Camera.main.depth + 1;

        // Optional cleanup
        overlayCam.clearFlags = CameraClearFlags.Depth;
        overlayCam.enabled = true;

        // Disable its AudioListener if present to avoid double-audio errors
        AudioListener listener = overlayCam.GetComponent<AudioListener>();
        if (listener != null) listener.enabled = false;
    }
}
