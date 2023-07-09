using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    
    
    UIManager _uiManager; 
    
    [Header("MY SETTINGS")]
    [SerializeField] bool _useWaveSpawning = true;
    [SerializeField] bool _spawnPowerups = true;
    [SerializeField] bool _spawnAsteroids = true;

    [Header("GENERAL")]
    [SerializeField, ReadOnly] bool _hasGameStarted = false;
    [SerializeField, ReadOnly] bool _isPlayerDead = false;
    [SerializeField, ReadOnly] bool _isWaveSystemTransitionining = false;
    
    /*[Header("WAVES")]
    [SerializeField, ReadOnly] int _waveNum = 0;
    [SerializeField, ReadOnly] int _waveEnemiesToSpawn = 0;
    [SerializeField, ReadOnly] int _waveEnemiesSpawned = 0;
    [SerializeField, ReadOnly] int _remainingEnemies = 0;*/



    [Header("ENEMIES")]
    [SerializeField] GameObject _enemyPrefab;
    [SerializeField] GameObject _enemyContainer;
    [SerializeField] WaveSystem _waveSystem;
    [SerializeField] float _startEnemyDelay = 2.0f;
    [SerializeField] float _spawnEnemyRate = 5.0f;
    [SerializeField] float _yPosAtSpawn = 7.0f;
    [SerializeField] float _xPosSpawnMin = -9.0f;
    [SerializeField] float _xPosSpawnMax = 9.0f;
    [SerializeField] int _maxEnemies = 10;
    [Space(10)]
    [SerializeField, ReadOnly] int _countEnemies = 0;
    [Space(5)]
    [SerializeField, ReadOnly] int _waveNum = 0;
    [SerializeField, ReadOnly] int _waveEnemiesToSpawn = 0;
    [SerializeField, ReadOnly] int _waveEnemiesSpawned = 0;
    [SerializeField, ReadOnly] int _waveEnemiesKilled = 0;
    [SerializeField, ReadOnly] int _remainingWaveEnemies = 0;
    public static List<Transform> EnemyTransforms = new List<Transform>();

    [Header("POWERUPS")]//0 = TripleShot  //1 = Speed  //2 = Shields   //3 = Ammo  //4 = Health   //5 = Homing
    [SerializeField] GameObject powerup_TRIPLESHOT_prefab;
    [SerializeField] GameObject powerup_SPEED_prefab;
    [SerializeField] GameObject powerup_SHIELD_prefab;
    [SerializeField] GameObject powerup_AMMO_prefab;
    [SerializeField] GameObject powerup_HEALTH_prefab;
    [SerializeField] GameObject powerup_HOMING_prefab;

    [SerializeField] int _countPowerupsSpawned = 0;
    [SerializeField] bool _randomizePowerups = true;
    [SerializeField] List<GameObject> _List_Powerups = new List<GameObject>();
    [SerializeField] float _startPowerupDelay = 10f;
    [SerializeField] bool _randomizePowerupSpawnRate = true;
    [SerializeField] float _minPowerupSpawnRate = 3.0f;
    [SerializeField] float _maxPowerupSpawnRate = 7.0f;
    [SerializeField] float _spawnPowerupRate = 7.0f;
    bool _canBeSameAsLastPowerup = true;

    [Header("CORRUPTIONS")] //Negative Powerups
    //If new Enemies are spawned while an effect is in play, it is granted to them as well.
    [SerializeField] GameObject corruption_TRIPLESHOT_prefab;
    [SerializeField] GameObject corruption_SPEED_prefab;
    [SerializeField] GameObject corruption_SHIELD_prefab; 
    [SerializeField] GameObject corruption_AMMO_prefab;
    [SerializeField] GameObject corruption_HEALTH_prefab;
    [SerializeField] GameObject corruption_HOMING_prefab;

    [SerializeField, ReadOnly] bool _areCorruptionsInPlay = false;
    [SerializeField, ReadOnly] List<int> _CorruptionsInPlay_byInt = new List<int>();
    [SerializeField, ReadOnly] bool _CRPT_BurstFire = false;            //    int = 0
    [SerializeField, ReadOnly] float _timeLeft_BurstFire = 0.0f;
    [SerializeField, ReadOnly] bool _CRPT_EnemySpeedBoost = false;      //    int = 1
    [SerializeField, ReadOnly] float _timeLeft_EnemySpeedBoost = 0.0f;
    [SerializeField, ReadOnly] bool _CRPT_EnemyShields = false;      //    int = 2
    [SerializeField, ReadOnly] float _timeLeft_EnemyShields = 0.0f;
    /*[SerializeField, ReadOnly] bool _CRPT_EnemyAmmo = false;          //    int = 3
    [SerializeField, ReadOnly] float _timeLeft_EnemyAmmo = 0.0f;*/
    /*[SerializeField, ReadOnly] bool _CRPT_EnemyHealth = false;        //    int = 4
    [SerializeField, ReadOnly] float _timeLeft_EnemyHealth = 0.0f;*/
    /*[SerializeField, ReadOnly] bool _CRPT_EnemyHoming = false;        //    int = 5
    [SerializeField, ReadOnly] float _timeLeft_EnemyHoming = 0.0f;*/


    [Header("ASTEROIDS")]
    [SerializeField] bool _useAsteroidToStart = true;
    [Space(10)]
    [SerializeField] GameObject _asteroidShell;
    bool _hasStartedAsteroids = false;
    [Space(5)]
    [SerializeField] float _startDelaySpawnAsteroids = 15.0f;
    [Space(10)]
    [SerializeField] float _minSpeed_Asteroids = 4f;
    [SerializeField] float _maxSpeed_Asteroids = 8f;
    [Space(5)]
    [SerializeField] float _minScale_Asteroids = 0.25f;
    [SerializeField] float _maxScale_Asteroids = 0.75f;
    [Space(10)]
    [SerializeField] bool _doAllow_NoAsteroidEvents = true;
    [SerializeField][Range(1, 100)] int _numChancesFor_0_AsteroidEvents = 33;
    [Space(5)]
    [SerializeField] bool _doLimitAsteroidEvents = true;
    [SerializeField] [Range(1, 100)] int _numChancesFor_1AsteroidEvent = 50;
    [Space(5)]
    [SerializeField] bool _canUseAsteroidClusters = true;
    [SerializeField] int _numAsteroids_BeforeClustersAllowed = 10;
    [SerializeField] [Range(1, 100)] int _numChancesFor_ClusterEvent = 20;
    [Space(5)]
    [SerializeField] bool _canUseAsteroidShowers = true;
    [SerializeField] int _numAsteroids_BeforeShowersAllowed = 50;
    [SerializeField] [Range(1, 100)] int _numChancesFor_ShowerEvent = 5;
    [Space(10)]
    [SerializeField] AudioSource _event_AudioSource;
    public List<AudioClip> List_AsteroidEvent_AudioClips = new List<AudioClip>();       //not set up yet
    [Space(10)]
    [SerializeField] GameObject _ast_WarningLight_Left;
    [SerializeField] GameObject _ast_WarningLight_TopLeft;
    [SerializeField] GameObject _ast_WarningLight_TopRight;
    [SerializeField] GameObject _ast_WarningLight_Right;
    [Space(10)]
    [SerializeField, ReadOnly] int _countAsteroidsSpawned = 0;
    [SerializeField, ReadOnly] int _countAsteroidClusterEvents = 0;
    [SerializeField, ReadOnly] int _countAsteroidShowerEvents = 0;
    [SerializeField] List<int> List_IntsForAsteroidEvents = new List<int>(); // ASTEROIDS     0 = Single     1 = Cluster     2 = Shower
    [Space(30)]
    [SerializeField] int _minNumInCluster = 3;
    [SerializeField] int _maxNumInCluster = 8;
    [Space(30)]
    [SerializeField] int _minNumInShower = 25;
    [SerializeField] int _maxNumInShower = 50;
    [SerializeField] float _minShift_X_Shower = -10.0f;
    [SerializeField] float _maxShift_X_Shower = 10.0f;
    [SerializeField] float _marginError_X_Shower = 1.0f;
    [SerializeField] float _marginError_Y_Shower = 1.0f;

    [SerializeField, ReadOnly] AsteroidState _asteroidState;
    [SerializeField, ReadOnly] AsteroidState _lastAsteroidState;

    public enum AsteroidState
    {
        NoState,
        Asteroid_Single,
        Asteroid_Cluster,
        Asteroid_Shower,
    };

    void Start()
    {
        _asteroidState = AsteroidState.NoState;
        _lastAsteroidState = _asteroidState;
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
            Debug.LogError("SPAWN MANAGER: Start() UIManager is NULL");

        EnemyTransforms.Clear();
        if (_useAsteroidToStart == false)
        {
            _hasGameStarted = true;
            StartSimpleSpawning();
        }
            
    }

    public void StartSimpleSpawning()
    {
        _hasGameStarted = true; //  useWaveSpawning waits for this bool to be true;

        if(_useWaveSpawning == false)
        {
            StartCoroutine(SpawnEnemyRoutine(_maxEnemies, _spawnEnemyRate));
            StartCoroutine(SpawnPowerupRoutine());
            StartCoroutine(SpawnAsteroidsRoutine(0.0f));
        }
    }

    public void StartWaveSpawning(Wave wave)
    {
        StopAllCoroutines();

        if (_hasGameStarted)
        {
            StartCoroutine(WaveRoutine(wave));
        }
        else
            StartCoroutine(WaitForGameToStart(wave));
    }
    IEnumerator WaitForGameToStart(Wave wave)
    {
        while (_hasGameStarted == false)
            yield return null;
        StartWaveSpawning(wave);
    }

    /*IEnumerator WaveSpawningRoutine(Wave wave)
    {

        int waveNumber = wave.waveNum;
        int numEnemies = wave.numEnemies;
        float spawnRate = wave.spawnRate;


        if (_isPlayerDead == false)
        {
            StartCoroutine(SpawnEnemyRoutine(waveNumber, numEnemies, spawnRate));
            StartCoroutine(SpawnPowerupRoutine());
            StartCoroutine(SpawnAsteroidsRoutine(0.0f));
        }





    }*/

    #region former WaveSpawning code
    /*public void StartWaveSpawning(int waveNumber, int numEnemies, float spawnRate)
    {

        if (_hasGameStarted)
        {
            if(_isPlayerDead == false)
                StopAllCoroutines();
                
            StartCoroutine(SpawnEnemyRoutine(waveNumber, numEnemies, spawnRate));
            StartCoroutine(SpawnPowerupRoutine());
            StartCoroutine(SpawnAsteroidsRoutine(0.0f));
        }
                
        else
            StartCoroutine(WaitForGameToStart(waveNumber, numEnemies, spawnRate));
        
            
    }*/
    #endregion

    IEnumerator WaveRoutine(Wave wave)
    {
        if (_isPlayerDead == false)
        {
            bool bossBattle = wave.mainBOSS;
            if (bossBattle)
                StartCoroutine(StartBossBattle(wave));
            else
            {
                yield return StartCoroutine(_uiManager.DisplayWaveNumberText(wave));

                StartCoroutine(SpawnEnemy_WaveRoutine(wave));
                StartCoroutine(SpawnPowerup_WaveRoutine(wave));
                StartCoroutine(SpawnAsteroids_WaveRoutine(wave));
            }
        }
    }

   

    IEnumerator SpawnEnemy_WaveRoutine(Wave wave)
    {
        _waveNum = wave.waveNum;
        float spawnRate = wave.wave_ENEMIES.spawnRate;
        _waveEnemiesToSpawn = wave.wave_ENEMIES.numEnemies;
        _waveEnemiesSpawned = 0;
        _waveEnemiesKilled = 0;
        _remainingWaveEnemies = _waveEnemiesToSpawn;
        

        //  Spawn Enemies til target number is reached for wave
        while (_isPlayerDead == false && _waveEnemiesSpawned < _waveEnemiesToSpawn)
        {
            SpawnEnemy(wave);
            yield return new WaitForSeconds(spawnRate);
        }
        //  Wait til all Enemies are destroyed...
        while (_isPlayerDead == false && _remainingWaveEnemies > 0)
            yield return null;

        //      ...then Start Next Wave
        if (_isPlayerDead == false && _waveSystem != null)
            _waveSystem.StartNextWave();
    }


    IEnumerator SpawnEnemyRoutine(int numEnemies, float spawnRate)
    {
        
        while (_isPlayerDead == false && _countEnemies < numEnemies)
        {
            SpawnEnemy(null);
            yield return new WaitForSeconds(spawnRate);
        }
        
        
    }

    void SpawnEnemy(Wave wave)
    {
        float randomX = Random.Range(_xPosSpawnMin, _xPosSpawnMax);
        Vector3 spawnPos = new Vector3(randomX, _yPosAtSpawn, 0);
        GameObject newEnemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
        
        if(wave != null)
        {
            Enemy enemy = newEnemy.GetComponent<Enemy>();
            float moveSpeedMod = wave.wave_ENEMIES.speedBoost;
            float fireRateMod = wave.wave_ENEMIES.fireRateBoost;

            enemy.AdjustMySpeedBoostSpeed(moveSpeedMod);
            enemy.AdjustMyFireRateBoostSpeed(fireRateMod);
        }
        
        
        Transform transEnemy = newEnemy.transform;
        transEnemy.SetParent(_enemyContainer.transform);
        _countEnemies++;
        _waveEnemiesSpawned++;

        AddEnemyTransformToList(transEnemy);
    }

    static public void AddEnemyTransformToList(Transform trans)
    {
        EnemyTransforms.Add(trans);
    }
    static public void RemoveEnemyTransformFromList(Transform trans)
    {
        EnemyTransforms.Remove(trans);
    }


    //  ??  (6/27/23)
    //
    //  Are Powerups still spawning during wave transitions?








    IEnumerator SpawnPowerup_WaveRoutine(Wave wave)
    {
        var wave_Powerups = wave.wave_POWERUPS;

        var powerups = UpdatePowerupList(wave_Powerups);

        while (_isPlayerDead == false && _spawnPowerups)
        {
            int randomID = Random.Range(0, powerups.Count);
            float randomX = Random.Range(_xPosSpawnMin, _xPosSpawnMax);
            Vector3 spawnPos = new Vector3(randomX, _yPosAtSpawn, 0);

            if (powerups != null && powerups.Count > 0)
            {
                var randomPowerup = powerups[randomID];
                Instantiate(randomPowerup, spawnPos, Quaternion.identity);
                _countPowerupsSpawned++;
            }

            yield return new WaitForSeconds(_spawnPowerupRate);
        }
    }
    
    List<GameObject> UpdatePowerupList(Wave_Powerups wave_Powerups)
    {
        float spawnRate = wave_Powerups.spawnRate;
        AdjustPowerupSpawnRate(spawnRate);
        
        _List_Powerups.Clear();

        //  Add POWERUPS
        AddPowerups(powerup_AMMO_prefab, wave_Powerups.chances_AMMO);
        AddPowerups(powerup_HEALTH_prefab, wave_Powerups.chances_HEALTH);
        AddPowerups(powerup_SPEED_prefab, wave_Powerups.chances_SPEED);
        AddPowerups(powerup_SHIELD_prefab, wave_Powerups.chances_SHIELD);
        AddPowerups(powerup_TRIPLESHOT_prefab, wave_Powerups.chances_TRIPLESHOT);
        AddPowerups(powerup_HOMING_prefab, wave_Powerups.chances_HOMING);

        //  Add CORRUPTIONS
        AddPowerups(corruption_AMMO_prefab, wave_Powerups.chances_AMMO_Corrupt);
        AddPowerups(corruption_HEALTH_prefab, wave_Powerups.chances_HEALTH_Corrupt);
        AddPowerups(corruption_SPEED_prefab, wave_Powerups.chances_SPEED_Corrupt);
        AddPowerups(corruption_SHIELD_prefab, wave_Powerups.chances_SHIELD_Corrupt);
        AddPowerups(corruption_TRIPLESHOT_prefab, wave_Powerups.chances_TRIPLESHOT_Corrupt);
        AddPowerups(corruption_HOMING_prefab, wave_Powerups.chances_HOMING_Corrupt);

        return _List_Powerups;
    }

    void AdjustPowerupSpawnRate(float newRate)
    {
        _spawnPowerupRate = newRate;
    }
    void AddPowerups(GameObject powerup, int num)
    {
        for (int i = 0; i < num; i++) 
            _List_Powerups.Add(powerup);
    }

    IEnumerator SpawnPowerupRoutine()
    {
        if (_useWaveSpawning)
        {
            //HAS ITS OWN METHOD    (6/27/23)
            //
            //
            /*while (_isPlayerDead == false && _spawnPowerups)
            {
                int randomID = Random.Range(0, _List_Powerups.Count);
                float randomX = Random.Range(_xPosSpawnMin, _xPosSpawnMax);
                Vector3 spawnPos = new Vector3(randomX, _yPosAtSpawn, 0);
                
                if (_List_Powerups != null && _List_Powerups.Count > 0)
                {
                    var randomPowerup = _List_Powerups[randomID];
                    Instantiate(randomPowerup, spawnPos, Quaternion.identity);
                    _countPowerupsSpawned++;
                }

                    yield return new WaitForSeconds(_spawnPowerupRate);
            }*/
             
        }
        else
        {
            //making this an unreachable number
            int lastPowerupID = 1000;

            yield return new WaitForSeconds(_startPowerupDelay);
            while (_isPlayerDead == false && _spawnPowerups)
            {
                float randomX = Random.Range(_xPosSpawnMin, _xPosSpawnMax);
                Vector3 spawnPos = new Vector3(randomX, _yPosAtSpawn, 0);

                if (_List_Powerups != null && _List_Powerups.Count > 0)
                {
                    if (_randomizePowerups)
                    {
                        int randomID = Random.Range(0, _List_Powerups.Count);
                        if (_canBeSameAsLastPowerup == false && _List_Powerups.Count > 1)
                        {

                            while (randomID == lastPowerupID)
                            {
                                randomID = Random.Range(0, _List_Powerups.Count);
                            }


                        }
                        var randomPowerup = _List_Powerups[randomID];
                        Instantiate(randomPowerup, spawnPos, Quaternion.identity);

                        lastPowerupID = randomID;
                    }
                    else
                    {
                        //If not randomizing Powerups, Spawn them in order & loop

                        int nextID = 0;
                        if (_countPowerupsSpawned > 0)
                            nextID = lastPowerupID + 1;
                        if (nextID >= _List_Powerups.Count)
                            nextID = 0;

                        var nextPowerup = _List_Powerups[nextID];
                        Instantiate(nextPowerup, spawnPos, Quaternion.identity);

                        lastPowerupID = nextID;

                    }
                    _countPowerupsSpawned++;
                }
                else
                    Debug.Log("SPAWNMANAGER: There are no Powerups in List");



                float time;

                if (_randomizePowerupSpawnRate)
                    time = Random.Range(_minPowerupSpawnRate, _maxPowerupSpawnRate);
                else
                    time = _spawnPowerupRate;

                yield return new WaitForSeconds(time);
            }
        }
        
        
        

    }

   
    IEnumerator SpawnAsteroids_WaveRoutine(Wave wave)
    {
        //Reset the Asteroid Event chances
        _numChancesFor_0_AsteroidEvents = wave.wave_ASTEROIDS.chances_noAsteroid;
        _numChancesFor_1AsteroidEvent = wave.wave_ASTEROIDS.chances_1Asteroid;
        _numChancesFor_ClusterEvent = wave.wave_ASTEROIDS.chances_AsteroidCluster;
        _numChancesFor_ShowerEvent = wave.wave_ASTEROIDS.chances_AsteroidShower;

        List_IntsForAsteroidEvents.Clear();

        for (int i = 0; i < _numChancesFor_0_AsteroidEvents; i++)
            List_IntsForAsteroidEvents.Add(0);
        for (int i = 0; i < _numChancesFor_1AsteroidEvent; i++)
            List_IntsForAsteroidEvents.Add(1);
        for (int j = 0; j < _numChancesFor_ClusterEvent; j++)
            List_IntsForAsteroidEvents.Add(2);
        for (int k = 0; k < _numChancesFor_ShowerEvent; k++)
            List_IntsForAsteroidEvents.Add(3);

        float delay = wave.wave_ASTEROIDS.spawnRate;
        _spawnAsteroids = true; //  This is for shared Coroutine (non-wave)
        StartCoroutine(SpawnAsteroidsRoutine(delay));
        yield return null;
       /* while (_isPlayerDead == false)
        {
            yield return new WaitForSeconds(delay); 
            StartCoroutine(SpawnAsteroidsRoutine(delay));
            
        }*/
           
    }

    IEnumerator SpawnAsteroidsRoutine(float delay)
    {
        if (_isPlayerDead == false && _spawnAsteroids)
        {
            if (_useWaveSpawning == false && _hasStartedAsteroids == false)
            {
                if (_hasStartedAsteroids == false)
                {
                    yield return new WaitForSeconds(_startDelaySpawnAsteroids);
                    _hasStartedAsteroids = true;
                }
            }
            yield return new WaitForSeconds(delay);
            int randomIndex = GetRandomIndexOfAsteroidEvent();

            
            switch (randomIndex)
            {
                case 0:
                    //  Skip Spawning of Asteroids.
                    StartCoroutine(SpawnAsteroidsRoutine(delay));    
                    break;
                case 1:
                    SpawnSingleAsteroidEvent(delay);
                    break;
                case 2:
                    StartCoroutine(SpawnAsteroidClusterEvent(delay));
                    break;
                case 3:
                    StartCoroutine(SpawnAsteroidShowerEvent(delay));
                    break;
                case 4:
                    //StartCoroutine(SpawnAsteroidFieldEvent());
                    break;
                default:
                    Debug.Log("SpawnManager: SpawnAsteroidsRoutine() Tried to spawn Asteroid event, but int was outside bounds: >>> " + randomIndex);
                    break;
            }
        }
    }

    void SpawnSingleAsteroidEvent(float delay)
    {
        _asteroidState = AsteroidState.Asteroid_Single;
        _lastAsteroidState = _asteroidState;
        
        Vector2 startLoc = new Vector2(Random.Range(_xPosSpawnMin, _xPosSpawnMax), 9.0f);
        Vector2 targetLoc = new Vector2(Random.Range(_xPosSpawnMin, _xPosSpawnMax), -30.0f);
        GameObject newAsteroid = Instantiate(_asteroidShell, startLoc, Quaternion.identity);
        AsteroidShell asteroidShell = newAsteroid.GetComponent<AsteroidShell>();
        asteroidShell.speed = Random.Range(_minSpeed_Asteroids, _maxSpeed_Asteroids);
        asteroidShell.targetLoc = targetLoc;
        float randomScale = Random.Range(_minScale_Asteroids, _maxScale_Asteroids);
        newAsteroid.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        _countAsteroidsSpawned++;

        _asteroidState = AsteroidState.NoState;
        StartCoroutine(SpawnAsteroidsRoutine(delay));    //  Repeat Spawning of Asteroids.
    }
    IEnumerator SpawnAsteroidClusterEvent(float delay)
    {
        _asteroidState = AsteroidState.Asteroid_Cluster;
        _lastAsteroidState = _asteroidState;
        
        //  Play Asteroid Event SFX 1

        int randomInt = Random.Range(0, List_AsteroidEvent_AudioClips.Count);
        var eventClip = List_AsteroidEvent_AudioClips[randomInt];
        _event_AudioSource.clip = eventClip;
        _event_AudioSource.Play();
        
        //  DIsplay Red Warning Zone based on location
        
         if(_uiManager != null)
            _uiManager.FlashAsteroidWarning(_asteroidState);

        //  Create an initial Start/Target Locations (then slightly adjust each other spawn)
        Vector2 startLoc = new Vector2(Random.Range(_xPosSpawnMin, _xPosSpawnMax), 9.0f);
        Vector2 targetLoc = new Vector2(Random.Range(_xPosSpawnMin, _xPosSpawnMax), -30.0f);

        //  Use StartLoc to determine which Warning Lights to turn on
        float startZone_X = startLoc.x;
        if (startZone_X < -5.0f)
            _ast_WarningLight_Left.SetActive(true);
        if (startZone_X > -8.0 && startZone_X < 2.0f)
            _ast_WarningLight_TopLeft.SetActive(true);
        if (startZone_X > -2.0 && startZone_X < 8.0f)
            _ast_WarningLight_TopRight.SetActive(true);
        if (startZone_X > 5.0f)
            _ast_WarningLight_Right.SetActive(true);

        float waitForWarning = 2.5f;
        float elapsedTime = 0.0f;
        while(elapsedTime < waitForWarning)
        {
            //Flash Warning lights and message
            yield return null;
            elapsedTime += Time.deltaTime;

        }

        int numToSpawn = Random.Range(_minNumInCluster, _maxNumInCluster);

       

        int numSpawned = 0;

        while (numSpawned < numToSpawn)
        {
            
            
            float shift_X = Random.Range(-1.0f, 1.0f);
            float shift_Y = Random.Range(-1.0f, 1.0f);
            
            float marginError_X = Random.Range(-0.1f, 0.1f);    //only use for targetLoc
            float marginError_Y = Random.Range(-0.1f, 0.1f);    //only use for targetLoc

            Vector2 newStartLoc = new Vector2(startLoc.x + shift_X, startLoc.y + shift_Y);
            Vector2 newTargetLoc = new Vector2(targetLoc.x + shift_X + marginError_X, targetLoc.y + shift_Y + marginError_Y);

            GameObject newAsteroid = Instantiate(_asteroidShell, newStartLoc, Quaternion.identity);
            AsteroidShell asteroidShell = newAsteroid.GetComponent<AsteroidShell>();
            asteroidShell.speed = Random.Range(_minSpeed_Asteroids, _maxSpeed_Asteroids);
            asteroidShell.targetLoc = newTargetLoc;
            float randomScale = Random.Range(_minScale_Asteroids, _maxScale_Asteroids);
            newAsteroid.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
            //_countAsteroidsSpawned++;                 //  should I count these too??

            numSpawned++;
            float timeBetweenSpawns = Random.Range(0.15f, 0.25f);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
        //  Then turn off all Warning lights.
        _ast_WarningLight_Left.SetActive(false);
        _ast_WarningLight_TopLeft.SetActive(false);
        _ast_WarningLight_TopRight.SetActive(false);
        _ast_WarningLight_Right.SetActive(false);

        _countAsteroidClusterEvents++;
        _asteroidState = AsteroidState.NoState;
        
        StartCoroutine(SpawnAsteroidsRoutine(delay));    //  Repeat Spawning of Asteroids.
    }
    IEnumerator SpawnAsteroidShowerEvent(float delay)
    {
        _asteroidState = AsteroidState.Asteroid_Shower;
        _lastAsteroidState = _asteroidState;
        
        //  Play Asteroid Event SFX 1

        int randomInt = Random.Range(0, List_AsteroidEvent_AudioClips.Count);
        var eventClip = List_AsteroidEvent_AudioClips[randomInt];
        _event_AudioSource.clip = eventClip;
        _event_AudioSource.Play();



        //  DIsplay Red Warning Zone based on location
        if (_uiManager != null)
            _uiManager.FlashAsteroidWarning(_asteroidState);

        //  Create an initial Start/Target Locations for center asteroid (then slightly adjust each other spawn)
        Vector2 startLoc = new Vector2(Random.Range(_xPosSpawnMin, _xPosSpawnMax), 9.0f);
        Vector2 targetLoc = new Vector2(Random.Range(_xPosSpawnMin, _xPosSpawnMax), -30.0f);

        //  Use StartLoc to determine which Warning Lights to turn on
        float startZone_X = startLoc.x;
        if (startZone_X < -3.0f)
            _ast_WarningLight_Left.SetActive(true);
        if (startZone_X > -8.0 && startZone_X < 4.0f)
            _ast_WarningLight_TopLeft.SetActive(true);
        if (startZone_X > -4.0 && startZone_X < 8.0f)
            _ast_WarningLight_TopRight.SetActive(true);
        if (startZone_X > 3.0f)
            _ast_WarningLight_Right.SetActive(true);


        float waitForWarning = 2.5f;
        float elapsedTime = 0.0f;
        while (elapsedTime < waitForWarning)
        {
            //Flash Warning lights and message
            yield return null;
            elapsedTime += Time.deltaTime;

        }

        
        
        int numToSpawn = Random.Range(_minNumInShower, _maxNumInShower);

        

        int numSpawned = 0;

        while (numSpawned < numToSpawn)
        {


            /*float shift_X = Random.Range(-5.0f, 5.0f);
            float shift_Y = Random.Range(-5.0f, 5.0f);*/
            float shift_X = Random.Range(_minShift_X_Shower, _maxShift_X_Shower);
            float shift_Y = Random.Range(_minShift_X_Shower, _maxShift_X_Shower);

            /*float marginError_X = Random.Range(-0.5f, 0.5f);    //only use for targetLoc
            float marginError_Y = Random.Range(-0.5f, 0.5f);    //only use for targetLoc*/
            float marginError_X = Random.Range(-_marginError_X_Shower, _marginError_X_Shower);    //only use for targetLoc
            float marginError_Y = Random.Range(-_marginError_Y_Shower, _marginError_Y_Shower);    //only use for targetLoc

            Vector2 newStartLoc = new Vector2(startLoc.x + shift_X, startLoc.y + shift_Y);
            Vector2 newTargetLoc = new Vector2(targetLoc.x + shift_X + marginError_X, targetLoc.y + shift_Y + marginError_Y);

            GameObject newAsteroid = Instantiate(_asteroidShell, newStartLoc, Quaternion.identity);
            AsteroidShell asteroidShell = newAsteroid.GetComponent<AsteroidShell>();
            asteroidShell.speed = Random.Range(_minSpeed_Asteroids, _maxSpeed_Asteroids);
            asteroidShell.targetLoc = newTargetLoc;
            float randomScale = Random.Range(_minScale_Asteroids, _maxScale_Asteroids);
            newAsteroid.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
            //_countAsteroidsSpawned++;                 //  should I count these too??

            numSpawned++;
            float timeBetweenSpawns = Random.Range(0.15f, 0.25f);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
        //  Then turn off all Warning lights.
        _ast_WarningLight_Left.SetActive(false);
        _ast_WarningLight_TopLeft.SetActive(false);
        _ast_WarningLight_TopRight.SetActive(false);
        _ast_WarningLight_Right.SetActive(false);

        _countAsteroidShowerEvents++;
        _asteroidState = AsteroidState.NoState;
        
        StartCoroutine(SpawnAsteroidsRoutine(delay));    //  Repeat Spawning of Asteroids.
    }

    IEnumerator SpawnAsteroidFieldEvent(float delay)    //  TBD
    {
        yield return null;
    }


    IEnumerator StartBossBattle(Wave wave)
    {
        yield return null;








    }
    


    int GetRandomIndexOfAsteroidEvent()
    {
        
        if (_useWaveSpawning)
        {
            //The Ints were already adjusted for Wave Spawning
        }
        else
        {
            bool isUsingListForInts = false;
            bool useInt_ForClusters = false;
            bool useInt_ForShowers = false;


            if (_canUseAsteroidClusters)
            {

                if (_countAsteroidsSpawned > _numAsteroids_BeforeClustersAllowed)
                {
                    if (_countAsteroidClusterEvents < 1)
                        return 1;   //  A cluster event will always occur on that first time.
                    else
                    {
                        isUsingListForInts = true;
                        useInt_ForClusters = true;
                    }
                }

            }
            if (_canUseAsteroidShowers)
            {
                if (_countAsteroidsSpawned > _numAsteroids_BeforeShowersAllowed)
                {
                    if (_countAsteroidShowerEvents < 1)
                        return 2;   //  A Shower event will always occur on that first time.
                    else
                    {
                        isUsingListForInts = true;
                        useInt_ForShowers = true;
                    }
                }

            }

            if (isUsingListForInts)
            {
                List_IntsForAsteroidEvents.Clear();
                for (int i = 0; i < _numChancesFor_0_AsteroidEvents; i++)
                {
                    List_IntsForAsteroidEvents.Add(0);
                }
                for (int i = 0; i < _numChancesFor_1AsteroidEvent; i++)
                {
                    List_IntsForAsteroidEvents.Add(1);
                }
                if (useInt_ForClusters)
                {
                    for (int j = 0; j < _numChancesFor_ClusterEvent; j++)
                        List_IntsForAsteroidEvents.Add(2);
                }
                if (useInt_ForShowers)
                {
                    for (int k = 0; k < _numChancesFor_ShowerEvent; k++)
                        List_IntsForAsteroidEvents.Add(3);
                }
                /*var ranInt = Random.Range(0, List_IntsForAsteroidEvents.Count);
                randomInt = List_IntsForAsteroidEvents[ranInt];*/
            }

            //return randomInt;
        }

        int ranInt = Random.Range(0, List_IntsForAsteroidEvents.Count);
        int randomIndex_AsteroidEvent = List_IntsForAsteroidEvents[ranInt];
        return randomIndex_AsteroidEvent;

    }


    public void OnPlayerDeath()
    {
        _isPlayerDead = true;
    }

    public void ReduceEnemyCount(Transform trans)
    {
        _countEnemies--;
        _waveEnemiesKilled++;
        RemoveEnemyTransformFromList(trans);
        if(_countEnemies <= 0)
        {
            _countEnemies = 0;
            /*if (_useWaveSpawning && _waveSystem != null)
                _waveSystem.StartNextWave();*/
        }
        _remainingWaveEnemies = _uiManager.AdjustEnemyThreat();

    }

    public AsteroidState GetAsteroidState()
    {
        return _asteroidState;
    }
    public bool IsUsingAsteroidToStart()
    {
        return _useAsteroidToStart;
    }
    void ReduceCorruptionTimes()
    {
        if (_areCorruptionsInPlay)
        {
            if (_CRPT_BurstFire)
                if(_timeLeft_BurstFire > 0)
                {
                    _timeLeft_BurstFire -= Time.deltaTime;
                    if (_timeLeft_BurstFire <= 0.0f)
                    {
                        _timeLeft_BurstFire = 0.0f;
                        _CorruptionsInPlay_byInt.RemoveAll(x => x == 0);
                        _CRPT_BurstFire = false;
                    }
                }
            if (_CRPT_EnemySpeedBoost)
                if (_timeLeft_EnemySpeedBoost > 0)
                {
                    _timeLeft_EnemySpeedBoost -= Time.deltaTime;
                    if (_timeLeft_EnemySpeedBoost <= 0.0f)
                    {
                        _timeLeft_EnemySpeedBoost = 0.0f;
                        _CorruptionsInPlay_byInt.RemoveAll(x => x == 1);
                        _CRPT_EnemySpeedBoost = false;
                    }
                }
            if (_CRPT_EnemyShields)
                if (_timeLeft_EnemyShields > 0)
                {
                    _timeLeft_EnemyShields -= Time.deltaTime;
                    if (_timeLeft_EnemyShields <= 0.0f)
                    {
                        _timeLeft_EnemyShields = 0.0f;
                        _CorruptionsInPlay_byInt.RemoveAll(x => x == 2);
                        _CRPT_EnemyShields = false;
                    }
                }
            //Add Others when ready...


                    

        }
    }
    //0 = EN_BurstFire  //1 = EN_Speed  //2 = EN_Shields   //3 = ____  //4 = EN_Invulerna   //5 = _____
    public void AddEnemyCorruption(int indexOfCRPT, float durationAdded)
    {
        //If multiple negative powerups are gained.
        //  ...Consider adding elevated situations
        //  ...Permanent negative effect?
        //
        if (_CorruptionsInPlay_byInt.Contains(indexOfCRPT) == false)
            _CorruptionsInPlay_byInt.Add(indexOfCRPT);


        switch (indexOfCRPT)
        {
            case 0:
                _CRPT_BurstFire = true;
                _timeLeft_BurstFire += durationAdded;
                break;
            case 1:
                _CRPT_EnemySpeedBoost = true;
                _timeLeft_EnemySpeedBoost += durationAdded;
                break;
            case 2:
                _CRPT_EnemyShields = true;
                _timeLeft_EnemyShields += durationAdded;
                break;
            default:
                break;

        }
        if(EnemyTransforms != null && EnemyTransforms.Count > 0)
        {
            for (int i = 0; i < EnemyTransforms.Count; i++)
            {
                var en_Trans = EnemyTransforms[i];
                Enemy enemy = en_Trans.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.GainCorruption(indexOfCRPT, durationAdded);   
            }
        }
        

    }

}
