using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Numerics;

public class ArduinoTest : MonoBehaviour
{

    public int numSensors;
    UnityEngine.Vector3[] finalPoses;
    SerialPort data_stream = new SerialPort("/dev/cu.usbserial-1130", 115200);
    public float[] length;
    // Start is called before the first frame update
    void Start()
    {
        finalPoses = new UnityEngine.Vector3[numSensors];
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

            for(int i = 1; i < 4; i++){
                orientation[i-1] = float.Parse(oS[i]) * Mathf.PI / 180;
                //print(orientation[i - 1]);
            }
            UnityEngine.Vector3 finalPos;
            finalPos.z = Mathf.Cos(orientation[2]) * Mathf.Cos(orientation[1]);
            finalPos.x = Mathf.Sin(orientation[2]) * Mathf.Cos(orientation[1]);
            finalPos.y = -Mathf.Sin(orientation[1]);
            finalPos.Normalize();
            finalPos *= length[int.Parse(oS[0])];
            finalPoses[int.Parse(oS[0])] = finalPos;
            print(finalPos);
            //transform.GetChild(int.Parse(oS[0])).transform.position = finalPoses[int.Parse(oS[0])];
            
        }

    }
}