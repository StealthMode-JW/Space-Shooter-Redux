using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static UnityEditor.PlayerSettings;
using Random = UnityEngine.Random;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using static HeatSeekingMissile;
using Unity.VisualScripting;
using System.Net;
using TMPro.SpriteAssetUtilities;

public class Player : MonoBehaviour
{
    
    
    [SerializeField]
    SpawnManager _spawnManager;
    [SerializeField]
    UIManager _uiManager;
    [SerializeField] 
    ThrusterController _thrusterController;
    [SerializeField]
    DamageFragments _damageFragments;
    [SerializeField]
    CameraShake _cameraShake;


    [SerializeField]
    GameObject _explosionPrefab;

    [SerializeField]
    GameObject _laserPrefab;
    [SerializeField]
    Transform _laserContainer;
    /*[SerializeField]
    float _speed = 7f;*/
    [SerializeField]
    float _laser_yOffset = 0.8f;

    [Space(15)]


    [SerializeField]
    int _lives = 3;
    [SerializeField]
    int _maxLives;

   

    [SerializeField]
    Transform _trans_ShipUI_1;
    [SerializeField]
    Transform _trans_ShipUI_2;
    SpriteRenderer _sprite_Ship1;
    SpriteRenderer _sprite_Ship2;
    
    [SerializeField]
    Vector2 _startPos1 = new Vector2(-12f, -7);
    [SerializeField]
    Vector2 _startPos2 = new Vector2(-8f, -7);
    [SerializeField]
    Vector2 _endPos1 = new Vector2(-7.84f, 4.5f);
    [SerializeField]
    Vector2 _endPos2 = new Vector2(-7f, 4.5f);
    [SerializeField]
    Vector3 _controlPoint = new Vector3(-6f, -0.6f, 0f);

    [SerializeField]
    float _timeToMove = 1f;

    [SerializeField]
    ParticleSystem _burstParticles_1;
    [SerializeField]
    ParticleSystem _burstParticles_2;


    [Space(5)]
    
    int _score;
    [Space(10)]

    [SerializeField]
    Vector2 _playerStartPos = new Vector2(0f, -3.69f);
    [SerializeField]
    Vector2 _playerIntroPos = new Vector2(0f, -7f);

    [SerializeField]
    AudioSource _audioSource1;  //  Starts regularTime
    [SerializeField]
    AudioSource _audioSource2;  //  Starts later in Clip (plays sound earlier)
    [SerializeField]
    float _audio2_StartTime = 0.5f;
    /*[SerializeField]
    AudioClip _laserSoundClip;*/
    [SerializeField]
    List<AudioClip> List_LaserFireSounds = new List<AudioClip>();
    [SerializeField]
    List<AudioClip> List_TripleShotSounds = new List<AudioClip>();
    [SerializeField]
    AudioClip _shieldDamagedSound;

    [Space(10)]

    [SerializeField]
    Cooldown _cooldown;

    [SerializeField]
    float _fireDelay = 0.5f;
    float _canFire = 0.0f;


    [Header("POWERUPS")]
    [SerializeField]
    GameObject _tripleShotPrefab;
    [SerializeField]
    Transform _tripleShotContainer;
    [SerializeField]
    float _tripleShotPowerDownTime = 5.0f;
    [Space(5)]

    [SerializeField]
    float _speed = 7f;
    [SerializeField]
    float _speedBoost = 2.0f;
    [SerializeField]
    float _thrusterSpeedBoost = 1.5f;


    [SerializeField]
    float _speedPowerDownTime = 5.0f;
    [Space(5)]

    [Header("SHIELDS")]
    [SerializeField]
    GameObject _shieldVisualizer;
    [SerializeField]
    SpriteRenderer _shieldSprite;
    [SerializeField]
    ParticleSystem _shieldExplosionEffect;

    [SerializeField]
    int _shieldLife = 0;

    [SerializeField]
    bool _isInvulnerable;
    [SerializeField]
    float _invulnerabilityTime = 1.5f;

