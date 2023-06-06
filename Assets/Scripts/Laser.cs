using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class Laser : MonoBehaviour
{
    [SerializeField]
    float _speed = 8.0f;

    Player _player;

    [SerializeField]
    float _timeTilCanHitPlayer = 0.5f;
    [SerializeField]
    bool _canHitPlayer = false; //want to make sure his own fire doesn't hit him.

    private void Start()
    {
        
        //StartCoroutine(SetupLaser());
        
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
            if (_canHitPlayer == false)
            {
                _timeTilCanHitPlayer -= Time.deltaTime;
                if (_timeTilCanHitPlayer <= 0)
                    _canHitPlayer = true;
            }


            transform.Translate(Vector3.up * _speed * Time.deltaTime);

            if (transform.position.y > 6.0f)


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
        if (other.tag == "Player" && _canHitPlayer)
        {
            _player = other.transform.GetComponent<Player>();
            if (_player != null)
                _player.Damage(gameObject, 1);

            Destroy(this.gameObject);
        }
        
    }
}
