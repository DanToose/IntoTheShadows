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

This script contains the functionality for a basic third-person camera setup.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{


    private GameObject cameraTargetObject;
    private bool lockCursor = true;
    float cameraMoveSpeed = 800.0f;
    float maxHighAngle = 60.0f;
    float maxLowAngle = -40.0f;
    public float mouseSensitivity = 150.0f;
    float mouseX;
    float mouseY;
    float smoothX;
    float smoothY;
    private float rotX = 0.0f;
    private float rotY = 0.0f;

    //CAMERA COLLISION VARIABLES
    public Camera cameraObject;
    public float minDistance = 2.0f;
    public float maxDistance = 4.0f;
    private float smooth = 10.0f;
    private Vector3 dollyDir;
    private float distance;
    private float percentageOffset = 0.8f;



    void Start()
    {
        cameraTargetObject = GameObject.FindGameObjectWithTag("Player");

        dollyDir = cameraObject.transform.localPosition.normalized;
        distance = cameraObject.transform.localPosition.magnitude;


        //Finding the root first is necessary in this case as both the PlayerController and
        //CameraController are grouped under an empty object and thus affecting the rotation
        //values
        Vector3 rot = transform.root.localRotation.eulerAngles;
        rotX = rot.x;
        rotY = rot.y;

        //Lock the cursor so it stays centered
        Cursor.lockState = CursorLockMode.Locked;
        //Make the cursor invisible so it won't obscure the screen
        Cursor.visible = false;

    }

    void FixedUpdate()
    {

        ManageCursor();

        //Get the current position of the mouse
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        //Calculate new camera position
        rotX += -mouseY * mouseSensitivity * Time.deltaTime;
        rotY += mouseX * mouseSensitivity * Time.deltaTime;

        //Stop the camera from going too far below and above the player
        rotX = Mathf.Clamp(rotX, maxLowAngle, maxHighAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;

        //Check if the camera is colliding with something and move as necessary
        Vector3 desiredCameraPos = transform.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        if (Physics.Linecast(transform.position, desiredCameraPos, out hit))
        {
            distance = Mathf.Clamp((hit.distance * percentageOffset), minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }

        cameraObject.transform.localPosition = Vector3.Lerp(cameraObject.transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);

    }

    void LateUpdate()
    {
        CameraUpdater();
    }

    private void CameraUpdater()
    {
        //Set the target that the camera will follow
        Transform target = cameraTargetObject.transform;

        //Move towards the camera's target
        float step = cameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }

    private void ManageCursor()
    {
        //Show/Hide cursor with ESCAPE key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            lockCursor = !lockCursor;
        }
        //If the cursor is currently visible/not locked and the player clicks on the window,
        //turn off and lock the cursor again
        if (Input.GetMouseButtonDown(0) && lockCursor == false)
        {
            lockCursor = true;
        }

        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;
    }
}
