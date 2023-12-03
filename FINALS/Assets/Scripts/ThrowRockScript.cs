using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon.StructWrapping;

public class ThrowRockScript : MonoBehaviourPunCallbacks
{
    int maxRocks = 3;
    [SerializeField] public GameObject rocks;
    [SerializeField] public PlayerMovementScript Player;
    public Transform rockTransform;
    public bool canThrow;
    private float Timer;
    public float TimeBetweenThrowing;

    private Camera mainCam;
    private Vector3 mosuePos;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        canThrow = true;
        Player = this.GetComponent<PlayerMovementScript>();
        //rocks = GameObject.FindGameObjectWithTag("Rocks").GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        mosuePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        
        if(!canThrow){
            Timer += Time.deltaTime;
            if(Timer > TimeBetweenThrowing){
                canThrow = true;
                Timer = 0;
            }
        }

        if(Input.GetMouseButton(0) && canThrow){
            Fire();
        }
    }

    public void Fire()
    {
        Vector3 throwPoint = new Vector3(mosuePos.x, mosuePos.y, -10);
        if(maxRocks > 0){
            Instantiate(rocks, throwPoint, Quaternion.identity);

            canThrow = false;
            maxRocks--;
        }
        else if(maxRocks <= 0){
            Debug.Log("No More Rocks");
        }
    }

    [PunRPC]
    public void PushOthers(PhotonMessageInfo info)
    {
        if(mosuePos.x == Player.transform.position.x){
            Debug.Log(info.Sender.NickName + " blocked " + info.photonView.Owner);
        }
    }
}
