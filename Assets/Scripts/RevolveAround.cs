using Palmmedia.ReportGenerator.Core;
using UnityEngine;

public class RevolveAround : MonoBehaviour
{
    public float mainSpeed = 100f;  //  Speed of Main Orbitor;

    public float baseSpeed = 6f;
    public float min_multiplySpeedBy_Factor = 1.0f;
    public float max_multiplySpeedBy_Factor = 25.0f;
    [Space(10)]
    [SerializeField,ReadOnly] float finalSpeed;
    [Space(20)]
    public float min_MultiplyScaleBy_Factor = 0.5f;
    public float max_MultiplyScaleBy_Factor = 1.5f;
    [Space(10)]
    [SerializeField, ReadOnly] Vector3 finalScale;
    [Space(20)]

    public bool useRandomStartRange = false;
    public float minPos_X = -0.5f;
    public float maxPos_X = 2.5f;
    public float minPos_Y = -0.5f;
    public float maxPos_Y = 2.5f;
    public float minPos_Z = 0f;
    public float maxPos_Z = 0f;

    public float distance;//distance from center
    public float Rotation;//rotation angle
    public float currentTime = 0f;//Time start when Instantiated
    public float lastTime;

    [SerializeField,ReadOnly] Vector3 initialPosition;

    public float angleOffset = 0f; // This will be our changing angle offset
    public float offsetChange = 1.0f; // How much we change the offset each revolution
    float revolutions = 0f;


    private void Start()
    {
        if (useRandomStartRange)    //  For Orbiters
        {
            //  Set Random Position
            Vector3 randomStartPos = new Vector3(
                Random.Range(minPos_X, maxPos_X),
                Random.Range(minPos_Y, maxPos_Y),
                Random.Range(minPos_Z, maxPos_Z)
            );
            transform.localPosition = randomStartPos;

            //  Set Random Rotation and distance
            Rotation = Random.Range(-2.0f, 2.0f);
            distance = Random.Range(0.5f, 1.5f);
           
            //  Set random Speed
            float speedFactor = Random.Range(min_multiplySpeedBy_Factor, max_multiplySpeedBy_Factor);
            finalSpeed = baseSpeed * speedFactor;

            //  Set random Scale
            float scaleFactor = Random.Range(min_MultiplyScaleBy_Factor, max_MultiplyScaleBy_Factor);
            Vector3 currScale = transform.localScale;
            finalScale = currScale * scaleFactor;
            transform.localScale = finalScale;
        }


        initialPosition = transform.localPosition; //ReadOnly
        
    }

    void Update()
    {
        //  For Orbiters
        if (useRandomStartRange)    
        {
            //Timer
            currentTime += Time.deltaTime;

            float completedRevolutions = currentTime * finalSpeed / (2 * Mathf.PI);
            if (completedRevolutions > revolutions)
            {
                // We've completed a full revolution, so increase the angle offset
                angleOffset += offsetChange * (completedRevolutions - revolutions);
                revolutions = completedRevolutions;
            }

            var X = distance * Mathf.Cos(currentTime * finalSpeed + angleOffset);//X
            var Y = distance * Mathf.Sin(currentTime * finalSpeed + angleOffset);//Y
            var R = Rotation * Mathf.Cos(currentTime * finalSpeed);//Rotation
            transform.position = transform.parent.position + new Vector3(X, R, Y);// RotateAround parent
        }
        
        //  For Main Orbiter (Danger Symbol)
        else
        {
            // Makes object revolve around its parent's position in counter-clockwise direction.
            transform.RotateAround(transform.parent.position, Vector3.down, mainSpeed * Time.deltaTime);
        }
    }


    public void AdjustMyFinalSpeed(float newSpeed)
    {
        finalSpeed = newSpeed;
    }
    float GetMyFinalSpeed()
    {
        return finalSpeed;
    }
}
//Basic Orbit:
//this is simple way that you can orbitting objects around other object
/*transform.Rotate(transform.up * speed * Time.deltaTime);
transform.RotateAround(transform.parent.position, Vector3.up, speed * Time.deltaTime);*/