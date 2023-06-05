using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;
using System;
public class AOI_Handler : MonoBehaviour
{
    string filename = "AOI_labels.json";
    AOI_label_Datastructure AOIlabelDatastructure = new AOI_label_Datastructure();
    AOI_labels AOI_labels = new AOI_labels();
    ToggleGroup toggleGroup_AOIs;
    Toggle toggle_last_selected_AOI = null;
    public Dictionary<string, Color> virtual_AOIs = new Dictionary<string, Color>();
    public Dictionary<string, Color> realAOIs = new Dictionary<string, Color>();
    public GameObject AOI_Canvas_virtual;
    public GameObject AOI_Canvas_real;
    public GameObject AOI_label_prefab;
    public Toggle toggle_colorAOI;
    public Toggle toggle_VirtualRealAOI;
    public GameObject toggle_AOIs;
    public GameObject toggle_AOIs_group;
    public bool clickedOnAOIbttn = false;
    public GameObject FrameManager;
    FrameManager_withoutLayout frameManagerScript;
    List<GameObject> frameDurObjs = new List<GameObject>();
    public Color real_color;
    public Color virtual_color;
    public Color real_virtual_color;
    FrameAnnotator frameAnnotatorscript;
    public Toggle realToggle;
    public Toggle virtualToggle;

    public Text virt_amount_txt;
    public Text real_amount_txt;
    Dictionary<string, bool> dic_Participants = new Dictionary<string, bool>();

    List<string> AOI_keys = new List<string> { "virt_feather", "virt_roboPix", "virt_hough", "virt_bubble", "virt_lincoln" };

    List<string> realAOI_keys = new List<string> { "real_feather", "real_roboPix", "real_hough", "real_bubble", "real_lincoln", "unknown" };

    List<List<string>> AOI_values = new List<List<string>>() { new List<string>() {"virtual_img_1_plane","virtual_img_1_txt_eng","virtual_img_1_txt_germ"},
            new List<string>() {"virtual_img_2_background","Video_Image","virtual_img_2_txt_eng", "virtual_img_2_txt_germ"} ,
        new List<string>() { "virtual_Img_3_InputImage", "Virtual_Image_3_descr_Image","virtual_img_3","hough_answer_1","hough_answer_2","hough_answer_3","hough_answer_4","virtual_img_3_txt_eng", "virtual_img_3_txt_germ"},
        new List<string>() {"virtual_img_4", "virtual_Img_4_answer1", "virtual_Img_4_answer2", "virtual_Img_4_answer3", "virtual_Img_4_answer4", "virtual_img_4_txt_eng","virtual_img_4_txt_germ"}, new List<string>() {"virtual_img_5","virtual_img_5_txt_eng", "virtual_img_5_txt_germ"}};

    public static List<string> list_virtualAOIs_study = new List<string>() { "virtual_img_1_plane", "virtual_img_1_txt_eng","virtual_img_1_txt_germ","virtual_img_2_background","Video_Image",
        "virtual_img_2_txt_eng", "virtual_img_2_txt_germ","virtual_img_3_InputImage","Virtual_Image_3_descr_Image","virtual_img_3","hough_answer_1","hough_answer_2","hough_answer_3","hough_answer_3","virtual_img_3_txt_eng", "virtual_img_3_txt_germ",
        "virtual_img_4", "virtual_img_4_answer1","virtual_img_4_answer2","virtual_img_4_answer3", "virtual_img_4_answer4","virtual_img_4_txt_eng","virtual_img_4_txt_germ","virtual_img_5","virtual_img_5_txt_eng", "virtual_img_5_txt_germ"};
    int count_colors = 0;

    string[] colors_given = new string[] { "#a6cee3", "#1f78b4", "#b2df8a", "#33a02c", "#fb9a99", "#e31a1c", "#ad5cdb", "#ff7f00", "#36DACA", "#6a3d9a", "#ffff99", "#b15928", "#cab2d6", "#E549E9", "#ABE543", "#fdbf6f", "#cacecf" };
    List<string> createdVirtualAOILabels = new List<string>();
    List<string> createdRealAOILabels = new List<string>();

    public static Dictionary<string, List<string>> AOI_grouped_labels = new Dictionary<string, List<string>>();
    public static Dictionary<string, Tuple<List<string>, bool, bool>> AOI_grouped_labels_rv = new Dictionary<string, Tuple<List<string>, bool, bool>>();

