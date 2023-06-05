using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AOIRegionManager : MonoBehaviour
{
    public GameObject frameRegion_prefab;
    public GameObject frameRegion_Canvas;
    private List<string> showedRegions = new List<string>();
    public Camera camera;
    public GameObject cube;
    public GameObject AOI_region_parent;
    public GameObject AOI_handler;
    public GameObject createAOI_handler;
    public GameObject editAOI_handler;
    public Toggle createAOItoggle;
    public Toggle editAOItoggle;



    //handles when the clicked fixations in timeline visualization have to be shown in gaze replay (as a green box)
    void Update()
    {

        if (SceneDataHandler.FrameClicked)
        {
            Vector3 gazePoint = SceneDataHandler.gazePoint;
            string ID = SceneDataHandler.hittedFrame_ID;

            if (showedRegions.IndexOf(ID) == -1)
            {
                showedRegions.Add(ID);
                showFrameinRegion(gazePoint, ID);
                SceneDataHandler.FrameClicked = false;
            }
            else if (showedRegions.IndexOf(ID) != -1)
            {
                showedRegions.Remove(ID);
                removeFrameInRegion(ID);
                SceneDataHandler.FrameClicked = false;

            }
        }


        if (SceneDataHandler.FramesToShowInReplay.Count > 0)
        {
            foreach (KeyValuePair<string, Tuple<bool, Vector3>> frame in SceneDataHandler.FramesToShowInReplay)
            {
                Vector3 gazePoint = frame.Value.Item2;
                string ID = frame.Key;

                if (showedRegions.IndexOf(ID) == -1)
                {
                    showedRegions.Add(ID);
                    showFrameinRegion(gazePoint, ID);
                }
                else if (showedRegions.IndexOf(ID) != -1)
                {
                    showedRegions.Remove(ID);
                    removeFrameInRegion(ID);

                }
            }
            SceneDataHandler.FramesToShowInReplay.Clear();
        }

        if (SceneDataHandler.framesAnnotated)
        {
            foreach (Transform frame in frameRegion_Canvas.transform)
            {
                GameObject.Destroy(frame.gameObject);
            }
            SceneDataHandler.framesAnnotated = false;
        }
    }

    public void showFrameinRegion(Vector3 gazePoint, string ID)
    {
        GameObject frameRegion = Instantiate(frameRegion_prefab);
        frameRegion.name = ID;
        RectTransform frameRegion_transform = frameRegion.GetComponent<RectTransform>();
        frameRegion_transform.localPosition = gazePoint;
        frameRegion.transform.SetParent(frameRegion_Canvas.transform, false);
        if (gazePoint.x >= 6.4)
            frameRegion_transform.Rotate(0, 90, 0);
        else if (gazePoint.x <= -5)
        {
            frameRegion_transform.Rotate(0, -90, 0);

        }
    }

    public void removeFrameInRegion(string ID)
    {
        GameObject frame = GameObject.Find(ID);
        Destroy(frame);
    }


    // to create AOI cubes:
    public void enableNewAOIdefinition(bool isToggle)
    {
        AOI_handler.SetActive(isToggle);
        createAOI_handler.SetActive(isToggle);
        editAOI_handler.SetActive(isToggle);
    }


    public void enableEditAOI(bool isToggle)
    {
        AOI_handler.SetActive(isToggle);
        editAOI_handler.SetActive(isToggle);
        createAOI_handler.SetActive(false);
    }

    public void disableEditAOI()
    {

    }

   
}
