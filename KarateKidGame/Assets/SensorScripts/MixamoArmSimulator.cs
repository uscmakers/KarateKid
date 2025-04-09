using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class MixamoArmFromArduino : MonoBehaviour
{
    public bool testMode = false;

    public string portName = "/dev/cu.usbserial-1130"; // Replace with your actual port
    public int baudRate = 115200;

    public string[] boneNames = new string[3] {
        "mixamorig:RightShoulder",
        "mixamorig:RightArm",
        "mixamorig:RightHand"
    };

    private Transform[] joints = new Transform[3]; // Shoulder, Elbow, Wrist
    private SerialPort data_stream;
    private float testTime = 0;

    // Arm segment lengths
    public float upperArmLength = 0.5f;
    public float forearmLength = 0.4f;

    private Dictionary<int, Vector3> jointOrientations = new Dictionary<int, Vector3>();

    private Vector3[][] presetPoses = new Vector3[][] {
        new Vector3[] { new Vector3(10, 30, 0),  new Vector3(15, -10, 5) },
        new Vector3[] { new Vector3(0, 45, 0),   new Vector3(20, -5, 15) },
        new Vector3[] { new Vector3(-10, 25, 0), new Vector3(10, 10, -5) },
        new Vector3[] { new Vector3(15, -15, 0), new Vector3(-5, 5, 10) },
        new Vector3[] { new Vector3(0, 0, 0),    new Vector3(0, 0, 0) }
    };

    void Start()
    {
        for (int i = 0; i < boneNames.Length; i++)
        {
            joints[i] = transform.FindDeepChild(boneNames[i]);
            if (joints[i] == null)
                Debug.LogWarning($"Could not find bone: {boneNames[i]}");
        }

        if (!testMode)
        {
            data_stream = new SerialPort(portName, baudRate);
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
        jointOrientations.Clear();

        if (!testMode && data_stream != null && data_stream.IsOpen)
        {
            try
            {
                while (data_stream.BytesToRead > 0)
                {
                    string raw = data_stream.ReadLine();
                    Debug.Log(raw); // Optional: show raw data
                    string[] parts = raw.Split(' ');
                    if (parts.Length >= 4)
                    {
                        int id = int.Parse(parts[0]);
                        float roll = float.Parse(parts[1]) * Mathf.Deg2Rad;
                        float pitch = float.Parse(parts[2]) * Mathf.Deg2Rad;
                        float yaw = float.Parse(parts[3]) * Mathf.Deg2Rad;
                        jointOrientations[id] = new Vector3(roll, pitch, yaw);
                    }
                }
            }
            catch { }
        }

        ApplyPose();
    }

    void ApplyPose()
    {
        Vector3 shoulderEuler = Vector3.zero;
        Vector3 elbowEuler = Vector3.zero;

        if (testMode)
        {
            int poseIndex = Mathf.FloorToInt(testTime % (presetPoses.Length * 2)) / 2;
            shoulderEuler = presetPoses[poseIndex][0] * Mathf.Deg2Rad;
            elbowEuler = presetPoses[poseIndex][1] * Mathf.Deg2Rad;
            testTime += Time.deltaTime;
        }
        else
        {
            shoulderEuler = jointOrientations.GetValueOrDefault(0, Vector3.zero);
            elbowEuler = jointOrientations.GetValueOrDefault(1, Vector3.zero);
        }

        Vector3 shoulderPos = joints[0].position;
        Vector3 elbowOffset = DirectionFromEuler(shoulderEuler) * upperArmLength;
        Vector3 wristOffset = DirectionFromEuler(elbowEuler) * forearmLength;

        Vector3 elbowPos = shoulderPos + elbowOffset;
        Vector3 wristPos = elbowPos + wristOffset;

        // Optional: move elbow and wrist for visualization (won’t animate bones properly in Mixamo unless rig modified)
        if (joints[1] != null)
        {
            joints[1].position = elbowPos;
            joints[1].rotation = Quaternion.LookRotation(wristOffset.normalized);
        }

        if (joints[2] != null)
        {
            joints[2].position = wristPos;
            joints[2].rotation = Quaternion.LookRotation(wristOffset.normalized);
        }

        Debug.Log($"ElbowPos: {elbowPos:F2}, WristPos: {wristPos:F2}");
    }

    Vector3 DirectionFromEuler(Vector3 eulerRad)
    {
        Quaternion rotation = Quaternion.Euler(eulerRad * Mathf.Rad2Deg);
        return rotation * Vector3.forward;
    }

    void OnApplicationQuit()
    {
        if (data_stream != null && data_stream.IsOpen)
            data_stream.Close();
    }
}
