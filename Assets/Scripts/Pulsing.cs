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
        }
    }
}
