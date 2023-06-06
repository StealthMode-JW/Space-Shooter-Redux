using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;
//using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;


public class TimeWarp : MonoBehaviour
{
    [SerializeField]
    CameraShake _cameraShake;
    
    [SerializeField]
    AudioSource _audioSource;
    [SerializeField]
    AudioClip _timeWarp_Clip;


    [Header("LENS DISTORTION INTENSITY")]
    [SerializeField]
    float _normalLensDistort = 0.0f;
    [SerializeField]
    float _minLensDistort = -100.0f;
    [SerializeField]
    float _maxLensDistort = 100.0f;

    [Header("LENS SCALING")]
    [SerializeField]
    float _normalLensScale = 1.0f;
    [SerializeField]
    float _minLensScale = 0.01f;
    

    [Header("LENS DISTORTION TIMING")]
    [SerializeField]
    float timeTilMaxIntenseMinScale = 1.5f; //  time til intensity reaches 100 while scale goes to 0.01.
    [SerializeField]
    float timeTilMinIntense = 1.5f; //  time til intensity reaches -100 
    [SerializeField]
    float timeTilReturnNormal = 1.5f;    //  time til both return to normal.

    
    [SerializeField]
    bool _didTimeWarpComplete;

    
    public UnityEngine.Rendering.Volume volume;         //just using this for testing
    public UnityEngine.Rendering.VolumeProfile volumeProfile;
    public UnityEngine.Rendering.Universal.LensDistortion lensDistortion;
    

    //void Start()
    void Awake()
    {
        volume = GetComponent<Volume>();
        if (!volume) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.Volume));

        volumeProfile = GetComponent<UnityEngine.Rendering.Volume>()?.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));

        if (!volumeProfile.TryGet(out lensDistortion)) throw new System.NullReferenceException(nameof(lensDistortion));
        
        lensDistortion.intensity.Override(0.00f);

        if (_cameraShake == null)
            _cameraShake = GetComponentInParent<CameraShake>();



        /*if (volume.sharedProfile.TryGet<LensDistortion>(out lensDis))
            lensDistortion = lensDis;*/

        _didTimeWarpComplete = false;

        

        /*if (volume.profile.TryGet(out lensDis))
            lensDistortion = lensDis;*/



    }


    public IEnumerator TimeWarpRoutine()
    {
        lensDistortion.intensity.Override(0.0f);    


        float elapsedTime = 0.0f;

        float startIntensity = 0.0f;
        float newValue = startIntensity;
        
        lensDistortion.intensity.Override(startIntensity);

        var lensScale = lensDistortion.scale;
        float startScale = 1.0f;
        lensScale.value = startScale;

        _audioSource.clip = _timeWarp_Clip;
        _audioSource.Play();

        //  Step 1 (of 3)   INTENSITY to max, SCALE to min
        while (elapsedTime < timeTilMaxIntenseMinScale)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            //intensity.value = Mathf.Lerp(startIntensity, _maxLensDistort, elapsedTime / timeTilMaxIntenseMinScale);
            newValue = Mathf.Lerp(startIntensity, _maxLensDistort, elapsedTime / timeTilMaxIntenseMinScale);

            lensScale.value = Mathf.Lerp(startScale, _minLensScale, elapsedTime / timeTilMaxIntenseMinScale);
            //lensDistortion.intensity = intensity;
            lensDistortion.intensity.Override(newValue);
            lensDistortion.scale = lensScale;
        }

        elapsedTime = 0.0f;
        startIntensity = _maxLensDistort;
        //intensity.value = startIntensity;
        lensDistortion.intensity.Override(startIntensity);

        //  Step 2 (of 3)   INTENSITY to min
        if(_cameraShake != null)
            _cameraShake.StartShake(3f, 0.25f);
        while (elapsedTime < timeTilMinIntense)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            //intensity.value = Mathf.Lerp(startIntensity, _minLensDistort, elapsedTime / timeTilMinIntense);
            newValue = Mathf.Lerp(startIntensity, _minLensDistort, elapsedTime / timeTilMinIntense);
            //lensDistortion.intensity = intensity;
            lensDistortion.intensity.Override(newValue);
        }
        elapsedTime = 0.0f;

        startIntensity = _minLensDistort;
        //intensity.value = startIntensity;
        lensDistortion.intensity.Override(startIntensity);

        startScale = _minLensScale;
        lensScale.value = startScale;

        //  Step 3 (of 3)   INTENSITY to norm, SCALE to norm
        while (elapsedTime < timeTilReturnNormal)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            //intensity.value = Mathf.Lerp(startIntensity, _normalLensDistort, elapsedTime / timeTilReturnNormal);
            newValue = Mathf.Lerp(startIntensity, _normalLensDistort, elapsedTime / timeTilReturnNormal);
            lensScale.value = Mathf.Lerp(startScale, _normalLensScale, elapsedTime / timeTilReturnNormal);
            //lensDistortion.intensity = intensity;
            lensDistortion.intensity.Override(newValue);
            lensDistortion.scale = lensScale;
        }
        _didTimeWarpComplete = true;
    }

    public bool DidTimeWarpEnd()
    {
        bool didWarpEnd = _didTimeWarpComplete;
        return didWarpEnd;
    }
}
