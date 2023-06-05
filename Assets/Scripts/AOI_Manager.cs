using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AOI_Manager : MonoBehaviour
{
    string filename = "AOIregions.json";
    AOIRegion_Datastructure AOI_datastructure = new AOIRegion_Datastructure();
    AOI_region AOI_region_data;
    List<string> AOI_region_names = new List<string>();
    public GameObject frameRegion_Canvas;
    public Camera camera;
    public GameObject cube;
    public GameObject AOI_region_parent;
    public Button defineAOIbttn;
    public Button scaleAOIbttn;
    public Button moveAOIbttn;
    public Button editAOIbttn;
    public Button saveAOIbttn;
    public Toggle virtual_Toggle;
    public Toggle real_Toggle;
    public Toggle edit_Toggle;
    Ray screen_point;
    bool defineAOI_ = false;
    bool editAOI = false;
    bool moveAOI = false;
    bool scaleAOI = false;
    bool editAOI_select = false;
    bool editFunctionOn = false;
    int AOI_Layer;
    Vector3 view_point;
    public LayerMask AOI_mask;
    public LayerMask AOI_region_mask;
    public LayerMask Replay_mask;
    public LayerMask moveAxis_mask;
    public LayerMask scaleAxis_mask;
    GameObject object_edit;
    GameObject AOI_region;
    public float sizingFactor = 0.03f;
    public InputField AOI_name;
    public float move_offset = 0.001f;
    public static bool AOI_saved = false;
    public Vector3 InitialPos;
    public object InitialScale;
    bool moveAxis_hitted = false;

    private RaycastHit m_RayCastHit;
    private GameObject m_CurrentObject;
    private Transform current_axis;
    private Vector3 m_LastMousePos;
    private float m_DeltaTime = 0.001f;
    public float move_deltaTime = 1;
    private float curr_dist;
    
    private float start_dist;
    string AOI_name_from_legend;
   
    



    void Start()
    {
        AOI_datastructure.list_AOIregions = new List<AOI_region>();
        loadAOIregionsFromJSON();
        string replayLayer = "replayLayer";
        AOI_Layer = LayerMask.NameToLayer("AOI_region");
    }

    public void loadAOIregionsFromJSON()
    {
        if (FileHandler.ReadFromJSON<AOIRegion_Datastructure>(filename) != null)
        {
            AOI_datastructure = FileHandler.ReadFromJSON<AOIRegion_Datastructure>(filename);
            for (int i = 0; i < AOI_datastructure.list_AOIregions.Count; i++)
            {
                AOI_region_names.Add(AOI_datastructure.list_AOIregions[i].name);
            }
        }
    }


    // handles how the AOI cubes are defined, created, scaled and moved within gaze replay
    void Update()
    {
        view_point = camera.ScreenToViewportPoint(Input.mousePosition);
        screen_point = camera.ScreenPointToRay(Input.mousePosition);
        if (editAOI_select)
        {
            if (Input.GetMouseButtonDown(0))
            {
                editAOIbttn.interactable = true;
                if (Physics.Raycast(screen_point, out RaycastHit hit, 1000, AOI_region_mask.value) && isWithinViewport())
                {
                    object_edit = hit.collider.gameObject;
                    saveAOIbttn.interactable = true;
                    editAOI_select = false;
                }
            }
        }

        if (SceneDataHandler.AOItoggledOn)
        {
            AOItoggleChanged();
            SceneDataHandler.AOItoggledOn = false;
        }
        if (defineAOI_)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(screen_point, out RaycastHit hit, 1000, Replay_mask.value) && isWithinViewport())
                {

                    createAOI(hit);
                    defineAOI_ = false;
                    editAOIbttn.interactable = true;
                    saveAOIbttn.interactable = true;
                }
            }
        }
        if (editAOI)
        {
            
            if (Input.GetMouseButton(0) && !moveAOI && !scaleAOI && object_edit)
            {
                if (Physics.Raycast(screen_point.origin, screen_point.direction, out RaycastHit hit2, 1000, Replay_mask.value) && isWithinViewport())
                {
                    object_edit.transform.position = new Vector3(hit2.point.x, hit2.point.y, hit2.point.z);
                }
            }
            if (moveAOI)
            {
                // moveAOI_old();  // is freemoving, without axis
                moveAOI_(); // moves only in direction of the hitted axis
            }

            if (Input.GetKeyDown(KeyCode.Delete) && object_edit != null)
            {
                GameObject.Destroy(object_edit);
                clearSettings();
            }


            if (scaleAOI)
            {
                scaleAOI_();  // scales only in direction of the axis
            //  scaleAOI_old();// scales in 2 directions, not in z direction, without axis
            }

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
    bool zAxis_b = false;
    bool xAxis_b = false;

    public static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }


    //move AOI cube in different directions using the axes
    public void moveAOI_()
    {

        if (Input.GetMouseButtonDown(0) && editAOI && moveAOI && object_edit)
        {
            object_edit.transform.GetChild(0).transform.gameObject.SetActive(true);
            if (Physics.Raycast(screen_point, out RaycastHit hit, 1000, moveAxis_mask.value) && isWithinViewport())
            {
                moveAxis_hitted = true;
                m_LastMousePos = hit.point;
                current_axis = hit.transform.parent;
                start_dist = Vector3.Distance(object_edit.transform.position, hit.point);
            }
            else
                moveAxis_hitted = false;
        }
        if (Input.GetMouseButton(0) && moveAOI && object_edit&&moveAxis_hitted)
        {
            Transform xAxis = object_edit.transform.Find("moveAxis/AxisPointerX");
            Transform yAxis = object_edit.transform.Find("moveAxis/AxisPointerY");
            Transform zAxis = object_edit.transform.Find("moveAxis/AxisPointerZ");
            float object_edit_rot = WrapAngle(object_edit.transform.localEulerAngles.y);
            //the definition of the axes depends on which wall the cube is created (can be figured out by the rotation of the created cube)

            switch (object_edit_rot)
            {
                case float n when (n >= -100f && n <= -70f):
                    zAxis_b = true;
                    xAxis_b = false;
                    zAxis = object_edit.transform.Find("moveAxis/AxisPointerX");
                    yAxis = object_edit.transform.Find("moveAxis/AxisPointerY");
                    xAxis = object_edit.transform.Find("moveAxis/AxisPointerZ");
                    break;

                case float n when (n >= 70f && n <= 100f):
                    zAxis_b = false;
                    xAxis_b = true;
                    zAxis = object_edit.transform.Find("moveAxis/AxisPointerX");
                    yAxis = object_edit.transform.Find("moveAxis/AxisPointerY");
                    xAxis = object_edit.transform.Find("moveAxis/AxisPointerZ");
                    break;

                case float n when (n >= 150f && n <= 200f):
                    zAxis_b = false;
                    xAxis_b = false;
                    xAxis = object_edit.transform.Find("moveAxis/AxisPointerX");
                    yAxis = object_edit.transform.Find("moveAxis/AxisPointerY");
                    zAxis = object_edit.transform.Find("moveAxis/AxisPointerZ");
                    break;
            }

            if (Physics.Raycast(screen_point.origin, screen_point.direction, out RaycastHit hit, 1000) && isWithinViewport())
            {
                Transform axis = hit.collider.gameObject.transform;
                curr_dist = Vector3.Distance(object_edit.transform.position, hit.point);
                Vector3 pos = object_edit.transform.position;
                if ((curr_dist - start_dist)> move_offset)
                {
                    if (current_axis == xAxis)
                    {
                        Vector3 dist = (hit.point - m_LastMousePos);
                        pos.x = xAxis_b ? pos.x + (hit.point - m_LastMousePos).magnitude * move_deltaTime : pos.x - (hit.point - m_LastMousePos).magnitude * move_deltaTime;
                        pos.x = pos.x + dist.x * move_deltaTime;

                        object_edit.transform.position = pos;
                    }

                    if (current_axis == yAxis)
                    {
                        pos.y = pos.y + (hit.point - m_LastMousePos).magnitude * move_deltaTime;
                        object_edit.transform.position = pos;

                    }
                    if (current_axis == zAxis)
                    {
                        pos.z = zAxis_b ? pos.z + (hit.point - m_LastMousePos).magnitude * move_deltaTime : pos.z - (hit.point - m_LastMousePos).magnitude * move_deltaTime;
                        object_edit.transform.position = pos;

                    }

                }
                if ((start_dist - curr_dist) > move_offset)
                {
                    if (current_axis == xAxis)
                    {
                        Vector3 dist = (hit.point - m_LastMousePos);
                        pos.x = xAxis_b ? pos.x - (hit.point - m_LastMousePos).magnitude * move_deltaTime : pos.x + (hit.point - m_LastMousePos).magnitude * move_deltaTime; 
                        pos.x = pos.x + dist.x * move_deltaTime;
                       object_edit.transform.position = pos;
                       
                    }

                    if (current_axis == yAxis)
                    {
                        pos.y = pos.y - (hit.point - m_LastMousePos).magnitude * move_deltaTime;
                         object_edit.transform.position = pos;

                    }
                    if (current_axis == zAxis)
                    {
                        pos.z = zAxis_b ? pos.z - (hit.point - m_LastMousePos).magnitude * move_deltaTime : pos.z + (hit.point - m_LastMousePos).magnitude * move_deltaTime;
                         object_edit.transform.position = pos;
                            
                    }
                }
            }
        }
    }

    public void moveAOI_old()
    {
        if (Input.GetMouseButton(0) && moveAOI && object_edit)
        {
            if (Physics.Raycast(screen_point.origin, screen_point.direction, out RaycastHit hit, 1000, Replay_mask.value) && isWithinViewport())
            {
                object_edit.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
        }
    }

    //scale AOI cube in different directions using the axes

    public void scaleAOI_()
    {
        if (Input.GetMouseButtonDown(0) && editAOI && scaleAOI)
        {
            object_edit.transform.GetChild(1).transform.gameObject.SetActive(true);
            if (Physics.Raycast(screen_point, out RaycastHit hit, 1000, scaleAxis_mask.value) && isWithinViewport())
            {
                m_LastMousePos = hit.point;
                current_axis = hit.transform.parent;
                start_dist = Vector3.Distance(object_edit.transform.position, hit.point);

            }
        }

        if (Input.GetMouseButton(0) && scaleAOI && object_edit)
        {


            Transform xAxis = object_edit.transform.Find("scaleAxis/AxisPointerX");
            Transform yAxis = object_edit.transform.Find("scaleAxis/AxisPointerY");
            Transform zAxis = object_edit.transform.Find("scaleAxis/AxisPointerZ");

            object_edit.transform.Find("scaleAxis").SetParent(object_edit.transform, true);

            if (Physics.Raycast(screen_point.origin, screen_point.direction, out RaycastHit hit, 1000, scaleAxis_mask.value) && isWithinViewport())
            {
                Transform axis = hit.collider.gameObject.transform;
                curr_dist = Vector3.Distance(object_edit.transform.position, hit.point);
                Vector3 scale = object_edit.transform.localScale;
                if (start_dist > curr_dist)
                {
                    if (current_axis == xAxis)
                    {
                        scale.x = scale.x - (hit.point - m_LastMousePos).magnitude;
                        if (scale.x > 0.1)
                            object_edit.transform.localScale = scale;
                    }

                    if (current_axis == yAxis)
                    {
                        scale.y = scale.y - (hit.point - m_LastMousePos).magnitude;
                        if (scale.y > 0.1)
                            object_edit.transform.localScale = scale;
                    }
                    if (current_axis == zAxis)
                    {
                        scale.z = scale.z - (hit.point - m_LastMousePos).magnitude;
                        if (scale.z > 0.08)
                            object_edit.transform.localScale = scale;
                    }

                    m_LastMousePos = hit.point;
                    start_dist = curr_dist;

                }
                if (start_dist < curr_dist)
                {
                    if (current_axis == xAxis)
                    {
                        scale.x = scale.x + (hit.point - m_LastMousePos).magnitude;
                        if (scale.x > 0.1)
                            object_edit.transform.localScale = scale;

                    }

                    if (current_axis == yAxis)
                    {
                        scale.y = scale.y + (hit.point - m_LastMousePos).magnitude;
                        if (scale.y > 0.1)
                            object_edit.transform.localScale = scale;
                    }
                    if (current_axis == zAxis)
                    {
                        scale.z = scale.z + (hit.point - m_LastMousePos).magnitude;
                        if (scale.z > 0.08)
                            object_edit.transform.localScale = scale;
                    }

                    m_LastMousePos = hit.point;
                    start_dist = curr_dist;
                }
            }
        }

       
    }

    public void scaleAOI_old()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_LastMousePos = Input.mousePosition;
            if (Physics.Raycast(screen_point.origin, screen_point.direction, out m_RayCastHit, 1000, AOI_mask.value) && isWithinViewport())
            {
                if (object_edit)
                {
                    m_CurrentObject = object_edit;
                    start_dist = Vector3.Distance(m_CurrentObject.transform.position, m_RayCastHit.point);
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(screen_point.origin, screen_point.direction, out m_RayCastHit, 1000, AOI_mask.value) && isWithinViewport())
            {
                curr_dist = Vector3.Distance(m_CurrentObject.transform.position, m_RayCastHit.point);

                Vector3 scale = m_CurrentObject.transform.localScale;

                if (start_dist > curr_dist)
                {
                    scale.x = scale.x - (Mathf.Sign((Input.mousePosition.x - m_LastMousePos.x)) * (Input.mousePosition.x - m_LastMousePos.x)) * m_DeltaTime;
                    scale.y = scale.y - (Mathf.Sign(Input.mousePosition.y - m_LastMousePos.y) * (Input.mousePosition.y - m_LastMousePos.y)) * m_DeltaTime;
                    //constraint for max min scale size
                    scale.x = Mathf.Clamp(scale.x, 0.1f, 5f);
                    scale.y = Mathf.Clamp(scale.y, 0.1f, 5f);
                    m_CurrentObject.transform.localScale = scale;

                }
                else if (start_dist < curr_dist)
                {
                    scale.x = scale.x + (Mathf.Sign(Input.mousePosition.x - m_LastMousePos.x) * (Input.mousePosition.x - m_LastMousePos.x)) * m_DeltaTime;
                    scale.y = scale.y + (Mathf.Sign(Input.mousePosition.y - m_LastMousePos.y) * (Input.mousePosition.y - m_LastMousePos.y)) * m_DeltaTime;
                    //constraint for max min scale size
                    scale.x = Mathf.Clamp(scale.x, 0.1f, 5f);
                    scale.y = Mathf.Clamp(scale.y, 0.1f, 5f);
                    m_CurrentObject.transform.localScale = scale;
                }
            }
        }
    }

    bool isWithinViewport()
    {
        if (!(view_point.x < 0 || view_point.x > 1 || view_point.y < 0 || view_point.y > 1))
        {
            return true;
        }
        else return false;
    }
    public void defineAOIactive()
    {
        defineAOI_ = true;
        editAOIbttn.interactable = false;
        editAOI = false;
    }
    public void onClickToggle()
    {
        if (SceneDataHandler.toggledAOIvirtual || SceneDataHandler.toggledAOIreal)
        {
            saveAOIbttn.interactable = true;
        }
    }
    public void editAOIactive()
    {
        editAOI = true;
        defineAOIbttn.interactable = defineAOI_;
        scaleAOIbttn.interactable = editAOI;
        moveAOIbttn.interactable = editAOI;
        moveAOI = false;
        scaleAOI = false;

    }
    public void moveAOIactive()
    {
        deleteAxis();
        moveAOI = !moveAOI;
        BttnColor(moveAOIbttn, moveAOI);
        if (moveAOI)
        {
            scaleAOI = false;
            BttnColor(scaleAOIbttn, scaleAOI);
            editAOIbttn.interactable = false;
        }
        else
        {
            scaleAOI = false;
            BttnColor(scaleAOIbttn, scaleAOI);
            if (object_edit)
                object_edit.transform.GetChild(0).transform.gameObject.SetActive(false);


        }
    }

    public void scaleAOIactive()
    {
        deleteAxis();
        scaleAOI = !scaleAOI;
        if (scaleAOI)
        {
            moveAOI = false;
            BttnColor(moveAOIbttn, moveAOI);
            BttnColor(scaleAOIbttn, scaleAOI);
            editAOIbttn.interactable = false;


        }
        else
        {
            moveAOI = false;
            BttnColor(moveAOIbttn, moveAOI);
            BttnColor(scaleAOIbttn, scaleAOI);

            if (object_edit)
                object_edit.transform.GetChild(1).transform.gameObject.SetActive(false);

        }

    }


    void BttnColor(Button button, bool bttnActive)
    {
        if (bttnActive)
        {
            ColorBlock cb = button.colors;
            cb.normalColor = Color.grey;
            cb.selectedColor = Color.grey;
            button.colors = cb;
        }
        else
        {
            ColorBlock cb = button.colors;
            cb.normalColor = Color.white;
            cb.selectedColor = Color.white;
            button.colors = cb;
        }
    }
    public void saveAOI()
    {
        bool real = SceneDataHandler.toggledAOIreal;
        bool virt = SceneDataHandler.toggledAOIvirtual;
        if (real || virt || editFunctionOn)
        {
            deleteAxis();
            SceneDataHandler.AOI_region_.Add(object_edit, new bool[] { virt, real });
            SceneDataHandler.AOI_region = object_edit;
            saveAOIregionToJson();
            object_edit = null;
            clearSettings();
            AOI_saved = true;
        }
        else
            AOI_saved = false;
    }

    void deleteAxis()
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            object_edit.transform.GetChild(i).transform.gameObject.SetActive(false);
        }
    }

    // created AOI cubes are saved in AOIregions.json
    public void saveAOIregionToJson()
    {
        if (!AOI_region_names.Contains(object_edit.name))
        {
            string name = object_edit.name;
            Vector3 pos = object_edit.transform.position;
            Vector3 scale = object_edit.transform.localScale;
            Quaternion rotation = object_edit.transform.localRotation;
            bool real = SceneDataHandler.toggledAOIreal;
            bool virt = SceneDataHandler.toggledAOIvirtual;
            AOI_region_data = new AOI_region(name, pos, scale, virt, real);
            AOI_datastructure.list_AOIregions.Add(AOI_region_data);
            FileHandler.SaveToJSON(AOI_datastructure, filename);
        }
    }

    public void clearSettings()
    {
        object_edit = null;
        defineAOI_ = false;
        editAOI = false;
        moveAOI = false;
        scaleAOI = false;
        BttnColor(scaleAOIbttn, scaleAOI);
        BttnColor(moveAOIbttn, moveAOI);
        defineAOIbttn.interactable = defineAOI_;
        moveAOIbttn.interactable = moveAOI;
        scaleAOIbttn.interactable = scaleAOI;
        saveAOIbttn.interactable = false;
        editAOIbttn.interactable = editAOI;
        AOI_name.text = "";
        SceneDataHandler.toggledAOIreal = false;
        SceneDataHandler.toggledAOIvirtual = false;
    }
    public void writeAOIname(string input)
    {
        defineAOIbttn.interactable = !string.IsNullOrWhiteSpace(input);
    }

    public void getToggleAOIName()
    {
        AOI_name_from_legend = SceneDataHandler.toggledAOIname;
    }
    public void AOItoggleChanged()
    {
        getToggleAOIName();
        defineAOIbttn.interactable = true;
    }

    public void createAOI(RaycastHit hit)
    {

        AOI_region = Instantiate(cube, hit.point, Quaternion.identity);
        AOI_region.name = AOI_name_from_legend;
        AOI_region.transform.LookAt(hit.point + hit.normal);
        AOI_region.transform.SetParent(AOI_region_parent.transform, false);
        AOI_region.layer = AOI_Layer;
        object_edit = AOI_region;
        defineAOIbttn.interactable = false;
    }

    public void editAOI_()
    {
        editAOI_select = edit_Toggle.isOn;
        editFunctionOn = edit_Toggle.isOn;
    }

    public void destroy_frameRegions()
    {
        foreach (Transform child in AOI_region_parent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
