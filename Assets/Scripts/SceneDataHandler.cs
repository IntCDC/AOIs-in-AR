using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneDataHandler : MonoBehaviour
{
    public static string testString;
    public static List<float> timestmp;
    public static float currentTmstp;
    public static bool timeline_slider_moved = false;
    public static bool replay_slider_moved = false;
    public static Vector3 gazePoint;
    public static bool FrameClicked=false;
    public static bool framesAnnotated = false;
    public static string hittedFrame_ID;
    public static GameObject AOI_region=null;
    public static Dictionary<GameObject, bool[]> AOI_region_ = new Dictionary<GameObject, bool[]>();
    public static bool participant_setActive = false;
    public static Dictionary<string, bool> participant_active = new Dictionary<string, bool>();
    public static bool AOItoggledOn;
    public static bool toggledAOIvirtual;
    public static bool toggledAOIreal;
    public static string toggledAOIname;
    public static Dictionary<string, Tuple<bool, Vector3>> FramesToShowInReplay = new Dictionary<string, Tuple<bool, Vector3>>();
}
