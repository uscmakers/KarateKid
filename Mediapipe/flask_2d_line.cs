using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LineDetectionReceiver : MonoBehaviour
{
    [Header("Server Settings")]
    public string serverURL = "http://localhost:5051/wrist_data";
    public float pollInterval = 0.05f; // Poll 20 times per second

    [Header("UI Settings")]
    public Text statusText;
    public Text movementText;
    public Image detectionIndicator;
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.red;

    [Header("Detection Threshold")]
    public int movementThreshold = 300; // Should match Python threshold

    [System.Serializable]
    private class WristData
    {
        public bool success;
        public Position position;
        public bool movement_detected;
        public float x_movement;
        public int threshold;
        public double timestamp;
    }

    [System.Serializable]
    private class Position
    {
        public int x;
        public int y;
    }

    private bool lineDetected = false;
    private float lastDetectionTime = 0f;
    private float displayDuration = 1f; // How long to show detection

    void Start()
    {
        StartCoroutine(PollWristData());
        
        // Initialize UI
        if (detectionIndicator != null)
        {
            detectionIndicator.color = inactiveColor;
        }
    }

    IEnumerator PollWristData()
    {
        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(serverURL))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = webRequest.downloadHandler.text;
                    ProcessWristData(jsonResponse);
                }
                else
                {
                    Debug.LogWarning("Request failed: " + webRequest.error);
                    UpdateStatus("Server connection failed");
                }
            }
            yield return new WaitForSeconds(pollInterval);
        }
    }

    private void ProcessWristData(string jsonData)
    {
        try
        {
            WristData data = JsonUtility.FromJson<WristData>(jsonData);
            
            if (data.success)
            {
                UpdateStatus($"Tracking: X={data.position.x}, Y={data.position.y}");
                UpdateMovementDisplay(data.x_movement);

                if (data.movement_detected)
                {
                    lineDetected = true;
                    lastDetectionTime = Time.time;
                    Debug.Log($"Line detected! Movement: {data.x_movement}px");
                }
            }
            else
            {
                UpdateStatus("Wrist not detected");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON parse error: " + e.Message);
            UpdateStatus("Data error");
        }
    }

    void Update()
    {
        // Handle the line detection display
        if (lineDetected && Time.time - lastDetectionTime > displayDuration)
        {
            lineDetected = false;
        }

        // Update UI elements
        UpdateDetectionIndicator();
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    private void UpdateMovementDisplay(float movement)
    {
        if (movementText != null)
        {
            movementText.text = $"Movement: {movement:F0}px";
        }
    }

    private void UpdateDetectionIndicator()
    {
        if (detectionIndicator != null)
        {
            detectionIndicator.color = lineDetected ? activeColor : inactiveColor;
            
            // Optional: Add visual feedback when line is detected
            if (lineDetected)
            {
                detectionIndicator.transform.localScale = Vector3.one * 1.2f;
            }
            else
            {
                detectionIndicator.transform.localScale = Vector3.one;
            }
        }
    }
}