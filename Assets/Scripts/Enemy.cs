using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
//using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
//using static UnityEditor.PlayerSettings;

public class Enemy : MonoBehaviour
{
    SpawnManager spawnManager;
    Player _player;
    public static int nextEnemyID = 0;
    [SerializeField, ReadOnly] string enemyID;
    [SerializeField] CameraShake _cameraShake;

    [Header("MOVEMENT")]
    [SerializeField] float _baseSpeed = 6.0f;
    [SerializeField] float _circleSpeed = 9.0f;
    [SerializeField] bool _randomizeSpeed;
    [SerializeField] float _randomSpeedFactor = 1.0f;
    [SerializeField, ReadOnly] float _boostSpeedMod = 0.0f;  // multiplier from Wave Spawn ROutine
    [Space(10)]
    [SerializeField, ReadOnly] float _finalMoveSpeed;
    [Space (50)]

    [SerializeField] float _delayRespawn = 1.5f;
    [SerializeField] bool _useRandomVariableDelay = true;
    [SerializeField] float _ranVarDelay = 1.0f;
    [SerializeField, ReadOnly] bool _canMove = false;
    [SerializeField] float _ySpawnHeight = 7.0f;
    [SerializeField] float minY = -7.0f;
    [SerializeField] float xPosSpawnMin = -9.0f;
    [SerializeField] float xPosSpawnMax = 9.0f;

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

    [Header("FIRE RATE")]
    [SerializeField, ReadOnly] bool _canFire = true;
    [SerializeField] GameObject _dualShotPrefab;
    [SerializeField] Transform _laserContainer;
    [Space(10)]
    
    [SerializeField] float _baseFireRate;
    [SerializeField] bool _randomizeFireRate = true;
    [SerializeField] float _randomFireRateFactor = 1.0f; // +/-1
    [SerializeField, ReadOnly] float _boostFireRateMod = 0.0f;  //  Set by Wave Spawner
    [SerializeField, ReadOnly] float _timeTilNextFire = 0.0f;
    [Space(10)]
    [SerializeField, ReadOnly] float _finalFireRate;
    [Space(50)]
    [SerializeField] float _timeToDestroy = 2.5f;    //  currently based on explosionanimation

    [SerializeField] bool _isUsingRotationScaleUponDestroy;
    [SerializeField] float _minX_rotation = -20.0f;
    [SerializeField] float _maxX_rotation = 20.0f;
    [SerializeField] float _minY_rotation = -30.0f;
    [SerializeField] float _maxY_rotation = 30.0f;
    [SerializeField] float _minZ_rotation = -45.0f;
    [SerializeField] float _maxZ_rotation = 45.0f;

    [SerializeField] float _minScaleMultiplier = 0.5f;
    [SerializeField] float _maxScaleMultiplier = 1.0f;

    [Header("AUDIO")]
    [SerializeField] AudioSource _audioSource_Explosion;
    [SerializeField] AudioSource _audioSource_DualLaser;

    [SerializeField] List<AudioClip> List_EnemyExplosionClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> List_EnemyDualLaserClips = new List<AudioClip>();
    [SerializeField] AudioClip _thrusterInUseSound;
    [SerializeField] AudioClip _thrusterCooldownSound;

    Animator _anim;

    [Header("ADVANCED MOVEMENT")]
    [SerializeField] bool _doAdvancedEnemyMovement;
    [SerializeField] EnemyState _currentState;
    [SerializeField] GameObject _thruster;
    [SerializeField] private Vector3 _startPoint;
    [SerializeField] private Vector3 _targetPoint;
    [SerializeField] private Vector3 _controlPoint;
    [SerializeField] private float _tValue;
    

    [SerializeField]
    bool _isCircling = false;
    [SerializeField]
    SpriteRenderer _spriteRenderer;
    [SerializeField]
    private float _circlingSpeedMultiplier = 0.75f;
    [SerializeField]
    float _veerTowardPlayerFactor = 0.1f;

