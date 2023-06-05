using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class dataHandler
{
    private List<string> allDir;
    public List<int> framesCount;
    public List<string> folderNames;
    public List<string> FolderNames;
    public List<string> allDataFiles;
    public List<string> allVideoFiles;
    public List<string> allFixationFiles;
    RScriptManager RScriptManager = new RScriptManager();

    Dictionary<string, bool> dic_Participants = new Dictionary<string, bool>();


    //get active participants
    public void getActiveParticipants()
    {
        dic_Participants = participantManager.dic_participants;

    }




    //get video and csv data of all participants
    public void getDataFilePath(out List<string> FolderNames, out List<string> allDataFiles, out List<string> allVideoFiles)
    {
        allDataFiles = new List<string>();
        allVideoFiles = new List<string>();
        allDir = getAllDir("study_data");
        FolderNames = getAllFolderNames("study_data");

        for (int i = 0; i < allDir.Count; i++)
        {
            string name = FolderNames[i];
            string dataFile = Path.Combine(allDir[i], name);
            dataFile = dataFile + ".csv";
            string videoFile = Path.Combine(allDir[i], name);
            videoFile = videoFile + ".mp4";
            allDataFiles.Add(dataFile);
            allVideoFiles.Add(videoFile);
        }
    }

    public void getDataFilePath()

    {
        allDataFiles = new List<string>();
        allVideoFiles = new List<string>();
        allFixationFiles = new List<string>();
        allDir = getDirActive("study_data");
        FolderNames = getFolderNamesActive("study_data");

        for (int i = 0; i < allDir.Count; i++)
        {
            string name = FolderNames[i];
            string dir = Path.Combine(allDir[i], name);
            string dataFile = dir + ".csv";
            string videoFile = dir + ".mp4";
            string fixationdataFile = dir + "_fixation.csv";
            allDataFiles.Add(dataFile);
            allVideoFiles.Add(videoFile);
            allFixationFiles.Add(fixationdataFile);
        }
    }

    public void getDataFilePath_forFrames()

    {
        allDataFiles = new List<string>();
        allVideoFiles = new List<string>();
        allFixationFiles = new List<string>();
        allDir = getAllDir("study_data");
        FolderNames = getAllFolderNames("study_data");

        for (int i = 0; i < allDir.Count; i++)
        {
            string name = FolderNames[i];
            string dir = Path.Combine(allDir[i], name);
            string dataFile = dir + ".csv";
            string videoFile = dir + ".mp4";
            string fixationdataFile = dir + "_fixation.csv";
            allDataFiles.Add(dataFile);
            allVideoFiles.Add(videoFile);
            allFixationFiles.Add(fixationdataFile);
        }
    }




    public void getFolderNames(out List<string> FolderNames)
    {
        getDataFilePath();
        FolderNames = this.FolderNames;
    }
    public void getDataFiles(out List<string> allDataFiles)
    {
        getDataFilePath();
        allDataFiles = this.allDataFiles;
    }
    public void getVideoFiles(out List<string> allVideoFiles)
    {
        getDataFilePath();
        allVideoFiles = this.allVideoFiles;
    }

    public void getFixationFiles(out List<string> allFixationFiles)
    {
        getDataFilePath();
        allFixationFiles = this.allFixationFiles;
    }


    public void getDataFiles_Frames(out List<string> allDataFiles)
    {
        getDataFilePath_forFrames();
        allDataFiles = this.allDataFiles;
    }
    public void getFolderFiles_Frames(out List<string> allDataFiles)
    {
        getDataFilePath_forFrames();
        allDataFiles = this.allDataFiles;
    }
    public void getFixationFiles_Frames(out List<string> allFixationFiles)
    {
        getDataFilePath_forFrames();
        allFixationFiles = this.allFixationFiles;
    }

    public void callRScript(MonoBehaviour mono)
    {
        mono.StartCoroutine(RScriptManager.runR(allDataFiles));
    }

    public void getFramesDataPath(out List<string> FramesAllDir, out List<string> FramesFolderNames)
    {
        FramesAllDir = getDirActive("Frames");
        FramesFolderNames = getFolderNamesActive("Frames");
    }

    private List<string> getAllDir(string folder)
    {
        allDir = new List<string>();
        allDir = Directory.GetDirectories(Path.Combine(Application.streamingAssetsPath, folder)).ToList();

        return allDir;
    }

    public List<string> getAllFolderNames(string folder)
    {
        folderNames = Directory.EnumerateDirectories(Path.Combine(Application.streamingAssetsPath, folder))
            .Select(d => new DirectoryInfo(d).Name).ToList();
        return folderNames;
    }

    private List<string> getDirActive(string folder)
    {

        getActiveParticipants();
        allDir = new List<string>();
        allDir = Directory.GetDirectories(Path.Combine(Application.streamingAssetsPath, folder)).ToList();
        List<string> DirectoriesActive = new List<string>();

        if (dic_Participants.Count != 0)
        {
            for (int i = 0; i < allDir.Count; i++)
            {
                string folder_name = new DirectoryInfo(allDir[i]).Name;
                bool state = dic_Participants[folder_name];
                if (state)
                {
                    DirectoriesActive.Add(allDir[i]);
                }
            }
        }

        return DirectoriesActive;
    }

    IEnumerator wait()
    {
        getActiveParticipants();
        yield return new WaitUntil(() => dic_Participants.Count != 0);
    }
    public List<string> getFolderNamesActive(string folder)
    {
        getActiveParticipants();
        folderNames = Directory.EnumerateDirectories(Path.Combine(Application.streamingAssetsPath, folder))
            .Select(d => new DirectoryInfo(d).Name).ToList();
        List<string> folderNameActive = new List<string>();
        if (dic_Participants.Count != 0)
        {
            for (int i = 0; i < folderNames.Count; i++)
            {

                bool state = dic_Participants[folderNames[i]];
                if (state)
                {
                    folderNameActive.Add(folderNames[i]);
                }
            }
        }
        return folderNameActive;
    }

    public List<int> getFramesCount()
    {
        allDir = new List<string>();
        allDir = getDirActive("Frames");
        framesCount = new List<int>();
        for (int i = 0; i < allDir.Count; i++)
        {
            framesCount.Add(Directory.GetFiles(allDir[i], "*.png", SearchOption.AllDirectories).Count());
        }
        return framesCount;
    }


    public void getFramesName(string Dir, out List<string> FrameNames, out List<int> FrameIndexes)
    {
        FrameNames = new List<string>();
        FrameIndexes = new List<int>();
        List<string> FramePaths = new List<String>();
        FramePaths = Directory.GetFiles(Dir, "*.png").ToList();

        foreach (var frame in FramePaths)
        {
            string framename = Path.GetFileName(frame.ToString());
            int frameIndex = int.Parse(frame.Split(new string[] { "_" }, StringSplitOptions.None).Last().Split(new string[] { ".png" }, StringSplitOptions.None)[0].Trim());
            FrameIndexes.Add(frameIndex);
            FrameNames.Add(framename);

        }
    }



    public void getGazeData(string path, out List<float> timestmp, out List<Vector3> gazeOrigin, out List<Vector3> gazeDir, out List<Vector3> gazePoint, out List<string> hittedObj)
    {
        readFromCSV(path, "gaze");
        timestmp = this.timestmp;
        gazeOrigin = this.gazeOrigin;
        gazeDir = this.gazeDir;
        gazePoint = this.gazePoint;
        hittedObj = this.hittedObj;
    }


    int timestampIndex = 0;
    int gazeDataExistIndex = 4;
    int gazeScreenCoordX_Index = 43;
    int gazeScreenCoordY_Index = 44;
    int gazeScreenCoordZ_Index = 45;
    int gazePointX_Index = 18;
    int gazePointY_Index = 19;
    int gazePointZ_Index = 20;
    int gazeOriginIdX_Index = 8;
    int gazeOriginIdY_Index = 9;
    int gazeOriginIdZ_Index = 10;
    int gazeDirX_Index = 11;
    int gazeDirY_Index = 12;
    int gazeDirZ_Index = 13;
    int classification_Index = 78;
    int hittedObj_Index = 21;
    // if dispersion classifier applied than use these parameters:   
    /*
    int eventIndex = 77;
    int eventDuration_Index = 79;
    int fixationX_Index = 80;
    int fixationY_Index = 81;
    int fixationZ_Index = 82;
    */
    //if velocity classifier applied than use these parameters:
    // /* 
    int eventIndex = 79;
    int eventDuration_Index = 81;
    int fixationX_Index = 82;
    int fixationY_Index = 83;
    int fixationZ_Index = 84;
    // */


    DateTime timestamp1, timestamp2, timeStamp;
    long eyeDataTimeStamp;
    float timestampDiff;
    float gazeScreenX, gazeScreenY, gazeScreenZ;
    List<float> timestmp_seconds;
    List<Vector3> gazeOrigin, gazeDir, gazePoint;

    float gazeOriginX, gazeOriginY, gazeOriginZ;
    float gazeDirX, gazeDirY, gazeDirZ;
    float gazePointX, gazePointY, gazePointZ;
    float fixationPointX, fixationPointY, fixationPointZ;
    string classification;
    int fixationIndex;
    float fixationDuration;
    List<Vector3> gazeScreenList;
    List<float> timestmp;
    List<int> listEventIndex;
    List<float> listFixationDuration;
    List<Vector3> listFixationPoint;
    List<string> hittedObj;

    private void readFromCSV(string path, string dType)
    {
        bool firstRun = true;

        ArrayList gazeData = new ArrayList();
        ArrayList gazeData_valid = new ArrayList();
        timestamp1 = new DateTime(0);
        timestamp2 = new DateTime(0);
        timestmp_seconds = new List<float>();
        gazeOrigin = new List<Vector3>();
        gazeDir = new List<Vector3>();
        gazePoint = new List<Vector3>();
        gazeScreenList = new List<Vector3>();
        timestmp = new List<float>();
        listEventIndex = new List<int>();
        listFixationDuration = new List<float>();
        listFixationPoint = new List<Vector3>();
        hittedObj = new List<string>();


        timestampDiff = 0f;
        StreamReader strReader = new StreamReader(path);

        bool endOfFile = false;
        while (!endOfFile)
        {
            string dataString = strReader.ReadLine();
            if (dataString == null)
            {
                endOfFile = true;
                break;
            }
            var data_values = dataString.Split(';');
            gazeData.Add(data_values);

        }

        float timeSeconds;

        if (dType == "fixation")
        {
            foreach (String[] line in gazeData)
            {
                classification = line[classification_Index];

                if (classification.Contains("fixation"))
                {
                    gazeData_valid.Add(line);
                }
            }
        }

        if (dType == "gaze")
        {
            foreach (String[] line in gazeData)
            {
                if (line[gazeDataExistIndex] == "TRUE")
                {
                    gazeData_valid.Add(line);
                }
            }

        }

        foreach (String[] line in gazeData_valid)
        {
            if (firstRun == true)
            {
                //get first timestamp from gazeData (before filtering to fixation) because first datasample could not be a fixation
                string[] gazeData_ = (string[])gazeData[1];
                //first timestamp must not be a fixationData
                long.TryParse(gazeData_[0], out eyeDataTimeStamp);
                timestamp1 = UnixToUtc(eyeDataTimeStamp);
                firstRun = false;


            }

            long.TryParse(line[0], out eyeDataTimeStamp);
            //Convert Unix to normal time 
            timeStamp = UnixToUtc(eyeDataTimeStamp);
            timestamp2 = timeStamp;


            timeSeconds = (timeStamp.Minute * 60f) + (timeStamp.Second) + (timeStamp.Millisecond / 1000f);

            float.TryParse(line[gazeScreenCoordX_Index], out gazeScreenX);
            float.TryParse(line[gazeScreenCoordY_Index], out gazeScreenY);
            float.TryParse(line[gazeScreenCoordZ_Index], out gazeScreenZ);

            float.TryParse(line[gazeOriginIdX_Index], out gazeOriginX);
            float.TryParse(line[gazeOriginIdY_Index], out gazeOriginY);
            float.TryParse(line[gazeOriginIdZ_Index], out gazeOriginZ);
            float.TryParse(line[gazeDirX_Index], out gazeDirX);
            float.TryParse(line[gazeDirY_Index], out gazeDirY);
            float.TryParse(line[gazeDirZ_Index], out gazeDirZ);
            float.TryParse(line[gazePointX_Index], out gazePointX);
            float.TryParse(line[gazePointY_Index], out gazePointY);
            float.TryParse(line[gazePointZ_Index], out gazePointZ);

            int.TryParse(line[eventIndex], out fixationIndex);
            float.TryParse(line[eventDuration_Index], out fixationDuration);
            float.TryParse(line[fixationX_Index], out fixationPointX);
            float.TryParse(line[fixationY_Index], out fixationPointY);
            float.TryParse(line[fixationZ_Index], out fixationPointZ);
            timestampDiff = (float)(timestampDiff + (timestamp2.Subtract(timestamp1)).TotalMilliseconds);
            timestamp1 = timeStamp;

            Vector3 gazeScreen = new Vector3(gazeScreenX, gazeScreenY, gazeScreenZ);
            Vector3 gazeOriginCur = new Vector3(gazeOriginX, gazeOriginY, gazeOriginZ);
            Vector3 gazeDirCur = new Vector3(gazeDirX, gazeDirY, gazeDirZ);
            Vector3 gazePointCur = new Vector3(gazePointX, gazePointY, gazePointZ);
            Vector3 fixationPointCur = new Vector3(fixationPointX, fixationPointY, fixationPointZ);
            gazeScreenList.Add(gazeScreen);
            timestmp.Add(timestampDiff);
            timestmp_seconds.Add(timeSeconds);
            gazeOrigin.Add(gazeOriginCur);
            gazeDir.Add(gazeDirCur);
            gazePoint.Add(gazePointCur);
            listEventIndex.Add(fixationIndex);
            listFixationDuration.Add(fixationDuration);
            listFixationPoint.Add(fixationPointCur);
            hittedObj.Add(line[hittedObj_Index]);
        }
    }



    public void getDataWithFixations(string path, out List<float> timestmp, out List<Vector3> gazeScreenList, out List<int> listEventIndex, out List<float> listFixationDuration, out List<Vector3> listFixationPoint, out List<string> hittedObj)
    {
        readFromCSV(path, "fixation");

        gazeScreenList = this.gazeScreenList;
        timestmp = this.timestmp;
        listEventIndex = this.listEventIndex;
        listFixationDuration = this.listFixationDuration;
        listFixationPoint = this.listFixationPoint;
        hittedObj = this.hittedObj;

    }

    DateTime UnixToUtc(long unixTime)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(unixTime);
        DateTime dateTime = dateTimeOffset.UtcDateTime;
        return dateTime;
    }

}
