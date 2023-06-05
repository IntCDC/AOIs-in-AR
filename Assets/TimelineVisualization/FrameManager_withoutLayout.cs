
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//using TMPro;
public class FrameManager_withoutLayout : MonoBehaviour
{
    findFixations fixationScript = new findFixations();
    dataHandler datahandler = new dataHandler();
    [SerializeField] string filename;
    List<FrameDataStructure> entries = new List<FrameDataStructure>();
    public ListFrame listFrame = new ListFrame();
    public Slider slider;
    public GameObject framePrefab;
    public GameObject participantPrefab;
    public GameObject partTextPrefab;
    public GameObject canvas; //I use this to set the canvas after assigning this script to an empty gameobject
    public GameObject panel; // I do the same for the panel
    public GameObject participant_panel;
    public GameObject frameDurationPrefab;
    public GameObject fixDurationPrefab;
    public GameObject frameWithoutScriptPrefab;
    public GameObject timeline_time_prefab;
    public GameObject TimelineContent;
    int testI = 0;
    private Texture2D thisTexture;
    byte[] bytes;
    private Vector2 ImageViewCount;
    public Vector2 ImageViewSize; 
    public Vector2 InitialImageViewPosition;
    public Vector2 ImageViewPositionOffset; 
    public static List<GameObject> listFrameObjects = new List<GameObject>();
    public List<GameObject> listFrameDurObjects = new List<GameObject>();
    private List<string> allframes;
    private List<string> allDir = new List<string>();
    private List<string> FolderNames = new List<string>();
    private List<int> framesCount = new List<int>();
    private bool participantAdded = false;
    private bool ZoomingChanged = false;
    Vector2 txtParticipantSize;
    private List<string> allFixationFiles = new List<string>();
    List<float> timestmp;
    List<float> timestmp_;
    List<float> timestmp_fix_strt;
    List<float> timestmp_fix_end;
    List<float> fix_duration;
    List<Vector3> gazeScreenList;
    List<float> listFixationDuration;
    List<Vector3> listFixationPoint;
    List<string> hittedObj;
    int json_part_index;
    int json_frame_index;
    RectTransform sliderRectTransform;
    float sliderSizeY;
    List<float> list_frameXPosition = new List<float>();
    List<float> list_frameXPosStart = new List<float>();
    List<float> list_fix_end = new List<float>();
    List<float> list_frames_fix_start = new List<float>();
    List<float> list_frames_fix_end = new List<float>();
    List<GameObject> list_timeline_intervals = new List<GameObject>();
    List<GameObject> list_part_frames = new List<GameObject>();

    public bool framesCreated = false;

    void Start()
    {
        findFixations fixationScript = GetComponent<findFixations>();
        datahandler.getFixationFiles(out allFixationFiles);
        listFrame.framesData = new List<FrameDataStructure>();
        if (FileHandler.ReadFromJSON<ListFrame>(filename) != null)
        {
            listFrame = FileHandler.ReadFromJSON<ListFrame>(filename);
        }

        GenerateImageView();
        buildTimelineUI();
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

    }


    private void Update()
    {
        SceneDataHandler.timeline_slider_moved = false;
        if (SceneDataHandler.replay_slider_moved && SceneDataHandler.currentTmstp>= slider.minValue && SceneDataHandler.currentTmstp <= slider.maxValue)
        {
            slider.value = SceneDataHandler.currentTmstp;
        }

        RectTransform contentRect = TimelineContent.GetComponent<RectTransform>();

        if (contentRect.transform.localScale.x <= 0.0016)
        {

            foreach (GameObject line in list_timeline_intervals)
            {
                line.SetActive(false);
            }
        }
        else
        {
            ZoomingChanged = false;
            foreach (GameObject line in list_timeline_intervals)
            {
                line.SetActive(true);
            }
        }

    }

