using UnityEngine;
using TMPro;

public class LevelHeading : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI levelText;    
    public float lifetime = 5f;

    void Start()
    {
        if (levelText == null)
        {
            Debug.LogError("LevelHeading: No TextMeshProUGUI assigned!");
            return;
        }

        Destroy(levelText.gameObject, lifetime);
    }
}
