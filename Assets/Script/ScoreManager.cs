using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public GameObject scoreText;
    public GameObject gameOverText;
    public GameObject resetButton;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.ScoreManager += UpdateScore;
        EventManager.GameOver += GameOver;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateScore(int num){
        scoreText.GetComponent<TMPro.TextMeshProUGUI>().text = "Score: " + num;
    }

    private void GameOver(){
        gameOverText.GetComponent<TMPro.TextMeshProUGUI>().text = "Game Over";
        resetButton.SetActive(true);
    }
}