    [SerializeField]
    Light2D _shieldsLight;
    [SerializeField]
    Color _fullShieldsCol = Color.blue;
    [SerializeField]
    Color _halfShieldsCol = Color.yellow;
    [SerializeField]
    Color _criticalShieldsCol = Color.red;
    private Color _targetColor;

    [SerializeField]
    float _steadyPulseSpeed = 5.0f;
    [SerializeField]
    float _transitionPulseSpeed = 30f;
    [SerializeField]
    float _colorTransitionSpeed = 3.0f;
    [SerializeField]
    float _minPulseIntensity = 0f;
    [SerializeField]
    float _maxPulseIntensity = 0.75f;
    



    [Space(5)]

    [SerializeField]
    List<GameObject> List_Engines = new List<GameObject>();


    [Space(5)]

    [SerializeField]
    bool _doPlayerIntro = false;

    [SerializeField]
    bool _isTripleShotEnabled = false;
    [SerializeField]
    bool _isSpeedEnabled = false;
    [SerializeField]
    bool _isShieldEnabled = false;
    /*[SerializeField]
    bool _isThurstersEnabled = false;*/


    [SerializeField] 
    float _spreadFireDelay = 4.0f;
    float _canFireSpread = 0.0f;


    
    [SerializeField]
    GridLayoutGroup _laserGrid;
    [SerializeField]
    int _ammoCount = 15;
    [SerializeField]
    List<AudioClip> List_FailedLaserSounds = new List<AudioClip>();


    [Header("MISSILES")]

    [SerializeField]
    GameObject _heatseekerPrefab;

    [SerializeField]
    bool _canMissilesHurtSelf = false;
    [SerializeField]
    bool _canFireMissiles = false;
    
    [SerializeField]
    float _timeToReadyMissiles = 0.75f;
    
    Vector2 _startPosLeftMissile = new Vector2(-0.5f, -0.3f);
    Vector2 _startPosRightMissile = new Vector2(0.5f, -0.3f);
    Vector2 _readyPosLeftMissile = new Vector2(-1f, -0.3f);
    Vector2 _readyPosRightMissile = new Vector2(1f, -0.3f);

    GameObject _currentLeftMissile;
    GameObject _currentRightMissile;



    #region more complex heatseeker variable list
    /*[Header("MISSILES")]
    [SerializeField]
    GameObject _heatseekerPrefab;

    [SerializeField]
    bool _canMissilesHurtSelf = false;
    [SerializeField]
    bool _canMissilesAutoRefresh = false;
    [SerializeField]
    bool _canMissilesStack = false;
    [SerializeField]
    bool _hasMissiles = false;
    [SerializeField]
    bool _canFireMissiles = false;
    [SerializeField]
    bool _willReloadMissiles = false;

    [SerializeField]
    float _durationForMissiles = 5.0f;
    [SerializeField]
    float _timeBetweenMissiles = 1.5f;
    [SerializeField]
    float _timeToReadyMissiles = 0.75f;

    Vector2 _startPosLeftMissile = new Vector2(-0.5f, -0.3f);
    Vector2 _startPosRightMissile = new Vector2(0.5f, -0.3f);
    Vector2 _readyPosLeftMissile = new Vector2(-1f, -0.3f);
    Vector2 _readyPosRightMissile = new Vector2(1f, -0.3f);

    GameObject _currentLeftMissile;
    GameObject _currentRightMissile;*/

    #endregion

