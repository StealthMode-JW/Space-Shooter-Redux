using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    float minRotateSpeed = 2.0f;
    [SerializeField]
    float maxRotateSpeed = 20.0f;
    float randomRotateSpeed = 0.0f;

    [SerializeField]
    bool _isStartingAsteroid = false; //  Only set 1st Asteroid true & SpawnManager must have (useAsteroidToStart) true
    [SerializeField]
    bool _didStart = false;

    float reverseRotation = 1.0f;

    [SerializeField]
    GameObject _explosionPrefab;
    [SerializeField]
    Transform _transPlayer;

    [SerializeField]
    bool _didSplitAlready = false;

    [SerializeField]
    CameraShake _cameraShake;
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (_isStartingAsteroid)
            SetupStartingAsteroid();

        int ran0or1 = Random.Range(0, 2);
        if(ran0or1 == 0)
            reverseRotation = -1.0f;
        else
            reverseRotation = 1.0f;

        randomRotateSpeed = Random.Range(minRotateSpeed, maxRotateSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isGamePaused)
        {
            transform.Rotate(Vector3.forward * randomRotateSpeed * reverseRotation * Time.deltaTime);
        }
       

        
    }

    void SetupStartingAsteroid()
    {
        transform.position = new Vector3(0, 4, 0);
        
        
    }

    void NotifyToStartGame()
    {
        if(_didStart == false)
        {
            SpawnManager spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
            if (spawnManager != null)
            {
                if (spawnManager.IsUsingAsteroidToStart())
                {
                    spawnManager.StartSpawning();
                    _didStart = true;
                }
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if(_isStartingAsteroid)
                NotifyToStartGame();
            DestroySelf();
        }
        else if(other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
                player.Damage(gameObject, 1);

            DestroySelf();
        }
        else if(other.tag == "Enemy")
        {
            //Enemy will destroy itself
            DestroySelf();
        }
        else if (other.tag == "Missile")
        {
            //Should I do anything here?
            DestroySelf();
        }
        else if(other.tag == "Asteroid")
        {
            //Should I do anything here? Split them off?
        }
        else if (other.tag == "Explosion")
        {
            //Should I do anything here?
            DestroySelf();
        }
    }

    //  When Asteroid explodes...
    void DestroySelf()
    {
        //  Only grab components if being destroyed
        if (_cameraShake == null)
            _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        if (_transPlayer == null)
            _transPlayer = GameObject.Find("Player").transform;
        if(_transPlayer != null && _cameraShake != null)
        {
            //  duration and magnitude increase the larger the asteroid
            float dur = transform.localScale.x * 0.25f;
            float mag = dur;
            
            Vector3 playerPos = _transPlayer.position;
            _cameraShake.StartDynamicShake(dur, mag, playerPos, transform.position);
        }
        //  Explosion sizes are relative to size of Asteroid.
        GameObject newExplosion = 
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Vector3 myScale = transform.localScale;
        newExplosion.transform.localScale = myScale;
        Destroy(gameObject, 0.1f);
    }
}