    [Header("CORRUPTED POWERS")]//Negative Powerups (Notified by SpawnManager
    [SerializeField, ReadOnly] bool _hasCorruptedPower;
    [Space(10)]
    [SerializeField, ReadOnly] bool _hasBurstFire_CRPT;                  //    int = 0
    [SerializeField, ReadOnly] float _timeLeft_BurstFire_CRPT = 0.0f;
    [Space(5)]
    [SerializeField, ReadOnly] bool _hasSpeedBoost_CRPT;                 //     int = 1
    [SerializeField, ReadOnly] float _timeLeft_SpeedBoost_CRPT = 0.0f;
    [Space(5)]
    [SerializeField, ReadOnly] bool _hasShields_CRPT;                    //    int = 2
    [SerializeField] GameObject _shieldObj;
    [SerializeField] Shields _shields;
    [SerializeField, ReadOnly] float _timeLeft_Shields_CRPT = 0.0f;
    //[Space(5)]
    /*[SerializeField, ReadOnly] bool _hasAmmo_CRPT = false;          //    int = 3
    [SerializeField, ReadOnly] float _timeLeft_Ammo_CRPT = 0.0f;*/
    //[Space(5)]
    //*[SerializeField, ReadOnly] bool _hasHealth_CRPT = false;        //    int = 4
    //[SerializeField, ReadOnly] float _timeLeft_Health_CRPT = 0.0f;*/
    //[Space(5)]
    /*[SerializeField, ReadOnly] bool _EnemyHoming_CRPT = false;        //    int = 5
    [SerializeField, ReadOnly] float _timeLeft_Homing_CRPT = 0.0f;*/



    [SerializeField]
    private enum EnemyState
    {
        DiveLt,
        DiveRt,

        FlankLt,
        FlankRt,

        Cir_BOTtoLT,
        Cir_BOTtoRT,
        Cir_LTtoTOP,
        Cir_RTtoTOP
    }

    private void Awake()
    {
        int ID = nextEnemyID++;
        enemyID = "EN" + ID + "_" + System.Guid.NewGuid().ToString();
    }


    void Start()
    {
        //  SpawnManager spawns Enemies above screen
        //   ...at Vector3(RandomRange(-9.0f, 9.0f), 8.0f, 0);

        //  Initial Dive (Lt / Rt) is determined by initial spawnX value
        SetState(transform.position.x < 0 ?
                EnemyState.DiveLt : EnemyState.DiveRt);
        
        if(_laserContainer == null)
            _laserContainer = GameObject.Find("LaserContainer").transform;
        if (_damageFragments == null)
            _damageFragments = GetComponent<DamageFragments>();
        if(_cameraShake == null)
            _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();

        if (_randomizeSpeed)
        {
            float factor = Random.Range(-_randomSpeedFactor, _randomSpeedFactor);
            _baseSpeed += factor;
        }
        if (_randomizeFireRate)
        {
            float ranStartFire = Random.Range(-_randomFireRateFactor, _randomFireRateFactor);
            _baseFireRate += ranStartFire;
        }

        //transform.Rotate(0, 0, 180); // Flip the enemy ship at start
    }

    private void OnEnable()
    {
        _anim = GetComponent<Animator>();

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
            Debug.LogError("The Player is NULL!!");
        
        _canMove = true;
        SetupEnemy();
    }

    void SetupEnemy()
    {
        // randomize initial Enemy Dive state
        /*_currentState = Random.value > 0.5 ? 
            EnemyState.DiveLt : EnemyState.DiveRt; */
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_thruster != null)
            _thruster.SetActive(false);

        _randomizedDestroySpeedMultiplier = Random.Range(_minDestroyedSpeedMultiplier, _maxDestroyedSpeedMultiplier);
        //_audioSource_Explosion = GetComponent<AudioSource>();

