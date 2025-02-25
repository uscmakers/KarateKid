using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

//todo: check how much of the car is waxed, remove wax and make shiny!, check for circular motion, make sure still works when camera changes orientation
public class CarHandControl : MonoBehaviour
{
    public float dDepth;
    private Vector3[] prevPos;
    public GameObject wax;
    public Transform waxHolder;
    public LayerMask carLayerMask;
    public float detectDistance;
    public LayerMask stain;
    public LayerMask dirt;
    bool hittingCar;
    bool waxed;
    // Start is called before the first frame update
    void Start()
    {
        prevPos = new[]{Vector3.zero, Vector3.zero};
        waxed = false;
        hittingCar = false;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void OnMouseDrag(){
        //getting position from mouse for dragging
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, transform.position.z - Camera.main.transform.position.z));
        transform.position = new Vector3(worldPoint.x, worldPoint.y, worldPoint.z);

        //depth from w and s
        if(Input.GetKey(KeyCode.W) && !hittingCar){
            transform.position = transform.position + transform.forward * dDepth;
            //print("away");
        }
        else if(Input.GetKey(KeyCode.S)){
            transform.position = transform.position - transform.forward * dDepth;
            //print("toward");
        }

        //spawning method
        //raycast method
        //if more time maybe shoot multiple rays and then spawn multiple small ones instead of one ray and spawn one big one
        // RaycastHit hit;
        // if(Physics.Raycast(transform.position, transform.forward, out hit, detectDistance))
        // {
        //     print(hit.transform.gameObject);
        //     if(hit.transform.tag == "Car")
        //     {
        //         print(hit.transform.gameObject);
        //         print(hit.transform.parent);
        //         hittingCar = true;
        //         Instantiate(wax, transform.position + transform.forward * (hit.distance - 0.1f), UnityEngine.Quaternion.FromToRotation(transform.forward, -hit.normal), waxHolder.transform);
        //     }  
        // }
        // else{
        //     hittingCar = false;
        // }

        //curvature method
        Vector3 v1 = new Vector3((transform.position.x - prevPos[0].x) / Time.deltaTime, (transform.position.y - prevPos[0].y) / Time.deltaTime, (transform.position.z - prevPos[0].z) / Time.deltaTime);
        Vector3 v2 = new Vector3((prevPos[0].x - prevPos[1].x) / Time.deltaTime, (prevPos[0].y - prevPos[1].y) / Time.deltaTime, (prevPos[0].z - prevPos[1].z) / Time.deltaTime);
        Vector3 vAvg = new Vector3((v1.x + v2.x) / 2, (v1.y + v2.y) / 2, (v1.z + v2.z) / 2);
        print(vAvg.magnitude);
        // Vector3 a = new Vector3((v1.x - v2.x) / Time.deltaTime, (v1.y - v2.y) / Time.deltaTime, (v1.z - v2.z) / Time.deltaTime);
        // double curvature = Vector3.Cross(vAvg, a).magnitude / Math.Pow(vAvg.magnitude, 3);
        // if(curvature > 0.1){
        //     print("circle");
        // }

        //transparency method
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, detectDistance, dirt)){
            print("cleaning");
            hit.transform.gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.Lerp(hit.transform.gameObject.GetComponent<Renderer>().material.GetColor("_BaseColor"), Color.clear, vAvg.magnitude / 5000));
        }



        //update prevPos
        prevPos[1] = prevPos[0];
        prevPos[0] = transform.position;


    }
}