    GameObject AOI_region = null;
    string newAOIFromInput = null;
    public string latestAOI = null;
    bool writeAOIAmount = true;
    public GameObject frameAnnotator;
    int AOI_label_layer;
    public List<Toggle> virtualAOI_ToggleList = new List<Toggle>();
    public List<Toggle> realAOI_ToggleList = new List<Toggle>();
    void Start()
    {
        frameAnnotatorscript = frameAnnotator.GetComponent<FrameAnnotator>();
        AOI_label_layer = LayerMask.NameToLayer("AOI_label");
        AOIlabelDatastructure.AOIlabels = new AOI_labels();
        AOI_labels.realAOIs = new List<AOIs>();
        AOI_labels.virtualAOIs = new List<AOIs>();

        // write from json into dictionary
        if (FileHandler.ReadFromJSON<AOI_label_Datastructure>(filename) != null)
        {
            AOIlabelDatastructure = FileHandler.ReadFromJSON<AOI_label_Datastructure>(filename);
            AOI_labels.virtualAOIs = AOIlabelDatastructure.AOIlabels.virtualAOIs;
            AOI_labels.realAOIs = AOIlabelDatastructure.AOIlabels.realAOIs;

            List<AOIs> realAOIs_ = AOI_labels.realAOIs;
            List<AOIs> virtualAOIs_ = AOI_labels.virtualAOIs;

            for (int i = 0; i < realAOIs_.Count; i++)
            {

                string AOI_name = realAOIs_[i].AOI_name;
                Color color = realAOIs_[i].color;
                bool virtual_ = realAOIs_[i].virtual_;
                bool real = realAOIs_[i].real;
                if (!realAOIs.ContainsKey(AOI_name))
                {
                    realAOIs.Add(AOI_name, color);
                }
                createAOILabel(AOI_name, virtual_, real);
            }

            for (int i = 0; i < virtualAOIs_.Count; i++)
            {
                string AOI_name = virtualAOIs_[i].AOI_name;
                Color color = virtualAOIs_[i].color;
                bool virtual_ = virtualAOIs_[i].virtual_;
                bool real = virtualAOIs_[i].real;
                if (!virtual_AOIs.ContainsKey(AOI_name))
                {
                    virtual_AOIs.Add(AOI_name, color);
                }
                createAOILabel(AOI_name, virtual_, real);
            }
        }
        toggleGroup_AOIs = toggle_AOIs_group.GetComponent<ToggleGroup>();

        for (int i = 0; i < AOI_keys.Count; i++)
        {
            AOI_grouped_labels.Add(AOI_keys[i], AOI_values[i]);
            AOI_grouped_labels_rv.Add(AOI_keys[i], new Tuple<List<string>, bool, bool>(AOI_values[i], true, false));
            createAOILabel(AOI_keys[i], true, false);

        }

        for (int i = 0; i < realAOI_keys.Count; i++)
        {
            createAOILabel(realAOI_keys[i], false, true);

        }

        frameManagerScript = FrameManager.GetComponent<FrameManager_withoutLayout>();

        StartCoroutine(writeAOI_percent());
    }

    public void deselectToggles()
    {
        toggleGroup_AOIs.SetAllTogglesOff();
    }
    void click_AOI_toggle(bool isOn, string toggleName)
    {
        if (isOn)
        {
            SceneDataHandler.AOItoggledOn = true;
            SceneDataHandler.toggledAOIname = toggleName;
            if (realAOIs.ContainsKey(toggleName))
            {
                realToggle.isOn = true;
                virtualToggle.isOn = false;

            }
            else if (virtual_AOIs.ContainsKey(toggleName))
            {
                realToggle.isOn = false;
                virtualToggle.isOn = true;
            }
        }
        else
        {
            SceneDataHandler.AOItoggledOn = false;

            if (realAOIs.ContainsKey(toggleName))
            {
                realToggle.isOn = false;

            }
            else if (virtual_AOIs.ContainsKey(toggleName))
            {
                virtualToggle.isOn = false;

            }
        }
        SceneDataHandler.toggledAOIvirtual = virtualToggle.isOn;
        SceneDataHandler.toggledAOIreal = realToggle.isOn;
    }


