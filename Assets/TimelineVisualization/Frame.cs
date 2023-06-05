using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//each fixation contains this script, it gives any information about the fixation
public class Frame : MonoBehaviour
{

    public Frames frames = new Frames();
    public FrameManager_withoutLayout FrameManager;
    public GameObject aoiHandler;
    public AOI_Handler aoi_Handler_script;
    public string participant;
    public string frameName;
    public float timestamp;
    public Vector3 screenPoint;
    int json_part_index;
    int json_frame_index;
    string AOI;
    public Vector3 gazePoint;
    public string ID;
    public bool real;
    public bool virtual_;
    RawImage frameImage;
    Color defaultColor;
    Color realColor;
    Color virtualColor;
    Color realVirtualColor;
    private void Start()
    {
        frameImage = GetComponent<RawImage>();
        aoiHandler = GameObject.Find("AOIhandler");
        aoi_Handler_script = aoiHandler.GetComponent<AOI_Handler>();
        realColor = aoi_Handler_script.real_color;
        virtualColor = aoi_Handler_script.virtual_color;
        realVirtualColor = aoi_Handler_script.real_virtual_color;
        gazePoint = frames.frameData.gazePoint;
        ID = frames.ID;
        real = frames.frameData.real;
        defaultColor = frameImage.color;
        participant = transform.parent.name;
        timestamp = frames.frameData.timestamp;
        screenPoint = frames.frameData.gazeScreenPoint;
    }


    public void getAnnotation(string annotation, bool real, bool virtual_, bool is_annotated)
    {
        frames.frameData.AOI = annotation;
        frames.frameData.real = real;
        frames.frameData.virtual_ = virtual_;
        frames.frameData.is_annotated = is_annotated;

    }

    public void loadFrameData(List<FrameDataStructure> listJson)
    {
        participant = transform.parent.name;
        frameName = transform.GetChild(0).name;
        string frameTimestamp = frameName.Split('_')[1].Split('.')[0];
        float framestmp = float.Parse(frameTimestamp);
        for (int i = 0; i < listJson.Count; i++)
        {
            if (listJson[i].participant == this.participant)
            {
                json_part_index = i;
                for (int j = 0; j < listJson[i].frames.Count; j++)
                {
                    if (listJson[i].frames[j].frameData.timestamp == framestmp)
                    {
                        json_frame_index = j;
                        frames = listJson[i].frames[j];
                    }
                }

            }
        }
    }
    public Frames saveAnnotation(out int json_part_index, out int json_frame_index)
    {
        json_frame_index = this.json_frame_index;
        json_part_index = this.json_part_index;

        return frames;
    }



    public void colorFrame(bool colored)
    {
        string AOI = frames.frameData.AOI;
        //virtual AOIs defined in the study are now grouped
        Color color;
        string AOI_grouped = null;
        foreach (KeyValuePair<string, List<string>> kvp in AOI_Handler.AOI_grouped_labels)
        {

            if (kvp.Value.Contains(AOI))
            {
                AOI_grouped = kvp.Key;
                break;
            }
        }
        if (AOI_grouped != null && aoi_Handler_script.virtual_AOIs.TryGetValue(AOI_grouped, out color))
        {
            if (colored)
            {
                frameImage.color = new Color(color.r, color.g, color.b, frameImage.color.a);
            }
            else
                frameImage.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, frameImage.color.a);;
        }
        //if its a newly created AOI
        else if (aoi_Handler_script.virtual_AOIs.TryGetValue(AOI, out color))
        {
            if (colored)
                frameImage.color = new Color(color.r, color.g, color.b, frameImage.color.a);
            else
                frameImage.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, frameImage.color.a);
        }
        else if (aoi_Handler_script.realAOIs.TryGetValue(AOI, out color))
        {

            if (colored)
                frameImage.color = new Color(color.r, color.g, color.b, frameImage.color.a);
            else
                frameImage.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, frameImage.color.a);
        }
    }

    public void colorFrameVirtualReal(bool active)
    {

        if (active)
        {
            bool virtual_ = frames.frameData.virtual_;
            bool real_ = frames.frameData.real;
            if (virtual_ && !real_)
            {
                frameImage.color = new Color(virtualColor.r, virtualColor.g, virtualColor.b, frameImage.color.a);
            }
            else if (real_ && !virtual_)
            {
                frameImage.color = new Color(realColor.r, realColor.g, realColor.b, frameImage.color.a);
            }
            else if (real_ && virtual_)
            {
                frameImage.color = new Color(realVirtualColor.r, realVirtualColor.g, realVirtualColor.b, frameImage.color.a);
            }
        }
        else
        {
            frameImage.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, frameImage.color.a);
        }
    }

    public void getGroupedAOI()
    {
        string AOI = frames.frameData.AOI;
        string AOI_grouped = null;
        foreach (KeyValuePair<string, Tuple<List<string>, bool,bool>> kvp in AOI_Handler.AOI_grouped_labels_rv)
        {
            if (kvp.Value.Item1.Contains(AOI))
            {
                AOI_grouped = kvp.Key;
                frames.frameData.AOI = AOI_grouped;
                frames.frameData.virtual_ = kvp.Value.Item2;
                frames.frameData.real = kvp.Value.Item3;
                break;
            }
        }
    }

    public void getAOI_fix_dur(out string part, out string frameAOI, out float fixDur, out bool virt, out bool real)
    {
        frameAOI = frames.frameData.AOI;
        fixDur = frames.frameData.fixation_duration;
        part = transform.parent.name;
        real = frames.frameData.real;
        virt = frames.frameData.virtual_;
    }
}
