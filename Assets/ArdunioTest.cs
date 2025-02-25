using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

public class ArduinoTest : MonoBehaviour
{
    SerialPort data_stream = new SerialPort("COM4", 9600);
    public float length = 0f;
    // Start is called before the first frame update
    void Start()
    {
        data_stream.Open();
    }

    // Update is called once per frame
    void Update()
    {
        string raw = data_stream.ReadLine();
        if(raw != null){
            print(raw);
            //fills in starting from 1
            string[] oS =  raw.Split(' ');
            //print(oS[1]);
            //0 is roll, 1 is pitch, 2 is yaw, heading towards 2 big holes 
            float[] orientation = new float[3];

            for(int i = 0; i < 3; i++){
                orientation[i] = float.Parse(oS[i]) * Mathf.PI / 180;
                print(orientation[i]);
            }
            Vector3 finalPos;
            finalPos.z = Mathf.Cos(orientation[2]) * Mathf.Cos(orientation[1]);
            finalPos.x = Mathf.Sin(orientation[2]) * Mathf.Cos(orientation[1]);
            finalPos.y = -Mathf.Sin(orientation[1]);
            finalPos.Normalize();
            finalPos *= length;
            print(finalPos);
            transform.position = finalPos;
        }

    }
}
