using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class player : MonoBehaviour
{
    public List<float> tmstmp = new List<float>();
    public List<Vector3> gazeOrigin = new List<Vector3>();
    public List<Vector3> gazeDir = new List<Vector3>();
    public List<Vector3> gazePoint = new List<Vector3>();
    public List<string> hittedObj = new List<string>();
    public List<float> tmstmp_lerp = new List<float>();
    public List<Vector3> gazeOrigin_lerp = new List<Vector3>();
    public List<Vector3> gazeDir_lerp = new List<Vector3>();
    public List<Vector3> gazePoint_lerp = new List<Vector3>();

    private LineRenderer linerenderer;
    private Ray ray;
    float t = 0.0f;
    public int index = 0;
    float timeSetted = 0;
    float lastTime = 0;
    public bool startPlayer = false;
    Vector3 startPosition = new Vector3(0, 0, 0);
    public GameObject gaze;
    int prevIndex = 0;
    GameObject gazePointsParent;
    List<GameObject> gazePoints = new List<GameObject>();
    string playerName;
    GameObject playerN;
    public bool visualizeGaze = false;

    //simulates movement of the participants and visualize their gaze ray

    void Start()
    {
        gazePointsParent = GameObject.Find("gazePointParent");
        linerenderer = GetComponent<LineRenderer>();
        linerenderer.enabled = true;
        playerName = transform.name;
        playerN = new GameObject();
        playerN.name = playerName;
        playerN.transform.SetParent(gazePointsParent.transform,false);
    }

    void Update()
    {
        if (startPlayer && index < gazeOrigin.Count - 2)
        {
            movePlayer_movetowards();
            index = index + 1;
        }

        if (visualizeGaze)
        {
            visualizeGazePoint();
        }
        else
        {
            for(int i = 0; i < gazePoints.Count; i++)
            {
                gazePoints[i].SetActive(false);
            }
        }
    }

    public void gazePointsSetActive(bool state)
    {
        for (int i = 0; i < gazePoints.Count; i++)
        {
            gazePoints[i].SetActive(state);
            prevIndex = 0;
        }
    }
    void createGaze(int i)
    {
        GameObject hittedGaze = Instantiate(gaze);
        hittedGaze.name = i.ToString();
        hittedGaze.transform.position = gazePoint[i];
        gazePoints.Add(hittedGaze);
        hittedGaze.transform.SetParent(playerN.transform, false);
    }

   public void visualizeGazePoint()
    {
        //visualize gaze points
        //visualize gaze points

        if (prevIndex == index)
        {
            for (int i = 0; i < gazePoints.Count; i++)
            {
                gazePoints[i].SetActive(true);
            }
        }
            if (prevIndex < index && index < gazeOrigin.Count)
            {
                for (int i = prevIndex; i < index; i++)
                {
                if (GameObject.Find(i.ToString()) == null)
                {
                    createGaze(i);

                }
                else
                {
                    GameObject.Find(i.ToString()).SetActive(true);
                }
                }
                prevIndex = index;
            }
            else if (prevIndex > index && index < gazeOrigin.Count)
            {
                for (int i = prevIndex; i > index; i--)
                {
                
                    string gaze = i.ToString();
                    string path_gaze = "gazePointParent/" + playerName + "/" + gaze;
                    if (GameObject.Find(path_gaze) != null)
                    {
                        GameObject gazeToRemove = GameObject.Find(path_gaze);
                    gazePoints.Remove(gazeToRemove);
                    Destroy(gazeToRemove);
                    }
                }
                prevIndex = index;
            }
    }

    public float getCurrentTmstp()
    {
        float tmstmp = this.tmstmp[index];
        return tmstmp;
    }

    public void setData(List<float> tmstmp, List<Vector3> gazeOrigin, List<Vector3> gazeDir, List<Vector3> gazePoint, List<string> hittedObj)
    {
        this.tmstmp = tmstmp;
        this.gazeOrigin = gazeOrigin;
        this.gazeDir = gazeDir;
        this.gazePoint = gazePoint;
        this.hittedObj = hittedObj;
    }

    public void setPlayerPosition(float timeSetted)
    {
        this.timeSetted = timeSetted;

        if (tmstmp.IndexOf(timeSetted) == -1)
        {
            float closest = tmstmp.Aggregate((x, y) => Math.Abs(x - timeSetted) < Math.Abs(y - timeSetted) ? x : y);
            if ((closest < timeSetted) && (tmstmp[tmstmp.Count - 1] > timeSetted))
            {
                index = tmstmp.IndexOf(closest);
            }
            else
            {
                index = tmstmp.IndexOf(closest) - 1;
            }
            float lerp_value = (closest - tmstmp[index]) / (tmstmp[index + 1] - tmstmp[index]);
            startPosition = Vector3.Lerp(gazeOrigin[index], gazeOrigin[index + 1], lerp_value);
            if ((closest < timeSetted) && (tmstmp[tmstmp.Count - 1] < timeSetted))
            {
                index = tmstmp.Count - 1;
                startPosition = gazeOrigin[index];
            }
        }
        else
        {
            index = tmstmp.IndexOf(timeSetted);
            startPosition = gazeOrigin[index];

        }
        transform.position = startPosition;
        showGaze(index);
    }

    public void movePlayer_movetowards()
    {
            Vector3 target = Vector3.MoveTowards(transform.position, gazeOrigin[index+1], Time.deltaTime * 2);
            transform.position = target;
            showGaze(index);

        
    }

    public void playPlayer()
    {
        startPlayer =! startPlayer;
    }

    public IEnumerator movePlayer_lerp()
    {
        float duration = timeSetted / 1000f;
        for (int i = index + 1; i < tmstmp.Count - 1; i++)
        {         
            float time_ = 0;
            while (time_ < duration)
            {
                transform.position = Vector3.Lerp(startPosition, gazeOrigin[i], time_ / duration);
                showGaze(i);
                time_ += Time.deltaTime;

                yield return null;
            }
            transform.position = gazeOrigin[i];
            duration = (tmstmp[i + 1] - tmstmp[i]) / 1000f;
            yield return null;
           

            startPosition = transform.position;
        }

        yield return null;

    }


    public IEnumerator lerpLoop(float duration, Vector3 startPosition)
    {
        for (int i = index + 1; i < tmstmp.Count - 1; i++)
        {
            float time = 0;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, gazeOrigin[i], time / duration);
            showGaze(i);
            time += Time.deltaTime;

                yield return null;
        }
        transform.position = gazeOrigin[i];
        duration = (tmstmp[i + 1] - tmstmp[i]) / 1000f;
            yield return null;
            startPosition = transform.position;
        }
    }
    public IEnumerator movePlayer()
    {

        for (int i = 0; i < tmstmp.Count; i++)
        {
            float time = 0;
            Vector3 startPosition = transform.position;
            float duration = (tmstmp[i + 1] - tmstmp[i]) / 1000f;
            while (time < duration)
            {
                transform.position = Vector3.Lerp(startPosition, gazeOrigin[i], time / duration);
                time += Time.deltaTime;
                showGaze(i);
                yield return null;
            }
            transform.position = gazeOrigin[i];
        }
    }
    public IEnumerator movePlayer2()
    {
        for (int i = 0; i < tmstmp.Count; i++)
        {
            Vector3 target = Vector3.MoveTowards(transform.position, gazeOrigin[i], Time.deltaTime * 2);
            showGaze(i);
            transform.position = target;
            float time = (tmstmp[i + 1] - tmstmp[i]) / 1000f;
            yield return new WaitForSeconds(time);
        }
    }

    private void showGaze(int i)
    {
        ray = new Ray(gazeOrigin[i], gazeDir[i]);
        if (Physics.Raycast(gazeOrigin[i], gazeDir[i], 100))
        {
            linerenderer.SetPosition(0, transform.position);
            linerenderer.SetPosition(1, gazePoint[i]);
        }

    }

}