    private void Start()
    {
        if(_audioSource1 == null)
            _audioSource1 = GetComponent<AudioSource>();
        if (_audioSource1 == null)
            Debug.LogError("PLAYER: AudioSource is NULL!");

        if (_audioSource2 != null)
            _audioSource2.time = _audio2_StartTime;
       
        if(_spawnManager == null)
            _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
            Debug.Log("SpawnManager is NULL!");

        if(_uiManager == null)
            _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
            Debug.Log("UIManager == NULL!!");

        if (_doPlayerIntro)
            StartCoroutine(PlayerIntroRoutine());
        else
            transform.position = _playerStartPos;

        _ammoCount = 15;
        _maxLives = _lives;

        
        if (_trans_ShipUI_1 != null)
        {
            _sprite_Ship1 = _trans_ShipUI_1.GetComponent<SpriteRenderer>();
            Reset_ShipUIs(_trans_ShipUI_1, _sprite_Ship1, _startPos1);
        }
                
        if (_trans_ShipUI_2 != null)
        {
            _sprite_Ship2 = _trans_ShipUI_2.GetComponent<SpriteRenderer>();
            Reset_ShipUIs(_trans_ShipUI_2, _sprite_Ship2, _startPos2);
        }

        if (_damageFragments == null)
            _damageFragments = GetComponent<DamageFragments>();

        /*if (_rect_ShipUI_1 != null && _cg_ShipUI1 != null)
            _uiManager.Reset_ShipUIs(_rect_ShipUI_1, _cg_ShipUI1, _startPos1);
        if (_rect_ShipUI_2 != null && _cg_ShipUI2 != null)
            _uiManager.Reset_ShipUIs(_rect_ShipUI_2, _cg_ShipUI2, _startPos2);*/
        if(_cameraShake == null)
            _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
       

    }


    void Update()
    {
        if(GameManager.isGamePaused == false)
        {
            if(_doPlayerIntro == false)
            {
                CalculateMovement();

                if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
                    FireLaser();

                if (HasMissiles() && _canFireMissiles)
                    if(Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(1))
                        LaunchHeatSeekers();
                    
                

                UpdateShieldVisuals();
            }
        }
    }
    //StartCoroutine(LaunchHeatSeekersRoutine());

    void CalculateMovement()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(hor, ver, 0);

        float finalSpeed = _speed;
        if (_isSpeedEnabled)
            finalSpeed *= _speedBoost;
        if (_thrusterController.IsThrusterPlaying())
            finalSpeed *= _thrusterSpeedBoost;
        
