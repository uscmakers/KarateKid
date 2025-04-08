using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [Tooltip("Name of the scene to load")]
    public string sceneToLoad = "Module1Practice";

    [Tooltip("Reference to the Countdown Timer script")]
    public CountdownTimer countdownTimer;  // Drag your timer object here

    void Update()
    {
        // 1. If player presses space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadNextScene();
        }

        // 2. If timer hits 0 (or lower)
        if (countdownTimer != null && countdownTimer.timeRemaining <= 0)
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
