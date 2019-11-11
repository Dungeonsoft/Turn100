using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameLoadingPanelScript : MonoBehaviour
{
    public Transform bgImage;
    public GameObject InGames;

    private bool isStart = false;

    public void UserStart()
    {
        isStart = true;
        bgImage.localScale = Vector3.one;
    }

    private void Update()
    {
        bgImage.localScale += Vector3.one * Time.deltaTime;

        if (bgImage.localScale.x >= 1.4)
        {
            
            InGames.SetActive(true);
            isStart = false;
            this.gameObject.SetActive(false);
        }
    }
}
