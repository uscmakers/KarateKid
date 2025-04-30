using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining = 59f;
    public TextMeshProUGUI countdownText; 
    private bool isCounting = true;

    void Update()
    {
        if (isCounting && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay(Mathf.CeilToInt(timeRemaining));
        }
        else if (isCounting && timeRemaining <= 0)
        {
            isCounting = false;
            timeRemaining = 0;
            UpdateTimerDisplay(0);
        }
    }

    void UpdateTimerDisplay(int seconds)
    {
        countdownText.text = seconds.ToString("00");
    }
}
