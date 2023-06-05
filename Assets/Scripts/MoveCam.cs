using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{

    // move freely within gaze replay by moving the camera using the arrow keys
    void Start()
    {
        rotZ = 0.0f;
    }
    public float rotationSpeed = 50f;
    public float moveSpeed = 6f;
    float inputX, inputZ,ctrl,alt;
    private float rotZ;
    public float keyboardSensitivity=15f;
    private Quaternion localRotation;
    public Camera cam;


    void Update()
    {
        Vector3 view_point = cam.ScreenToViewportPoint(Input.mousePosition);

        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
        ctrl = Input.GetAxis("Fire1");
        alt = Input.GetAxis("Fire2");
        if (view_point.y >= 0)
        {
            if (inputX != 0 && ctrl == 0 && alt == 0)
            {
                moveRight();
            }
            if (inputZ != 0 && ctrl == 0 && alt == 0)
            {
                moveForward();
            }
            if (ctrl != 0 && inputZ != 0 && alt == 0)
            {
                moveUp();
            }
            if (alt != 0 && inputX != 0 && ctrl == 0)
            {
                rotateX();
            }

            if (alt != 0 && inputZ != 0 && ctrl == 0)
            {
                //rotateY();
            }
        }
        
    }

    private void rotateY()
    {
        transform.Rotate(new Vector3(inputZ * Time.deltaTime * rotationSpeed,0f, 0f));
    }

    private void moveUp()
    {
        transform.position += transform.up * inputZ * Time.deltaTime * moveSpeed;
    }

    private void moveRight()
    {
        transform.position += transform.right * inputX * Time.deltaTime * moveSpeed;
    }

    private void moveForward()
    {
        transform.position += transform.forward * inputZ * Time.deltaTime*moveSpeed;
    }

    private void rotateX()
    {
        transform.Rotate(new Vector3(0f, inputX*Time.deltaTime*rotationSpeed,0f));
    }
}
