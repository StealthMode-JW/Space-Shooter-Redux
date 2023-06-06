using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestroyMe : MonoBehaviour
{
    //public List<DestroyCondition> destroyConditions = new List<DestroyCondition>();

    public bool destroyAfterTime;
    public DestroyCon_AfterTime destroyCon_AfterTime;

    //public List <DestroyCon_AfterTime> destroyCons_AfterTime = new List<DestroyCon_AfterTime> ();
    bool startedCounting = false;

    [Space(10)]

    public bool destroyByLocation;
    public DestroyCon_ByLocation destoryCon_ByLocation;
    //public List<DestroyCon_ByLocation> destoryCons_ByLocation = new List<DestroyCon_ByLocation> ();
    [Space(10)]


    public bool destroyIfObjectNull;
    public DestroyCon_IfObjectNull destroyCon_IfObjectNull;

    //public List<DestroyCon_IfObjectNull> destroyCons_IfObjectNull = new List<DestroyCon_IfObjectNull> ();
    public enum DestroyCondition
    {
        Unassigned,
        AfterTime,
        ByLocation,
        IfParentNull,
        IfChildNull
    };

    void Start()
    {
        SetupChildObjects();
    }

    private void Update()
    {
        CheckDestroyConditions();

    }

    void SetupChildObjects()
    {
        if (destroyIfObjectNull)
        {
            if (destroyCon_IfObjectNull.ifANY_OfThese_ChildObjects)
            {
                if(destroyCon_IfObjectNull.anyChildObjects.Count > 0)
                {
                    for (int i = 0; i < destroyCon_IfObjectNull.anyChildObjects.Count; i++)
                    {
                        var childCon = destroyCon_IfObjectNull.anyChildObjects[i];
                        var index = childCon.indexOfChild;
                        childCon.childObject = transform.GetChild(index).gameObject;
                    }
                    
                }
            }
            if (destroyCon_IfObjectNull.ifALL_OfThese_ChildObjects)
            {
                if (destroyCon_IfObjectNull.allChildObjects.Count > 0)
                {
                    for (int j = 0; j < destroyCon_IfObjectNull.allChildObjects.Count; j++)
                    {
                        var childCon = destroyCon_IfObjectNull.allChildObjects[j];
                        var index = childCon.indexOfChild;
                        childCon.childObject = transform.GetChild(index).gameObject;
                    }

                }
            }
        }
    }

    void CheckDestroyConditions()
    {
        if (destroyAfterTime)
        {
            DestroyAfterTime();
        }
        if (destroyByLocation)
        {
            DestroyByLocation();
        }
        if (destroyIfObjectNull)
        {
            DestroyIfObjectNull();
        }
    }

    void DestroyAfterTime()
    {
        if(startedCounting == false)
        {
            startedCounting = true;
            float dur = destroyCon_AfterTime.duration;
            Destroy(gameObject, dur);
        }
    }

    void DestroyByLocation()
    {

    }

    void DestroyIfObjectNull()
    {
        var des = destroyCon_IfObjectNull;
        if (des.ifANY_OfThese_ChildObjects)
        {
            if(des.anyChildObjects.Count > 0)
            {
                int countDestroyed = 0;
                for (int i = 0; i < des.anyChildObjects.Count; i++)
                {
                    var childCon = des.anyChildObjects[i];
                    if(childCon.childObject == null)
                    {
                        childCon.isDestroyed = true;
                        countDestroyed++;
                    }
                }
                if (countDestroyed > 0)
                    Destroy(gameObject);
            }
        }
        if (des.ifALL_OfThese_ChildObjects)
        {
            if (des.allChildObjects.Count > 0)
            {
                int countDestroyed = 0;
                for (int j = 0; j < des.allChildObjects.Count; j++)
                {
                    var childCon = des.allChildObjects[j];
                    if (childCon.childObject == null)
                    {
                        childCon.isDestroyed = true;
                        countDestroyed++;
                    }
                }
                if (countDestroyed >= des.allChildObjects.Count)
                    Destroy(gameObject);
            }
        }
    }




}








[System.Serializable]
public class DestroyConditions
{

}

[System.Serializable]
public class DestroyCon_AfterTime
{
    public float duration = 1.0f;
}

[System.Serializable]
public class DestroyCon_ByLocation
{
    
   

    public enum LocationType
    {
        Unassigned,
        IfBelowHere,
        IfAboveHere,
        IfLeftOfHere,
        IfRightOfHere

    };



}
[System.Serializable]
public class DestroyCon_IfObjectNull
{
    public bool ifANY_OfThese_ChildObjects;
    public List<ChildObject> anyChildObjects = new List<ChildObject>();

    public bool ifALL_OfThese_ChildObjects;
    public List<ChildObject> allChildObjects = new List<ChildObject>();
}

[System.Serializable]
public class ParentObject
{
    public GameObject parent;
}
[System.Serializable]

public class ChildObject
{
    public bool isDestroyed;
    public int indexOfChild;
    public GameObject childObject;
}