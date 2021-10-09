using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject deathScreen;
    public TextMeshProUGUI spedometerText;
    private float startSeconds;
    public Volume globalVolume;
    private DepthOfField _depthOfField;
    private FilmGrain _filmGrain;
    private ColorAdjustments _colorAdjustments;
    private LensDistortion _lensDistortion;
    public Transform spedometerDial;
    
    public GameObject pausePanel;
    private bool isPaused;

    private float currentSpeed;
    
    private void Start()
    {
        startSeconds = Time.time;
        globalVolume.profile.TryGet(out _filmGrain);
        globalVolume.profile.TryGet(out _colorAdjustments);
        globalVolume.profile.TryGet(out _lensDistortion);
    }


    public void OnDeath()
    {
        deathScreen.SetActive(true);
        float secondsSurvived = Time.time - startSeconds;
        globalVolume.profile.TryGet(out _depthOfField);
        _depthOfField.focalLength.value = 300f;
        deathScreen.GetComponent<TextMeshProUGUI>().text = "YOU SURVIVED FOR " + secondsSurvived.ToString("N") + " SECONDS";
    }

    public void HandleSpeedIncrease(float speed)
    {
        // Set spedometer
        currentSpeed = speed;
        spedometerText.text = (3 * speed).ToString("N1") + "km/h";
        spedometerDial.RotateAround(spedometerDial.position, Vector3.back, currentSpeed / 160);
        
        // Set GlobalVolume effects
        if (_filmGrain != null)
        {
            //_filmGrain.intensity.value = currentSpeed / 100;
        }

        if (_colorAdjustments != null)
        {
            _colorAdjustments.hueShift.value = currentSpeed / 2;
            //_colorAdjustments.hueShift.value = currentSpeed;
            _colorAdjustments.contrast.value = currentSpeed / 2;
        }

        if (_lensDistortion != null)
        {
            _lensDistortion.intensity.value = -currentSpeed / 200;
        }
    }

    public void Pause()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1;
            pausePanel.SetActive(false);
        }
        else
        {
            isPaused = true;
            Time.timeScale = 0;
            pausePanel.SetActive(true);
        }
    }

    public void PlayAgainButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
