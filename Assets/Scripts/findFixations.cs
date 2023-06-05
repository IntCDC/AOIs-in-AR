using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class findFixations : MonoBehaviour
{
    dataHandler datahandler = new dataHandler();
    public List<float> timestmp = new List<float>();
    public List<Vector3> gazeScreenList = new List<Vector3>();
    public List<float> tmstp;
    public List<Vector3> screenCoordinates;
    List<int> listEventIndex;
    List<float> listFixationDuration;
    List<Vector3> listFixationPoint;
    List<string> listHittedObject;
    public List<float> tmstp_fixation;
    public List<Vector3> screenCoordinates_fixation;
    public List<int> listEventIndex_fixation;
    public List<float> listFixationDuration_fixation;
    public List<Vector3> listFixationPoint_fixation;
    public List<string> listHittedObject_fixation;
    private List<string> allDataFiles = new List<string>();
    private List<string> allFixationFiles = new List<string>();
    private List<float> tmstp_fix_start;
    private List<float> tmstp_fix_end;

    public List<int> indexStartTest;
    public List<int> indexEndTest;

    RScriptManager rScriptManager = new RScriptManager();

    void Start()
    {
        datahandler.getDataFiles(out allDataFiles);
        datahandler.getFixationFiles(out allFixationFiles);
        
           if (!File.Exists(allFixationFiles[0]))
        {
            StartCoroutine(rScriptManager.runR(allDataFiles));
        }

    }


    //extract fixations from gaze data by calling rscript file (of ARETT) and writing into a new file named x_fixation.csv
    public void createFixationFile(List<string> dataFilePath)
    {
        startRscript(dataFilePath);
    }

    public void startRscript(List<string> dataFilePath)
    {
        RScriptManager rScriptManager = new RScriptManager();
        StartCoroutine(rScriptManager.runR(dataFilePath));
    }

    public void getTmstp_fixation(string dataFilePath, out List<float> tmstp_fixation, out List<float> tmstp_fix_start, out List<float> tmstp_fix_end, out List<float> fixDuration)
    {
        getFixations(dataFilePath);
        tmstp_fixation = this.tmstp_fixation;
        tmstp_fix_start = this.tmstp_fix_start;
        tmstp_fix_end = this.tmstp_fix_end;
        fixDuration = listFixationDuration_fixation;
    }

    public List<Vector3> getscreenCoord_fixation(string dataFilePath)
    {
        getFixations(dataFilePath);
        return screenCoordinates_fixation;
    }
    public List<Vector3> getFixPoint_fixation(string dataFilePath)
    {
        getFixations(dataFilePath);
        return listFixationPoint_fixation;
    }

    public List<float> getFixDuration_fixation(string dataFilePath)
    {
        getFixations(dataFilePath);
        return listFixationDuration_fixation;
    }
    public List<int> getEventInd_fixation(string dataFilePath)
    {
        getFixations(dataFilePath);
        return listEventIndex_fixation;
    }
  
    //consider only one fixation point for each fixation
    public void getFixations(string dataFilePath)
    {
        datahandler.getDataWithFixations(dataFilePath, out tmstp, out screenCoordinates, out listEventIndex, out listFixationDuration, out listFixationPoint, out listHittedObject);
        int event_count = 0;
        int event_last = listEventIndex[0];
        tmstp_fixation = new List<float>();
        screenCoordinates_fixation = new List<Vector3>();
        listEventIndex_fixation = new List<int>();
        listFixationDuration_fixation = new List<float>();
        listFixationPoint_fixation = new List<Vector3>();
        listHittedObject_fixation = new List<string>();
        tmstp_fix_start = new List<float>();
        tmstp_fix_end = new List<float>();
        indexStartTest = new List<int>();
        indexEndTest = new List<int>();
        for (int i = 0; i < listEventIndex.Count; i++)
        {
            int event_curr = listEventIndex[i];
            if (event_curr == event_last)
            {
                event_count += 1;
            }
            else
            {
                int event_count_end = event_count;
                int event_count_middle = (int)(event_count_end / 2);
                int i_fix = i - 1 - event_count_middle;
                tmstp_fixation.Add(tmstp[i_fix]);
                tmstp_fix_end.Add(tmstp[i - 1]);
                tmstp_fix_start.Add(tmstp[i - event_count_end]);
                indexStartTest.Add(i - event_count_end);
                indexEndTest.Add(i - 1);

                screenCoordinates_fixation.Add(screenCoordinates[i_fix]);
                listEventIndex_fixation.Add(listEventIndex[i_fix]);
                listFixationDuration_fixation.Add(listFixationDuration[i_fix]);
                listFixationPoint_fixation.Add(listFixationPoint[i_fix]);
                string hittObj = listHittedObject[i_fix].Replace("\"", "");
                listHittedObject_fixation.Add(hittObj);


                event_count = 1;
            }
            event_last = event_curr;
        }

    }

    public void getFixationData(string dataFilePath, out List<float> timestmp, out List<Vector3> gazeScreenList, out List<float> listFixationDuration, out List<Vector3> listFixationPoint, out List<string> hittedObj)
    {
        getFixations(dataFilePath);
        timestmp = tmstp_fixation;
        gazeScreenList = screenCoordinates_fixation;
        listFixationDuration = listFixationDuration_fixation;
        listFixationPoint = listFixationPoint_fixation;
        hittedObj = listHittedObject_fixation;

    }

    public void getScreenCoordinates(string dataFilePath, out List<float> tmstp, out List<Vector3> ScreenCoordinates)
    {

        getFixations(dataFilePath);
        tmstp = tmstp_fixation;
        ScreenCoordinates = screenCoordinates_fixation;

    }



}

