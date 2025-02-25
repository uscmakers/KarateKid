using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CircleManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.CircleManager += UpdateCircles;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDisable(){
        EventManager.CircleManager -= UpdateCircles;
    }

    void UpdateCircles(int n){
        gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Circles: " + n;
    }
}
