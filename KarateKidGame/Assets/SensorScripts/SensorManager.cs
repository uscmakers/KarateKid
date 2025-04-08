using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArmSimulator : MonoBehaviour
{
    public bool testMode = true;
    public float[] segmentLengths = new float[3]; // 0 = upper arm, 1 = forearm, 2 = hand
    public GameObject jointPrefab; // Optional: assign a Sprite or Sphere

    private SerialPort data_stream;
    private Vector3[] jointPositions = new Vector3[3]; // 0 = shoulder, 1 = elbow, 2 = wrist
    private GameObject[] jointObjects = new GameObject[3];
    private float testTime = 0;

    void Start()
    {
        // Initialize joint markers (sphere/sprite)
        for (int i = 0; i < 3; i++)
        {
            GameObject joint;

            if (jointPrefab != null)
                joint = Instantiate(jointPrefab, transform);
            else
            {
                joint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                joint.transform.localScale = Vector3.one * 0.2f;
                joint.transform.parent = transform;
            }

            joint.name = (i == 0 ? "Shoulder" : (i == 1 ? "Elbow" : "Wrist"));
            jointObjects[i] = joint;
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
        // Simulate or read orientations
        Vector3[] orientations = new Vector3[3]; // Each is roll, pitch, yaw

        if (testMode)
        {
            for (int i = 0; i < 3; i++)
            {
                orientations[i] = SimulateOrientation(i);
            }
            testTime += Time.deltaTime;
        }
        else if (data_stream != null && data_stream.IsOpen)
        {
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    string raw = data_stream.ReadLine();
                    string[] parts = raw.Split(' ');
                    if (parts.Length >= 4 && int.Parse(parts[0]) == i)
                    {
                        float roll = float.Parse(parts[1]) * Mathf.Deg2Rad;
                        float pitch = float.Parse(parts[2]) * Mathf.Deg2Rad;
                        float yaw = float.Parse(parts[3]) * Mathf.Deg2Rad;
                        orientations[i] = new Vector3(roll, pitch, yaw);
                    }
                }
            }
            catch (System.Exception) { return; }
        }

        // Calculate positions
        Vector3 shoulderPos = Vector3.zero;
        Vector3 elbowPos = shoulderPos + DirectionFromOrientation(orientations[0]) * segmentLengths[0];
        Vector3 wristPos = elbowPos + DirectionFromOrientation(orientations[1]) * segmentLengths[1];

        jointPositions[0] = shoulderPos;
        jointPositions[1] = elbowPos;
        jointPositions[2] = wristPos;

        // Apply positions
        for (int i = 0; i < 3; i++)
        {
            jointObjects[i].transform.localPosition = jointPositions[i];
        }

        Debug.DrawLine(jointPositions[0], jointPositions[1], Color.red);
        Debug.DrawLine(jointPositions[1], jointPositions[2], Color.blue);
    }

    Vector3 SimulateOrientation(int index)
    {
        float baseTime = testTime + index * 1.3f;
        float roll = Mathf.Sin(baseTime) * 20f;
        float pitch = Mathf.Cos(baseTime * 0.8f) * 30f;
        float yaw = Mathf.Sin(baseTime * 0.5f) * 40f;
        return new Vector3(roll, pitch, yaw) * Mathf.Deg2Rad;
    }

    Vector3 DirectionFromOrientation(Vector3 eulerRad)
    {
        float roll = eulerRad.x;
        float pitch = eulerRad.y;
        float yaw = eulerRad.z;

        Vector3 direction;
        direction.z = Mathf.Cos(yaw) * Mathf.Cos(pitch);
        direction.x = Mathf.Sin(yaw) * Mathf.Cos(pitch);
        direction.y = -Mathf.Sin(pitch);
        return direction.normalized;
    }

    void OnApplicationQuit()
    {
        if (data_stream != null && data_stream.IsOpen)
        {
            data_stream.Close();
        }
    }
}
