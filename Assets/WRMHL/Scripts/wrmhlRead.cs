using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
This script is used to read all the data coming from the device. For instance,
If arduino send ->
								{"1",
								"2",
								"3",}
readQueue() will return ->
								"1", for the first call
								"2", for the second call
								"3", for the thirst call

This is the perfect script for integration that need to avoid data loose.
If you need speed and low latency take a look to wrmhlReadLatest.
*/

public class wrmhlRead : MonoBehaviour {

	wrmhl myDevice = new wrmhl(); // wrmhl is the bridge beetwen your computer and hardware.

	[Tooltip("SerialPort of your device.")]
	public string portName = "COM8";

	[Tooltip("Baudrate")]
	public int baudRate = 250000;


	[Tooltip("Timeout")]
	public int ReadTimeout = 20;

	[Tooltip("QueueLenght")]
	public int QueueLenght = 1;

	private float[] calibAccel; 
	bool initialized;
	private float[] calibValAccel;
	void Start () {
		myDevice.set (portName, baudRate, ReadTimeout, QueueLenght); // This method set the communication with the following vars;
		//                              Serial Port, Baud Rates, Read Timeout and QueueLenght.
		myDevice.connect(); // This method open the Serial communication with the vars previously given.
		calibValAccel = new float[]{0, 0, 0};
		//string[] accel = myDevice.readQueue().Split("/");
		calibAccel = new float[]{0, 0, 0};
		initialized = false;
	}

	// Update is called once per frame
	void FixedUpdate () {
		String rawAccel = myDevice.readQueue();
		print(rawAccel);
		// if(rawAccel != null){
		// 	string[] accels = rawAccel.Split("/");
		// 	if(!initialized){
		// 		calibValAccel[0] = float.Parse(accels[0]);
		// 		calibValAccel[1] = float.Parse(accels[1]);
		// 		calibValAccel[2] = float.Parse(accels[2]);
		// 		print(accels[0]);
		// 		print(accels[1]);
		// 		print(accels[2]);
		// 		initialized = true;
		// 	}
		// 	else{
		// 		for(int i = 0; i < 3; i++){
		// 			calibAccel[i] = float.Parse(accels[i]) - calibValAccel[i];
		// 			if(Mathf.Abs(calibAccel[i]) < 0.1){
		// 				calibAccel[i] = 0;
		// 			}
		// 		}
		// 	}
		// }
		// print("Acceleration: " + calibAccel[0] + "/" + calibAccel[1] + "/" + calibAccel[2]);
		// float[] velocity = new float[]{0.0f, 0.0f, 0.0f};
		// float[] position = new float[]{0.0f, 0.0f, 0.0f};
		// for(int i = 0; i < 3; i++){
		// 	velocity[i] = calibAccel[i] * Time.fixedDeltaTime;	
		// 	position[i] = velocity[i] * Time.fixedDeltaTime;
		// } 

		// gameObject.transform.position = new Vector3(position[0], position[1], position[2]);
		// print("Velocity: " + velocity[0] + "/" + velocity[1] + "/" + velocity[2]);
		// print("Position: " + position[0] + "/" + position[1] + "/" + position[2]);
	}

	void OnApplicationQuit() { // close the Thread and Serial Port
		myDevice.close();
	}
}
