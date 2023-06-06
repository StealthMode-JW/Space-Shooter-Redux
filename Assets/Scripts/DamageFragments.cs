using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFragments : MonoBehaviour
{
    [SerializeField]
    List<GameObject> _fragments = new List<GameObject>();

    public void Fragmentize()
    {
        if (_fragments.Count > 0)
        {
            int ranAmount = Random.Range(1, _fragments.Count + 1);

            for (int i = 0; i < ranAmount; i++)
            {
                int randomIndex = Random.Range(0, _fragments.Count);

                Vector3 pos = transform.position;
                var newFragment = Instantiate(_fragments[randomIndex], pos, Quaternion.identity);
                var fragment = newFragment.GetComponent<Fragment>();
                if (fragment != null)
                {
                    float random_DirAngle = Random.Range(0, 360f);
                    float random_DirZ = Random.Range(-10, 10f);
                    float random_DirSpeed = Random.Range(2.0f, 6.0f);
                    float random_RotSpeed = Random.Range(2.0f, 120.0f);
                    int randomBool = Random.Range(0, 2);
                    bool isTrue = (randomBool == 1) ? true : false;

                    fragment.SetDirectionRotation(random_DirAngle, random_DirZ,
                        random_DirSpeed, random_RotSpeed, isTrue);
                }
            }
        }
    }
    /*[SerializeField]
    List<GameObject> _fragments = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fragmentize()
    {
        if(_fragments.Count > 0)
        {
            float ranAmount = Random.Range(0, _fragments.Count);
            
            for(int i = 0; i < ranAmount; i++)
            {
                int randomIndex = Random.Range(0, _fragments.Count);
                
                Vector2 pos = transform.position;
                var newFragment = Instantiate(_fragments[randomIndex], pos, Quaternion.identity);
                var fragment = newFragment.GetComponent<Fragment>();
                if(fragment != null)
                {
                    float random_xPos = Random.Range(-360, 360f);
                    float random_yPos = Random.Range(-360, 360f);
                    float random_zPos = Random.Range(-10, 10f);
                    float random_DirSpeed = Random.Range(2.0f, 6.0f);
                    float random_RotSpeed = Random.Range(2.0f, 6.0f);
                    int randomBool = Random.Range(0, 1);
                    bool isTrue = false;
                    if (randomBool == 1)
                        isTrue = true;

                    fragment.SetDirectionRotation(random_xPos, random_yPos, random_zPos, 
                        random_DirSpeed, random_RotSpeed, isTrue);
                }
                
                
            }
        }
    }*/

}
