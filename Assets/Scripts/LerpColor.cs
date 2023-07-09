using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpColor : MonoBehaviour
{
    public float duration = 1.0f; // Duration of the color change.
    private float elapsed = 0.0f; // Elapsed time since the color change started.
    private SpriteRenderer spriteRenderer;
    Color originalColor;
    public Color colorToLerp;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        // Compute the lerp fraction based on a sine wave that oscillates between 0 and 1.
        float t = (Mathf.Sin(Time.time * duration) + 1) / 2;

        // Interpolate between the start and end colors using the fraction t.
        Color newColor = Color.Lerp(originalColor, colorToLerp, t);

        // Assign the new color to the sprite renderer.
        spriteRenderer.color = newColor;
    }
}
