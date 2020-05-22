using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Timer : MonoBehaviour
{
    const float sub_time = 0.1f;
    float time = 60.0f;

    Slider slider;
    GameObject resultPanel;
    Text resultScore;
    User user;

    void Start()
    {
        slider = GameObject.Find("Canvas").transform.Find("Slider").GetComponent<Slider>();
        slider.maxValue = time;

        resultPanel = GameObject.Find("Canvas").transform.Find("ResultPanel").gameObject;
        resultPanel.SetActive(false);

        resultScore = GameObject.Find("Canvas").transform.Find("ResultPanel").Find("Score").GetComponent<Text>();

        user = GameObject.Find("GameManager").GetComponent<User>();

        StartCoroutine("FixedTimeTimer");
    }

    IEnumerator FixedTimeTimer()
    {
        Color imageColor = slider.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color;

        // Change TimeBar Color
        while (time > 0)
        {
            time -= sub_time;
            slider.value = time;

            switch ((int)time)
            {
                case 10:
                    slider.transform.Find("Fill Area").Find("Fill").
                    GetComponent<Image>().color = new Color(1.0f, 0, 0);
                    break;

                case 25:
                    slider.transform.Find("Fill Area").Find("Fill").
                    GetComponent<Image>().color = new Color(1.0f, 0.35f, 0);
                    break;

                case 40:
                    slider.transform.Find("Fill Area").Find("Fill").
                    GetComponent<Image>().color = new Color(1.0f, 0.5f, 0);
                    break;
            }
            yield return new WaitForSeconds(sub_time);
        }
        resultPanel.SetActive(true);
        resultScore.text = user.score.ToString();
    }
}
