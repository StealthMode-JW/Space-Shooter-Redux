using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Wave
{
    public int waveNum;
    [Space(10)]
    public bool mainBOSS;
    [Space(10)]
    public Wave_Enemies wave_ENEMIES;
    [Space(10)]
    public Wave_Asteroids wave_ASTEROIDS;
    [Space(10)]
    public Wave_Powerups wave_POWERUPS;
}

[System.Serializable]
public class Wave_Enemies
{
    public int numEnemies;
    [Space(5)]
    public float spawnRate;
    [Space(5)]
    public float speedBoost;
    public float fireRateBoost;
}

[System.Serializable]
public class Wave_Asteroids
{
    public float spawnRate;
    [Space(5)]
    public int chances_noAsteroid;
    public int chances_1Asteroid;
    public int chances_AsteroidCluster;
    public int chances_AsteroidShower;
}

[System.Serializable]
public class Wave_Powerups
{
    public float spawnRate;
    [Header("POWERUPS")]
    public int chances_AMMO;
    public int chances_HEALTH;
    public int chances_SPEED;
    public int chances_SHIELD;
    public int chances_TRIPLESHOT;
    public int chances_HOMING;

    [Header("CORRUPTIONS")]
    public int chances_AMMO_Corrupt;
    public int chances_HEALTH_Corrupt;
    public int chances_SPEED_Corrupt;
    public int chances_SHIELD_Corrupt;
    public int chances_TRIPLESHOT_Corrupt;
    public int chances_HOMING_Corrupt;
}

public class WaveSystem : MonoBehaviour
{
    [SerializeField] List<Wave> waves = new List<Wave>();

    [SerializeField] SpawnManager _spawnManager;
    [Space(10)]
    [SerializeField, ReadOnly] int _currentWave = 0;
    [Space(5)]
    [SerializeField, ReadOnly] int _maxEnemiesThisWave = 0;
    [SerializeField, ReadOnly] int _remainingEnemies = 0;

    void Start()
    {
        StartNextWave();
    }

    //  Called by SpawnManager when previous wave ends.
    public void StartNextWave()
    {
        _currentWave++;
        int index = _currentWave - 1;
        if (waves.Count > index)
        {
            Wave nextWave = waves[index];

            if (_spawnManager != null)
                _spawnManager.StartWaveSpawning(nextWave);
        }
    }

    public int GetCurrentWaveNumber()
    {
        return _currentWave;
    }
    

    


}
