using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static event Action<int> ChangeInstructions;

    public static event Action<int> CircleManager;

    public static event Action CameraManager;

    public static event Action<int> ScoreManager;

    public static event Action GameOver;

    public static void StartChangeInstructions(int num){
        ChangeInstructions?.Invoke(num);
    }

    public static void StartCircleManager(int num){
        CircleManager?.Invoke(num);
    }

    public static void StartCameraManager(){
        CameraManager.Invoke();
    }

    public static void StartScoreManager(int n){
        ScoreManager.Invoke(n);
    }

    public static void StartGameOver(){
        GameOver.Invoke();
    }

    
}
