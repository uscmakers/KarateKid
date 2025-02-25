using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StainManager : MonoBehaviour
{
    public float alphaMax;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<Renderer>().material.GetColor("_BaseColor").a < alphaMax){
            Destroy(gameObject);
        }
    }
}
