using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    Vector3 _originalCamPos = new Vector3(0, 0, -10);
    //Coroutine _currentShakeCoroutine;
    //Coroutine _currentDynamicShakeRoutine;


    public void StartShake(float duration, float magnitude)
    {
        //  Stop any existing shakes
        StopAllCoroutines();
        StartCoroutine(Shake(duration, magnitude));
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        //  Reset Camera's origional position
        transform.localPosition = _originalCamPos;
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;
    }

    public void StartDynamicShake(float duration, float magnitude, Vector3 playerPos, Vector3 shakeOrigin)
    {
        StopAllCoroutines(); 
        StartCoroutine(DynamicShake(duration, magnitude, playerPos, shakeOrigin));
    }

    public IEnumerator DynamicShake(float duration, float magnitude, Vector3 playerPosition, Vector3 shakeOrigin)
    {
        //  Reset Camera's original position
        transform.localPosition = _originalCamPos;
        Vector3 originalPos = _originalCamPos;
        float elapsed = 0.0f;

        // Calculate distance between player and shake origin.
        float distance = Vector3.Distance(playerPosition, shakeOrigin);

        // Set a max distance for shake effect.
        float maxDistance = 50f;

        // Calculate shake intensity based on distance.
        //      Objects farther than maxDistance will have no shake.
        float intensity = Mathf.Clamp01((maxDistance - distance) / maxDistance);

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude * intensity;
            float y = Random.Range(-1f, 1f) * magnitude * intensity;

            transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}