    void Update()
    {

        if (newAOIFromInput != FrameAnnotator.newAOIFromInput) //new AOI defined from Timeline
        {
            newAOIFromInput = FrameAnnotator.newAOIFromInput;
            AOI_keys.Add(newAOIFromInput);
        }

        if (frameManagerScript.framesCreated && writeAOIAmount)
        {

            setGroupedAOI_Label();
            writeAOI_amount();
            writeAOIAmount = false;
        }
    }

    public void createAOI_Label_from_Replay()
    {
        if (SceneDataHandler.AOI_region_.Count > 0 && AOI_region != SceneDataHandler.AOI_region_.First().Key) //new AOI region defined from replay
        {
            AOI_region = SceneDataHandler.AOI_region_.First().Key;
            string AOI_key = AOI_region.name;
            bool virtual_ = SceneDataHandler.AOI_region_.First().Value[0];
            bool real = SceneDataHandler.AOI_region_.First().Value[1];
            createAOILabel(AOI_key, virtual_, real);
            AOI_keys.Add(AOI_key);
            writeAOI_amount();

        }
    }
    public void setGroupedAOI_Label()
    {
        frameDurObjs = frameManagerScript.listFrameDurObjects;
        for (int i = 0; i < frameDurObjs.Count; i++)
        {
            Frame frameDurScript = frameDurObjs[i].GetComponent<Frame>();
            frameDurScript.getGroupedAOI();

        }
    }

    public List<string> getAOI_keys()
    {
        List<string> listAOI_key = new List<string>();
        listAOI_key = realAOIs.Keys.Union(virtual_AOIs.Keys).ToList();
        return listAOI_key;
    }

    public void createAOILabel(string AOI_name, bool virtual_, bool real)
    {

        if (!createdRealAOILabels.Contains(AOI_name) && !createdVirtualAOILabels.Contains(AOI_name))
        {
            latestAOI = AOI_name;
        }
        Color AOI_color;
        if (count_colors < colors_given.Length)
        {
            ColorUtility.TryParseHtmlString(colors_given[count_colors], out AOI_color);
            count_colors++;
        }
        else
        {
            AOI_color = (UnityEngine.Random.ColorHSV(0f, 1f, 0.1f, .9f, 0.6f, 0.9f));
        }
        if (virtual_ && !createdVirtualAOILabels.Contains(AOI_name))
        {
            createdVirtualAOILabels.Add(AOI_name);
            if (!virtual_AOIs.ContainsKey(AOI_name))
            {
                if (realAOIs.ContainsKey(AOI_name))
                    AOI_color = realAOIs[AOI_name];
                virtual_AOIs.Add(AOI_name, AOI_color);
                AOIs AOIs = new AOIs(AOI_name, AOI_color, virtual_, real);
                AOI_labels.virtualAOIs.Add(AOIs);
            }
            else
            {
                AOI_color = virtual_AOIs[AOI_name];
            }
            GameObject AOI_label_virtual = Instantiate(AOI_label_prefab);
            AOI_label_virtual.name = AOI_name;
            AOI_label_virtual.layer = AOI_label_layer;
            Toggle AOI_toggle = AOI_label_virtual.transform.GetComponent<Toggle>();
            AOI_toggle.onValueChanged.AddListener((value) => { click_AOI_toggle(value, AOI_toggle.name); });
            AOI_toggle.group = AOI_Canvas_virtual.transform.GetComponent<ToggleGroup>();

            RawImage image = AOI_label_virtual.transform.GetChild(2).GetComponent<RawImage>();
            image.color = AOI_color;
            Text AOI_text = AOI_label_virtual.transform.GetChild(1).GetComponent<Text>();
            AOI_text.text = AOI_name;
            AOI_label_virtual.transform.SetParent(AOI_Canvas_virtual.transform, false);
            virtualAOI_ToggleList.Add(AOI_toggle);
        }

        if (real && !createdRealAOILabels.Contains(AOI_name))
        {
            createdRealAOILabels.Add(AOI_name);
            if (!realAOIs.ContainsKey(AOI_name))
            {
                realAOIs.Add(AOI_name, AOI_color);
                AOIs AOIs = new AOIs(AOI_name, AOI_color, virtual_, real);
                AOI_labels.realAOIs.Add(AOIs);
            }
            else
            {
                AOI_color = realAOIs[AOI_name];
            }
            GameObject AOI_label_real = Instantiate(AOI_label_prefab);
            AOI_label_real.name = AOI_name;
            Toggle AOI_toggle = AOI_label_real.transform.GetComponent<Toggle>();
            AOI_toggle.onValueChanged.AddListener((value) => { click_AOI_toggle(value, AOI_toggle.name); });

            AOI_toggle.group = AOI_Canvas_virtual.transform.GetComponent<ToggleGroup>();

            RawImage image = AOI_label_real.transform.GetChild(2).GetComponent<RawImage>();
            image.color = AOI_color;
            Text AOI_text = AOI_label_real.transform.GetChild(1).GetComponent<Text>();
            AOI_text.text = AOI_name;
            AOI_label_real.transform.SetParent(AOI_Canvas_real.transform, false);
            realAOI_ToggleList.Add(AOI_toggle);

        }

        AOIlabelDatastructure.AOIlabels = AOI_labels;
        FileHandler.SaveToJSON(AOIlabelDatastructure, filename);
    }


