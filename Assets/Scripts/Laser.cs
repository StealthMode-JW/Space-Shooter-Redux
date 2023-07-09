using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class Laser : MonoBehaviour
{
    public static int nextLaserID = 0;
    [SerializeField] string laserID;
    public string myCreatorID;
    
    [SerializeField]
    float _speed = 8.0f;

    Player _player;

    /*[SerializeField]
    float _timeTilCanHitPlayer = 0.01f;
    
    [SerializeField]
    bool _canHitPlayer = false; //want to make sure his own fire doesn't hit him.
    [SerializeField]
    float _timeTilEnableCollider = 0.01f;*/
    BoxCollider2D _boxCollider;
    //bool _canCollide = false;


    private void Awake()
    {
        int ID = nextLaserID++;
        laserID = "LA" + ID + "_" + System.Guid.NewGuid().ToString();
    }

    private void Start()
    {
        //This is for transformation 
        //
        //StartCoroutine(SetupLaser()); 
        
    }
    private void OnEnable()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        //_boxCollider.enabled = false;
        _boxCollider.enabled = true;
    }

    public string GetMyLaserID()
    {
        return laserID;
    }
    public string GetMyCreatorsID()
    {
        return myCreatorID;
    }


    IEnumerator SetupLaser()
    {
        
        if (Transformation.IsUsingTransformation())
        {
            var spriteRend = GetComponent<SpriteRenderer>();
            var capRend = GetComponentInChildren<MeshRenderer>();

            var gameMode = Transformation.GetGameMode();
            if (gameMode == Transformation.GameMode.Mode_2D)
            {
                spriteRend.enabled = true;
                capRend.enabled = false;

            }
            else if(gameMode == Transformation.GameMode.Mode_3D)
            {
                spriteRend.enabled = false;
                capRend.enabled = true;
            }
        }
            
            yield return null;
    }

    void Update()
    {
        if (!GameManager.isGamePaused)
        {
            /*if (_canHitPlayer == false)
            {
                _timeTilCanHitPlayer -= Time.deltaTime;
                if (_timeTilCanHitPlayer <= 0)
                    _canHitPlayer = true;
            }
            if(_canCollide == false)
            {
                _timeTilEnableCollider -= Time.deltaTime;
                if (_timeTilEnableCollider <= 0) 
                {
                    _canCollide = true;
                    _boxCollider.enabled = true;
                }

                    
            }*/

            transform.Translate(Vector3.up * _speed * Time.deltaTime);

            if (transform.position.y > 6.0f || transform.position.y < -6.0f || transform.position.x > 10.0f || transform.position.x < -10.0f)
            {
                if (transform.parent != null)
                {
                    Debug.Log("parent object = " + transform.parent.gameObject.name);


                    if (transform.parent.tag != "Container" && transform.parent != null)
                        Destroy(transform.parent.gameObject);
                }
                Destroy(this.gameObject);

            }
        }
        
            
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.tag == "Player" && _canHitPlayer)
        if (other.tag == "Player")
        {
            _player = other.transform.GetComponent<Player>();
            if (_player != null)
            {
                string playerID = _player.GetMyPlayerID();
                if (playerID == myCreatorID)
                    return;
                else
                    _player.Damage(gameObject, 1);

                Destroy(this.gameObject);
            }
        }
    }
}
