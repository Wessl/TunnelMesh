using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FlashCameraController : MonoBehaviour
{
    public static float colortimer = 0.0f;
    public float colorduration = 0.4F;
    public Conductor conductor;
    
    float lastbeat; //this is the ‘moving reference point’

    float bpm;
    
    private float crotchet;
    public Volume globalVolume;
    private ColorAdjustments _colorAdjustments;
    private PaniniProjection _panini;
    private Vignette _vignette;

    public int beatsUntilDrop;
    private int currentBeats;
    private bool beatHasDropped;
    public Material tunnelBGMateral;

    [SerializeField] private string noiseInfluenceStrRef;
    [SerializeField] private string textureScrollSpeedRef;
    [SerializeField] private string twirlStrengthRef;


    void Start()
    {
        globalVolume.profile.TryGet(out _colorAdjustments);
        globalVolume.profile.TryGet(out _panini);
        globalVolume.profile.TryGet(out _vignette);
        conductor = GameObject.FindWithTag("Conductor").GetComponent<Conductor>();
        bpm = conductor.Bpm;
        lastbeat = 0;
        crotchet = 60 / bpm;
        // Make sure material float references are set to default values on start
        tunnelBGMateral.SetFloat(noiseInfluenceStrRef, 0);
        tunnelBGMateral.SetFloat(textureScrollSpeedRef, 0);
        tunnelBGMateral.SetFloat(twirlStrengthRef, 0);
    }



    void Update(){

        if (conductor.songPosition > lastbeat + crotchet) {
            // add one beat
            currentBeats++;
            if (currentBeats == beatsUntilDrop)
            {
                beatHasDropped = true;
            }
            Flash(beatHasDropped);
            lastbeat += crotchet;
        }
        colortimer += Time.deltaTime;

        if (beatHasDropped)
        {
            _panini.distance.value = Mathf.Lerp(0.3f, 0.0f, colortimer/colorduration);
            _vignette.intensity.value = Mathf.Lerp(0.25f, 0.0f, colortimer / colorduration);
            var noiseInfluenceStrength = Mathf.Lerp(0.0f, 5f, colortimer/colorduration);
            tunnelBGMateral.SetFloat(noiseInfluenceStrRef, noiseInfluenceStrength);
            tunnelBGMateral.SetFloat(textureScrollSpeedRef, 1);
            tunnelBGMateral.SetFloat(twirlStrengthRef, 1f);
        }
    }

    private void Flash(bool beatHasDropped){
        colortimer = 0;
        if (!beatHasDropped)
        {
            return;
        }
        _colorAdjustments.hueShift.value -= 20;
        if (_colorAdjustments.hueShift.value <= _colorAdjustments.hueShift.min)
        {
            _colorAdjustments.hueShift.value = _colorAdjustments.hueShift.max;
        }
    }
}
