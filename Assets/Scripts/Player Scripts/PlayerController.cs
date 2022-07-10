/*
///////////////////////////////////////////////////
///                                             ///
/// © Academy of Interactive Entertainment 2021 ///
///                                             ///
/// Developed by Bethany Cabezas-Heathwood      ///
/// Unity version: 2019.3.6f1                   ///
/// Last updated: 31/05/21                      ///
/// bethanych@aie.edu.au                        ///
///                                             ///
///////////////////////////////////////////////////

This script contains the functionality for a basic third-person player controller using rigidbody physics. It uses
WASD or arrow keys for movement, shift for sprint, space for jump, and left control for crouch.

NOTE: This script will require a reference to a base object for the camera in order to determine its orientation for 
camera-based forward directions

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]


public class PlayerController : MonoBehaviour
{

    private GameObject cameraObject;
    private Rigidbody rb;
    private CapsuleCollider playerCollider;

    //MOVEMENT VARIABLES
    private float moveHorizontal;
    private float moveVertical;
    [Tooltip("The walking speed of the player.")]
    [SerializeField, Range(1.0f, 50.0f)] private float walkSpeed = 2.0f;
    [Tooltip("The sprinting speed of the player.")]
    [SerializeField, Range(1.0f, 50.0f)] private float sprintSpeed = 5.0f;
    [Tooltip("The crouching speed of the player.")]
    [SerializeField, Range(1.0f, 50.0f)] private float crouchSpeed = 1.0f;
    private float currentSpeed;
    private Vector3 movement;

    //JUMP VARIABLES
    [Tooltip("The amount of upwards force when the player jumps.")]
    [SerializeField, Range(1.0f, 30.0f)] private float jumpHeight = 5;
    private bool grounded = false;
    private bool jumping = false;
    private int layerMask;
    private float playerHeight;
    private float checkHeight;

    //CROUCH VARIABLES
    private float originalHeight;
    private Vector3 originalCenter;
    private float crouchHeight;
    private Vector3 crouchCenter;
    private bool isCrouching = false;     //This is a hook for animation purposes

    private bool underRoof = false;
    private bool pendingStand = false;


    //TESTING
    private bool dashing = false;

    private void Awake()
    {
        //Set the tag as "Player" in case it wasn't set manually
        gameObject.tag = "Player";
    }

    void Start()
    {

        //Get the Rigidbody component of the player
        rb = GetComponent<Rigidbody>();
        //Get a reference to the camera object for forwards momentum - find a way to do this without camera reference?
        cameraObject = GameObject.Find("CameraController");

        //Set the layermask to ignore the "Player" layer
        layerMask = ~LayerMask.GetMask("Player");

        //Get the Capsule Collider component of the player
        playerCollider = GetComponent<CapsuleCollider>();

        if (playerCollider.height <= playerCollider.radius)
        {
            playerHeight = playerCollider.radius * 2;
        }
        else
        {
            playerHeight = playerCollider.height;
        }
        originalHeight = playerHeight;
        originalCenter = playerCollider.center;
        crouchHeight = playerHeight / 2;
        crouchCenter = new Vector3(0.0f, originalCenter.y - (crouchHeight / 2), 0.0f);
        //Set the raycast distance to check for the ground when jumping
        checkHeight = playerHeight / 2 + 0.01f;
    }


    void FixedUpdate()
    {

        if (rb.velocity.magnitude > currentSpeed)
        {
            Vector3 xzVelocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
            Vector3 yVelocity = new Vector3(0.0f, rb.velocity.y, 0.0f);

            xzVelocity = Vector3.ClampMagnitude(xzVelocity, currentSpeed);
            rb.velocity = xzVelocity + yVelocity;
        }

        //Get keyboard input
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveVertical = Input.GetAxisRaw("Vertical");

        //Calculate movement and forward direction based on camera view
        movement =
            Quaternion.Euler(0.0f, cameraObject.transform.eulerAngles.y, 0.0f) *
            new Vector3(moveHorizontal, 0.0f, moveVertical);

        //Calculating upwards force if player is jumping
        if (jumping)
        {
            grounded = false;
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.Impulse);
            jumping = false;
        }

        if (dashing)
        {
            rb.AddRelativeForce(Vector3.forward * 100, ForceMode.Impulse);
            dashing = false;
        }

        rb.AddForce(movement * currentSpeed, ForceMode.VelocityChange);

        if (moveHorizontal == 0 && moveVertical == 0)
        {
            rb.velocity = new Vector3(0.0f, rb.velocity.y, 0.0f);
        }
    }

    private void Update()
    {
        CheckForGround();
        CheckForRoof();

        if (Input.GetKeyDown(KeyCode.Space) && grounded && isCrouching == false)
        {
            jumping = true;
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            dashing = true;
        }

        //Snap character rotation to match the camera's rotation
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            Quaternion targetRotation = cameraObject.transform.rotation;
            targetRotation.x = 0.0f;
            targetRotation.z = 0.0f;

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 20.0f * Time.deltaTime);
        }

        //Player is running if Left Shift is held down
        if (Input.GetKey(KeyCode.LeftShift) && isCrouching == false)
        {
            currentSpeed = sprintSpeed;
        }
        else if (isCrouching == true)
        {
            currentSpeed = crouchSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        //Player is crouching if Left Control is held down
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            playerCollider.height = crouchHeight;
            playerCollider.center = crouchCenter;

            isCrouching = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (!underRoof)
            {
                Stand();
            }
            else
            {
                pendingStand = true;
            }
        }

        if (pendingStand && !underRoof)
        {
            Stand();
        }
    }

    private void CheckForGround()
    {
        RaycastHit hitInfo;

        if (Physics.SphereCast(
                        transform.position,
                        playerCollider.radius,
                        Vector3.down,
                        out hitInfo,
                        checkHeight,
                        layerMask,
                        QueryTriggerInteraction.Ignore))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }

    private void CheckForRoof()
    {
        RaycastHit hitInfo;

        if (Physics.SphereCast(
                        transform.position,
                        playerCollider.radius,
                        Vector3.up,
                        out hitInfo,
                        checkHeight,
                        layerMask,
                        QueryTriggerInteraction.Ignore))
        {
            underRoof = true;
        }
        else
        {
            underRoof = false;
        }
    }

    private void Stand()
    {
        playerCollider.height = originalHeight;
        playerCollider.center = originalCenter;

        isCrouching = false;
        pendingStand = false;
    }

    public bool GetGrounded()
    {
        return grounded;
    }

    public bool GetJumping()
    {
        return jumping;
    }

    public void SetWalkSpeed(float speedToSet)
    {
        walkSpeed = speedToSet;
    }

    public float GetWalkSpeed()
    {
        return walkSpeed;
    }

    public void SetSprintSpeed(float speedToSet)
    {
        sprintSpeed = speedToSet;
    }

    public float GetSprintSpeed()
    {
        return sprintSpeed;
    }

    public void SetCrouchSpeed(float speedToSet)
    {
        crouchSpeed = speedToSet;
    }

    public float GetCrouchSpeed()
    {
        return crouchSpeed;
    }

    public bool GetCrouching()
    {
        return isCrouching;
    }

}
