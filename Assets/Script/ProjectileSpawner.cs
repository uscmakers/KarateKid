using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public GameObject projectile;
    public Transform projectileHolder;
    public float minTime;
    public float maxTime;
    private int score;
    private float dTime;
    private float prevTime = 0;
    public float force;
    private bool shooting = true;
    // Start is called before the first frame update
    void Start()
    {
        dTime = Random.Range(minTime, maxTime);
        prevTime = 0;
        EventManager.GameOver += StopShooting;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - prevTime > dTime && shooting){
            GameObject proj = Instantiate(projectile, transform.position, Quaternion.FromToRotation(transform.forward, transform.right), projectileHolder);
            proj.GetComponent<Rigidbody>().AddForce((force + score * 5) * transform.forward);
            prevTime = Time.time;
            dTime = Random.Range(minTime, maxTime);
            score++;
            EventManager.StartScoreManager(score);
        }
    }

    void StopShooting(){
        shooting = false;
    }


}
