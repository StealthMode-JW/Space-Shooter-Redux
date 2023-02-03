using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                player.Damage();

            DestroySelf();
        }
        else if(other.tag == "Enemy")
        {
            //Enemy will destroy itself
            Debug.Log("ASTEROID: OnTriggerEnter2D() Hit Enemy ");
            DestroySelf();
        }
        else if(other.tag == "Asteroid")
        {
            //Should I do anything here?
        }
    }
    void DestroySelf()
    {
        GameObject newExplosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Vector3 myScale = transform.localScale;
        newExplosion.transform.localScale = myScale;
        Destroy(gameObject, 0.1f);
    }

}
