using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    float _timeToDie = 3.0f;    // The lifetime of the explosion
    [SerializeField]
    bool _canChainExplode = false;
    [SerializeField]
    float _delayb4ColliderEnabled = 0.1f;
    [SerializeField]
    GameObject _chainExplosionPrefab;
    [SerializeField]
    float _delayb4ChainExplode = 0.1f;
    [SerializeField]
    CircleCollider2D _circleCollider2d;
    [SerializeField]
    Rigidbody2D _rb2d;

    [SerializeField] 
    float _maxColliderRadius = 3.0f;  // The maximum radius of explosion's collider
    
    float _timer;

    void Start()
    {
        if (_canChainExplode)
        {
            if(_circleCollider2d == null)
                _circleCollider2d = GetComponent<CircleCollider2D>();
            if (_rb2d != null)
                _rb2d = GetComponent<Rigidbody2D>();

            if(_circleCollider2d != null && _rb2d != null)
            {
                _circleCollider2d.enabled = false;
                StartCoroutine(EnableColliderRoutine());
            }
        }
        else
        {
            if(_circleCollider2d != null)
                _circleCollider2d.enabled = false;
        }

        Destroy(gameObject, _timeToDie);
    }


    /*private void Update()
    {
        if (_canChainExplode)
        {
            // Increase the size of the collider
            float progress = _timer / _timeToDie;
            _circleCollider2d.radius = progress * _maxColliderRadius;

            _timer += Time.deltaTime;

            // Destroy the explosion after the duration has elapsed
            if (_timer >= _timeToDie)
            {
                Destroy(gameObject);
            }
        }
    }*/

    private void Update()
    {
        if (_canChainExplode)
        {
            // The first 0.5 seconds will be used to expand the collider to its maximum size
            if (_timer <= 0.5f)
            {
                float progress = _timer / 0.5f;
                _circleCollider2d.radius = progress * _maxColliderRadius;
            }
            // The remaining 1.5 seconds will be used to shrink the collider back to zero
            else
            {
                float progress = (_timer - 0.5f) / 1.5f;
                _circleCollider2d.radius = (1 - progress) * _maxColliderRadius;
            }

            _timer += Time.deltaTime;

            // Destroy the explosion after the duration has elapsed
            if (_timer >= _timeToDie)
            {
                Destroy(gameObject);
            }
        }
    }


    IEnumerator EnableColliderRoutine()
     {
        yield return new WaitForSeconds(_delayb4ColliderEnabled);
        _circleCollider2d.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_canChainExplode && _circleCollider2d.enabled == true)
        {
            Vector2 collateralLocation = collision.transform.position;
            
            if (collision.gameObject.CompareTag("Enemy"))
            {
                // ENEMY will destroy itself
                StartCoroutine(ChainReactonRoutine(_delayb4ChainExplode, collateralLocation));
            }
            else if (collision.gameObject.CompareTag("Asteroid"))
            {
                // ASTEROID will destroy itself
                StartCoroutine(ChainReactonRoutine(_delayb4ChainExplode, collateralLocation));
            }
        }
    }

    IEnumerator ChainReactonRoutine(float delayExplosion, Vector2 loc)
    {
        yield return new WaitForSeconds(delayExplosion);
        
        if (_chainExplosionPrefab != null)
        {
            var chainExplosion = Instantiate(_chainExplosionPrefab, loc, Quaternion.identity);
            Explosion explosion = chainExplosion.GetComponent<Explosion>();
            explosion.ChangeIfCanExplode(true);
        }
    }

    public void ChangeIfCanExplode(bool canExplode)
    {
        _canChainExplode = canExplode;
    }

    public float GetMyScaleValue()
    {
        
        float scaleValue = transform.localScale.x;
        return scaleValue;
    }

    public void ChangeMyScaleValue(float newScale)
    {
        transform.localScale = new Vector2(newScale, newScale);
    }

    

}
