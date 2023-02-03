using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Powerup : MonoBehaviour
{
    [SerializeField]
    float _speed = 3.0f;
    
    [SerializeField]    //0 = TripleShot    //1 = Speed     //2 = Shields    //3 = Phasing?
    int _powerupID;

    [SerializeField]
    AudioClip _clip;
    [SerializeField]
    AudioSource _audioSource;

    [SerializeField]
    List <AudioClip> List_PowerupClips = new List<AudioClip>();

    [SerializeField]
    Transform _transCam;
    

    void Start()
    {
        if(_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (!GameManager.isGamePaused)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);

            if (transform.position.y < -7.0f)
                Destroy(gameObject);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if(player != null)
            {
                player.ActivatePowerup(_powerupID);
                if(List_PowerupClips != null && List_PowerupClips.Count > 0)
                {
                    int ranIndex = Random.Range(0, List_PowerupClips.Count);
                    AudioClip clip = List_PowerupClips[ranIndex];
                    _clip = clip;
                }
                AudioSource.PlayClipAtPoint(_clip, _transCam.position);
                //_audioSource.clip = _clip;
                //_audioSource.Play();
            }
            SpriteRenderer rend = GetComponent<SpriteRenderer>();
            rend.enabled = false;
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            collider.enabled = false;
            Destroy(this.gameObject, 3.0f);
        }
    }
}