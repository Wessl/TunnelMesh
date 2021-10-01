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

    public void SetSpedometer(float speed)
    {
        currentSpeed = speed;
        spedometerText.text = (3 * speed).ToString("N1") + "km/h";
        
        // Do some other stuff whenever speed is raised
        // better ways: listeners/events or renaming this function

        if (_filmGrain != null)
        {
            _filmGrain.intensity.value = currentSpeed / 100;
        }

        if (_colorAdjustments != null)
        {
            _colorAdjustments.saturation.value = currentSpeed / 2;
            _colorAdjustments.hueShift.value = currentSpeed;
            _colorAdjustments.contrast.value = currentSpeed / 2;
        }

        if (_lensDistortion != null)
        {
            _lensDistortion.intensity.value = -currentSpeed / 200;
        }
    }

    public void PlayAgainButton()
    {
        SceneManager.LoadScene(0);
    }
}
