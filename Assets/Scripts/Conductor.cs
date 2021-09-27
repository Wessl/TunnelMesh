using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    private AudioSource song;

    int crotchetsPerBar = 8;
    [SerializeField] private float bpm;
    public float crotchet;
    public float songPosition;
    public float deltaSongPosition;
    public float lastHit;
    public float actualLastHit;
    private float nextBeatTime = 0.0f;
    private float nextBarTime = 0.0f;
    public float offset = 0.2f;
    public float addOffSet;
    public static float offsetStatic = 0.4f;
    public static bool hasOffsetAdjusted = false;
    public int beatNumber = 0;
    public int barNumber = 0;
    public float pitch;
    
    void Start()
    {
        if (!hasOffsetAdjusted)
        {
            if (Application.platform == RuntimePlatform.OSXPlayer)
                offset = 0.35f;
            if (Application.platform == RuntimePlatform.WindowsPlayer)
                offset = 0.45f;
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                offset = 0.35f; // Extremely somewhat arbitrary values that I found online that may be incorrect I don't know
        }
        
        WaitForSongReadCompletion();
    }

    private void WaitForSongReadCompletion()
    {
        // Read stuff from audioFile once it's been loaded
        //offset = offsetStatic;
        crotchet = 60.0f / bpm;
        // Assign audioSource and correct song
        song = GetComponent<AudioSource>();
        // set song.clip not necessary with audiosource on conductor object?
        //  if (scrController.isgameworld)
        //      song.pitch = scrController.pitchchange;
        nextBeatTime = 0;
        nextBarTime = 0;

        StartMusic();
    }

    private void StartMusic()
    {
        song.Play();
    }

    void Update()
    {
        if (!song.isPlaying) return;
        crotchet = 60.0f / bpm;
        songPosition = song.timeSamples / 44100.0f - offset;
        //songPosition = (float)AudioSettings.dspTime * song.pitch;

        if (songPosition > nextBeatTime)
        {
            //OnBeat();
            nextBeatTime += crotchet;
            beatNumber++;
        }
        /*if (songPosition > nextBarTime)
        {
            OnBar();
            nextBarTime += crotchet * crotchetsPerBar;
            barNumber++;
        }*/

        pitch = song.pitch;

    }

    void OnBeat()
    {
        GameObject[] arrBeat = GameObject.FindGameObjectsWithTag("Beat");
        foreach (var objBeat in arrBeat)
        {
            objBeat.SendMessage("OnBeat");
        }
    }
    
    void OnBar()
    {
        GameObject[] arrBar = GameObject.FindGameObjectsWithTag("Bar");
        foreach (var objBar in arrBar)
        {
            objBar.SendMessage("OnBar");
        }
    }

    // Call this from other objects to turn on or off music
    public void SwitchMusicPause()
    {
        if (song.isPlaying)
        {
            song.Pause();
        }
        else
        {
            song.Play();
        }
    }
    

    public float Bpm => bpm;
}