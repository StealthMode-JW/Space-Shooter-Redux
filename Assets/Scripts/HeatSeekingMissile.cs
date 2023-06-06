using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class HeatSeekingMissile : MonoBehaviour
{
    private Rigidbody2D _rb;
    BoxCollider2D _boxCollider;
    GameObject _target;
    [SerializeField]
    CameraShake _cameraShake;
    [SerializeField]
    Transform _transPlayer;

    [SerializeField]
    float _speed = 7.5f;
    [SerializeField]
    float _rotateSpeed = 200f;

    [SerializeField]
    Transform _trans_Thruster;
    [SerializeField]
    ParticleSystem _flameParticles;
    [SerializeField]
    GameObject _bigExplosionPrefab;
    [SerializeField]
    bool _isArmed = false;
    [SerializeField]
    float _timeTilArmed = 0.75f;
    [SerializeField]
    float _scaleArmed = 0.5f;

    [SerializeField]
    AudioSource _audioSource1;
    [SerializeField]
    AudioSource _loopingAudioSource;

    [SerializeField]
    AudioClip _armingMissileClip;
    [SerializeField]
    AudioClip _launchingMissileClip;
    [SerializeField]
    AudioClip _seekingMissileClip;
    [SerializeField]
    AudioClip _explosionMissileClip;
    [SerializeField]
    AudioClip _massiveExplosionClip;

    Vector2 _startPosLeft = new Vector2(-0.6f, -0.25f);
    Vector2 _startPosRight = new Vector2(0.6f, -0.25f);
    Vector2 _armedPosLeft = new Vector2(-1.2f, -0.25f);
    Vector2 _armedPosRight = new Vector2(1.2f, -0.25f);

    public enum LeftOrRightMissile
    {
        Left,
        Right
    };

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        if(_boxCollider != null)
            _boxCollider.enabled = false;
        if(_cameraShake == null)
            _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        if (_transPlayer == null)
            _transPlayer = GameObject.Find("Player").transform;
    }

    private void FixedUpdate()
    {
        if (_isArmed)
        {
            if (_target == null)
            {
                // Find closest enemy
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                if (enemies.Length > 0)
                {
                    float closestDistance = Mathf.Infinity;
                    foreach (GameObject enemy in enemies)
                    {
                        float distance = Vector2.Distance(transform.position, 
                            enemy.transform.position);

                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            _target = enemy;
                        }
                    }
                }
            }
            else
            {
                // Move towards target
                Vector2 direction = 
                    (Vector2)_target.transform.position - _rb.position;
                direction.Normalize();
                float rotateAmount = Vector3.Cross(direction, transform.up).z;
                _rb.angularVelocity = -rotateAmount * _rotateSpeed;
                _rb.velocity = transform.up * _speed;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isArmed)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // Handle PLAYER collision
                Player player = collision.GetComponent<Player>();
                if(player != null)
                {
                    if (player.CanMissilesHurtPlayer())
                    {
                        player.Damage(gameObject, 2);
                        DestroyMe(); 
                    }
                }
            }
            else if (collision.gameObject.CompareTag("Enemy"))
            {
                // ENEMY will destroy itself
                DestroyMe();
            }
            else if (collision.gameObject.CompareTag("Asteroid"))
            {
                // ASTEROID will destroy itself
                DestroyMe();
            }
            else if (collision.gameObject.CompareTag("Missile"))
            {
                // When two MISSILES collide...
                MassiveExplosion();
            }
        }
    }

    public void DestroyMe()
    {
        _audioSource1.clip = _explosionMissileClip;
        _audioSource1.Play();
        if(_cameraShake != null)
            _cameraShake.StartDynamicShake(0.3f, 0.6f, _transPlayer.position, transform.position);

        if (_bigExplosionPrefab != null)
            Instantiate(_bigExplosionPrefab, transform.position, Quaternion.identity);
        _flameParticles.transform.SetParent(null);
        Destroy(_flameParticles.gameObject, 0.5f);
        Destroy(gameObject);
    }

    void MassiveExplosion()
    {
        _isArmed = false;
        _cameraShake.StartDynamicShake(0.5f, 1.0f, _transPlayer.position, transform.position);


        float randomX = Random.Range(-1.0f, 1.0f);
        float randomY = Random.Range(-1.0f, 1.0f);
        Vector3 vector3 = new Vector3(randomX, randomY, 0);
        Vector3 newLoc = vector3 + transform.position;

        _flameParticles.transform.SetParent(null);
        Destroy(_flameParticles.gameObject, 0.5f);
        
        if (_bigExplosionPrefab != null)
        {
            var massiveExplosion = 
                Instantiate(_bigExplosionPrefab, newLoc, Quaternion.identity);
            Explosion explosion = massiveExplosion.GetComponent<Explosion>();

            _audioSource1.clip = _explosionMissileClip;
            _audioSource1.Play();

            float startScale = explosion.GetMyScaleValue();
            float randomScaleBoost = Random.Range(0.25f, 1.0f);
            float newScale = startScale + randomScaleBoost;
            explosion.ChangeMyScaleValue(newScale);
        }
        Destroy(gameObject);
    }


    public void FireMissile(LeftOrRightMissile leftOrRight, Vector2 playerPos)
    {
        StartCoroutine(ArmMissileRoutine(leftOrRight, playerPos));
    }

    public IEnumerator ArmMissileRoutine(LeftOrRightMissile leftOrRight, Vector2 playerPos)
    {
        //  Detach from Player (no longer using localPosition)
        transform.SetParent(null);

        _audioSource1.clip = _launchingMissileClip;
        _audioSource1.Play();
        
        //  Grab reference to left missile (world) position
        Vector2 startPos = _startPosLeft;
        Vector2 armedPos = _armedPosLeft;   

        //  ...or right missile (world) position
        if(leftOrRight == LeftOrRightMissile.Right)
        {
            startPos = _startPosRight;
            armedPos = _armedPosRight;
        }

        //  Update position relative to Player's (world) position.  
        startPos += playerPos;
        armedPos += playerPos;

        //  Missiles float further to the side (away from Player) before activating
        float t = 0;
        while (t < _timeTilArmed)
        {
            t += Time.deltaTime;

            // Calculate current position using linear interpolation (Lerp)
            Vector2 currentPosition = Vector2.Lerp(startPos, armedPos, t / _timeTilArmed);

            // Update left/right missile position
            transform.position = currentPosition;

            yield return null;
        }
        // After coroutine is done, missile is armed and ready to seek targets
        ActivateMissile();
    }

    
    public void ActivateMissile()
    {
        //  When activated, Missile visuals come alive
        _isArmed = true;
        _boxCollider.enabled = true;
        _trans_Thruster.localScale = new Vector2(_scaleArmed, _scaleArmed);
        _loopingAudioSource.clip = _seekingMissileClip;
        _loopingAudioSource.Play();
        _flameParticles.Play();
    }

    
    
}
