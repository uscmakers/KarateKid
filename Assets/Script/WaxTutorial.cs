using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class WaxTutorial : MonoBehaviour
{
    private Vector2 prevPolarPosition;
    public float radius;
    private double totalChange;
    private float[] prevChanges;
    public bool RH;
    public float radiusDeviation;
    private Color ogColor;
    public float dDepth;
    static int instructions;

    // Start is called before the first frame update
    void Start()
    {
        prevPolarPosition = new Vector2(0, 0);
        totalChange = 0;
        prevChanges = new []{0.0f, 0.0f};
        instructions = 0;
        EventManager.StartChangeInstructions(instructions);
        EventManager.StartCircleManager(0);
        ogColor = gameObject.GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnMouseDrag(){
        //getting position from mouse for dragging
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, transform.position.z - Camera.main.transform.position.z));
        transform.position = new Vector3(worldPoint.x, worldPoint.y, transform.position.z);

        //depth from w and s
        if(Input.GetKey(KeyCode.W)){
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + dDepth);
            print("away");
        }
        else if(Input.GetKey(KeyCode.S)){
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - dDepth);
            print("toward");
        }

        //conver to polar coords
        float radians = System.MathF.Atan2(transform.position.y, transform.position.x) * 180 / Mathf.PI;
        Vector2 polarPos = new Vector2(new Vector2(transform.position.x, transform.position.y).magnitude, radians);
        print(polarPos.x);

        //sees the angle change from the previousCheck
        float angleDifference = polarPos.y - prevPolarPosition.y;
        //corrects for going around the circle
        if (angleDifference > 180) {
            angleDifference -= 360f;
        }
        if (angleDifference < -180) {
            angleDifference += 360f;
        }
        //if correct hand and going the correct way and incrementing the total change
        if(!RH && (angleDifference > 0 || prevChanges[0] > 0 || prevChanges[1] > 0) && Math.Abs(polarPos.x - radius) < radiusDeviation){
            totalChange += angleDifference;
        }
        else if(RH && (angleDifference < 0 || prevChanges[0] < 0 || prevChanges[1] < 0) && Math.Abs(polarPos.x - radius) < radiusDeviation){
            totalChange += angleDifference;
        }
        //if not moving total change goes to 0
        else{
            totalChange = 0;
        }

        //makes the component change color when close to the correct radius
        GetComponent<Renderer>().material.SetColor("_Color", Color.Lerp(Color.black, ogColor, System.Math.Abs((float)(polarPos.x - radius))));
       
        //updates previous positions
        prevPolarPosition = polarPos;
        prevChanges[1] = prevChanges[0];
        prevChanges[0] = angleDifference;

        //prints num circles
        EventManager.StartCircleManager((int)(Math.Abs(totalChange) / 360));
        
        //if done what's needed updates the amount to do
        if(RH && totalChange / 360 <= -5 ){
            totalChange = 0;
            instructions++;
            EventManager.StartChangeInstructions(instructions);
        }
        else if(!RH && totalChange / 360 >= 5 && (instructions == 2 || instructions == 3)){
            totalChange = 0;
            instructions++;
            EventManager.StartChangeInstructions(instructions);
        }

    }

}
