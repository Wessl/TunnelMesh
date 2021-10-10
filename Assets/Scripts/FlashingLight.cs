using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingLight : MonoBehaviour
{
    public Color colorStart = Color.white;
    public Color colorEnd = Color.clear;
    public static float colortimer = 0.0f;
    public float colorduration = 0.4F;
    public Conductor conductor;
    
    float lastbeat;             // this is the ‘moving reference point’ -not used atm, though very useful for rhythm games

    float bpm;

    private float crotchet;     // crotchet = seconds between each beat. e.g. 150bpm => 60/150 = 0.4, so every 0.4s one beat occurs
    private float matColorStrength;
    private Material material;

    [SerializeField] private string lightStrRef;

    void Start()
    {
        
        conductor = GameObject.FindWithTag("Conductor").GetComponent<Conductor>();
        bpm = conductor.Bpm;
        material = GetComponentInChildren<Renderer>().material;
        matColorStrength = material.GetFloat(lightStrRef);
        lastbeat = 0;
        crotchet = 60 / bpm;
        
    }

    void Update(){
        /*
         * The color strength (value between 1 and 0) is set to 1 every time the time is just barely past the
         * latest time of a beat occuring, and lerps back to a base value over a set period of time (usually less than
         * or almost less than a crotchet)
         */ 
        if (conductor.songPosition > lastbeat + crotchet) {
            Flash();
            lastbeat += crotchet;
        }
        colortimer += Time.deltaTime;
        
        matColorStrength = Mathf.Lerp(1f, 0.3f, colortimer/colorduration);
        material.SetFloat(lightStrRef, matColorStrength);
    }

    private static void Flash(){
        colortimer = 0;
    }
}