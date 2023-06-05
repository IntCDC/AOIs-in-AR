using UnityEngine;
using System.Collections.Generic;
using System;
public class CameraFollow : MonoBehaviour
{
    public Vector3 targetObject;
    public Vector3 initalOffset;
    public float speed=5f;
    string frameId;

    void Start()
    {
        targetObject = SceneDataHandler.gazePoint;
    }


    void Update()
    {
       
        if (SceneDataHandler.hittedFrame_ID != null)
        {
            if(frameId != SceneDataHandler.hittedFrame_ID)
            {
                frameId = SceneDataHandler.hittedFrame_ID;
                transform.LookAt(SceneDataHandler.gazePoint);
            }
        }
    }
}
