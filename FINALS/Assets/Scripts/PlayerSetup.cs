using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject camera;
    [SerializeField] Text playerNameText;

    ThrowRockScript throwRock;

    void Start()
    {
        throwRock = this.gameObject.GetComponent<ThrowRockScript>();

        if(photonView.IsMine){
            transform.GetComponent<PlayerMovementScript>().enabled = true;
            camera.GetComponent<Camera>().enabled = true;
            throwRock.GetComponent<ThrowRockScript>().enabled = true;
            
        }
        else{
            transform.GetComponent<PlayerMovementScript>().enabled = false;
            camera.GetComponent<Camera>().enabled = false;
            throwRock.GetComponent<ThrowRockScript>().enabled = false;
        }
        playerNameText.text = photonView.Owner.NickName;  
    }
}