        transform.Translate(direction * finalSpeed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11)
            transform.position = new Vector3(-11, transform.position.y, 0);
        else if (transform.position.x < -11)
            transform.position = new Vector3(11, transform.position.y, 0);
    }

    IEnumerator PlayerIntroRoutine()
    {
        transform.position = _playerIntroPos;

        /*float yPos_Start = transform.position.y;
        float yPos_Target = _playerStartPos.y + 0.25f;
        float yPos_Current;*/

        yield return new WaitForSeconds(0.5f);

        float yPos_Target = _playerStartPos.y + 1.0f;
        float duration = 0.75f;
        float time = 0;
        float startValue = transform.position.y;
        float yPos = startValue;
        while (time < duration)
        {
            yPos = Mathf.Lerp(startValue, yPos_Target, time / duration);
            time += Time.deltaTime;
            transform.position = new Vector2(0.0f, yPos);
            yield return null;
        }
        yPos = yPos_Target;
        transform.position = new Vector2(0.0f, yPos);

        yPos_Target = _playerStartPos.y;
        duration = 0.75f;
        time = 0;
        startValue = transform.position.y;
        yPos = startValue;
        while (time < duration)
        {
            yPos = Mathf.Lerp(startValue, yPos_Target, time / duration);
            time += Time.deltaTime;
            transform.position = new Vector2(0.0f, yPos);
            yield return null;
        }
        yPos = yPos_Target;
        transform.position = _playerStartPos;

        _doPlayerIntro = false;
    }
    

    

    void FireLaser()
    {
        AudioClip ranClip = List_LaserFireSounds[0]; // initialize with default

        if (_ammoCount > 0)
        {
            _canFire = Time.time + _fireDelay;
            Vector3 pos = new Vector3(transform.position.x, 
                transform.position.y + _laser_yOffset, 0);
            
            if (_isTripleShotEnabled && _ammoCount > 2)
            {
                _ammoCount -= 3;
                
                GameObject newTripleShot = Instantiate(_tripleShotPrefab, 
                    pos, Quaternion.identity);
                newTripleShot.transform.parent = _tripleShotContainer;

                int ranIndex = Random.Range(0, List_TripleShotSounds.Count);
                ranClip = List_TripleShotSounds[ranIndex];
                _audioSource1.clip = ranClip;
            }
            else
            {
                _ammoCount--;

                Instantiate(_laserPrefab, pos, Quaternion.identity, _laserContainer);
                /*GameObject newLaser = Instantiate(_laserPrefab, pos, 
                    Quaternion.identity, _laserContainer);*/
                //newLaser.transform.parent = _laserContainer;

                int ranIndex = Random.Range(0, List_LaserFireSounds.Count);
                ranClip = List_LaserFireSounds[ranIndex];
                _audioSource1.clip = ranClip;
            }
        }
        else
        {
            if(List_FailedLaserSounds.Count > 0)
            {
                //Choose random failed laser soundFX.
                int ranIndex = Random.Range(0, List_FailedLaserSounds.Count);
                ranClip = List_FailedLaserSounds[ranIndex];
            }
            
        }

        if (List_LaserFireSounds != null && List_LaserFireSounds.Count > 0)
            _audioSource1.PlayOneShot(ranClip);

        UpdateAmmoCountContainer();
    }


    void FireSpread()
    {
        _canFireSpread = Time.time + _spreadFireDelay;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + _laser_yOffset, 0);

        Instantiate (_laserPrefab, pos, Quaternion.identity);
        var newSpread1 = Instantiate(_laserPrefab, pos, Quaternion.identity);
        newSpread1.transform.eulerAngles = new Vector3(0, 0, 5.0f);
        var newSpread2 = Instantiate(_laserPrefab, pos, Quaternion.identity);
        newSpread2.transform.eulerAngles = new Vector3(0, 0, -5.0f);
        var newSpread3 = Instantiate(_laserPrefab, pos, Quaternion.identity);
        newSpread3.transform.eulerAngles = new Vector3(0, 0, 10.0f);
        var newSpread4 = Instantiate(_laserPrefab, pos, Quaternion.identity);
        newSpread4.transform.eulerAngles = new Vector3(0, 0, -10.0f);

        StartCoroutine(_cooldown.CooldownRoutine(_spreadFireDelay));
    }

    void Malfunction()
    {
        
    }

    void UpdateAmmoCountContainer()
    {
        for(int i = 0; i < _laserGrid.transform.childCount; i++)
        {
            var laser = _laserGrid.transform.GetChild(i);
            if (i < _ammoCount)
                laser.gameObject.SetActive(true);
            else
                laser.gameObject.SetActive(false);
        }
    }


    
    public void Damage(GameObject dmgGO, int dmgAmount)
    {
        //  Check the tags for who damaged us for special interactions
        //      "Enemy" "Laser" "Asteroid"  "Missile"
        
        if (_isInvulnerable)
            return;

        if(dmgGO.tag == "Missile")
        {
            if (_canMissilesHurtSelf == false) //   Set in Editor
                return;
        } 

        if (_shieldLife > 0)
        {
            if(_cameraShake != null)
                _cameraShake.StartShake(0.05f, 0.1f);
            StartCoroutine(ShieldHit(dmgAmount));
            dmgAmount -= _shieldLife;
            if (dmgAmount < 1)
                return;
        }

        _lives -= dmgAmount;
        _isInvulnerable = true;
        _uiManager.UpdateLives(_lives);
        if (_damageFragments != null)
            _damageFragments.Fragmentize();

        if (_lives == 2)
        {
            if (_cameraShake != null)
                _cameraShake.StartShake(0.15f, 0.4f);

            if (List_Engines != null && List_Engines.Count > 0)
            {
                int randomIndex = Random.Range(0, List_Engines.Count);
                GameObject engine1 = List_Engines[randomIndex];
                engine1.SetActive(true);
            }
            StartCoroutine(ResetInvulernability(0.5f));
        }
        else if(_lives == 1)
        {
            if (_cameraShake != null)
                _cameraShake.StartShake(0.25f, 0.6f);

            if (List_Engines != null && List_Engines.Count > 0)
            {
                GameObject engine1 = List_Engines[0];
                GameObject engine2 = List_Engines[1];
                engine1.SetActive(true);
                engine2.SetActive(true);
                
                /*GameObject engine2;
                if (List_Engines[0].activeSelf == true)
                {
                    engine2 = List_Engines[1];
                }
                else
                    engine2 = List_Engines[0];
                
                engine2.SetActive(true);*/
            }
            StartCoroutine(ResetInvulernability(0.5f));
        }
        else if(_lives < 1)
        {
            if (_cameraShake != null)
                _cameraShake.StartShake(0.35f, 0.8f);

            if (_spawnManager != null)
            {
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                _spawnManager.OnPlayerDeath();

                 Destroy(gameObject);
            }
        }
    }

    

    IEnumerator ResetInvulernability(float timeToRestore)
    {
        yield return new WaitForSeconds(timeToRestore);
        _isInvulnerable = false;
    }


    private IEnumerator ShieldHit(int dmgAmount)
    {
        _isInvulnerable = true;
        StartCoroutine(PlayEnergyExplosion());

        for (float t = 0; t < _invulnerabilityTime; t += Time.deltaTime)
        {
            // Light2D intensity pulse when invulnerable
            float pulse = Mathf.Sin(t * _transitionPulseSpeed) * 0.5f + 0.5f;
            _shieldsLight.intensity = 
                Mathf.Lerp(_minPulseIntensity, _maxPulseIntensity, pulse);
            yield return null;
        }
        _shieldLife -= dmgAmount;
        if(_shieldLife < 0)
            _shieldLife = 0;

        _audioSource2.clip = _shieldDamagedSound;
        _audioSource2.time = _audio2_StartTime;
        _audioSource2.Play();

        _isInvulnerable = false;
    }

    private void UpdateShieldVisuals()
    {
        if (_isInvulnerable)
        {
            // Color transition during invulnerability
            _shieldsLight.color = Color.Lerp(_shieldsLight.color, _targetColor,
                Time.deltaTime * _colorTransitionSpeed);
        }
        else
        {
            Color col = _shieldSprite.color;
            float alpha = col.a;

            switch (_shieldLife)
            {
                case 3:
                    alpha = 1.0f; 
                    _targetColor = _fullShieldsCol;
                    break;
                case 2:
                    alpha = 0.9f; 
                    _targetColor = _halfShieldsCol;
                    break;
                case 1:
                    alpha = 0.8f; 
                    _targetColor = _criticalShieldsCol;
                    break;
                case 0:
                    _shieldVisualizer.SetActive(false);
                    break;
            }
            //  Shield Alpha change when not invulnerable
            col.a = alpha;
            _shieldSprite.color = col;

            // Color change when not invulnerable
            _shieldsLight.color = _targetColor;

            // Light2D intensity pulse when not invulnerable
            float pulse = Mathf.Sin(Time.time * _steadyPulseSpeed) *
                (_maxPulseIntensity - _minPulseIntensity) * 0.5f +
                (_maxPulseIntensity + _minPulseIntensity) * 0.5f;

            _shieldsLight.intensity = pulse;
        }
    }
    IEnumerator PlayEnergyExplosion()
    {
        if(_shieldExplosionEffect != null)
        {
            _shieldExplosionEffect.Play();
            /*while (_shieldExplosionEffect.IsAlive(true))
                yield return null;*/

            yield return new WaitForSeconds(1.2f);

            _shieldExplosionEffect.Stop();
        }
        
    }


    public void ActivatePowerup(int powerupID)
    {
        StartCoroutine(PowerDownRoutine(powerupID));
    }

    IEnumerator PowerDownRoutine(int powerupID)
    {
        switch (powerupID)
        {
            case 0:
                _isTripleShotEnabled = true;
                yield return new WaitForSeconds(_tripleShotPowerDownTime);
                _isTripleShotEnabled = false;
                break;

            case 1:
                _isSpeedEnabled = true;
                yield return new WaitForSeconds(_speedPowerDownTime);
                _isSpeedEnabled = false;
                break;

            case 2:
                _shieldVisualizer?.SetActive(true);
                _shieldLife = 3;
                _shieldsLight.color = _fullShieldsCol;
                StartCoroutine(PlayEnergyExplosion());
                break;

            case 3:
                _ammoCount = 15;
                UpdateAmmoCountContainer();
                break;

            case 4:
                StartCoroutine(RegainHealthRoutine());
                break;

            case 5:
                StartCoroutine(ReadyMissilesRoutine());
                break;

            default:
                Debug.Log("Wrong Powerup ID");
                break;
        }
    }
    public IEnumerator RegainHealthRoutine()
    {
        _isInvulnerable = true;
        SpriteRenderer sprite = _sprite_Ship1;
        ParticleSystem particles = _burstParticles_1;

        int maxLives = 3;
        int numLivesToRegain = maxLives - _lives;
        int spriteIndex = 0;

        List<Transform> Ship_UIs = new List<Transform>();
        if (numLivesToRegain == 1)
            Ship_UIs.Add(_trans_ShipUI_2);
        else if(numLivesToRegain == 2)
        {
            Ship_UIs.Add(_trans_ShipUI_1);
            Ship_UIs.Add(_trans_ShipUI_2);
        }
            
        for(int i = 0; i < Ship_UIs.Count; i++)
        {
            var shipUI = Ship_UIs[i];
            Vector2 startPos = Vector2.zero;
            Vector2 endPos = Vector2.zero;
            if(shipUI == _trans_ShipUI_1)
            {
                sprite = _sprite_Ship1;
                startPos = _startPos1;
                endPos = _endPos1;
                spriteIndex = 2;
                particles = _burstParticles_1;
            }
            else if(shipUI == _trans_ShipUI_2)
            {
                sprite = _sprite_Ship2;
                startPos = _startPos2;
                endPos = _endPos2;
                spriteIndex = 3;
                particles = _burstParticles_2;
            }
            RegainLife();
            StartCoroutine(MoveShipUI_Routine(shipUI, sprite, spriteIndex, startPos, endPos, 
                _controlPoint, _timeToMove, particles));
            yield return new WaitForSeconds(0.33f); // delay before next one takes off
        }
        yield return new WaitForSeconds(0.5f);  //  delay for animation before being vulnerable
        _isInvulnerable = false;
    }

    void RegainLife()
    {
        _lives++;
        if(_lives > _maxLives)
            _lives = _maxLives;

        if (_lives == 2)
        {
            if (List_Engines != null && List_Engines.Count > 0)
            {
                int randomIndex = Random.Range(0, List_Engines.Count);
                GameObject engine1 = List_Engines[randomIndex];
                engine1.SetActive(false);
            }
        }
        else if (_lives == 3)
        {
            List_Engines[0].SetActive(false);
            List_Engines[1].SetActive(false);
        }
    }
    IEnumerator MoveShipUI_Routine(Transform t_ShipUI, SpriteRenderer sprite, int spriteIndex, 
        Vector3 startPos, Vector3 endPos, Vector3 controlPoint, float timeToMove, ParticleSystem particles)
    {
        float t = 0f;
        float randomXpos_ControlPoint = controlPoint.x;
        randomXpos_ControlPoint = Random.Range(randomXpos_ControlPoint - 2f, randomXpos_ControlPoint + 2f);
        controlPoint.x = randomXpos_ControlPoint;

        float ranTimeToMove = Random.Range(timeToMove - 0.15f, timeToMove + 0.15f);

        float xPos = Random.Range(-12, -8);
        startPos = new Vector3(xPos, startPos.y, startPos.z);

        

        while (t < 1f)
        {
            t += Time.deltaTime / ranTimeToMove;

            // Fade in the SpriteRenderer
            if (t < 0.9f)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, t);                    // This will reach 0.75 when t is 0.9
            }
            else
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.9f);                 // Once t reaches 0.9, maintain the transparency at 0.9
            }

            // Use Bezier curve to smoothly transition from start position to  end position
            Vector3 newPosition = (1 - t) * (1 - t) * startPos + 2 * (1 - t) 
                * t * controlPoint + t * t * endPos;

            t_ShipUI.localPosition = newPosition;

            yield return null;
        }
        particles.Play();

        if (_uiManager != null)
        {
            _uiManager.SwapLivesSprite(spriteIndex);
        }

        yield return new WaitForSeconds(0.1f);
        Reset_ShipUIs(t_ShipUI, sprite, startPos);
    }

    void Reset_ShipUIs(Transform t_ShipUI, SpriteRenderer sprite, Vector3 startPos)
    {
        t_ShipUI.localPosition = startPos;
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0); // Reset alpha to 0
    }

    /*void HeatSeekers()
    {
        *//*if (_hasMissiles)
            if (_canMissilesStack == false)
                return;*/


    /*if (_canMissilesAutoRefresh)
        _willReloadMissiles = true;*//*


    StartCoroutine(ReadyMissilesRoutine());
    StartCoroutine(DeactivateMissileRoutine());
}*/
    #region simpler heatseeker code -- new

    

    public IEnumerator ReadyMissilesRoutine()
    {
        //  Don't allow new missiles if already have missiles
        if (HasMissiles())
            yield break;

        //  Spawn both left/right missiles behind Player ship
        GameObject leftMissile = Instantiate(_heatseekerPrefab, 
            _startPosLeftMissile, Quaternion.identity, this.transform);
        GameObject rightMissile = Instantiate(_heatseekerPrefab, 
            _startPosRightMissile, Quaternion.identity, this.transform);

        Transform t_LeftMissile = leftMissile.transform;
        Transform t_RightMissile = rightMissile.transform;

        //  Missiles float outward to ready position (under each wing)
        float t = 0;
        while (t < _timeToReadyMissiles)
        {
            t += Time.deltaTime;

            // Calculate each missile's position using linear interpolation (Lerp)
            Vector2 currentPos_LeftMissile = Vector2.Lerp(_startPosLeftMissile, 
                _readyPosLeftMissile, t / _timeToReadyMissiles);
            Vector2 currentPos_RightMissile = Vector2.Lerp(_startPosRightMissile, 
                _readyPosRightMissile, t / _timeToReadyMissiles);

            // Update positions of both missiles
            t_LeftMissile.localPosition = currentPos_LeftMissile;
            t_RightMissile.localPosition = currentPos_RightMissile;

            yield return null;
        }
        
        // After coroutine, each missile is armed and ready to seek targets
        _currentLeftMissile = leftMissile;
        _currentRightMissile = rightMissile;

        _canFireMissiles = true;
    }

    bool HasMissiles()
    {
        bool hasMissiles = false;

        //  Player "HasMissiles" if either missile is being tracked
        if (_currentLeftMissile != null || _currentRightMissile != null)
            hasMissiles = true;

        return hasMissiles;
    }



    void LaunchHeatSeekers()
    {
        if(_currentLeftMissile != null && _currentRightMissile != null)
        {
            HeatSeekingMissile hsm_Left = 
                _currentLeftMissile.GetComponent<HeatSeekingMissile>();
            HeatSeekingMissile hsm_Right = 
                _currentRightMissile.GetComponent<HeatSeekingMissile>();

            Vector2 playerPos = transform.position;

            //  Signal each missile to fire, passing in Player's Position
            if (hsm_Left != null && hsm_Right != null)
            {
                hsm_Left.FireMissile(LeftOrRightMissile.Left, playerPos);
                hsm_Right.FireMissile(LeftOrRightMissile.Right, playerPos);
            }
            //  Player can no longer fire missiles
            _canFireMissiles = false;

            //  ...but can now pick up new missiles
            _currentLeftMissile = null;
            _currentRightMissile = null;
        }
    }



