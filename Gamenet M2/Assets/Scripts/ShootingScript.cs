using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon.StructWrapping;

public class ShootingScript : MonoBehaviourPunCallbacks
{
    public Camera camera;
    public GameObject hitEffectPrefab;

    [Header("HP Related Stuff")]
    public float startHealth = 100;
    private float health;
    public Image healthBar;

    private Animator animator;

    public int kills;
    public int nonFPSkills;

    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Fire()
    {
        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(.5f, .5f));

        if(Physics.Raycast(ray, out hit, 200)){
            Debug.Log(hit.collider.gameObject.name);

            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);

            if(hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine){
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);
            }
        }
    }

    [PunRPC]
    public void TakeDamage(int dmg, PhotonMessageInfo info)
    {
        this.health -= dmg;
        this.healthBar.fillAmount = health / startHealth;

        if(health <= 0){
            Die();
            Debug.Log(info.Sender.NickName + " Killed " + info.photonView.Owner.NickName);

            if(photonView.IsMine){
                info.Sender.AddScore(1);
                nonFPSkills = info.Sender.GetScore();

                if(nonFPSkills == 0){
                    info.Sender.SetScore(3);
                }
            }
            else if(!photonView.IsMine){
                PhotonNetwork.LocalPlayer.AddScore(1);
                kills = PhotonNetwork.LocalPlayer.GetScore();

                if(kills == 0){
                    PhotonNetwork.LocalPlayer.SetScore(3);
                }
            }

            //Who killed Who UI
            GameObject killFeedUI = GameObject.Find("KillFeedUI");
            killFeedUI.GetComponent<Text>().text = info.Sender.NickName + " Killed " + info.photonView.Owner.NickName;
        }

        //Winner
        GameObject WinnerNameText = GameObject.Find("WINNERTEXT");

        if(PhotonNetwork.LocalPlayer.GetScore() == 10){
            //DoWinning(PhotonNetwork.LocalPlayer.NickName);
            transform.GetComponent<PlayerMovementController>().enabled = false;
            WinnerNameText.GetComponent<Text>().text = PhotonNetwork.LocalPlayer.NickName + " has won the game!, going back to lobby.";
            transform.GetComponent<PlayerMovementController>().enabled = false;

            PhotonNetwork.LeaveLobby();
        }
        else if(info.Sender.GetScore() == 10){
            //DoWinning(info.Sender.NickName);
            transform.GetComponent<PlayerMovementController>().enabled = false;
            WinnerNameText.GetComponent<Text>().text = info.Sender.NickName + " has won the game!, going back to lobby.";
            transform.GetComponent<PlayerMovementController>().enabled = false;

            PhotonNetwork.LeaveLobby();
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position){
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, .2f);
    }

    public void Die()
    { 
        if(photonView.IsMine){
            StartCoroutine(RespawnCountDown());
            animator.SetBool("isDead", true);
        }

        //will say 8 but it is actually 10
        Debug.Log(kills);
        Debug.Log(nonFPSkills);
    }

    IEnumerator RespawnCountDown()
    {
        GameObject respawnText = GameObject.Find("RespawnText");
        float respawnTime = 5.0f;

        while (respawnTime > 0){
            yield return new WaitForSeconds(1f);
            respawnTime--;

            transform.GetComponent<PlayerMovementController>().enabled = false;
            respawnText.GetComponent<Text>().text = "You were killed, Respawning in " + respawnTime.ToString(".00");
        }

        animator.SetBool("isDead", false);
        respawnText.GetComponent<Text>().text = "";

        //spawning
        GameObject[] spawnPoints = SpawnPointSingleton.Instance.GetMySpawnPoints();

        int spawnSelect = Random.Range(0, spawnPoints.Length); 
        this.transform.position = new Vector3(spawnPoints[spawnSelect].transform.position.x, 0, spawnPoints[spawnSelect].transform.position.z);
        transform.GetComponent<PlayerMovementController>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);

        GameObject killFeedUI = GameObject.Find("KillFeedUI");
        killFeedUI.GetComponent<Text>().text = " ";
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = 100;
        healthBar.fillAmount = health / startHealth;
    }
}
