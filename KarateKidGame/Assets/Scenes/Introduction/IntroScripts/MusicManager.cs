using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip backgroundMusic; 
    private AudioSource audioSource;
    private static MusicManager instance; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true; 
        audioSource.playOnAwake = false;
        audioSource.volume = 0.1f; 
        audioSource.Play();
    }

    public AudioSource GetAudioSource()
    {
        return audioSource;
    }
}