#endregion

    #region complex heatseekers -- old code

    /*IEnumerator DeactivateMissileRoutine()
    {
        yield return null;
        yield return new WaitForSeconds(_durationForMissiles);
        _willReloadMissiles = false;
    }

    public IEnumerator ReadyMissilesRoutine()
    {
        StartCoroutine(DeactivateMissileRoutine());

        if (_hasMissiles)
            if (_canMissilesStack == false)
                yield break;

        if (_canMissilesAutoRefresh || HasMissiles() == false)
            _willReloadMissiles = true;

        if (_willReloadMissiles == false)
            yield break;

        GameObject leftMissile = Instantiate(_heatseekerPrefab,
            _startPosLeftMissile, Quaternion.identity, this.transform);
        GameObject rightMissile = Instantiate(_heatseekerPrefab,
            _startPosRightMissile, Quaternion.identity, this.transform);

        Transform t_LeftMissile = leftMissile.transform;
        Transform t_RightMissile = rightMissile.transform;

        _hasMissiles = true;
        //Play sound effect of Missiles being reloaded

        float t = 0;
        while (t < _timeToReadyMissiles)
        {
            t += Time.deltaTime;

            // Calculate the current position using linear interpolation (Lerp)
            Vector2 currentPos_LeftMissile = Vector2.Lerp(_startPosLeftMissile, _readyPosLeftMissile, t / _timeToReadyMissiles);
            Vector2 currentPos_RightMissile = Vector2.Lerp(_startPosRightMissile, _readyPosRightMissile, t / _timeToReadyMissiles);

            // Update the position of the missile
            t_LeftMissile.localPosition = currentPos_LeftMissile;
            t_RightMissile.localPosition = currentPos_RightMissile;

            yield return null;
        }

        _currentLeftMissile = leftMissile;
        _currentRightMissile = rightMissile;

        // After the coroutine is done, the missile is armed and ready to seek targets
        AllowMissilesToFire();
    }

    void AllowMissilesToFire()
    {
        _canFireMissiles = true;
    }

    bool HasMissiles()
    {
        if (_currentLeftMissile == null && _currentRightMissile == null)
            _hasMissiles = false;
        else
            _hasMissiles = true;

        return _hasMissiles;
    }



    IEnumerator LaunchHeatSeekersRoutine()
    {
        if (_currentLeftMissile != null && _currentRightMissile != null)
        {
            HeatSeekingMissile hsm_Left = _currentLeftMissile.GetComponent<HeatSeekingMissile>();
            HeatSeekingMissile hsm_Right = _currentRightMissile.GetComponent<HeatSeekingMissile>();

            Vector2 playerPos = transform.position;

            if (hsm_Left != null && hsm_Right != null)
            {
                hsm_Left.FireMissile(LeftOrRightMissile.Left, playerPos);
                hsm_Right.FireMissile(LeftOrRightMissile.Right, playerPos);
            }

            _canFireMissiles = false;
            _hasMissiles = false;

            _currentLeftMissile = null;
            _currentRightMissile = null;

            if (_willReloadMissiles)
            {
                yield return new WaitForSeconds(_timeBetweenMissiles);
                StartCoroutine(ReadyMissilesRoutine());
            }
        }
    }*/

    #endregion


    public void AddScore(int points)
    {
        _score += points;
        if (_uiManager != null)
            _uiManager.UpdateScoreText(_score);

    }

    public bool IsInvulnerable()
    {
        bool isInvulnerable = _isInvulnerable;
        return isInvulnerable;

    }

    public bool CanMissilesHurtPlayer()
    {
        return _canMissilesHurtSelf;
    }

    


    //PRIOR GDHQ METHOD
    /*public void TripleShotActive()
    {

        _isTripleShotEnabled = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(_tripleShotPowerDownTime);
        _isTripleShotEnabled = false;
    }*/

}