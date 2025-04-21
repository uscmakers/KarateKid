using UnityEngine;
using UnityEngine.SceneManagement;

public class OverlayModule1Demo : MonoBehaviour
{
    public string demoSceneName = "Module1Demo";
    public GameObject cameraClonePrefab;  // Drag prefab from step 2
    private bool hasSpawned = false;

    void Start()
    {
        SceneManager.LoadScene(demoSceneName, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == demoSceneName && !hasSpawned)
        {
            // Clone the prefab into current scene
            Instantiate(cameraClonePrefab);
            hasSpawned = true;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
