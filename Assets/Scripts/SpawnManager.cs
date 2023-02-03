using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    UIManager _uiManager;
    
    [SerializeField]
    GameObject _enemyPrefab;
    [SerializeField]
    GameObject _enemyContainer;
    [SerializeField] 
    float _startEnemyDelay = 2.0f;
    [SerializeField] 
    float _spawnEnemyRate = 5.0f;

    [SerializeField]
    bool _spawnPowerups = true;
    [SerializeField]
    int _countPowerupsSpawned = 0;
   
    [SerializeField]
    bool _randomizePowerups = true;
    [SerializeField]
    bool _canBeSameAsLastPowerup = true;



    [SerializeField]
    List<GameObject> _List_Powerups = new List<GameObject>();



    [SerializeField]
    float _startPowerupDelay = 10f;
    [SerializeField]
    bool _randomizePowerupSpawnRate = true;
    [SerializeField]
    float _minPowerupSpawnRate = 3.0f;
    [SerializeField]
    float _maxPowerupSpawnRate = 7.0f;
    [SerializeField]
    float _spawnPowerupRate = 7.0f;

    [SerializeField] 
    float _yPosAtSpawn = 7.0f;

    [SerializeField] 
    float _xPosSpawnMin = -9.0f;
    [SerializeField] 
    float _xPosSpawnMax = 9.0f;

    [SerializeField] 
    int _maxEnemies = 10;
    [SerializeField] 
    int _countEnemies = 0;
    bool _isPlayerDead = false;


    [Header("ASTEROIDS")]
    [SerializeField]
    bool _useAsteroidToStart = true;
    [Space(10)]
    [SerializeField]
    bool _doSpawnAsteroids = true;
    [SerializeField]
    GameObject _asteroidShell;
    bool _hasStartedAsteroids = false;
    [Space(5)]
    [SerializeField]
    float _startDelaySpawnAsteroids = 15.0f;
    [Space(10)]
    [SerializeField]
    float minXStart = -20.0f;
    [SerializeField]
    float maxXStart = 20.0f;
    
    [Space(5)]
    [SerializeField]
    float _minSpeed_Asteroids = 4f;
    [SerializeField]
    float _maxSpeed_Asteroids = 8f;
    [Space(5)]
    [SerializeField]
    float _minScale_Asteroids = 0.25f;
    [SerializeField]
    float _maxScale_Asteroids = 0.75f;

    [Space(10)]
    [SerializeField]
    bool _doLimitAsteroidEvents = true;
    [SerializeField]
    [Range(1, 100)]
    int _numChancesFor_1AsteroidEvent = 50;
    [Space(5)]
    [SerializeField]
    bool _canUseAsteroidClusters = true;
    [SerializeField]
    int _numAsteroids_BeforeClustersAllowed = 10;
    [SerializeField]
    [Range(1, 100)]
    int _numChancesFor_ClusterEvent = 20;
    [Space(5)]
    [SerializeField]
    bool _canUseAsteroidShowers = true;
    [SerializeField]
    int _numAsteroids_BeforeShowersAllowed = 50;
    [SerializeField]
    [Range(1, 100)]
    int _numChancesFor_ShowerEvent = 5;

    [Space(10)]
    [SerializeField]
    AudioSource _event_AudioSource;
    public List<AudioClip> List_AsteroidEvent_AudioClips = new List<AudioClip>();       //not set up yet

    [Space(10)]
    [SerializeField]
    GameObject _ast_WarningLight_Left;
    [SerializeField]
    GameObject _ast_WarningLight_TopLeft;
    [SerializeField]
    GameObject _ast_WarningLight_TopRight;
    [SerializeField]
    GameObject _ast_WarningLight_Right;
    [Space(10)]

    [SerializeField]
    int _countAsteroidsSpawned = 0;
    bool _hasAsteroidClusterOccured = false;
    [SerializeField]
    int _countAsteroidClusterEvents = 0;
    bool _hasAsteroidShowerOccured = false;
    [SerializeField]
    int _countAsteroidShowerEvents = 0;
    [SerializeField]
    List<int> List_IntsForAsteroidEvents = new List<int>(); // ASTEROIDS     0 = Single     1 = Cluster     2 = Shower
    [Space(30)]
    [SerializeField]
    int _minNumInCluster = 3;
    [SerializeField]
    int _maxNumInCluster = 8;
    [Space(30)]
    [SerializeField]
    int _minNumInShower = 25;
    [SerializeField]
    int _maxNumInShower = 50;
    [SerializeField]
    float _minShift_X_Shower = -10.0f;
    [SerializeField]
    float _maxShift_X_Shower = 10.0f;
    [SerializeField]
    float _marginError_X_Shower = 1.0f;
    [SerializeField]
    float _marginError_Y_Shower = 1.0f;

    public AsteroidState asteroidState;
    public AsteroidState lastAsteroidState;

    public enum AsteroidState
    {
        NoState,
        Asteroid_Single,
        Asteroid_Cluster,
        Asteroid_Shower,
    };

    void Start()
    {
        asteroidState = AsteroidState.NoState;
        lastAsteroidState = asteroidState;
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
            Debug.LogError("SPAWN MANAGER: Start() UIManager is NULL");

        if (_useAsteroidToStart == false)
            StartSpawning();
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnAsteroidsRoutine(0.0f));
    }

    public bool IsUsingAsteroidToStart()
    {
        return _useAsteroidToStart;
    }


    IEnumerator SpawnEnemyRoutine()
    {
        
        yield return new WaitForSeconds(_startEnemyDelay);
        _startEnemyDelay = 0;
        while(_isPlayerDead == false && _countEnemies < _maxEnemies)
        {
            float randomX = Random.Range(_xPosSpawnMin, _xPosSpawnMax);
            Vector3 spawnPos = new Vector3(randomX, _yPosAtSpawn, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
            newEnemy.transform.SetParent(_enemyContainer.transform);
            _countEnemies++;
            
                
            yield return new WaitForSeconds(_spawnEnemyRate);
        }
    }



  
    IEnumerator SpawnPowerupRoutine()
    {
        //making this an unreachable number
        int lastPowerupID = 1000;

        yield return new WaitForSeconds(_startPowerupDelay);
        while (_spawnPowerups)
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


    IEnumerator SpawnAsteroidsRoutine(float timeToDelayTilNextEvent)
    {
        if (_doSpawnAsteroids)
        {
            
            if(_hasStartedAsteroids == false)
            {
                yield return new WaitForSeconds(_startDelaySpawnAsteroids);
                _hasStartedAsteroids = true;
            }
            yield return new WaitForSeconds(timeToDelayTilNextEvent);

            int randomIndex = GetRandomIndexOfAsteroidEvent();

            
            switch (randomIndex)
            {
                case 0:
                    Debug.Log("SpawnManager: SpawnAsteroidsRoutine() SINGLE ASTEROID Event   due to int >>>> " + randomIndex);
                    SpawnSingleAsteroidEvent();
                    break;
                case 1:
                    Debug.Log("SpawnManager: SpawnAsteroidsRoutine()  ASTEROID CLUSTER Event   due to int >>>> " + randomIndex);
                    StartCoroutine(SpawnAsteroidClusterEvent());
                    break;
                case 2:
                    Debug.Log("SpawnManager: SpawnAsteroidsRoutine() ASTEROID SHOWER Event   due to int >>>> " + randomIndex);
                    StartCoroutine(SpawnAsteroidShowerEvent());
                    break;
                case 3:
                    Debug.Log("SpawnManager: SpawnAsteroidsRoutine() ASTEROID FIELD Event   due to int >>>> " + randomIndex);
                    //StartCoroutine(SpawnAsteroidFieldEvent());
                    break;
                default:
                    Debug.Log("SpawnManager: SpawnAsteroidsRoutine() Tried to spawn Asteroid event, but int was outside bounds: >>> " + randomIndex);
                    break;
            }
        }
    }

    void SpawnSingleAsteroidEvent()
    {
        asteroidState = AsteroidState.Asteroid_Single;
        lastAsteroidState = asteroidState;
        
        Vector2 startLoc = new Vector2(Random.Range(_xPosSpawnMin, _xPosSpawnMax), 9.0f);
        Vector2 targetLoc = new Vector2(Random.Range(_xPosSpawnMin, _xPosSpawnMax), -30.0f);
        GameObject newAsteroid = Instantiate(_asteroidShell, startLoc, Quaternion.identity);
        AsteroidShell asteroidShell = newAsteroid.GetComponent<AsteroidShell>();
        asteroidShell.speed = Random.Range(_minSpeed_Asteroids, _maxSpeed_Asteroids);
        asteroidShell.targetLoc = targetLoc;
        float randomScale = Random.Range(_minScale_Asteroids, _maxScale_Asteroids);
        newAsteroid.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        _countAsteroidsSpawned++;

        asteroidState = AsteroidState.NoState;
        StartCoroutine(SpawnAsteroidsRoutine(5.0f));    //  Repeat Spawning of Asteroids.
    }
    IEnumerator SpawnAsteroidClusterEvent()
    {
        asteroidState = AsteroidState.Asteroid_Cluster;
        lastAsteroidState = asteroidState;
        
        //  Play Asteroid Event SFX 1

        int randomInt = Random.Range(0, List_AsteroidEvent_AudioClips.Count);
        var eventClip = List_AsteroidEvent_AudioClips[randomInt];
        _event_AudioSource.clip = eventClip;
        _event_AudioSource.Play();
        
        //  DIsplay Red Warning Zone based on location
        
         if(_uiManager != null)
            _uiManager.FlashAsteroidWarning(asteroidState);

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
        asteroidState = AsteroidState.NoState;
        StartCoroutine(SpawnAsteroidsRoutine(5.0f));    //  Repeat Spawning of Asteroids.
    }
    IEnumerator SpawnAsteroidShowerEvent()
    {
        asteroidState = AsteroidState.Asteroid_Shower;
        lastAsteroidState = asteroidState;
        
        //  Play Asteroid Event SFX 1

        int randomInt = Random.Range(0, List_AsteroidEvent_AudioClips.Count);
        var eventClip = List_AsteroidEvent_AudioClips[randomInt];
        _event_AudioSource.clip = eventClip;
        _event_AudioSource.Play();



        //  DIsplay Red Warning Zone based on location
        if (_uiManager != null)
            _uiManager.FlashAsteroidWarning(asteroidState);

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
        asteroidState = AsteroidState.NoState;
        StartCoroutine(SpawnAsteroidsRoutine(5.0f));    //  Repeat Spawning of Asteroids.
    }

    IEnumerator SpawnAsteroidFieldEvent()    //  TBD
    {
        yield return null;
    }

    
    

    int GetRandomIndexOfAsteroidEvent()
    {
        int randomInt = 0;      //  single Asteroid by default
        bool isUsingListForInts = false;
        bool useInt_ForClusters = false;
        bool useInt_ForShowers = false;

        if (_canUseAsteroidClusters)
        {
            
            if(_countAsteroidsSpawned > _numAsteroids_BeforeClustersAllowed)
            {
                if(_countAsteroidClusterEvents < 1)
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
                if(_countAsteroidShowerEvents < 1)
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
            for(int i = 0; i < _numChancesFor_1AsteroidEvent; i++)
            {
                List_IntsForAsteroidEvents.Add(0);
            }
            if (useInt_ForClusters)
            {
                for (int j = 0; j < _numChancesFor_ClusterEvent; j++)
                    List_IntsForAsteroidEvents.Add(1);
            }
            if (useInt_ForShowers)
            {
                for (int k = 0; k < _numChancesFor_ShowerEvent; k++)
                    List_IntsForAsteroidEvents.Add(2);
            }
            var ranInt = Random.Range(0, List_IntsForAsteroidEvents.Count);
            randomInt = List_IntsForAsteroidEvents[ranInt];
        }

        return randomInt;
    }


    public void OnPlayerDeath()
    {
        _isPlayerDead = true;
    }

    public void ReduceEnemyCount()
    {
        _countEnemies--;
    }

    public AsteroidState GetAsteroidState()
    {
        AsteroidState _asteroidState = asteroidState;
        return _asteroidState;
    }
}
