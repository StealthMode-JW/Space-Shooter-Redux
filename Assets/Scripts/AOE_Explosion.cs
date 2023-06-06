using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE_Explosion : MonoBehaviour
{
    public float duration = 1.0f;  // The lifetime of the explosion
    public float speed = 5.0f;  // The speed at which the explosion moves
    public float maxColliderRadius = 5.0f;  // The maximum radius of the explosion's collider

    private CircleCollider2D circleCollider;
    private Vector2 direction;
    private float timer;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider == null)
        {
            Debug.LogError("FireballExplosion requires a CircleCollider2D component.");
            enabled = false;
        }
    }

    public void Initialize(Vector2 direction)
    {
        this.direction = direction;
    }

    private void Update()
    {
        // Move the explosion in the set direction
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Increase the size of the collider
        float progress = timer / duration;
        circleCollider.radius = progress * maxColliderRadius;

        // Update the timer
        timer += Time.deltaTime;

        // Destroy the explosion after the duration has elapsed
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}
