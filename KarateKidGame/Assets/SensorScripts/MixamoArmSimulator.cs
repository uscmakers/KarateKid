using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class MixamoArmSimulator : MonoBehaviour
{
    public bool testMode = true;
    private string[] boneNames = new string[4] {
        "mixamorig:RightArm", 
        "mixamorig:RightForeArm",
        "mixamorig:LeftArm",
        "mixamorig:LeftForeArm"
    };

    public bool[] active = new bool[4];

    private Vector3[] rotations = new Vector3[4];

    private Vector3[] prevRotations = new Vector3[4];
    private Transform[] joints = new Transform[4]; // Shoulder, Elbow, Wrist
    private wrmhl data_stream = new wrmhl();
    private float testTime = 0;

    Vector3 shoulderEulerRad = new Vector3(0, 0, 0);
    Vector3 elbowEulerRad = new Vector3(0, 0, 0);

    // Test poses 
    private Vector3[][] presetPoses = new Vector3[][]
    {
        new Vector3[] { new Vector3(10, 30, 0),  new Vector3(15, -10, 5) },
        new Vector3[] { new Vector3(0, 45, 0),   new Vector3(20, -5, 15) },
        new Vector3[] { new Vector3(-10, 25, 0), new Vector3(10, 10, -5) },
        new Vector3[] { new Vector3(15, -15, 0), new Vector3(-5, 5, 10) },
        new Vector3[] { new Vector3(0, 0, 0),    new Vector3(0, 0, 0) }
    };

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            joints[i] = transform.FindDeepChild(boneNames[i]);
            if (joints[i] == null)
                Debug.LogWarning($"Could not find bone: {boneNames[i]}");
        }

        if (!testMode)
        {
            
            try
            {
                data_stream.set("COM11", 115200, 20, 4);
                data_stream.connect();
                print("Succesful opening port");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Serial port failed: " + e.Message);
            }
        }
    }

    void FixedUpdate()
    {

        if (testMode)
        {
            int poseIndex = Mathf.FloorToInt(testTime % (presetPoses.Length * 2)) / 2;
            shoulderEulerRad = presetPoses[poseIndex][0] * Mathf.Deg2Rad;
            elbowEulerRad = presetPoses[poseIndex][1] * Mathf.Deg2Rad;
            testTime += Time.deltaTime;
        }
        else
        {
            //print("Reading orientation");
            Vector3 prev;
            for(int i = 0; i < 4; i++){
                if(active[i]){
                    prev = rotations[i];
                    rotations[i] = ReadOrientationFromSerial(i);
                    if(rotations[i].Equals(new Vector3(0, 0, 0))){
                        rotations[i] = prev;
                    }
                    else if(i == 1 || i == 3){
                        rotations[i] -= rotations[i-1];
                    }
                    
                    joints[i].Rotate(rotations[i] - prevRotations[i]);
                    prevRotations[i] = rotations[i];
                    //joints[i].localEulerAngles = rotations[i];
                    print(i + " " + rotations[i]);                
                }
            }
        }

        // Vector3 shoulderPos = joints[0].position;
        // Vector3 elbowOffset = DirectionFromOrientation(shoulderEulerRad) * 0.5f;
        // Vector3 wristOffset = DirectionFromOrientation(elbowEulerRad) * 0.4f;

        // Vector3 elbowPos = shoulderPos + elbowOffset;
        // Vector3 wristPos = elbowPos + wristOffset;

        // if (joints[1] != null)
        //     joints[1].LookAt(elbowPos);
        // if (joints[2] != null)
        //     joints[2].LookAt(wristPos);

        //Debug.Log($"Shoulder: {shoulderPos:F2}, Elbow: {elbowPos:F2}, Wrist: {wristPos:F2}");
    }

    Vector3 ReadOrientationFromSerial(int jointID)
    {
        if (data_stream != null)
        {
            //print("Object Id: " + jointID);
            try
            {
                string raw = data_stream.readQueue();
                if(!raw.Equals("NULL")){
                    print(raw);
                    string[] parts = raw.Split(' ');
                    if (parts.Length >= 4 && int.Parse(parts[0]) == jointID)
                    {
                        // float roll = float.Parse(parts[1]) * Mathf.Deg2Rad;
                        // float pitch = float.Parse(parts[2]) * Mathf.Deg2Rad;
                        // float yaw = float.Parse(parts[3]) * Mathf.Deg2Rad;
                        float roll = float.Parse(parts[1]);
                        float pitch = float.Parse(parts[2]);
                        float yaw = float.Parse(parts[3]);
                        //print(new UnityEngine.Vector3(pitch, roll, yaw));
                        return new UnityEngine.Vector3(-roll, -pitch, yaw);
                    }
                }
            }
            catch (System.Exception) {/*print("Failed to read"); */}
        }

        return UnityEngine.Vector3.zero;
    }

    UnityEngine.Vector3 DirectionFromOrientation(UnityEngine.Vector3 eulerRad) {
    UnityEngine.Quaternion rotation = UnityEngine.Quaternion.Euler(eulerRad * Mathf.Rad2Deg);
    return rotation * UnityEngine.Vector3.forward;
        // UnityEngine.Vector3 finalPos;
        // finalPos.z = Mathf.Cos(eulerRad[2]) * Mathf.Cos(eulerRad[1]);
        // finalPos.x = Mathf.Sin(eulerRad[2]) * Mathf.Cos(eulerRad[1]);
        // finalPos.y = -Mathf.Sin(eulerRad[1]);
        // finalPos.Normalize();
        // return finalPos;
    }


    void OnApplicationQuit()
    {
        if (data_stream != null)
            data_stream.close();
    }
}