    private void buildTimelineUI()
    {
        //size of panel needs to be equal to imagesequence length to enable scrolling
        RectTransform rectTransformPanel = panel.GetComponent<RectTransform>();
        RectTransform rectTransformPartPanel = participant_panel.GetComponent<RectTransform>();
        float panelSizeX = list_frameXPosition.Max() + ImageViewSize.x;
        float panelSizeY = (ImageViewPositionOffset.y * allDir.Count) + ImageViewSize.y;


        rectTransformPanel.sizeDelta = new Vector2(panelSizeX, panelSizeY);
        Vector2 partSize = partTextPrefab.GetComponent<RectTransform>().sizeDelta;
        //set slider position
        sliderRectTransform = slider.GetComponent<RectTransform>();
        sliderRectTransform.anchoredPosition = new Vector2(0, 0);

        sliderSizeY = sliderRectTransform.sizeDelta.y;
        rectTransformPartPanel.sizeDelta = new Vector2(0, panelSizeY + sliderSizeY);


        sliderRectTransform.sizeDelta = new Vector2(list_frameXPosition.Max(), sliderSizeY);

        slider.maxValue = list_frameXPosition.Max();

        int time_breaks_count = (int)(slider.maxValue/ 1000);
        for (int i = 0; i < time_breaks_count*2; i++)
        {
            GameObject timelineInterval = Instantiate(timeline_time_prefab);
            timelineInterval.transform.SetParent(slider.transform, false);
            RectTransform time_line = timelineInterval.GetComponent<RectTransform>();
            int time_break = i * 500;
            //add 9 bc 
            time_line.anchoredPosition = new Vector2(9 + time_break, 0);

            Text time = timelineInterval.GetComponentInChildren<Text>();
            TimeSpan t = TimeSpan.FromMilliseconds(time_break);
            string t_ = t.ToString();
            string answer = string.Format("{0:D2}:{1:D2}:{2:D2}m",
               t.Minutes,
               t.Seconds,
               t.Milliseconds);
            time.text = answer;//t_;
            list_timeline_intervals.Add(timelineInterval);
        }
    }



    public void moveTimelineSlider(float value)
    {
            SceneDataHandler.currentTmstp = value;
            SceneDataHandler.timeline_slider_moved = true;       
    }


    public void saveToJson()
    {
        foreach (var frameobj in listFrameObjects)
        {
            Frames frames_of_obj = frameobj.GetComponentInParent<Frame>().saveAnnotation(out json_part_index, out json_frame_index);
            listFrame.framesData[json_part_index].frames[json_frame_index] = frames_of_obj;

        }
        FileHandler.SaveToJSON<ListFrame>(listFrame, "frames.json");
    }

    void GenerateImageView()
    {
        datahandler.getFramesDataPath(out allDir, out FolderNames);
        datahandler.getFixationFiles(out allFixationFiles);
        framesCount = datahandler.getFramesCount();
        ImageViewCount = new Vector2(framesCount.Max(), allDir.Count);
        List<float> list_maxScreenCoord_z = new List<float>();

        for (int i = 0; i < allFixationFiles.Count; i++)
        {
            float ScreenCord_max = new List<Vector3>(fixationScript.getscreenCoord_fixation(allFixationFiles[i])).Max(v => v.z);
            list_maxScreenCoord_z.Add(ScreenCord_max);
        }
        float max_screenZ_value = list_maxScreenCoord_z.Max();
        for (int a = 0; a < ImageViewCount.y; a++)
        {
            List<string> FrameNames = new List<string>();
            List<int> FrameIndexes = new List<int>();
            timestmp = new List<float>();
            timestmp_ = new List<float>();
            timestmp_fix_strt = new List<float>();
            timestmp_fix_end = new List<float>();
            fix_duration = new List<float>();

            gazeScreenList = new List<Vector3>();
            listFixationDuration = new List<float>();
            listFixationPoint = new List<Vector3>();
            hittedObj = new List<string>();
            datahandler.getFramesName(allDir[a], out FrameNames, out FrameIndexes);

            //all frames of participant are sorted in right order
            allframes = Directory.GetFiles(allDir[a], "*.png", SearchOption.AllDirectories).OrderBy(f => f).ToList();
            allframes.Sort((x, y) => NaturalComparer(x, y));
            string participant = FolderNames[a].ToString();

            GameObject part_frames = Instantiate(participantPrefab);
            part_frames.name = participant;
            part_frames.transform.SetParent(panel.transform, false);

            list_part_frames.Add(part_frames);

            GameObject text_participant = Instantiate(partTextPrefab);
            RectTransform partRectTransform = text_participant.GetComponent<RectTransform>();

            text_participant.name = participant;
            text_participant.transform.GetChild(0).name = participant;

            text_participant.GetComponentInChildren<Text>().text = participant;
            partRectTransform.anchoredPosition = new Vector2(InitialImageViewPosition.x, InitialImageViewPosition.y - (ImageViewPositionOffset.y * a) + sliderSizeY);
            text_participant.transform.SetParent(participant_panel.transform, false);
            txtParticipantSize = partRectTransform.sizeDelta;
            FrameNames.Sort((x, y) => NaturalComparer(x, y));

            //get timestmp of fixationpoint to move frame in the timeline
            fixationScript.getTmstp_fixation(allFixationFiles[a], out timestmp_, out timestmp_fix_strt, out timestmp_fix_end, out fix_duration);
 
            string frame_tmstmp = FrameNames[0].Split('_')[1].Split('.')[0];
            float t = float.Parse(frame_tmstmp);
           
            List<float> timestmp_new = timestmp_.Skip(timestmp_.IndexOf(t)).ToList(); //timestmp_.SkipWhile(x => !x.Equals(t)).ToList();
            List<float> timestmp_fix_strt_new = timestmp_fix_strt.Skip(timestmp_.IndexOf(t)).ToList();
            List<float> timestmp_fix_end_new = timestmp_fix_end.Skip(timestmp_.IndexOf(t)).ToList();
            List<float> fix_duration_end = fix_duration.Skip(timestmp_.IndexOf(t)).ToList();

            List<Vector3> screenCoord = fixationScript.getscreenCoord_fixation(allFixationFiles[a]).Skip(timestmp_.IndexOf(t)).ToList();
            SceneDataHandler.timestmp = timestmp_;

            //if participant is not already in json than add it
            if (participantToJson(FolderNames[a]))
            {
                addParticipantToList(FolderNames[a], FrameNames, FrameIndexes, allFixationFiles[a]);
            }


            for (int b = 0; b < framesCount[a]; b++)
            {
                float frameDur_xPos = InitialImageViewPosition.x + timestmp_fix_strt_new[b];
                float frameXPosition = InitialImageViewPosition.x + (timestmp_new[b] - timestmp_fix_strt_new[b]);
                float distFrame_factor = (screenCoord[b].z / max_screenZ_value);
                fixDurPlusFrameBuilder(distFrame_factor, allframes[b], ImageViewSize.x, new Vector2(frameDur_xPos, partRectTransform.anchoredPosition.y), fix_duration_end[b], part_frames.transform, frameXPosition);
                float frameDur_xPos_length = InitialImageViewPosition.x + timestmp_fix_strt_new[b] + fix_duration_end[b];
                list_frameXPosition.Add(frameDur_xPos_length);
                list_fix_end.Add(frameDur_xPos);

            }

            float pos = InitialImageViewPosition.x + timestmp_fix_strt_new[0];
            list_frameXPosStart.Add(pos);

        }

        if (participantAdded)
        {
            FileHandler.SaveToJSON(listFrame, filename);
        }

        framesCreated = true;
    }

