using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Camera cameraToLookAt;

    private void Start()
    {
        if(cameraToLookAt == null)
            cameraToLookAt = Camera.main;
    }

    void Update()
    {
        transform.LookAt(cameraToLookAt.transform.position);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
