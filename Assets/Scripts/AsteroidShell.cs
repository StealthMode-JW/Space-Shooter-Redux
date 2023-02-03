using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidShell : MonoBehaviour
{

   
    public Vector3 startLoc;
    
    public Vector3 targetLoc = Vector3.zero;
    [SerializeField]
    AudioSource _entry_AudioSource;
    [SerializeField]
    AudioSource _traveling_AudioSource;
    
    public List<AudioClip> List_AsteroidEntry_AudioClips = new List<AudioClip> ();
    public List<AudioClip> List_AsteroidTraveling_AudioClips = new List<AudioClip> ();

    public float speed = 5.0f;
    [SerializeField]
    bool isUsingVariableSpeed = true;
    /*[SerializeField]          
    float minSpeed = 4.0f;      //right now speed is informed by SpawnManager
    [SerializeField]
    float maxSpeed = 6.0f;*/

    Transform childAsteroid;
    
    // Start is called before the first frame update
    void Start()
    {
        startLoc = transform.localPosition;
        childAsteroid = transform.GetChild(0);
        
        PlaySoundFX();

    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.localPosition = Vector3.MoveTowards(transform.position, targetLoc, step);

        if(childAsteroid == null)
            Destroy(gameObject);

        if(transform.position.y <= -20.0f)
        {
            Destroy(gameObject, 5.0f);
        }

       
    }

    void PlaySoundFX()
    {
        if(_entry_AudioSource != null && List_AsteroidEntry_AudioClips != null)
        {
            if(List_AsteroidEntry_AudioClips.Count > 0)
            {
                int ranIndex = Random.Range(0, List_AsteroidEntry_AudioClips.Count);
                var entryClip = List_AsteroidEntry_AudioClips[ranIndex];
                _entry_AudioSource.clip = entryClip;
                _entry_AudioSource.Play();
            }
        }
        if (_traveling_AudioSource != null && List_AsteroidTraveling_AudioClips != null)
        {
            if (List_AsteroidTraveling_AudioClips.Count > 0)
            {
                int ranIndex = Random.Range(0, List_AsteroidTraveling_AudioClips.Count);
                var entryClip = List_AsteroidTraveling_AudioClips[ranIndex];
                _traveling_AudioSource.clip = entryClip;
                _traveling_AudioSource.Play();
            }
        }
    }
            
}
