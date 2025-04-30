using UnityEngine;

public class SceneEntryWhoosh : MonoBehaviour
{
    public AudioClip whooshSound;
    private AudioSource audioSource;

    void Start()
    {
        if (whooshSound == null)
        {
            Debug.LogWarning("Whoosh sound not assigned.");
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f;

        audioSource.PlayOneShot(whooshSound);
    }
}
