using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    private AsyncOperation loadOp;
    public GameObject titleText;
    public GameObject playButton;
    public Slider slider;
    
    public void Play()
    {
        loadOp = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        titleText.SetActive(false);
        playButton.SetActive(false);
        slider.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (loadOp != null)
        {
            Debug.Log(loadOp.progress);
            slider.value = Mathf.Clamp01(loadOp.progress / 0.9f);
        }
    }
}
