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
    
    float lastbeat; //this is the ‘moving reference point’

    float bpm;

    private float crotchet;
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

        if (conductor.songPosition > lastbeat + crotchet) {
            Flash();
            lastbeat += crotchet;
        }
        colortimer += Time.deltaTime;
        matColorStrength = Mathf.Lerp(1f, 0.0f, colortimer/colorduration);
        material.SetFloat(lightStrRef, matColorStrength);
    }

    private static void Flash(){
        colortimer = 0;
    }
}