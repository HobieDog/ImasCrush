using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    bool beClicked = false;

    void Start()
    {
        
    }

    void Update()
    {
        CheckClick();
    }

    void CheckClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            beClicked = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            beClicked = false;
        }
    }
}
