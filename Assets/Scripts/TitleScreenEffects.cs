using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TitleScreenEffects : MonoBehaviour
{
    public Volume volume;
    private ColorAdjustments _colorAdjustments;
    
    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGet(out _colorAdjustments);
    }

    // Update is called once per frame
    void Update()
    {
        _colorAdjustments.hueShift.value += Time.deltaTime * 100;
        Debug.Log(_colorAdjustments.hueShift.value);
        if (_colorAdjustments.hueShift.value >= _colorAdjustments.hueShift.max)
        {
            _colorAdjustments.hueShift.value = _colorAdjustments.hueShift.min;
        }
    }
}
