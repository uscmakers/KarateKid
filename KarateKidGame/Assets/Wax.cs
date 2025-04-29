using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Wax : MonoBehaviour
{
    private Vector2 prevPolarPosition;
    public float radius;
    private double totalChange;
    private float[] prevChanges;
    
    private Queue<Vector2> prevPositions;
    public bool RH;
    public float radiusDeviation;
    //private Color ogColor;
    public float dDepth;
    static int instructions;



    // Start is called before the first frame update
    void Start()
    {
        prevPolarPosition = new Vector2(0, 0);
        totalChange = 0;
        prevChanges = new []{0.0f, 0.0f};
        instructions = 0;
        // EventManager.StartChangeInstructions(instructions);
        // EventManager.StartCircleManager(0);
        //ogColor = gameObject.GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 center = determineCenter(prevPositions);

        //conver to polar coords
        float radians = System.MathF.Atan2(transform.position.y - center.y, transform.position.x - center.x) * 180 / Mathf.PI;
        Vector2 polarPos = new Vector2(new Vector2(transform.position.x, transform.position.y).magnitude, radians);
        //print(polarPos.x);

        prevPositions.Enqueue(new Vector2(transform.position.x, transform.position.y));
        if(prevPositions.Count > 3){
            prevPositions.Dequeue();
        }

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
        //GetComponent<Renderer>().material.SetColor("_Color", Color.Lerp(Color.black, ogColor, System.Math.Abs((float)(polarPos.x - radius))));
       
        //updates previous positions
        prevPolarPosition = polarPos;
        prevChanges[1] = prevChanges[0];
        prevChanges[0] = angleDifference;

        //prints num circles
        //EventManager.StartCircleManager((int)(Math.Abs(totalChange) / 360));
        
        //if done what's needed updates the amount to do
        if(RH && totalChange / 360 <= -5 ){
            totalChange = 0;
            instructions++;
            //EventManager.StartChangeInstructions(instructions);
        }
        else if(!RH && totalChange / 360 >= 5 && (instructions == 2 || instructions == 3)){
            totalChange = 0;
            instructions++;
            //EventManager.StartChangeInstructions(instructions);
        }
    }

    Vector2 determineCenter(Queue<Vector2> q){
    float[] x = new float[3];
    float[] y = new float[3];
    int i = 0 ;
    if(q.Count < 3){
        return new Vector2(0, 0);
    }
    foreach(Vector2 v in q){
        x[i] = v.x;
        y[i] = v.y;
    }
    float x12 = x[0] - x[1];
    float x13 = x[0] - x[2];

    float y12 = y[0] - y[1];
    float y13 = y[0] - y[2];

    float y31 = y[2] - y[0];
    float y21 = y[1] - y[0];

    float x31 = x[2] - x[0];
    float x21 = x[1] - x[0];

    // x[0]^2 - x[2]^2
    float sx13 = (float)(Math.Pow(x[0], 2) -
                    Math.Pow(x[2], 2));

    // y[0]^2 - y[2]^2
    float sy13 = (float)(Math.Pow(y[0], 2) -
                    Math.Pow(y[2], 2));

    float sx21 = (float)(Math.Pow(x[1], 2) -
                    Math.Pow(x[0], 2));
                    
    float sy21 = (float)(Math.Pow(y[1], 2) -
                    Math.Pow(y[0], 2));

    float f = ((sx13) * (x12)
            + (sy13) * (x12)
            + (sx21) * (x13)
            + (sy21) * (x13))
            / (2 * ((y31) * (x12) - (y21) * (x13)));
    float g = ((sx13) * (y12)
            + (sy13) * (y12)
            + (sx21) * (y13)
            + (sy21) * (y13))
            / (2 * ((x31) * (y12) - (x21) * (y13)));

    float c = -(float)Math.Pow(x[0], 2) - (float)Math.Pow(y[0], 2) -
                                2 * g * x[0] - 2 * f * y[0];

    // eqn of circle be x^2 + y^2 + 2*g*x + 2*f*y + c = 0
    // where centre is (h = -g, k = -f) and radius r
    // as r^2 = h^2 + k^2 - c
    float h = -g;
    float k = -f;
    return new Vector2(h, k);
    }


}
