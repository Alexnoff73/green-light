using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private GameManager m_Game;
    
    private const string BEST = "bestScore";
    
    public int Value { get; private set; }

    public int Best
    {
        get => PlayerPrefs.GetInt(BEST, 0);
        set => PlayerPrefs.SetInt(BEST, value);
    }

    private void Awake()
    {
        m_Game = GameManager.Instance;
    }

    public void Reset()
    {
        Value = 0;
    }

    public void SubmitScore(int score)
    {
        if (score > Best)
        {
            Best = score;
        }
    }
}