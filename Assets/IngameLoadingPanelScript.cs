using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameLoadingPanelScript : MonoBehaviour
{
    public Transform bgImage;
    public GameObject inGames;

#pragma warning disable 414
    private bool _isStart = false;
#pragma warning restore 414

    public void UserStart()
    {
        _isStart = true;
        bgImage.localScale = Vector3.one;
    }

    private void Update()
    {
        if (_isStart == false) return;
        bgImage.localScale += 0.1f * Time.deltaTime*Vector3.one;

        if (bgImage.localScale.x >= 1.4)
        {
            
            inGames.SetActive(true);
            _isStart = false;
            this.gameObject.SetActive(false);
        }
    }
}
