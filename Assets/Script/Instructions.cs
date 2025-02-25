using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Instructions : MonoBehaviour
{
    public GameObject InstructText;
    public GameObject circleImg;
    public GameObject guidanceText;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.ChangeInstructions += InstructionManager;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable(){
        EventManager.ChangeInstructions -= InstructionManager;
    }

    void InstructionManager(int num){
        if(num == 0){
            InstructText.GetComponent<TMPro.TextMeshProUGUI>().text = "Draw 5 Counterclockwise Circles with the Right Hand.";
        }
        else if(num == 1){
            circleImg.SetActive(false);
            guidanceText.GetComponent<TMPro.TextMeshProUGUI>().text = "Now freehand! Darker the color, the better!";
        }
        else if(num == 2){
            InstructText.GetComponent<TMPro.TextMeshProUGUI>().text = "Draw 5 Clockwise Circles with the Left Hand.";
            circleImg.SetActive(true);
            guidanceText.GetComponent<TMPro.TextMeshProUGUI>().text = "Trace the Circle!";
        }
        else if(num == 3){
            circleImg.SetActive(false);
            guidanceText.GetComponent<TMPro.TextMeshProUGUI>().text = "Now freehand! Darker the color, the better!";
        }
        else if(num == 4){
            InstructText.GetComponent<TMPro.TextMeshProUGUI>().text = "Next Level";
            SceneManager.LoadScene(1);
        }
    }
}
