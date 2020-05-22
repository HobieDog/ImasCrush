using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class User : MonoBehaviour
{
    const int score_point = 500;

    GameManager gameManager;
    Text scoreText;
    int score;

    private void Awake()
    {
        scoreText = GameObject.Find("Canvas").transform.Find("ScoreBar").Find("Score").GetComponent<Text>();
        gameManager = GetComponent<GameManager>();
    }

    private void FixedUpdate()
    {
        
    }

    public void AddScore(int val)
    {
        score += score_point * val;
        scoreText.text = score.ToString();
    }
}
