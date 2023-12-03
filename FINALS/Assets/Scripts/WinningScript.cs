using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class WinningScript : MonoBehaviourPunCallbacks
{
    public Text WinningText;

    // Start is called before the first frame update
    void Start()
    {  
        WinningText = GameObject.FindGameObjectWithTag("WinningText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovementScript player = other.gameObject.GetComponent<PlayerMovementScript>();

        if(player != null){
            WinningText.text = "Congrats to " + PhotonNetwork.LocalPlayer.NickName + " for reaching the top w/ everyone! unless, you didnt.";
            if(photonView.IsMine){
                WinningText.text = "Congrats to " + PhotonNetwork.LocalPlayer.NickName + "for reaching the top w/ everyone!";
            }
            if(!photonView.IsMine){
                WinningText.text = "Congrats to " + photonView.Owner.NickName + "for reaching the top w/ everyone!";
            }
        }
    }
}
