using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetToStartScreen : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 1. Stop music
            MusicManager musicManager = FindObjectOfType<MusicManager>();
            if (musicManager != null)
            {
                AudioSource audioSource = musicManager.GetAudioSource();
                if (audioSource != null)
                {
                    audioSource.Stop();
                    audioSource.time = 0f;
                    audioSource.Play();
                }
            }

            // 2. Close serial port cleanly
            MixamoArmSimulator armSim = FindObjectOfType<MixamoArmSimulator>();
            if (armSim != null)
            {
                armSim.CloseSerial();
            }

            // 3. Load scene
            SceneManager.LoadScene("StartScreen");
        }
    }
}
