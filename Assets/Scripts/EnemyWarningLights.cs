using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWarningLights : MonoBehaviour
{
    public WarningZone[] warningZones;

    private void Start()
    {
        foreach (var zone in warningZones)
        {
            //zone.parentTrans = transform;
            zone.InitializePool();
        }
    }

    private void Update()
    {

        if(GameManager.isGameOver == false)
        {
            if(SpawnManager.EnemyTransforms.Count > 0 && SpawnManager.EnemyTransforms[0] != null)
            {
                foreach (var enemy in SpawnManager.EnemyTransforms)
                {
                    foreach (var zone in warningZones)
                    {
                        bool enemyIsBeingTracked = zone.trackedEnemies.Contains(enemy);

                        if (zone.bounds.Contains(enemy.position))
                        {
                            if (!enemyIsBeingTracked)
                            {
                                GameObject newLight = zone.GetPooledLight();
                                newLight.SetActive(true);
                                zone.lightInstances.Add(newLight);
                                zone.trackedEnemies.Add(enemy);
                            }

                            int enemyIndex = zone.trackedEnemies.IndexOf(enemy);
                            GameObject correspondingLight = zone.lightInstances[enemyIndex];

                            // Update position of the light to follow enemy, clamp within screen bounds
                            Vector3 newPos = zone.lockedAxis == WarningZone.Axis.X ?
                                new Vector3(zone.lockedAxisValue, Mathf.Clamp(enemy.position.y, zone.movableAxisMin, zone.movableAxisMax), 0f) :
                                new Vector3(Mathf.Clamp(enemy.position.x, zone.movableAxisMin, zone.movableAxisMax), zone.lockedAxisValue, 0f);

                            correspondingLight.transform.position = newPos;
                        }
                        else if (enemyIsBeingTracked)
                        {
                            int enemyIndex = zone.trackedEnemies.IndexOf(enemy);
                            GameObject correspondingLight = zone.lightInstances[enemyIndex];
                            correspondingLight.SetActive(false);
                            zone.lightInstances.RemoveAt(enemyIndex);
                            zone.trackedEnemies.Remove(enemy);
                        }

                        for (int i = zone.trackedEnemies.Count - 1; i >= 0; i--)
                        {
                            if (zone.trackedEnemies[i] == null)
                            {
                                zone.lightInstances[i].SetActive(false);
                                zone.lightInstances.RemoveAt(i);
                                zone.trackedEnemies.RemoveAt(i);
                            }
                        }
                    }
                }
            }
            
        }
        



    }

    
}

[System.Serializable]
public class WarningZone
{
    public string name;
    public Rect bounds;
    public GameObject lightPrefab;
    public Transform parentTrans;
    public List<GameObject> lightInstances = new List<GameObject>();
    public List<Transform> trackedEnemies = new List<Transform>();
    public List<GameObject> pooledLights = new List<GameObject>();
    public int lightsToPool = 10; // Adjust this to the maximum expected number of enemies
    public enum Axis { X, Y }
    public Axis lockedAxis;
    public float lockedAxisValue;
    public float movableAxisMin;
    public float movableAxisMax;

    public void InitializePool()
    {
        for (int i = 0; i < lightsToPool; i++)
        {
            GameObject newLight = Object.Instantiate(lightPrefab, parentTrans);
            newLight.SetActive(false);
            pooledLights.Add(newLight);
        }
    }

    public GameObject GetPooledLight()
    {
        foreach (GameObject light in pooledLights)
        {
            if (!light.activeInHierarchy)
            {
                return light;
            }
        }

        // If no inactive light is found, instantiate a new one and add it to the pool
        GameObject extraLight = Object.Instantiate(lightPrefab, parentTrans);
        pooledLights.Add(extraLight);
        return extraLight;
    }
}
