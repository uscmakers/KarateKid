using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;

public class PoseDataReceiver : MonoBehaviour
{
    public string flaskServerURL = "http://localhost:5050/landmarks";
    public float updateInterval = 0.1f; // Update 10 times per second
    public bool debugLogData = true; // Toggle for debug logging

    [System.Serializable]
    public class LandmarkData
    {
        public int x;
        public int y;
        public int z;
        public bool visible;
        public string movement;
    }

    private Dictionary<string, LandmarkData> landmarks = new Dictionary<string, LandmarkData>();

    void Start()
    {
        Debug.Log("PoseDataReceiver started. Connecting to: " + flaskServerURL);
        StartCoroutine(GetPoseData());
    }

    IEnumerator GetPoseData()
    {
        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(flaskServerURL))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = webRequest.downloadHandler.text;
                   
                    // Debug log the raw JSON response
                    if (debugLogData)
                    {
                        Debug.Log("Raw JSON Response: " + jsonResponse);
                    }

                    try
                    {
                        landmarks = JsonConvert.DeserializeObject<Dictionary<string, LandmarkData>>(jsonResponse);
                       
                        if (debugLogData && landmarks != null && landmarks.Count > 0)
                        {
                            Debug.Log("Successfully received " + landmarks.Count + " landmarks");
                            foreach (var kvp in landmarks)
                            {
                                Debug.Log($"{kvp.Key}: X={kvp.Value.x}, Y={kvp.Value.y}, Z={kvp.Value.z}, " +
                                         $"Visible={kvp.Value.visible}, Movement={kvp.Value.movement}");
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("JSON Parsing Error: " + e.Message);
                    }
                }
                else
                {
                    Debug.LogError("Request Error: " + webRequest.error);
                    Debug.LogError("Response Code: " + webRequest.responseCode);
                }
            }
            yield return new WaitForSeconds(updateInterval);
        }
    }

    void Update()
    {
        // Optional: Add any real-time processing here
    }
}