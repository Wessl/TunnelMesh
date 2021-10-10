using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/**
 * Handles every beat-synced global volume switch
 * Has references to shader variables that are updated depending on Conductor song time
 * Most changes occur only after a "beat drop" which has to be manually set (number of beats)
 * Needs a camera with post-processing enabled
 */
public class FlashCameraController : MonoBehaviour
{
    public static float colortimer = 0.0f;
    public float colorduration = 0.4F;
    public Conductor conductor;
    public GameObject exhaustFumes;
    
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
    public Texture2D caveTextureNormal;
    public Texture2D caveTexturePsychedelic;

    [SerializeField] private string noiseInfluenceStrRef;
    [SerializeField] private string textureScrollSpeedRef;
    [SerializeField] private string twirlStrengthRef;
    [SerializeField] private string caveTextureRef;
    [SerializeField] private string cloudTilingRef;


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
        tunnelBGMateral.SetTexture(caveTextureRef, caveTextureNormal);
        tunnelBGMateral.SetVector(cloudTilingRef, new Vector4(0.1f, 0.1f, 0, 0));
        
        if (Application.platform == RuntimePlatform.OSXPlayer)
            beatsUntilDrop--;
        if (Application.platform == RuntimePlatform.WindowsPlayer)
            beatsUntilDrop--;
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            beatsUntilDrop--; // Extremely somewhat arbitrary values that I found online that may be incorrect I don't know
    }



    void Update(){

        if (conductor.songPosition > lastbeat + crotchet) {
            // add one beat
            currentBeats++;
            if (currentBeats == beatsUntilDrop)
            {
                // Drop the beat
                beatHasDropped = true;
                exhaustFumes.SetActive(true);
                tunnelBGMateral.SetTexture(caveTextureRef, caveTexturePsychedelic);
                tunnelBGMateral.SetVector(cloudTilingRef, new Vector4(0.025f, 0.025f, 0, 0));
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
            tunnelBGMateral.SetFloat(textureScrollSpeedRef, 0.25f);
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
