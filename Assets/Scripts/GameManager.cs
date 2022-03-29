using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public static bool Playing { get; private set; }
    
    public static VehicleManager Vehicles { get; private set; }
    
    public static ScoreManager Score { get; private set; }

    public static int Points;
    
    public TextMeshProUGUI score;
    public TextMeshProUGUI best;
    public Button startGame;

    private void Awake()
    {
        Points = 0;
        if (Instance == null)
        {
            Instance = this;
        }else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        Vehicles = GetComponent<VehicleManager>();
        Score = GetComponent<ScoreManager>();
        Vehicles.Clear();
        StartGame();
    }
    
    public void StartGame()
    {
        Playing = true;
        //Score.Reset();
        Vehicles.StartSpawning();
    }

    public static void StopGame()
    {
        Playing = false;
        Vehicles.StopSpawning();
        //Score.SubmitScore(Score.Value);
    }
    
    //public class PointsClass {
    //    public void IncrementPoints()
    //    {
    //        Points++;
    //        Debug.Log(Points);
    //    }
    //}
    
    void Update()
    {
        //Debug.Log(Score);
        //score.text = "Score : " +  Score;
        //best.text = "Best : " +Score.Best;
    }
}