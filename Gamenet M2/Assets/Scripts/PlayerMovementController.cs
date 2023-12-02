using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    public Joystick joystick;
    public FixedTouchField fixedTouchField;

    private RigidbodyFirstPersonController rbFirstPersonController;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rbFirstPersonController = this.GetComponent<RigidbodyFirstPersonController>();
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        rbFirstPersonController.joystickInputAxis.x = joystick.Horizontal;
        rbFirstPersonController.joystickInputAxis.y = joystick.Vertical;
        rbFirstPersonController.mouseLook.lookInputAxis = fixedTouchField.TouchDist;

        animator.SetFloat("horizontal", joystick.Horizontal);
        animator.SetFloat("vertical", joystick.Vertical);


        if(MathF.Abs(joystick.Horizontal) > 0.9 || Mathf.Abs(joystick.Vertical) > 0.9){
            animator.SetBool("isRunning", true);
            rbFirstPersonController.movementSettings.ForwardSpeed = 10;
        }
        else{
            animator.SetBool("isRunning", false);
            rbFirstPersonController.movementSettings.ForwardSpeed = 5;
        }
    }
}
