/*
///////////////////////////////////////////////////
///                                             ///
/// © Academy of Interactive Entertainment 2021 ///
///                                             ///
/// Developed by Bethany Cabezas-Heathwood      ///
/// Unity Version: 2019.3.6f1                   ///
/// Last updated: 31/05/21                      ///
/// bethanych@aie.edu.au                        ///
///                                             ///
///////////////////////////////////////////////////

This script contains the state information for transitioning between player animations during the game.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class AnimationCode : MonoBehaviour
{
    private Animator characterAC;

    private GameObject playerObject;
    private bool idleAnim = false;
    private bool jumpAnim = false;
    private bool crouchAnim = false;
    private bool walkAnim = false;
    private bool runAnim = false;


    void Start()
    {
        characterAC = GetComponent<Animator>();

        playerObject = GameObject.FindGameObjectWithTag("Player");

        if (HasParameter("isRunning", characterAC) || HasParameter("isWalking", characterAC))
        {
            idleAnim = true;
        }
        jumpAnim = HasParameter("isJumping", characterAC);
        crouchAnim = HasParameter("isCrouching", characterAC);
        walkAnim = HasParameter("isWalking", characterAC);
        runAnim = HasParameter("isRunning", characterAC);
    }



    void Update()
    {
        //RUN/IDLE ANIMATION
        if (idleAnim)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                if (runAnim && Input.GetKey(KeyCode.LeftShift))
                {
                    characterAC.SetBool("isRunning", true);
                }
                else
                {
                    characterAC.SetBool("isRunning", false);
                    characterAC.SetBool("isWalking", true);
                }
            }
            else
            {
                characterAC.SetBool("isRunning", false);
                characterAC.SetBool("isWalking", false);
            }
        }

        //CROUCH ANIMATION
        if (crouchAnim)
        {
            if (playerObject.GetComponent<PlayerController>().GetCrouching() == true && playerObject.GetComponent<PlayerController>().GetGrounded() == true)
            {
                characterAC.SetBool("isCrouching", true);
            }
            else
            {
                characterAC.SetBool("isCrouching", false);
            }
        }

        //JUMP ANIMATION -- should override all other states if 'falling'
        if (jumpAnim)
        {
            if (playerObject.GetComponent<PlayerController>().GetGrounded() == false)
            {
                characterAC.SetBool("isJumping", true);
                characterAC.SetBool("isCrouching", false);
                characterAC.SetBool("isRunning", false);
                characterAC.SetBool("isWalking", false);
            }
            else
            {
                characterAC.SetBool("isJumping", false);
            }
        }
    }


    //NOTE: This isn't an efficient way to check for paramaters, especially with a lot of parameters to check
    private static bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }

}
