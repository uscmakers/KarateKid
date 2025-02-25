using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float length = 0f;
        Vector3 Orientation = new Vector3(); // yaw, pitch
        Vector3 finalPos;

        finalPos.z = Mathf.Sin(Orientation.y) * length;
        finalPos.x = Mathf.Cos(Orientation.x) * length;
        finalPos.y = Mathf.Sin(Orientation.x) * length;



    }
}
