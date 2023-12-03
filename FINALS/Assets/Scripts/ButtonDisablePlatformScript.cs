using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class ButtonDisablePlatformScript : MonoBehaviour
{
    public GameObject platform;
    public bool activated;
    public bool inRange;
    public bool isPressed;

    public float buttonTimer = 5f;
    public float buttonTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(activated){
            if(buttonTimer > 0) 
                buttonTimer--;
            
            if(buttonTimer <= 0){
                platform.SetActive(true);
                activated = false;
                buttonTimer += buttonTime;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovementScript player = other.gameObject.GetComponent<PlayerMovementScript>();
        if(other != null){
            Debug.Log("Player in reach");
            platform.SetActive(false);
            Debug.Log("Disabled Platform");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        PlayerMovementScript player = other.gameObject.GetComponent<PlayerMovementScript>();
        if(other != null){  
            activated = true;
        }
    }
}
