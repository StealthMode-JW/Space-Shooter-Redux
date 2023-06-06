using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

public class Enemy : MonoBehaviour
{
    SpawnManager spawnManager;
    Player _player;
    [SerializeField]
    CameraShake _cameraShake;
    [SerializeField]
    float _speed = 4.0f;
    //public float startDelay = 2.0f;
    public float delayRespawn = 1.5f;
    public bool useRandomVariableDelay = true;
    public float ranVarDelay = 1.0f;
    [SerializeField]
    bool _canMove = false;
    public float ySpawnHeight = 7.0f;
    public float minY = -7.0f;
    public float xPosSpawnMin = -9.0f;
    public float xPosSpawnMax = 9.0f;

    [SerializeField]
    Transform _destroyedTransform;
    [SerializeField]
    DamageFragments _damageFragments;

    [SerializeField]
    float _minDestroyedSpeedMultiplier = 0.5f;
    [SerializeField]
    float _maxDestroyedSpeedMultiplier = 1.5f;
    float _randomizedDestroySpeedMultiplier;

    [SerializeField]
    bool _isDead = false;
    [SerializeField]
    bool _isRecycling = false;
    [SerializeField]
    bool _canFire = true;
    [SerializeField]
    GameObject _dualShotPrefab;
    [SerializeField]
    Transform _laserContainer;
    /*[SerializeField]
    float _laser1_X_Offset = -0.25f;
    [SerializeField]
    float _laser2_X_Offset = 0.25f;*/ // Dont need with dualShotPrefab
    [SerializeField]
    float _timeTilNextFire;
    [SerializeField]
    float _minTimeTilNextFire = 3.0f;
    [SerializeField]
    float _maxTimeTilNextFire = 7.0f;

    [SerializeField]
    float _timeToDestroy = 2.5f;    //  currently based on explosionanimation


    [SerializeField]
    bool _isUsingRotationScaleUponDestroy;

    [SerializeField]
    float _minX_rotation = -20.0f;
    [SerializeField]
    float _maxX_rotation = 20.0f;
    [SerializeField]
    float _minY_rotation = -30.0f;
    [SerializeField]
    float _maxY_rotation = 30.0f;
    [SerializeField]
    float _minZ_rotation = -45.0f;
    [SerializeField]
    float _maxZ_rotation = 45.0f;

    [SerializeField]
    float _minScaleMultiplier = 0.5f;
    [SerializeField]
    float _maxScaleMultiplier = 1.0f;

    
    [SerializeField]
    AudioSource _audioSource_Explosion;
    [SerializeField]
    AudioSource _audioSource_DualLaser;

    [SerializeField]
    List<AudioClip> List_EnemyExplosionClips = new List<AudioClip>();
    [SerializeField]
    List<AudioClip> List_EnemyDualLaserClips = new List<AudioClip>();



    Animator _anim;

    void Start()
    {
        
        float ranStartFire = Random.Range(1.0f, _maxTimeTilNextFire);
        _timeTilNextFire = ranStartFire;
        if(_laserContainer == null)
            _laserContainer = GameObject.Find("LaserContainer").transform;
        if (_damageFragments == null)
            _damageFragments = GetComponent<DamageFragments>();
        if(_cameraShake == null)
            _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
    }

    private void OnEnable()
    {
        _anim = GetComponent<Animator>();


        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
            Debug.LogError("The Player is NULL!!");
        
        _canMove = true;
        StartCoroutine(SetupEnemy());
    }

    IEnumerator SetupEnemy()
    {
        _randomizedDestroySpeedMultiplier = Random.Range(_minDestroyedSpeedMultiplier, _maxDestroyedSpeedMultiplier);
        //_audioSource_Explosion = GetComponent<AudioSource>();

        if (Transformation.IsUsingTransformation())
        {
            var spriteRend = GetComponent<SpriteRenderer>();
            var cubeRend = GetComponentInChildren<MeshRenderer>();

            var gameMode = Transformation.GetGameMode();
            if (gameMode == Transformation.GameMode.Mode_2D)
            {
                spriteRend.enabled = true;
                cubeRend.enabled = false;

            }
            else if (gameMode == Transformation.GameMode.Mode_3D)
            {
                spriteRend.enabled = false;
                cubeRend.enabled = true;
            }
        }

        yield return null;
    }

    void Update()
    {
        if (!GameManager.isGamePaused)
        {
            if (_canMove)
            {
                if (_isDead)
                {
                    transform.Translate(Vector3.down * _speed * _randomizedDestroySpeedMultiplier * Time.deltaTime);
                }
                else
                    transform.Translate(Vector3.down * _speed * Time.deltaTime);

                if (transform.position.y < minY && _isDead == false)
                {
                    StartCoroutine(RespawnRoutine());
                }
                else // Will only calculate time til next fire if Enemy is able to Move and not respawning (or Dead)
                {
                    _timeTilNextFire -= Time.deltaTime;
                    if (_timeTilNextFire <= 0)
                        _canFire = true;

                    if (_canFire && _isDead == false)
                    {
                        FireLasers();
                    }
                }

            }
        }
        
    }
    
