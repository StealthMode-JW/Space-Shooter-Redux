using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour
{
    public Slider slider;

    public float minValue = 0.0f;
    public float maxValue = 1.0f;
    public float currValue = 1.0f;

    public bool isReady = true;

    private void Start()
    {
        slider.value = currValue;
    }



    public IEnumerator CooldownRoutine(float duration)
    {
        slider.value = minValue;
        float inversedTime = 1 / duration;

        for (float val = minValue; val < maxValue; val += Time.deltaTime * inversedTime)
        {
            currValue = val;
            slider.value = currValue;
            yield return null;
        }

        slider.value = maxValue;
    }
}
