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

    public int beatsUntilDrop;
    private int currentBeats;
    private bool beatHasDropped;

    [SerializeField] private string lightStrRef;

    void Start()
    {
        globalVolume.profile.TryGet(out _colorAdjustments);
        globalVolume.profile.TryGet(out _panini);
        conductor = GameObject.FindWithTag("Conductor").GetComponent<Conductor>();
        bpm = conductor.Bpm;
        lastbeat = 0;
        crotchet = 60 / bpm;
        
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
            _panini.distance.value = Mathf.Lerp(0.2f, 0.0f, colortimer/colorduration);
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
