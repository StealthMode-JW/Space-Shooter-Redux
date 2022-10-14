using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 3.5f;

    void Update()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(hor, ver, 0);

        transform.Translate(direction * speed * Time.deltaTime);






        //transform.Translate(direction * Time.deltaTime);







        //CalculateMovement();
    }


    //[SerializeField]
    //float _speed = 3.5f;




    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
    }
    
    void OnGUI()
    {
        
    }
    
    

    void CalculateMovement()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(hor, ver, 0);
        //transform.Translate(direction * _speed * Time.deltaTime);

        if (transform.position.y > 0)
            transform.position = new Vector3(transform.position.x, 0, 0);
        else if (transform.position.y < -3.8f)
            transform.position = new Vector3(transform.position.x, -3.8f, 0);

        if (transform.position.x > 11)
            transform.position = new Vector3(-11, transform.position.y, 0);
        else if (transform.position.x < -11)
            transform.position = new Vector3(11, transform.position.y, 0);
    }


}