    private void fixDurPlusFrameBuilder(float distFrame, string imagePath, float frameHeight, Vector2 position, float fix_duration, Transform frameDurViewGroup, float frameXPosition)
    {
        Vector2 size = new Vector2(fix_duration, frameHeight * (1 - distFrame));
        string imageName = Path.GetFileName(imagePath);
        //create GameObject for each frame
        GameObject frameDurView = Instantiate(frameDurationPrefab);
        frameDurView.name = Path.GetFileNameWithoutExtension(imagePath) + "_duration.png";
        Frame frameScript = frameDurView.GetComponent<Frame>();
        listFrameDurObjects.Add(frameDurView.gameObject);

        RectTransform rectTransform = frameDurView.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;
        frameDurView.transform.SetParent(frameDurViewGroup, false);
        ImageViewBuilder(distFrame, ImageViewSize,
                new Vector2(frameXPosition, 0f),
                panel.transform, imagePath, frameDurView.transform);
        frameScript.loadFrameData(listFrame.framesData);
    }

    private void frameDurationBuilder(string imagePath, float frameHeight, Vector2 position, float fix_duration, Transform frameDurViewGroup)
    {
        Vector2 size = new Vector2(fix_duration, frameHeight);
        string imageName = Path.GetFileName(imagePath);
        //create GameObject for each frame
        GameObject frameDurView = Instantiate(frameDurationPrefab);
        frameDurView.name = Path.GetFileNameWithoutExtension(imagePath) + "_duration.png";
        Frame frameScript = frameDurView.GetComponent<Frame>();
        listFrameDurObjects.Add(frameDurView.gameObject);

        RectTransform rectTransform = frameDurView.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;
        frameDurView.transform.SetParent(frameDurViewGroup, false);
        frameScript.loadFrameData(listFrame.framesData);

    }

    public void addDataToFrame(out List<FrameDataStructure> listJson)
    {
        listJson = listFrame.framesData;
    }

    public bool participantToJson(string participant)
    {
        for (int i = 0; i < listFrame.framesData.Count; i++)
        {
            if (listFrame.framesData[i].participant == participant)
            {
                return false;
            }
        }
        return true;
    }

