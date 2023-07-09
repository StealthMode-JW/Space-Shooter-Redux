using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkColor : MonoBehaviour
{
    public float blinkTime = 0.5f;
    private SpriteRenderer sr;
    private Color originalColor;
    public Color colorToBlink;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            sr.color = colorToBlink;
            yield return new WaitForSeconds(blinkTime);
            sr.color = originalColor;
            yield return new WaitForSeconds(blinkTime);
        }
    }
}
