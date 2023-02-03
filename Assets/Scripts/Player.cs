using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static UnityEditor.PlayerSettings;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField]
    SpawnManager _spawnManager;
    [SerializeField]
    UIManager _uiManager;

    [SerializeField]
    GameObject _explosionPrefab;

    [SerializeField]
    GameObject _laserPrefab;
    [SerializeField]
    Transform _laserContainer;
    [SerializeField]
    float _speed = 3.5f;
    [SerializeField]
    float _laser_yOffset = 0.8f;

    [Space(15)]
    [SerializeField]
    int _lives = 3;
    [Space(5)]
    [SerializeField]
    int _score;
    [Space(10)]

    [SerializeField]
    Vector2 _playerStartPos = new Vector2(0f, -3.69f);
    [SerializeField]
    Vector2 _playerIntroPos = new Vector2(0f, -7f);

    [SerializeField]
    AudioSource _audioSource;
    /*[SerializeField]
    AudioClip _laserSoundClip;*/
    [SerializeField]
    List<AudioClip> List_LaserFireSounds = new List<AudioClip>();
    [SerializeField]
    List<AudioClip> List_TripleShotSounds = new List<AudioClip>();


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
    float _speedBoost = 2.0f;
    [SerializeField]
    float _speedPowerDownTime = 5.0f;
    [Space(5)]

    [SerializeField]
    GameObject _shieldVisualizer;

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


    [SerializeField] 
    float _spreadFireDelay = 4.0f;
    float _canFireSpread = 0.0f;

    private void Start()
    {
        if(_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            Debug.LogError("PLAYER: AudioSource is NULL!");
        /*else
            _audioSource.clip = _laserSoundClip;*/
       
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



                /*else if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    if (Time.time > _canFireSpread)
                        FireSpread();
                    else
                        Malfunction();
                }*/
            }


        }


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


        /*while (transform.position.y < yPos_Target)
        {
            yield return null;
            //transform.Translate(Vector2.up * _speed * 0.5f * Time.deltaTime);
            yPos_Current = Mathf.Lerp(yPos_Start, yPos_Target, _speed * Time.deltaTime);
            Vector2 pos = new Vector2(0, yPos_Current);
            transform.position = pos;
        }
        yield return new WaitForSeconds(0.15f);
        yPos_Start = transform.position.y;
        yPos_Target = _playerStartPos.y;
        while(transform.position.y > yPos_Target)
        {
            yield return null;
            //transform.Translate(Vector2.down * _speed * 0.5f * Time.deltaTime);
            yPos_Current = Mathf.Lerp(yPos_Start, yPos_Target, _speed * Time.deltaTime);
            Vector2 pos = new Vector2(0, yPos_Current);
            transform.position = pos;
        }
        transform.position = _playerStartPos;*/

        _doPlayerIntro = false;
    }
    void CalculateMovement()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(hor, ver, 0);
        
        if (_isSpeedEnabled)
            transform.Translate(direction * _speed * _speedBoost * Time.deltaTime);
        else
            transform.Translate(direction * _speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11)
            transform.position = new Vector3(-11, transform.position.y, 0);
        else if (transform.position.x < -11)
            transform.position = new Vector3(11, transform.position.y, 0);
    }

    

    void FireLaser()
    {
        _canFire = Time.time + _fireDelay;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + _laser_yOffset, 0);

        if (_isTripleShotEnabled)
        {
            GameObject newTripleShot = Instantiate(_tripleShotPrefab, pos, Quaternion.identity);
            newTripleShot.transform.parent = _tripleShotContainer;

            int ranIndex = Random.Range(0, List_TripleShotSounds.Count);
            AudioClip ranClip = List_TripleShotSounds[ranIndex];
            _audioSource.clip = ranClip;
        }
        else
        {
            GameObject newLaser = Instantiate(_laserPrefab, pos, Quaternion.identity);
            newLaser.transform.parent = _laserContainer;

            int ranIndex = Random.Range(0, List_LaserFireSounds.Count);
            AudioClip ranClip = List_LaserFireSounds[ranIndex];
            _audioSource.clip = ranClip;
        }
        if(List_LaserFireSounds != null && List_LaserFireSounds.Count > 0)
        {
            
            _audioSource.Play();
        }
        
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


    
    public void Damage()
    {

        if (_isShieldEnabled)
        {
            _isShieldEnabled = false;
            _shieldVisualizer.SetActive(false);
            return;
        }

        _lives--;
        _uiManager.UpdateLives(_lives);

        if (_lives == 2)
        {
            if(List_Engines != null && List_Engines.Count > 0)
            {
                int randomIndex = Random.Range(0, List_Engines.Count);
                GameObject engine1 = List_Engines[randomIndex];
                engine1.SetActive(true);
            }
        }
        else if(_lives == 1)
        {
            if (List_Engines != null && List_Engines.Count > 0)
            {
                GameObject engine2;
                if (List_Engines[0].activeSelf == true)
                {
                    engine2 = List_Engines[1];
                }
                else
                    engine2 = List_Engines[0];
                
                engine2.SetActive(true);
            }
        }
        else if(_lives < 1)
        {
            if(_spawnManager != null)
            {
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                _spawnManager.OnPlayerDeath();
                Destroy(gameObject);
            }
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
                _isShieldEnabled = true;
                _shieldVisualizer?.SetActive(true);
                break;

            default:
                Debug.Log("Wrong Powerup ID");
                break;

        }
        
    }


    public void AddScore(int points)
    {
        _score += points;
        if (_uiManager != null)
            _uiManager.UpdateScoreText(_score);

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