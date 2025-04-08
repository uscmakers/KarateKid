using UnityEngine;
using TMPro;

public class LevelHeading : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI levelText;    
    public float lifetime = 5f;          
    [Header("Sound")]
    public AudioClip whooshSound;       
    void Start()
    {
        if (levelText == null)
        {
            Debug.LogError("LevelHeading: No TextMeshProUGUI assigned!");
            return;
        }

        if (whooshSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(whooshSound, Camera.main.transform.position);
        }
        Destroy(levelText.gameObject, lifetime);
    }
}
