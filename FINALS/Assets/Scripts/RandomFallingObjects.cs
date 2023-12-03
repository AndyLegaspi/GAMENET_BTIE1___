using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFallingObjects : MonoBehaviour
{
    [SerializeField] public GameObject fallingObject;
    public float fallTime = 66f;
    public float fallTimer = 66f;

    [SerializeField] public GameObject[] fallObjectPoints;
    int spawnSelect;
    int spawnChance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spawnChance = Random.Range(1, 100);
        if(spawnChance > 0 && spawnChance <= 8){
            if(fallTimer > 0)
            {
                fallTimer--;
            }

            if(fallTimer <= 0)
            {
                int spawnSelect = Random.Range(1, fallObjectPoints.Length);
                Vector3 spawnPoint = new Vector3(fallObjectPoints[spawnSelect].transform.position.x, fallObjectPoints[spawnSelect].transform.position.y, fallObjectPoints[spawnSelect].transform.position.z);
                Instantiate(fallingObject, spawnPoint, Quaternion.identity);

                fallTimer += fallTime;
            }
        }
    }
}