    //if color AOIs Toggle is on then color all annotated frames in the corresponding AOI color
    public void ColorFramesOnClickHandler(bool toggle)
    {
        if (toggle)
            toggle_VirtualRealAOI.isOn = false;
        frameDurObjs = frameManagerScript.listFrameDurObjects;
        for (int i = 0; i < frameDurObjs.Count; i++)
        {
            Frame frameDurScript = frameDurObjs[i].GetComponent<Frame>();
            frameDurScript.colorFrame(toggle);
        }
    }

    // if virtual/real AOIs toggle is on then color frames with virtual/real AOIs different
    public void colorVirtualRealAOIsToggle(bool toggle)
    {
        getFramesOfFilteredPart();

        if (toggle)
            toggle_colorAOI.isOn = false;
        frameDurObjs = frameManagerScript.listFrameDurObjects;
        for (int i = 0; i < frameDurObjs.Count; i++)
        {
            Frame frameDurScript = frameDurObjs[i].GetComponent<Frame>();
            frameDurScript.colorFrameVirtualReal(toggle);

        }
    }


    public List<GameObject> getFramesOfFilteredPart()
    {
        frameDurObjs = frameManagerScript.listFrameDurObjects;
        Dictionary<string, bool> participants = participantManager.dic_participants;
        List<string> part_active = new List<string>();
        List<GameObject> frameDur_active_Part = new List<GameObject>();
        string part;
        foreach (string p in participants.Keys)
        {
            if (participants[p])
                part_active.Add(p);
        }

        for (int i = 0; i < frameDurObjs.Count; i++)
        {
            Frame frameScript = frameDurObjs[i].GetComponent<Frame>();
            frameScript.getAOI_fix_dur(out part, out _, out _, out _, out _);
            if (part_active.Contains(part))
            {
                frameDur_active_Part.Add(frameDurObjs[i]);
            }
        }
        return frameDur_active_Part;
    }

