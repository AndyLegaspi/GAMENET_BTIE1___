using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointSingleton : MonoBehaviour
{
    private static SpawnPointSingleton instance;

    private GameObject[] spawnPoints;

    public static SpawnPointSingleton Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<SpawnPointSingleton>();

                if(instance == null)
                {
                    GameObject gameObject = new GameObject("SpawnPoint");
                    instance = gameObject.AddComponent<SpawnPointSingleton>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this as SpawnPointSingleton;
        }
        else
        {
            if(instance != this)
            {
                Destroy(gameObject);
            }
        }

        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
    }

    public GameObject[] GetMySpawnPoints()
    {
        return spawnPoints;
    }
}
