using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject deathScreen;
    private float startSeconds;
    private void Start()
    {
        startSeconds = Time.time;
    }


    public void OnDeath()
    {
        deathScreen.SetActive(true);
        float secondsSurvived = Time.time - startSeconds;
        deathScreen.GetComponent<TextMeshProUGUI>().text = "YOU SURVIVED FOR " + secondsSurvived.ToString("N") + " SECONDS";
    }
}
