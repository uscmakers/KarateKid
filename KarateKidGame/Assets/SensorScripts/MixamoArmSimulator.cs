using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class MixamoArmSimulator : MonoBehaviour
{
    public bool testMode = true;
    public string[] boneNames = new string[3] {
        "mixamorig:RightShoulder", 
        "mixamorig:RightArm", 
        "mixamorig:RightHand"
    };

    private Transform[] joints = new Transform[3]; // Shoulder, Elbow, Wrist
    private SerialPort data_stream;
    private float testTime = 0;

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
        for (int i = 0; i < 3; i++)
        {
            joints[i] = transform.FindDeepChild(boneNames[i]);
            if (joints[i] == null)
                Debug.LogWarning($"Could not find bone: {boneNames[i]}");
        }

        if (!testMode)
        {
            data_stream = new SerialPort("/dev/cu.usbserial-022AB660", 115200);
            try
            {
                data_stream.Open();
                data_stream.ReadTimeout = 25;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Serial port failed: " + e.Message);
            }
        }
    }

    void Update()
    {
        Vector3 shoulderEulerRad;
        Vector3 elbowEulerRad;

        if (testMode)
        {
            int poseIndex = Mathf.FloorToInt(testTime % (presetPoses.Length * 2)) / 2;
            shoulderEulerRad = presetPoses[poseIndex][0] * Mathf.Deg2Rad;
            elbowEulerRad = presetPoses[poseIndex][1] * Mathf.Deg2Rad;
            testTime += Time.deltaTime;
        }
        else
        {
            shoulderEulerRad = ReadOrientationFromSerial(0);
            elbowEulerRad = ReadOrientationFromSerial(1);
        }

        Vector3 shoulderPos = joints[0].position;
        Vector3 elbowOffset = DirectionFromOrientation(shoulderEulerRad) * 0.5f;
        Vector3 wristOffset = DirectionFromOrientation(elbowEulerRad) * 0.4f;

        Vector3 elbowPos = shoulderPos + elbowOffset;
        Vector3 wristPos = elbowPos + wristOffset;

        if (joints[1] != null)
            joints[1].LookAt(elbowPos);
        if (joints[2] != null)
            joints[2].LookAt(wristPos);

        Debug.Log($"Shoulder: {shoulderPos:F2}, Elbow: {elbowPos:F2}, Wrist: {wristPos:F2}");
    }

    Vector3 ReadOrientationFromSerial(int jointID)
    {
        if (data_stream != null && data_stream.IsOpen)
        {
            try
            {
                string raw = data_stream.ReadLine();
                string[] parts = raw.Split(' ');
                if (parts.Length >= 4 && int.Parse(parts[0]) == jointID)
                {
                    float roll = float.Parse(parts[1]) * Mathf.Deg2Rad;
                    float pitch = float.Parse(parts[2]) * Mathf.Deg2Rad;
                    float yaw = float.Parse(parts[3]) * Mathf.Deg2Rad;
                    return new Vector3(roll, pitch, yaw);
                }
            }
            catch (System.Exception) { }
        }
        return Vector3.zero;
    }

    Vector3 DirectionFromOrientation(Vector3 eulerRad) {
    Quaternion rotation = Quaternion.Euler(eulerRad * Mathf.Rad2Deg);
    return rotation * Vector3.forward;
    }


    void OnApplicationQuit()
    {
        if (data_stream != null && data_stream.IsOpen)
            data_stream.Close();
    }
}