    public void addParticipantToList(string participant, List<string> frameNames, List<int> frameIndexes, string FixationFiles)
    {
        fixationScript.getFixationData(FixationFiles, out timestmp, out gazeScreenList, out listFixationDuration, out listFixationPoint, out hittedObj);
        FrameDataStructure framedata = new FrameDataStructure();
        List<Frames> listFrames = new List<Frames>();
        List<string> frameNames_ = new List<string>();
        List<float> timestmp_ = new List<float>();
        List<Vector3> gazeScreenList_ = new List<Vector3>();
        List<float> listFixationDuration_ = new List<float>();
        List<Vector3> listFixationPoint_ = new List<Vector3>();
        List<string> hittedObj_ = new List<string>();

        foreach (var frame in frameNames)
        {
            string frame_tmstmp = frame.Split('_')[1].Split('.')[0];
            float t = float.Parse(frame_tmstmp);
            int index = timestmp.IndexOf(t);
            timestmp_.Add(timestmp[index]);
            listFixationDuration_.Add(listFixationDuration[index]);
            gazeScreenList_.Add(gazeScreenList[index]);
            listFixationPoint_.Add(listFixationPoint[index]);
            hittedObj_.Add(hittedObj[index]);
            list_frames_fix_start.Add(timestmp[index]);

        }

        listFrames = addFramesToList(frameNames, frameIndexes, timestmp_, gazeScreenList_, listFixationDuration_, listFixationPoint_, hittedObj_);

        framedata.participant = participant;
        framedata.frames = listFrames;
        listFrame.framesData.Add(framedata);
        participantAdded = true;


    }

    public List<Frames> addFramesToList(List<string> frameNames, List<int> frameIndexes, List<float> timestmpList, List<Vector3> gazeScreenList, List<float> listFixationDuration, List<Vector3> listFixationPoint, List<string> hittedObj)
    {
        List<Frames> listFrames = new List<Frames>();
        for (int i = 0; i < frameNames.Count; i++)
        {
            string frameName = frameNames[i];
            string ID = System.Guid.NewGuid().ToString();
            float timestmp = timestmpList[i];
            Vector3 gazeScreen = gazeScreenList[i];
            float fixationDuration = listFixationDuration[i];
            Vector3 fixationPoint = listFixationPoint[i];
            string AOI = hittedObj[i];
            int frameIndex = frameIndexes[i];
            bool is_annotated = false;
            bool virtual_ = false;
            bool real = false;
            if (!AOI.Contains("SpatialMesh"))
            {
                is_annotated = true;
                if (AOI_Handler.list_virtualAOIs_study.Contains(AOI))
                {
                    virtual_ = true;
                    real = false;
                }
            }
            FrameData frameData = new FrameData(timestmp, fixationDuration, fixationPoint, gazeScreen, AOI, is_annotated, virtual_, real);

            Frames frames = new Frames(frameName, ID, frameIndex, frameData);
            listFrames.Add(frames);
        }

        return listFrames;

    }

    public void findCorrectFrame(string participant, int framesCount, List<GameObject> listFrameObjects)
    {
        for (int i = 0; i < listFrame.framesData.Count; i++)
        {
            if (listFrame.framesData[i].participant == participant)
            {
                for (int j = 0; j < framesCount; j++)
                {
                    listFrameObjects[j].GetComponent<Frame>().frames.ID = listFrame.framesData[i].frames[j].ID;

                }
            }
        }
    }


    //create FrameObject
    void ImageViewBuilder(float frameDist, Vector2 size, Vector2 position, Transform objectToSetImageView, string imagePath, Transform imageViewGroup)
    {

        string imageName = Path.GetFileName(imagePath);
        //create GameObject for each frame
        GameObject imageView = Instantiate(frameWithoutScriptPrefab);
        imageView.name = Path.GetFileName(imagePath);
        RawImage image = imageView.GetComponent<RawImage>();
        StartCoroutine(loadImage(imagePath, image));

        RectTransform rectTransform = imageView.GetComponent<RectTransform>();
        rectTransform.sizeDelta = (size * (1 - frameDist));
        rectTransform.anchoredPosition = position;
        imageView.transform.SetParent(imageViewGroup, false);
        listFrameObjects.Add(imageView.gameObject);
    }

    IEnumerator loadImage(string imagePath, RawImage image)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imagePath))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                thisTexture = new Texture2D(100, 100);
                thisTexture = DownloadHandlerTexture.GetContent(uwr);
                image.texture = thisTexture;
                testI++;
            }
        }
    }



    //sort the imageframes in the correct order
    static int NaturalComparer(string a, string b)
    {
        var num1 = a.Split(new string[] { "_" }, StringSplitOptions.None).Last().Split(new string[] { ".png" }, StringSplitOptions.None)[0].Trim();
        var num2 = b.Split(new string[] { "_" }, StringSplitOptions.None).Last().Split(new string[] { ".png" }, StringSplitOptions.None)[0].Trim();
        try
        {
            int n1 = int.Parse(num1), n2 = int.Parse(num2);
            if (n1 < n2) return -1;
            if (n1 > n2) return +1;
        }
        catch (FormatException e)
        {
            Debug.Log(e.Message);
        }
        return 0;
    }


    public void part_frames_SetActivate(string part_name, bool state)
    {

        GameObject part_act = list_part_frames.Find(part => part.name == part_name);
        part_act.SetActive(state);
    }


    void LoadScene()
    {
        GenerateImageView();
    }

}
