using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.UI;
using Photon.Pun.Demo.PunBasics;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject fpsModel;
    public GameObject nonFpsModel;

    public GameObject playerUIPrefab;

    public PlayerMovementController playerMovementController;
    public Camera fpsCamera;

    private Animator animator;
    public Avatar fpsAvatar, nonFpsAvatar;

    private ShootingScript shooting;

    [SerializeField] public TextMeshProUGUI TextplayerNameText;

    // Start is called before the first frame update
    void Start()
    {
        playerMovementController = this.GetComponent<PlayerMovementController>();
        animator = this.GetComponent<Animator>();

        fpsModel.SetActive(photonView.IsMine);
        nonFpsModel.SetActive(!photonView.IsMine);
        animator.SetBool("isLocalPlayer", photonView.IsMine);
        animator.avatar = photonView.IsMine ? fpsAvatar : nonFpsAvatar;

        shooting = this.GetComponent<ShootingScript>();

        if(photonView.IsMine){
            GameObject playerUI = Instantiate(playerUIPrefab);
            playerMovementController.fixedTouchField = playerUI.transform.Find("RotationTouchField").GetComponent<FixedTouchField>();
            playerMovementController.joystick = playerUI.transform.Find("Fixed Joystick").GetComponent<Joystick>();
            fpsCamera.enabled = true;

            playerUI.transform.Find("FireButton").GetComponent<Button>().onClick.AddListener(() => shooting.Fire());
        }
        else{
            playerMovementController.enabled = false;
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            fpsCamera.enabled = false;
        }
        TextplayerNameText.text = photonView.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}