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
    
    // Add bone references similar to MixamoArmFromArduino
    public string[] boneNames = new string[3] {
        "mixamorig:RightShoulder",
        "mixamorig:RightArm",
        "mixamorig:RightHand"
    };
    
    // Add more bones as needed for full body
    private Transform[] joints = new Transform[3];
    
    // Distance parameters similar to MixamoArm
    public float upperArmLength = 0.5f;
    public float forearmLength = 0.4f;
    
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
        
        // Find and store references to the character's bones
        for (int i = 0; i < boneNames.Length; i++)
        {
            joints[i] = transform.FindDeepChild(boneNames[i]);
            if (joints[i] == null)
                Debug.LogWarning($"Could not find bone: {boneNames[i]}");
        }
        
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
        // Apply the pose data to the character if we have valid landmarks
        if (landmarks != null && landmarks.Count > 0)
        {
            ApplyPose();
        }
    }
    
    void ApplyPose()
    {
        // This method uses the landmark data to position and rotate character bones
        // Customize this based on the specific landmark keys your Flask server provides
        
        // Example mapping for right arm (assuming your landmarks include these keys)
        if (landmarks.ContainsKey("right_shoulder") && 
            landmarks.ContainsKey("right_elbow") && 
            landmarks.ContainsKey("right_wrist") && 
            landmarks["right_shoulder"].visible && 
            landmarks["right_elbow"].visible && 
            landmarks["right_wrist"].visible)
        {
            // Convert landmark coordinates to Vector3 positions
            Vector3 shoulderPos = new Vector3(
                landmarks["right_shoulder"].x,
                landmarks["right_shoulder"].y,
                landmarks["right_shoulder"].z
            );
            
            Vector3 elbowPos = new Vector3(
                landmarks["right_elbow"].x,
                landmarks["right_elbow"].y,
                landmarks["right_elbow"].z
            );
            
            Vector3 wristPos = new Vector3(
                landmarks["right_wrist"].x,
                landmarks["right_wrist"].y,
                landmarks["right_wrist"].z
            );
            
            // Calculate directions for bone rotations
            Vector3 upperArmDir = (elbowPos - shoulderPos).normalized;
            Vector3 forearmDir = (wristPos - elbowPos).normalized;
            
            // Apply rotations to joints
            if (joints[0] != null) // Shoulder
            {
                joints[0].rotation = Quaternion.LookRotation(upperArmDir);
            }
            
            if (joints[1] != null) // Elbow
            {
                joints[1].rotation = Quaternion.LookRotation(forearmDir);
                // Optionally set position for visualization
                joints[1].position = shoulderPos + (upperArmDir * upperArmLength);
            }
            
            if (joints[2] != null) // Wrist
            {
                // Set wrist position and rotation
                Vector3 calculatedWristPos = joints[1].position + (forearmDir * forearmLength);
                joints[2].position = calculatedWristPos;
                joints[2].rotation = Quaternion.LookRotation(forearmDir);
            }
            
            Debug.Log($"Applied pose: Shoulder->Elbow: {upperArmDir}, Elbow->Wrist: {forearmDir}");
        }
        
        // Add similar code blocks for other body parts (left arm, legs, torso, etc.)
        // based on the landmarks your Flask server provides
    }
    
    // Helper method for finding deep child transforms (referenced in MixamoArmFromArduino)
    // You'll need to make sure this exists in your project, possibly in FindDeepChild.cs
    /* 
    If FindDeepChild isn't already implemented in your project, add this extension method:
    
    public static Transform FindDeepChild(this Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            
            var result = child.FindDeepChild(name);
            if (result != null)
                return result;
        }
        return null;
    }
    */
}
