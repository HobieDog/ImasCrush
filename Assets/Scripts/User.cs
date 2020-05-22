using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class User : MonoBehaviour
{
    const int score_point = 200;
    public int score = 0;

    GameManager gameManager;
    Text scoreText;

    private void Awake()
    {
        scoreText = GameObject.Find("Canvas").transform.Find("Score").GetComponent<Text>();
        gameManager = GetComponent<GameManager>();
        scoreText.text = score.ToString();
    }

    public void AddScore(int val)
    {
        score += score_point * val;
        scoreText.text = score.ToString();
    }
}
