using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchCam : MonoBehaviour
{
    public GameObject cam1;
    public GameObject cam2;
    public GameObject cam3;

    //    public Camera cam3;
    public void Start()
    {
        cam1.SetActive(true);
    }

    public void switchToCam(int index)
    {
        deactivateAllCam();
        if (index == 1)
        {
            cam1.SetActive(true);
        }
        else if (index == 2)
        {
            cam2.SetActive(true);
        }
        else
        {
            cam3.SetActive(true);
        }

    }

    public void deactivateAllCam()
    {
        cam1.SetActive(false);
        cam2.SetActive(false);
        cam3.SetActive(false);

    }
}
