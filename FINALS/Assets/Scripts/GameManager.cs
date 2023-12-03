using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady){

            GameObject[] spawnPoints = SpawnPointSingleton.Instance.GetMySpawnPoints();
            int spawnSelect = Random.Range(0, spawnPoints.Length);
            
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(spawnPoints[spawnSelect].transform.position.x, 0, spawnPoints[spawnSelect].transform.position.z), Quaternion.identity);
        }  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
