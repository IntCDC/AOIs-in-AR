using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class playerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject playerParent;
    List<float> tmstmp;
    List<Vector3> gazeOrigin;
    List<Vector3> gazeDir;
    List<Vector3> gazePoint;
    List<string> hittedObj;
    dataHandler datahandler = new dataHandler();
    List<string> folderNames = new List<string>();
    List<string> fixationFiles = new List<string>();
    public List<GameObject> listPlayers = new List<GameObject>();
    public List<float> listTmstmp = new List<float>();
    public sliderManager sliderMngr;
    public bool startBttnPressed = false;
    List<IEnumerator> listCoroutines = new List<IEnumerator>();
    player playerScript;
    List<player> playerList = new List<player>();
    bool startBttnClicked = false;
    int index_Part_MaxTmstp;
    bool sliderMoved_ = false;

    // enables simulation of the participants movement and gaze ray wenn timeslider is moved
    void Start()
    {
        sliderMngr = GetComponent<sliderManager>();
        datahandler.getFolderNames(out folderNames);
        datahandler.getFixationFiles(out fixationFiles);

        for (int i = 0; i < fixationFiles.Count; i++)
        {
            tmstmp = new List<float>();
            gazeOrigin = new List<Vector3>();
            gazeDir = new List<Vector3>();
            gazePoint = new List<Vector3>();
            hittedObj = new List<string>();
            string fileName = Path.GetFileName(folderNames[i].ToString());
            datahandler.getGazeData(fixationFiles[i], out tmstmp, out gazeOrigin, out gazeDir, out gazePoint, out hittedObj);
            createPlayer(fileName, tmstmp, gazeOrigin, gazeDir, gazePoint, hittedObj);
        }
        index_Part_MaxTmstp=listTmstmp.IndexOf(listTmstmp.Max());
        sliderMngr.getMaxValue(listTmstmp.Max());
        foreach (var player_ in listPlayers)
        {
            player playerScript = player_.GetComponent<player>();
            playerList.Add(playerScript);
        }

    }

    void Update()
    {
        if (startBttnClicked)
            updateSliderValue();
        if (SceneDataHandler.timeline_slider_moved)
        {
            sliderMngr.slider.value = SceneDataHandler.currentTmstp;
        }

        if (sliderMoved())
        {
            SceneDataHandler.replay_slider_moved = true;
        }
        else
        {
            SceneDataHandler.replay_slider_moved = false;
        }

        if (SceneDataHandler.participant_setActive)
        {
            string part_name = SceneDataHandler.participant_active.First().Key;
            bool part_state = SceneDataHandler.participant_active.First().Value;

            part_player_SetActive(part_name, part_state);
            SceneDataHandler.participant_setActive = false;
        }
    }

    public void createPlayer(string name, List<float> tmstmp,List<Vector3> gazeOrigin,List<Vector3> gazeDir,List<Vector3> gazePoint,List<string> hittedObj)
    {
        GameObject player = Instantiate(playerPrefab);
        LineRenderer linerenderer = player.GetComponent<LineRenderer>();
        player playerScript = player.GetComponent<player>();
        player.name = name;
        player.transform.position = gazeOrigin[0];
        linerenderer.enabled = true;
        linerenderer.SetPosition(0, gazeOrigin[0]);
        linerenderer.SetPosition(1, gazePoint[0]);
        TextMesh text_ = player.GetComponentInChildren<TextMesh>();
        text_.text = name;

        playerScript.setData(tmstmp, gazeOrigin, gazeDir, gazePoint, hittedObj);
        player.transform.SetParent(playerParent.transform, false);
        listPlayers.Add(player);
        listTmstmp.Add(tmstmp[tmstmp.Count - 1]);
    }

    public void callMovePlayer()
    {
        startBttnClicked =! startBttnClicked;

        foreach (var player_ in playerList)
        {
            player_.playPlayer();
        }
    }


    public float sliderValue=0;
    public void moveSlider(float newValue)
    {
        sliderValue = newValue;
        for (int i=0;i< playerList.Count;i++)
        {

            playerList[i].setPlayerPosition(sliderValue);
        }

        SceneDataHandler.currentTmstp = sliderValue;
        sliderMoved();
    }

    public bool sliderMoved()
    {
        sliderMoved_ = true;
        return sliderMoved_;
        
    }

    public void updateSliderValue()
    {
        float currTmstp = playerList[index_Part_MaxTmstp].getCurrentTmstp();
        sliderMngr.slider.value= currTmstp;
        sliderMoved();

    }

    public void part_player_SetActive(string player_name, bool state)
    {
        GameObject player_act = listPlayers.Find(player => player.name == player_name);
        player_act.GetComponent<player>().gazePointsSetActive(state);
        player_act.SetActive(state);
    }

    public void visualizeGazeToggle(bool toggle)
    {
        for (int i = 0; i < playerList.Count; i++)
        {

            playerList[i].visualizeGaze = toggle;
        }
    }
}