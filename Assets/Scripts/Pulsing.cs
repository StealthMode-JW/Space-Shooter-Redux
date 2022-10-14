using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsing : MonoBehaviour
{
    public CanvasGroup cg;

    public bool isPulsing;
    public float duration = 5.0f;
    public float delay = 0.5f;
    public float maxAlpha = 1.0f;
    public float minAlpha = 0.0f;

    // Update is called once per frame

    private void Start()
    {
        /*if (isPulsing)
        {
            if (cg != null)
            {
                cg.alpha = minAlpha;
                StartCoroutine(Pulse());
            }
        }*/
        
    }
    private void OnEnable()
    {
        if (isPulsing)
        {
            if (cg != null)
            {
                cg.alpha = minAlpha;
                StartCoroutine(Pulse());
            }
        }
    }
    void Update()
    {
        /*if (isPulsing)
        {

        }*/
    }
    public IEnumerator Pulse()
    {
        yield return new WaitForSeconds(delay * 3);
        while(isPulsing && cg != null)
        {

            
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                float normalizedTime = t/duration;
                var someAlphaValue = Mathf.Lerp(minAlpha, maxAlpha, normalizedTime);
                cg.alpha = someAlphaValue;
                yield return null;
            }
            cg.alpha = maxAlpha;
            yield return new WaitForSeconds(delay);

            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                float normalizedTime = t / duration;
                var someAlphaValue = Mathf.Lerp(maxAlpha, minAlpha, normalizedTime);
                cg.alpha = someAlphaValue;
                yield return null;
            }
            cg.alpha = minAlpha;
            yield return new WaitForSeconds(delay * 3);


            /*for(float i = minAlpha; i < maxAlpha; i += Time.deltaTime)
            {
                cg.alpha = i;
                yield return null;
            }
            yield return new WaitForSeconds(delay);
            for(float j = maxAlpha; j > minAlpha; j -= Time.deltaTime)
            {
                cg.alpha = j;
                yield return null;
            }
            yield return new WaitForSeconds(delay);*/
        }
    }
}
