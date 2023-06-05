using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class participantManager : MonoBehaviour
{
    public GameObject participant;
    public GameObject part_parent;
    public GameObject FrameManager;
    public GameObject AOI_Mnger;
    AOI_Handler AOI_handler;
    FrameManager_withoutLayout frame_mnger_Script;
    dataHandler datahandler = new dataHandler();
    List<string> part_Names = new List<string>();
    List<GameObject> part_Objects = new List<GameObject>(); 
    public static Dictionary<string, bool> dic_participants = new Dictionary<string, bool>();
    void Start()
    {
        frame_mnger_Script = FrameManager.GetComponent<FrameManager_withoutLayout>();
        AOI_handler = AOI_Mnger.GetComponent<AOI_Handler>();
        part_Names = datahandler.getAllFolderNames("study_data");
        for (int i = 0; i < part_Names.Count; i++)
        {
            string partName = part_Names[i];
            createParticipantLabel(partName);
            Toggle t = part_Objects[i].GetComponent<Toggle>();
            t.isOn = true;
            t.onValueChanged.AddListener(delegate
            {
                ToggleValueChanged(t);
            });

        }

        //save state of participants in dictionary
        dic_participants = new Dictionary<string, bool>();
        for (int i = 0; i < part_Objects.Count; i++)
        {
            string part_name = part_Objects[i].name;
            bool state = part_Objects[i].GetComponent<Toggle>().isOn;
            dic_participants.Add(part_name, state);
        }
    }
    void Update()
    {
        
    }
    
    public void changeParticipantState(string part_name, bool state)
    {
        dic_participants[part_name] = state;
    }


    //Send toggle information to dataHandler to know which participants are activated
    void ToggleValueChanged(Toggle participant)
    {        
        string part_name = participant.name;
        bool part_state = participant.isOn;
        changeParticipantState(part_name, part_state);
        frame_mnger_Script.part_frames_SetActivate(part_name, part_state);
        SceneDataHandler.participant_active = new Dictionary<string, bool>();
        SceneDataHandler.participant_active.Add(part_name, part_state);
        SceneDataHandler.participant_setActive = true;
        AOI_handler.writeAOI_amount();
    }

    private void createParticipantLabel(string part_name)
    {
        GameObject part_label = Instantiate(participant);
        part_label.name = part_name;
        Text AOI_text = part_label.transform.GetComponentInChildren<Text>();
        AOI_text.text = part_name;
        part_label.transform.SetParent(part_parent.transform, false);
        part_Objects.Add(part_label);
    }
}
