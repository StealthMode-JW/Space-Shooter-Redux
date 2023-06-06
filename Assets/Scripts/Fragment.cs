using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Fragment : MonoBehaviour
{

    [SerializeField]
    float _dir_X = 0f;
    [SerializeField]
    float _dir_Y = 0f;
    [SerializeField]
    float _dir_Z = 0f;
    [SerializeField]
    float _travelSpeed = 4.0f;

    [SerializeField]
    SpriteRenderer _spriteRenderer;
    [SerializeField]
    ParticleSystem _particleSystem;
    [SerializeField]
    bool _reverseRotate = false;


    private Coroutine _fadeCoroutine; 
    [SerializeField]
    CanvasGroup _canvasGroup;

    [SerializeField]
    float _rotateSpeed = 4.0f;


    [SerializeField]
    float lifetime = 4.0f;




    // Start is called before the first frame update
    void Start()
    {
        if(_spriteRenderer == null)
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if(_particleSystem == null)
            _particleSystem = GetComponentInChildren<ParticleSystem>();
        if(_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        StartCoroutine(FadeOut());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = new Vector3(_dir_X, _dir_Y, _dir_Z);
        transform.Translate(direction * _travelSpeed * Time.deltaTime);

        if (_spriteRenderer != null)
        {
            Vector3 rot = _reverseRotate ? Vector3.back : Vector3.forward;
            _spriteRenderer.transform.Rotate(rot * _rotateSpeed * Time.deltaTime);
        }

        Destroy(gameObject, lifetime);
        
        /*Vector3 direction = new Vector3(_dir_X, _dir_Y, _dir_Z);
        transform.Translate(direction * _travelSpeed * Time.deltaTime);

        if(_spriteRenderer != null)
        {
            Transform trans = _spriteRenderer.transform;

        }

        if (_spriteRenderer != null)
        {
            Transform trans = _spriteRenderer.transform;
            //trans.Rotate(_rotateVector * _rotateSpeed * Time.deltaTime);
            Vector3 rot = Vector3.forward;
            if (_reverseRotate)
                rot = Vector3.back;
            trans.Rotate(rot * _rotateSpeed * Time.deltaTime);

        }

        *//*if (!GameManager.isGamePaused)
        {
            transform.Rotate(Vector3.forward * randomRotateSpeed * reverseRotation * Time.deltaTime);
        }*//*

        // Destroy the fragment after a certain time
        Destroy(gameObject, lifetime);*/

    }
    public Vector3 GetDefault_Direction()
    {
        Vector3 defaultDirection = new Vector3 (_dir_X, _dir_Y, _dir_Z);
        return defaultDirection;
    }
    public float GetDefault_DirectionSpeed()
    {
        float dirSpeed = _travelSpeed;
        return _travelSpeed;
    }
    public float GetDefault_RotationSpeed()
    {
        float rotSpeed = _rotateSpeed;
        return rotSpeed;
    }



    //  SET DIRECTION AND ROTATION FIRST...

    public void SetDirectionRotation(float dirAngle, float dirZ, float dirSpeed,
        float rotSpeed, bool reverseRotate)
    {
        Vector3 dir = Quaternion.Euler(0f, 0f, dirAngle) * Vector3.right;
        _dir_X = dir.x;
        _dir_Y = dir.y;
        _dir_Z = dirZ;
        _travelSpeed = dirSpeed;
        _rotateSpeed = rotSpeed;
        _reverseRotate = reverseRotate;
    }
    /*public void SetDirectionRotation(float dirX, float dirY, float dirZ, float dirSpeed, float rotSpeed, bool reverseRotate)
    {
        _dir_X = dirX;
        _dir_Y = dirY;
        _dir_Z = dirZ;
        _travelSpeed = dirSpeed;

        if(reverseRotate)


        _rotateSpeed = rotSpeed;
    }*/
    //  ...THEN RANDOMIZE DIRECTION AND ROTATION FROM ORIGINAL SETTING
    public void RandomizeDirectionRotation(float dirXMargin, float dirYMargin, float dirZMargin, 
        float travelSpeedMargin, float rotationMargin, float rotationSpeedMargin)
    {
        // Randomize direction and speed
        _dir_X = Random.Range(-dirXMargin, dirXMargin);
        _dir_Y = Random.Range(-dirYMargin, dirYMargin);
        _dir_Z = Random.Range(-dirZMargin, dirZMargin);
        _travelSpeed = Random.Range(-travelSpeedMargin, travelSpeedMargin);

        // Randomize rotation
        _rotateSpeed = Random.Range(-rotationSpeedMargin, rotationSpeedMargin);
    }

    IEnumerator FadeOut()
    {
        if(_particleSystem != null)
        {
            float percentChance = Random.Range(0f, 100f);
            if (percentChance > 66f)
                _particleSystem.Play();
        }
        

        
        float duration = Random.Range(2.5f, 5.0f);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            _canvasGroup.alpha = 1.0f - (elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _canvasGroup.alpha = 0.0f;
        //Destroy(gameObject, 0.1f);
    }

    void OnDisable()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
    }

}
