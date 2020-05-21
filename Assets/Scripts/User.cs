using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class User : MonoBehaviour
{
    const int score_up = 25;

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
        scoreText.text = score.ToString();
    }

    public void AddScore(int val)
    {
        score += val;
    }
}