        if (Transformation.IsUsingTransformation())
        {
            var cubeRend = GetComponentInChildren<MeshRenderer>();

            var gameMode = Transformation.GetGameMode();
            if (gameMode == Transformation.GameMode.Mode_2D)
            {
                _spriteRenderer.enabled = true;
                cubeRend.enabled = false;

            }
            else if (gameMode == Transformation.GameMode.Mode_3D)
            {
                _spriteRenderer.enabled = false;
                cubeRend.enabled = true;
            }
        }
        SwitchMovementType(_doAdvancedEnemyMovement);
    }

    [SerializeField] Vector3 _dive_Target; // = new Vector3(Random.Range(-8.0f, 8.0f), -8.0f, 0); //  Both Dives aim below screen
    [SerializeField] Vector3 _flankLt_Target; // = new Vector3(12.0f, Random.Range(-5.0f, 0f), 0);// Aim bottom 1/2 of Rt screen
    [SerializeField] Vector3 _flankRt_Target; // = new Vector3(-12.0f, Random.Range(-5.0f, 0f), 0);// Aim bottom 1/2 of Lt screen
    [SerializeField] Vector3 _cirBtL_Target; // = new Vector3(-12.0f, Random.Range(0f, 5.0f), 0);// Aim top 1/2 of Lt screen
    [SerializeField] Vector3 _cirBtR_Target; // = new Vector3(12.0f, Random.Range(0f, 5.0f), 0);// Aim top 1/2 of Rt screen
    [SerializeField] Vector3 _cirLtT_Target; // = new Vector3(Random.Range(-10.0f, 0f), 8.0f, 0);// Aim Lt 1/2 above screen
    [SerializeField] Vector3 _cirRtT_Target; // = new Vector3(Random.Range(0f, 10.0f), 8.0f, 0);// Aim Rt 1/2 above screen
    
    [SerializeField] Vector3 _cirBtL_CurvePoint = new Vector3(-13.0f, -8.0f, 0);
    [SerializeField] Vector3 _cirBtR_CurvePoint = new Vector3(13.0f, -8.0f, 0);
    [SerializeField] Vector3 _cirLtT_CurvePoint = new Vector3(-13.0f, 8.0f, 0);
    [SerializeField] Vector3 _cirRtT_CurvePoint = new Vector3(13.0f, 8.0f, 0);
    [SerializeField] float _targetThreshold = 0.1f;
    bool _isTurning;
    [SerializeField] float _rotationSpeed = 200f;


    void SetState(EnemyState newState)
    {
        _currentState = newState;
        _startPoint = transform.position;
        _tValue = 0;
        _giveUpTrackingPlayer = false;

        // Set initial target location based on state
        switch (_currentState)
        {
                //  DIVE ATTACKS
            case EnemyState.DiveLt:         //  Both Dives aim below screen
                _dive_Target = new Vector3(Random.Range(-8.0f, 8.0f), -10.0f, 0); 
                _targetPoint = _dive_Target;
                _isCircling = false;
                break;
            case EnemyState.DiveRt:
                _dive_Target = new Vector3(Random.Range(-8.0f, 8.0f), -10.0f, 0); 
                _targetPoint = _dive_Target;
                _isCircling = false;
                break;

                //  FLANK ATTACKS
            case EnemyState.FlankLt:        // Aim bottom 1/2 of Rt screen
                _flankLt_Target = new Vector3(12.0f, Random.Range(-5.0f, 0f), 0);
                _targetPoint = _flankLt_Target;
                _isCircling = false;
                break;
            case EnemyState.FlankRt:        // Aim bottom 1/2 of Lt screen
                _flankRt_Target = new Vector3(-12.0f, Random.Range(-5.0f, 0f), 0);
                _targetPoint = _flankRt_Target;
                _isCircling = false;
                break;

                //          CIRCLE BACK (1st)
            case EnemyState.Cir_BOTtoLT:    // Aim top 1/2 of Lt screen
                _cirBtL_Target = new Vector3(-12.0f, Random.Range(0f, 5.0f), 0);
                _targetPoint = _cirBtL_Target;
                _controlPoint = _cirBtL_CurvePoint;
                _isCircling = true;
                break;
            case EnemyState.Cir_BOTtoRT:    // Aim top 1/2 of Rt screen
                _cirBtR_Target = new Vector3(12.0f, Random.Range(0f, 5.0f), 0);
                _targetPoint = _cirBtR_Target;
                _controlPoint = _cirBtR_CurvePoint;
                _isCircling = true;
                break;

                //          CIRCLE BACK (2nd)
            case EnemyState.Cir_LTtoTOP:    // Aim Lt 1/2 above screen
                _cirLtT_Target = new Vector3(Random.Range(-10.0f, 0f), 8.0f, 0);
                _targetPoint = _cirLtT_Target;
                _controlPoint = _cirLtT_CurvePoint;
                _isCircling = true;
                break;
            case EnemyState.Cir_RTtoTOP:    // Aim Rt 1/2 above screen
                _cirRtT_Target = new Vector3(Random.Range(0f, 10.0f), 8.0f, 0);
                _targetPoint = _cirRtT_Target;
                _controlPoint = _cirRtT_CurvePoint;
                _isCircling = true;
                break;
            default:
                break;
        }

        if (_isCircling == false)
        {
            bool isReturning = true;
            StartCoroutine(BlastOff(isReturning));
        }
    }

    bool HasReachedTarget()
    {
        return Vector3.Distance(transform.position, _targetPoint) < _targetThreshold;
    }

    public void SwitchMovementType(bool doAdvancedMovement)
    {
        _doAdvancedEnemyMovement = doAdvancedMovement;
        
        if (_spriteRenderer != null)
        {
            if (_doAdvancedEnemyMovement)
            {
                _spriteRenderer.flipY = true;
                transform.rotation = Quaternion.Euler(0, 0, -180f);
            }
            else
            {
                _spriteRenderer.flipY = false;
                transform.rotation = Quaternion.identity;
            }
        }
        else
            Debug.LogWarning("ENEMY's SpriteRenderer was inaccessible to flip.");
        
        
    }


    void Update()
    {
        /*float fireRate = _baseFireRate;
        float finalFireRate = fireRate;*/

        

        if (_doAdvancedEnemyMovement)
        {
            if (!GameManager.isGamePaused && !_isDead)
            {
                MoveEnemy_Advanced();

                // Only calculates time til next fire
                // ...while Enemy is focused on targeting player
                if (!_isCircling && !_giveUpTrackingPlayer)
                {
                    _timeTilNextFire -= Time.deltaTime;

                    if (_timeTilNextFire <= 0)
                        _canFire = true;

                    if (_canFire)
                        FireLasers();
                }
            }
        }
        else
        {
            //Do Simple movement
            if (!GameManager.isGamePaused)
            {
                if (_canMove)
                {
                    if (_isDead)
                    {
                        transform.Translate(Vector3.down * _baseSpeed * _randomizedDestroySpeedMultiplier * Time.deltaTime);
                    }
                    else
                        transform.Translate(Vector3.down * _baseSpeed * Time.deltaTime);

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

        LoseCorruptionPowers();
    }
    bool _giveUpTrackingPlayer = false;

    private Vector3 _finalTargetPoint;
    private float _transitionProgress = 0.0f;
    private Vector3 _lastFrameDirection;

    void MoveEnemy_Advanced()
    {
        Vector3 direction = (_targetPoint - transform.position).normalized;
        
        /*if (_player != null)
        {*/
            if (_currentState == EnemyState.DiveLt || 
                _currentState == EnemyState.DiveRt)
            {
                //  DIVE ATTACK
                AngledAttack(direction, 0.4f);

                if (_giveUpTrackingPlayer && HasReachedTarget())
                    SetState(transform.position.x < 0 ? 
                        EnemyState.Cir_BOTtoLT : EnemyState.Cir_BOTtoRT);
            }

            else if (_currentState == EnemyState.Cir_BOTtoLT || 
                _currentState == EnemyState.Cir_BOTtoRT)
            {
                //          CIRCLE BACK (1st of 2)
                float tVal = CircleBack(direction);

                if (tVal >= 1.0f)
                    SetState(_currentState == EnemyState.Cir_BOTtoLT ? 
                        EnemyState.FlankLt : EnemyState.FlankRt);
            }
            
            else if (_currentState == EnemyState.FlankLt || 
                _currentState == EnemyState.FlankRt)
            {
                //  FLANK ATTACK
                AngledAttack(direction, 0.33f);

                if (_giveUpTrackingPlayer && HasReachedTarget())
                    SetState(_currentState == EnemyState.FlankLt ? 
                        EnemyState.Cir_RTtoTOP : EnemyState.Cir_LTtoTOP);
            }
            
            else if (_currentState == EnemyState.Cir_LTtoTOP || 
                _currentState == EnemyState.Cir_RTtoTOP)
            {
                //          CIRCLE BACK (2nd of 2)
                float tVal = CircleBack(direction);

                if (tVal >= 1.0f)
                    SetState(transform.position.x < 0 ? 
                        EnemyState.DiveLt : EnemyState.DiveRt);
            }
        //}
    }

    //  DIVE or FLANK Attacks
    void AngledAttack(Vector3 direction, float lerpVal)
    {
        float mod = GetMyBoostSpeedMod();
        _finalMoveSpeed = _baseSpeed + mod;

        // Initialize _lastFrameDirection to targetDirection if first frame
        if (_lastFrameDirection == Vector3.zero)
            _lastFrameDirection = direction;

        if (_giveUpTrackingPlayer == false)
        {
            if(_player == null)
            {
                _giveUpTrackingPlayer = true;
                //  Blast off (without returning just yet)
                StartCoroutine(BlastOff(false));
            }
            else
            {
                if (Vector3.Distance(transform.position, _targetPoint) >
                Vector3.Distance(_player.transform.position, _targetPoint))
                {
                    //  While diving, also veer towards the player
                    Vector3 directionToPlayer =
                        (_player.transform.position - transform.position).normalized;

                    direction =
                        Vector3.Lerp(direction, directionToPlayer, lerpVal).normalized;
                }
                else    //  If too close to player, give up chase and blast off...
                {
                    _giveUpTrackingPlayer = true;
                    //  Blast off (without returning just yet)
                    StartCoroutine(BlastOff(false));
                }
            }
            
        }
        else        //      ...speeding off toward original Target Point
        {   
            direction = 
                Vector3.Lerp(_lastFrameDirection, direction, 0.66f).normalized;

            _lastFrameDirection = direction;
            _finalMoveSpeed = _circleSpeed;
        }

        transform.position += direction * _finalMoveSpeed * Time.deltaTime;
        RotateSprite(direction, _rotationSpeed);
    }

    float CircleBack(Vector3 direction)
    {
        Vector3 currentPos = transform.position;
        float rotationSpeed = _rotationSpeed * 100;

        // Continue along the curve
        _tValue += Time.deltaTime * 0.5f;
        Vector3 newPos = 
            CalculateBezierPoint(_tValue, _startPoint, _controlPoint, _targetPoint);
        transform.position = newPos;

        // Calculate direction of movement
        direction = (newPos - currentPos).normalized;

        // Rotate sprite to face direction of movement
        RotateSprite(direction, rotationSpeed);

        return _tValue;
    }

    IEnumerator BlastOff(bool isReturning)
    {
        yield return null;

        float fullAlpha = 1.0f;
        float transparentVal = 0.33f;

        float start = fullAlpha;
        float end = transparentVal;
        float duration = 1.0f;

        if (isReturning)
        {
            start = transparentVal;
            end = fullAlpha;
            _audioSource_Explosion.clip = _thrusterCooldownSound;
            _audioSource_Explosion.Play();
        }
        else
        {
            //  At Start, if blasting off (not returning), turn THRUSTERS on.
            if (_thruster != null)
                _thruster.SetActive(true);
            _audioSource_Explosion.clip = _thrusterInUseSound;
            _audioSource_Explosion.Play();
        }
        
        if(_spriteRenderer != null)
        {
            //  Fade Sprite In/Out depending on isReturning
            Color color = _spriteRenderer.color;
            float elapsed = 0.0f;
            color.a = start;
            _spriteRenderer.color = color;

            while(elapsed < duration)
            {
                float t = elapsed / duration;
                color.a = Mathf.Lerp(start, end, t);
                _spriteRenderer.color = color;

                elapsed += Time.deltaTime;
                yield return null;
            }

            color.a = end;
            _spriteRenderer.color = color;
        }

        //  When finished, if returning, turn THRUSTERS off.
        if(isReturning)
            if (_thruster != null)
                _thruster.SetActive(false);
    }

    void RotateSprite(Vector3 direction, float rotationSpeed)
    {
        if (rotationSpeed <= 0)
            rotationSpeed = _rotationSpeed;
        
        if (direction != Vector3.zero)
        {
            // Rotate the sprite to face the direction of movement
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion toRotation = 
                Quaternion.Euler(0, 0, angle - 90);
            transform.rotation = 
                Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 start, Vector3 control, Vector3 end)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 B = uu * start + 2 * u * t * control + tt * end;
        return B;
    }

    public IEnumerator RespawnRoutine()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.enabled = false;
        _canMove = false;
        if (_useRandomVariableDelay)
            _ranVarDelay = Random.Range(-_ranVarDelay, _ranVarDelay);
        else 
            _ranVarDelay = 0.0f;
        yield return new WaitForSeconds(_delayRespawn + _ranVarDelay);
        var newSpawnLoc = GetNewSpawnLocation();
        transform.position = newSpawnLoc;
        _canMove = true;
        collider.enabled = true;
    }
    public Vector3 GetNewSpawnLocation()
    {
        Vector3 spawnLocation = transform.position;
        float xSpawnPos = Random.Range(xPosSpawnMax, xPosSpawnMin);
        spawnLocation = new Vector3(xSpawnPos, _ySpawnHeight, 0);
        return spawnLocation;
    }
    
    void FireLasers()
    {
        DetermineNextFire();

        if (_hasBurstFire_CRPT)
            StartCoroutine(FireBurstShot());
        else
            FireDualShot();
        
    }

    void DetermineNextFire()
    {
        _canFire = false;
        _finalFireRate = _baseFireRate;
        if (_randomizeFireRate)
        {
            float factor = Random.Range(-_randomFireRateFactor, _randomFireRateFactor);
            _finalFireRate += factor;
        }
        float mod = GetMyBoostFireRateMod();
        float fireRateMod = _finalFireRate * mod;
        _finalFireRate += fireRateMod;
        _timeTilNextFire = _finalFireRate;
    }
    IEnumerator FireBurstShot()
    {
        int burst3 = 3;
        int shotsFired = 0;
        float burstDelay = 0.2f;

        while(shotsFired < burst3)
        {
            FireDualShot();

            yield return new WaitForSeconds(burstDelay);
        }
    }

    void FireDualShot()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, 0);
        Quaternion rotation = transform.rotation;
        if (_doAdvancedEnemyMovement)
        {
            rotation *= Quaternion.Euler(0, 0, 180); // Rotate by 180 degrees if in AdvancedMovement
        }

        var newDualShot = Instantiate(_dualShotPrefab, pos, rotation, _laserContainer);
        Laser laser1 = newDualShot.transform.GetChild(0).GetComponent<Laser>();
        laser1.myCreatorID = this.enemyID;
        Laser laser2 = newDualShot.transform.GetChild(1).GetComponent<Laser>();
        laser2.myCreatorID = this.enemyID;

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

            DamageSelf();
        }
        else if (other.tag == "Laser")
        {
            Laser laser = other.GetComponent<Laser>();
            string creatorID = laser.myCreatorID;

            if (creatorID == enemyID)
                return;
            else
            {
                Destroy(other.gameObject);
                DamageSelf();
                GameObject laserGO = other.gameObject;
            }
            
            
            
        }
        else if(other.tag == "Asteroid")
        {
            //Destroy(other.gameObject);        ! Asteroid will destroy itself
            DamageSelf();
        }
        else if (other.tag == "Missile")
        {
            //Destroy(other.gameObject);          //  Missile will detach its particle effect
            DamageSelf();
        }
        else if (other.tag == "Explosion")
        {
            DamageSelf();
        }
    }

    void DamageSelf()
    {
        if (_hasShields_CRPT)
        {
            //  this value can change if certain weapons do more damage
            var reducedlife = _shields.ReduceNumberOfShields(1);
            if (reducedlife < 0)
                DestroySelf();
            else if (reducedlife == 0)
            {
                _hasShields_CRPT = false;
                _shieldObj.SetActive(false);
            }
                
            else if(reducedlife > 0)
            {
                _hasShields_CRPT = true;
                _shieldObj.SetActive(true);
            }
        }
            
        else
        {
            DestroySelf();
        }


        DestroySelf();
    }


    //  When Enemy dies...
    public void DestroySelf()
    {
        _shieldObj?.SetActive(false);
        _isDead = true;
        if(_player != null)
        {
            Transform playerTrans = _player.transform;
            Vector3 playerPos = playerTrans.position;
            _cameraShake.StartDynamicShake(0.1f, 0.2f, playerPos, transform.position);
        }
        
        if (_anim != null)
            _anim.SetBool("OnEnemyDestroyed", true);

        if (spawnManager == null)
            spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_player != null)
            _player.AddScore(10);

        if (_thruster != null)
            _thruster.SetActive(false);
        if (_damageFragments != null)
            _damageFragments.Fragmentize();

        if (spawnManager != null)
        {
            //Manage Enemies in list?
            
            spawnManager.ReduceEnemyCount(transform);
            
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
    //0 = EN_BurstFire  //1 = EN_Speed  //2 = EN_Shields   //3 = ____  //4 = EN_Invulerna   //5 = _____

    public void GainCorruption(int indexOfCRPT, float durationAdded)
    {
        switch (indexOfCRPT)
        {
            case 0:
                _hasBurstFire_CRPT = true;
                _timeLeft_BurstFire_CRPT += durationAdded;
                break;
            case 1:
                _hasSpeedBoost_CRPT = true;
                _timeLeft_SpeedBoost_CRPT += durationAdded;
                break;
            case 2:
                ActivateShields(durationAdded);
                break;
            default:
                Debug.LogWarning("ENEMY: GainCorruption() " +
                    "CORRUPTION POWER NOT ADDED FOR INTEGER ===>  " + indexOfCRPT);
                break;

        }
    }

    public void LoseCorruptionPowers()
    {
        if (_hasBurstFire_CRPT)
            if (_timeLeft_BurstFire_CRPT > 0)
            {
                _timeLeft_BurstFire_CRPT -= Time.deltaTime;
                if (_timeLeft_BurstFire_CRPT <= 0.0f)
                {
                    _timeLeft_BurstFire_CRPT = 0.0f;
                    _hasBurstFire_CRPT = false;
                }
            }
        if (_hasSpeedBoost_CRPT)
            if (_timeLeft_SpeedBoost_CRPT > 0)
            {
                _timeLeft_SpeedBoost_CRPT -= Time.deltaTime;
                if (_timeLeft_SpeedBoost_CRPT <= 0.0f)
                {
                    _timeLeft_SpeedBoost_CRPT = 0.0f;
                    _hasSpeedBoost_CRPT = false;
                }
            }
        /* if (_hasShields_CRPT)   //  Shields last til they're gone.
             if (_timeLeft_Shields_CRPT > 0)
             {
                 _timeLeft_Shields_CRPT -= Time.deltaTime;
                 if (_timeLeft_Shields_CRPT <= 0.0f)
                 {
                     _timeLeft_Shields_CRPT = 0.0f;
                     _hasShields_CRPT = false;
                 }
             }*/
        if (_hasShields_CRPT)   //  Shields last til they're gone.
            if (_timeLeft_Shields_CRPT > 0)
            {
                _timeLeft_Shields_CRPT -= Time.deltaTime;
                if (_timeLeft_Shields_CRPT <= 0.0f)
                {
                    _timeLeft_Shields_CRPT = 0.0f;
                    _hasShields_CRPT = false;
                    _shields.AdjustShieldStrength(Shields.ShieldStrength.None);
                }
            }
        //  Add other powers.



    }

    public void ActivateShields(float durationAdded)
    {
        
        if(_shields != null)
        {
            _shieldObj.SetActive(true);
            _hasShields_CRPT = true;
            _timeLeft_Shields_CRPT += durationAdded;
            //_shields.AdjustShieldStrength(Shields.ShieldStrength.Full);
            _shields.GainNumberOfShieldStrength(1);
        }
    }


    public void AdjustMySpeedBoostSpeed(float boostSpeed)
    {
        _boostSpeedMod = boostSpeed;
    }
    public float GetMyBoostSpeedMod()
    {
        return _boostSpeedMod;
    }

    public void AdjustMyFireRateBoostSpeed(float boostFireRate)
    {
        _boostFireRateMod = boostFireRate;
    }
    public float GetMyBoostFireRateMod()
    {
        return _boostFireRateMod;
    }

}
