using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetToStartScreen : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
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

            SceneManager.LoadScene("StartScreen");
        }
    }
}