    void getAOI_amount(out double virtual_amount, out double real_amount, out Dictionary<string, double> virtualAOIs_amount, out Dictionary<string, double> realAOIs_amount)
    {
        string part;
        string frameAOI;
        float fixDur;
        bool virt;
        bool real;
        float sum_virtual_amount = 0f;
        float sum_real_amount = 0f;
        float sum_all_durations = 0f;
        Dictionary<string, double> dic_AOI_virtual_count = new Dictionary<string, double>();
        Dictionary<string, double> dic_AOI_real_count = new Dictionary<string, double>();
        realAOIs_amount = new Dictionary<string, double>();
        virtualAOIs_amount = new Dictionary<string, double>();




        for (int i = 0; i < virtual_AOIs.Count; i++)
        {
            dic_AOI_virtual_count.Add(virtual_AOIs.ElementAt(i).Key, 0f);
            virtualAOIs_amount.Add(virtual_AOIs.ElementAt(i).Key, 0f);
        }

        for (int i = 0; i < realAOIs.Count; i++)
        {

            dic_AOI_real_count.Add(realAOIs.ElementAt(i).Key, 0f);
            realAOIs_amount.Add(realAOIs.ElementAt(i).Key, 0f);
        }

        frameDurObjs = frameManagerScript.listFrameDurObjects;
        dic_Participants = participantManager.dic_participants;

        for (int i = 0; i < frameDurObjs.Count; i++)
        {
            Frame frameScript = frameDurObjs[i].GetComponent<Frame>();
            frameScript.getAOI_fix_dur(out part, out frameAOI, out fixDur, out virt, out real);
            if (virt && dic_Participants[part])
            {
                sum_virtual_amount = sum_virtual_amount + fixDur;
                if (dic_AOI_virtual_count.ContainsKey(frameAOI))
                {
                    dic_AOI_virtual_count[frameAOI] = dic_AOI_virtual_count[frameAOI] + fixDur;
                }
            }
            if (real && dic_Participants[part])
            {
                sum_real_amount = sum_real_amount + fixDur;
                if (dic_AOI_real_count.ContainsKey(frameAOI))
                {
                    dic_AOI_real_count[frameAOI] = dic_AOI_real_count[frameAOI] + fixDur;
                }
            }

            if (dic_Participants[part])
            {

                sum_all_durations = sum_all_durations + fixDur;
            }

        }

        virtual_amount = Math.Round((sum_virtual_amount / sum_all_durations) * 100, 2, MidpointRounding.AwayFromZero);
        real_amount = Math.Round((sum_real_amount / sum_all_durations) * 100, 2, MidpointRounding.AwayFromZero);
        for (int i = 0; i < realAOIs.Count; i++)
        {
            string AOI_name = realAOIs.ElementAt(i).Key;
            realAOIs_amount[AOI_name] = Math.Round((dic_AOI_real_count[AOI_name] / sum_all_durations) * 100, 2, MidpointRounding.AwayFromZero);
        }
        for (int i = 0; i < virtual_AOIs.Count; i++)
        {
            string AOI_name = virtual_AOIs.ElementAt(i).Key;
            virtualAOIs_amount[AOI_name] = Math.Round((dic_AOI_virtual_count[AOI_name] / sum_all_durations) * 100, 2, MidpointRounding.AwayFromZero);
        }

    }

    //get virtual/real AOIs and individual AOIs amounts in percent

    public IEnumerator writeAOI_percent()
    {
        yield return null;
        writeAOI_amount();
    }
    public void writeAOI_amount()
    {
        getAOI_amount(out double virtual_amount, out double real_amount, out Dictionary<string, double> virtualAOIs_amount, out Dictionary<string, double> realAOIs_amount);
        foreach (Transform child in AOI_Canvas_virtual.transform)
        {
            string legend_AOI = child.transform.GetChild(1).GetComponent<Text>().text.Split(':')[0];
            if (virtualAOIs_amount.TryGetValue(legend_AOI, out double amount))
            {
                child.transform.GetChild(1).GetComponent<Text>().text = legend_AOI + ": " + amount + "%";
            }
        }

        foreach (Transform child in AOI_Canvas_real.transform)
        {
            string legend_AOI = child.transform.GetChild(1).GetComponent<Text>().text.Split(':')[0];
            if (realAOIs_amount.TryGetValue(legend_AOI, out double amount))
            {
                child.transform.GetChild(1).GetComponent<Text>().text = legend_AOI + ": " + amount + "%";
            }
        }
        virt_amount_txt.text = virtual_amount.ToString() + "%";

        real_amount_txt.text = real_amount.ToString() + "%";
    }


    public void createAOI_options()
    {
        foreach (var aoi in realAOI_keys)
        {
            GameObject toggleAOI_elmnt = Instantiate(toggle_AOIs);
            toggleAOI_elmnt.name = aoi;
            Toggle AOI_toggle = toggleAOI_elmnt.transform.GetComponent<Toggle>();
            AOI_toggle.group = toggle_AOIs_group.transform.GetComponent<ToggleGroup>();
            Text AOI_text = toggleAOI_elmnt.transform.GetComponentInChildren<Text>();
            AOI_text.text = aoi;
            toggleAOI_elmnt.transform.SetParent(toggle_AOIs_group.transform, false);
        }
    }

    public void select_AOI_toggle(out string AOI)
    {
        bool toggleOn = AOI_Canvas_virtual.transform.GetComponent<ToggleGroup>().AnyTogglesOn();
        if (toggleOn)
        {
            Toggle selectedToggle = AOI_Canvas_virtual.transform.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault();
            AOI = selectedToggle.name.ToString();
        }
        else
        {
            AOI = null;
        }
    }
}
