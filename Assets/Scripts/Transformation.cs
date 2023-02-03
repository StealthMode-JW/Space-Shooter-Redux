using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Transformation : MonoBehaviour
{
    public static GameMode s_gameMode;
    public GameMode gameMode;
    
    public SpriteRenderer playerRend;
    public MeshRenderer playerCubeRend;
    [Space(5)]
    public SpriteRenderer bgRend;
    [Space(5)]
    public Transform spawnConTrans;
    public List<SpriteRenderer> enemyRends = new List<SpriteRenderer>();
    public List<MeshRenderer> enemyCubeRends = new List<MeshRenderer>();
    [Space(5)]
    public Transform laserConTrans;
    public List<SpriteRenderer> laserRends = new List<SpriteRenderer>();
    public List<MeshRenderer> laserCapsRends = new List<MeshRenderer>();
    [Space(10)]

    public bool doTransformation;
    public static bool static_doTransformation;

    public float timeTilSwitch = 3.0f;


    public bool doPulse = true;
    public int numPulses = 10;
    public float minAlphaToPulseTo = 0.5f;
    public float timePerPulse = 0.1f;

    public static bool IsUsingTransformation()
    {
        var _isUsingTransformation = static_doTransformation;
        return _isUsingTransformation;
    }

    public enum GameMode
    { 
        Mode_3D, 
        Mode_2D
    };

    public static GameMode GetGameMode()
    {
        var _gameMode = s_gameMode;
        return _gameMode;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (doTransformation)
        {
            static_doTransformation = true;
            StartCoroutine(Transform());
            s_gameMode = GameMode.Mode_2D;
        }
        else
            static_doTransformation = false;
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Transform()
    {
        while (doTransformation)
        {
            yield return new WaitForSeconds(timeTilSwitch);
            SwitchMode_3D();
            yield return new WaitForSeconds(timeTilSwitch);
            SwitchMode_2D();
        }
         
    }

    void SwitchMode_3D()
    {
        s_gameMode = GameMode.Mode_3D;
        gameMode = GameMode.Mode_3D;
        
        //Turn off PLAYER sprite and turn on primative cube
        playerRend.enabled = false;
        playerCubeRend.enabled = true;

        StartCoroutine(PulseImage(null, playerCubeRend));

        //Turn off bg
        bgRend.enabled = false;

        //Turn off ENEMY Sprites and turn on Primative cubes  
        enemyRends.Clear();
        enemyCubeRends.Clear();

        for (int i = 0; i < spawnConTrans.childCount; i++)
        {
            
            SpriteRenderer childRend = spawnConTrans.GetChild(i).GetComponent<SpriteRenderer>();
            enemyRends.Add(childRend);

            MeshRenderer cubeRend = childRend.GetComponentInChildren<MeshRenderer>();
            enemyCubeRends.Add(cubeRend);
        }

        for (int j = 0; j < enemyRends.Count; j++)
        {
            if (enemyRends[j] != null)
            {
                enemyRends[j].enabled = false;
                enemyCubeRends[j].enabled = true;

                StartCoroutine(PulseImage(null, enemyCubeRends[j]));
            }
            
        }

        //Turn off LASER Sprites and turn on Primative cubes.
        laserRends.Clear();
        laserCapsRends.Clear();
        for(int k = 0; k < laserConTrans.childCount; k++)
        {
            SpriteRenderer childRend = laserConTrans.GetChild(k).GetComponent<SpriteRenderer>();
            laserRends.Add(childRend);

            MeshRenderer capRend = childRend.GetComponentInChildren<MeshRenderer>();
            laserCapsRends.Add(capRend);

        }

        for (int l = 0; l < laserRends.Count; l++)
        {
            if (laserRends[l] != null)
            {
                laserRends[l].enabled = false;
                laserCapsRends[l].enabled = true;

                StartCoroutine(PulseImage(null, laserCapsRends[l]));
            }
                
        }
        
    }
    
    void SwitchMode_2D()
    {
        s_gameMode = GameMode.Mode_2D; 
        gameMode = GameMode.Mode_2D;
        //Turn on PLAYER sprite and turn off primative cube
        playerRend.enabled = true;
        playerCubeRend.enabled = false;

        StartCoroutine(PulseImage(playerRend, null));

        //Turn on bg
        bgRend.enabled = true;

        StartCoroutine(PulseImage(bgRend, null));

        //Turn on ENEMY Sprites and turn off Primative cubes  
        enemyRends.Clear();
        enemyCubeRends.Clear();

        for (int i = 0; i < spawnConTrans.childCount; i++)
        {

            SpriteRenderer childRend = spawnConTrans.GetChild(i).GetComponent<SpriteRenderer>();
            enemyRends.Add(childRend);

            MeshRenderer cubeRend = childRend.GetComponentInChildren<MeshRenderer>();
            enemyCubeRends.Add(cubeRend);
        }

        for (int j = 0; j < enemyRends.Count; j++)
        {
            if (enemyRends[j].gameObject != null)
            {
                enemyRends[j].enabled = true;
                enemyCubeRends[j].enabled = false;

                StartCoroutine(PulseImage(enemyRends[j], null));
            }

        }

        //Turn on LASER Sprites and turn off Primative cubes.
        laserRends.Clear();
        laserCapsRends.Clear();
        for (int k = 0; k < laserConTrans.childCount; k++)
        {
            SpriteRenderer childRend = laserConTrans.GetChild(k).GetComponent<SpriteRenderer>();
            laserRends.Add(childRend);

            MeshRenderer capRend = childRend.GetComponentInChildren<MeshRenderer>();
            laserCapsRends.Add(capRend);

        }

        for (int l = 0; l < laserRends.Count; l++)
        {
            if (laserRends[l].gameObject != null)
            {
                laserRends[l].enabled = true;
                laserCapsRends[l].enabled = false;

                StartCoroutine(PulseImage(laserRends[l], null));
            }

        }
    }

    public IEnumerator PulseImage(SpriteRenderer spriteRend, MeshRenderer meshRend)
    {

        if (doPulse)
        {
            Material mat;

            if (spriteRend != null)
                mat = spriteRend.material;
            else if (meshRend != null)
                mat = meshRend.material;
            else
                yield break;
            
            
            
            //Material material = rend.material;
            Color col = mat.color;

            int count = numPulses;

            while(count > 0)
            {
                for (float t = 0f; t < timePerPulse; t += Time.deltaTime)
                {
                    if(mat != null)
                    {
                        float normalizedTime = t / timePerPulse;
                        var someAlphaValue = Mathf.Lerp(1.0f, 0.5f, normalizedTime);
                        col.a = someAlphaValue;
                        mat.color = col;
                        yield return null;
                    }
                    
                }
                /*cg.alpha = maxAlpha;
                yield return new WaitForSeconds(delay);*/

                for (float t = 0f; t < timePerPulse; t += Time.deltaTime)
                {
                    if(mat != null)
                    {
                        float normalizedTime = t / timePerPulse;
                        var someAlphaValue = Mathf.Lerp(0.5f, 1.0f, normalizedTime);
                        col.a = someAlphaValue;
                        mat.color = col;
                        yield return null;
                    }
                    
                }
                col.a = 1.0f;
                mat.color = col;


                count--;
            }
        }
        






        yield return null;
    }


    
}