    public IEnumerator RespawnRoutine()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.enabled = false;
        _canMove = false;
        if (useRandomVariableDelay)
            ranVarDelay = Random.Range(-ranVarDelay, ranVarDelay);
        else 
            ranVarDelay = 0.0f;
        yield return new WaitForSeconds(delayRespawn + ranVarDelay);
        var newSpawnLoc = GetNewSpawnLocation();
        transform.position = newSpawnLoc;
        _canMove = true;
        collider.enabled = true;
    }
    public Vector3 GetNewSpawnLocation()
    {
        Vector3 spawnLocation = transform.position;
        float xSpawnPos = Random.Range(xPosSpawnMax, xPosSpawnMin);
        spawnLocation = new Vector3(xSpawnPos, ySpawnHeight, 0);
        return spawnLocation;
    }
    
    void FireLasers()
    {
        _canFire = false;
        _timeTilNextFire = Random.Range(_minTimeTilNextFire, _maxTimeTilNextFire);
        //Vector3 posA = new Vector3(transform.position.x, transform.position.y + _laser_yOffset, 0);
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, 0);
        Instantiate(_dualShotPrefab, pos, Quaternion.identity, _laserContainer);

        //var dualLasers = Instantiate(_dualShotPrefab, pos, Quaternion.identity);
        if (List_EnemyDualLaserClips != null && List_EnemyDualLaserClips.Count > 0)
        {
            int ranIndex = Random.Range(0, List_EnemyDualLaserClips.Count);
            AudioClip ranClip = List_EnemyDualLaserClips[ranIndex];
            _audioSource_DualLaser.clip = ranClip;
        }
        _audioSource_DualLaser.Play();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _player = other.transform.GetComponent<Player>();
            if (_player != null)
                _player.Damage(gameObject, 1);

            DestroySelf();
        }
        else if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            DestroySelf();
        }
        else if(other.tag == "Asteroid")
        {
            //Destroy(other.gameObject);        ! Asteroid will destroy itself
            DestroySelf();
        }
        else if (other.tag == "Missile")
        {
            //Destroy(other.gameObject);          //  Missile will detach its particle effect
            DestroySelf();
        }
        else if (other.tag == "Explosion")
        {
            DestroySelf();
        }
    }


    //  When Enemy dies...
    public void DestroySelf()
    {
        _isDead = true;
        if(_player != null)
        {
            Transform playerTrans = _player.transform;
            Vector3 playerPos = playerTrans.position;
            _cameraShake.StartDynamicShake(0.1f, 0.2f, playerPos, transform.position);
        }
        
        
        
        
        /*if(_player!= null)
        {
            StartCoroutine(_cameraShake.DynamicShake(0.1f, 0.2f, _player.transform.position, transform.position));

        }*/

        if (_anim != null)
            _anim.SetBool("OnEnemyDestroyed", true);

        if (spawnManager == null)
            spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_player != null)
            _player.AddScore(10);

        if (_damageFragments != null)
            _damageFragments.Fragmentize();

        if (spawnManager != null)
        {
            //Manage Enemies in list?
            spawnManager.ReduceEnemyCount();
            
        }
        BoxCollider2D enemyCollider = GetComponent<BoxCollider2D>();
        enemyCollider.enabled = false;
        if (_isUsingRotationScaleUponDestroy)
            StartCoroutine(RotateExplodingShip());

        if (List_EnemyExplosionClips != null && List_EnemyExplosionClips.Count > 0)
        {
            int ranIndex = Random.Range(0, List_EnemyExplosionClips.Count);
            AudioClip ranClip = List_EnemyExplosionClips[ranIndex];
            _audioSource_Explosion.clip = ranClip;
        }
        _audioSource_Explosion.Play();
        Destroy(gameObject, _timeToDestroy);
    }

    IEnumerator RotateExplodingShip()
    {
        

        float startRotationX = transform.eulerAngles.x;
        float endRotationX = Random.Range(_minX_rotation, _maxX_rotation);

        float startRotationY = transform.eulerAngles.y;
        float endRotationY = Random.Range(_minY_rotation, _maxY_rotation);

        float startRotationZ = transform.eulerAngles.z;
        float endRotationZ = Random.Range(_minZ_rotation, _maxZ_rotation);

        Vector3 startScale = transform.localScale;
        float ranScaleMultiplier = Random.Range(_minScaleMultiplier, _maxScaleMultiplier);

        float time = 0.0f;
        while(time < _timeToDestroy)
        {
            time += Time.deltaTime;
            float xRotation = Mathf.Lerp(startRotationX, endRotationX, time / _timeToDestroy);
            float yRotation = Mathf.Lerp(startRotationY, endRotationY, time / _timeToDestroy);
            float zRotation = Mathf.Lerp(startRotationZ, endRotationZ, time / _timeToDestroy);
            transform.eulerAngles = new Vector3(xRotation, yRotation, zRotation);

            float currScale = Mathf.Lerp(1.0f, ranScaleMultiplier, time / _timeToDestroy * 2f);
            transform.localScale = startScale * currScale;

            yield return null;
        }
    }

}
