using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;


public class CarMover : MonoBehaviour
{
    int cameraPos = 0;
    public Transform path;

    public Transform LH;
    public Transform RH;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.CameraManager += UpdateCameraPosition;
        UpdateCameraPosition();
    }

    // Update is called once per frame
    void Update()
    {
        //test camera movement
        if(Input.GetKeyDown(KeyCode.Space)){
            UpdateCameraPosition();
        }
    }

    void OnDrawGizmos(){
        Vector3 previousPosition = path.GetChild(0).position;
        foreach(Transform point in path){
            Gizmos.DrawSphere(point.position, 0.1f);
            Gizmos.DrawLine(previousPosition, point.position);
            Gizmos.DrawLine(point.position, point.forward + point.position);
            previousPosition = point.position;

        }
    }


    void UpdateCameraPosition(){
        if(cameraPos < path.childCount){
            transform.position = path.GetChild(cameraPos).transform.position;
            transform.rotation = path.GetChild(cameraPos).transform.rotation;
            LH.position = new Vector3(-1, 1, -8);
            RH.position = new Vector3(1, 1, -8);
        }
        cameraPos++;
    }
}
