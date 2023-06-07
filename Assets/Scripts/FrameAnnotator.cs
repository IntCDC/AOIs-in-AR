using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using UnityEngine.EventSystems;

public class FrameAnnotator : MonoBehaviour
{
    public string input_string;
    [SerializeField] private string selectableLayer;

    public GameObject canvas;
    public GameObject frameRegion_prefab;
    public GameObject AOI_handler;
    public GameObject scroll_handler;
    [HideInInspector] public GameObject image;
    List<GameObject> hittedObjects = new List<GameObject>();
    List<GameObject> listAllFrames = new List<GameObject>();
    List<GameObject> selectedFrames = new List<GameObject>();
    public Button AnnotateBttn;
    public InputField input;
    public Toggle virtualToggle;
    public Toggle realToggle;

    private string inputAOI;
    GameObject AOI_region = null;
    [SerializeField]
    private List<string> listDropDownOptions;
    AOI_Handler AOI_handler_script;
    part_panel_mnger scroll_mnger_script;
    Collider AOI_collider;
    string lastAOI = null;
    public static string newAOIFromInput;
    public Text numberFrames;
    public Toggle annotatedFramesToggle;
    public Toggle colorAOI_toggle;
    public Toggle colorVirtual_RealAOI_toggle;
    int selectObj_layer;
    int ignoreRaycast_layer;
    string annotation;
    GameObject lastHittedFrame;
    bool controlKeyDown = false;
    bool lastKeyRight = false;
    bool lastKeyLeft = false;
    int index_lastHittedFrame;
    GameObject nextFrame;
    public GameObject menu_virtualAOI;
    public GameObject menu_realAOI;
    public GameObject AOI_menu;
    public Camera camera;
    public GameObject Canvas;
    GameObject real_menu;
    GameObject virtual_menu;
    bool ToggleOn = false;
    bool menuOpened = false;
    ToggleGroup activeGroup;

    bool annotate_real = false;
    void Start()
    {
        AOI_handler_script = AOI_handler.GetComponent<AOI_Handler>();
        scroll_mnger_script = scroll_handler.GetComponent<part_panel_mnger>();
        listAllFrames = FrameManager_withoutLayout.listFrameObjects;
        selectObj_layer = LayerMask.NameToLayer("selectableObjects");
        ignoreRaycast_layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    IEnumerator Fill_DropdownWithAOIs()
    {
        yield return new WaitUntil(() => AOI_handler_script.getAOI_keys().Count >= 10);
        listDropDownOptions = AOI_handler_script.getAOI_keys();
        lastAOI = AOI_handler_script.latestAOI;
        getAnnotatedFrames();
    }

    void Update()
    {
        //Check if the left Mouse button is clicked, check if a fixation in timeline visualization is clicked
        if (Input.GetMouseButtonDown(0))
        {

            var pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            int layer = LayerMask.NameToLayer(selectableLayer.ToString());
            if ((raycastResults.Count > 0) && (raycastResults[0].gameObject.layer == layer) && hittedObjects.IndexOf(raycastResults[0].gameObject) == -1)
            {
                selectFrame(raycastResults[0].gameObject);
            }
            else if ((raycastResults.Count > 0) && (raycastResults[0].gameObject.layer == layer) && hittedObjects.IndexOf(raycastResults[0].gameObject) != -1)
            {
                unselectFrame(raycastResults[0].gameObject);
            }

        }

        // undo selected fixations
        if (Input.GetKeyDown(KeyCode.Escape) && hittedObjects.Count != 0)
        {
            unselect_All_Frames();
            numberSelectedFrames();
        }


        //multiple fixations can be selected by clicking on one fixation an then by holding control key and left/right arrow keys to select subsequent fixations


        if (Input.GetKeyDown(KeyCode.LeftControl) && hittedObjects.Count != 0)
        {
            lastHittedFrame = hittedObjects[hittedObjects.Count - 1];
            index_lastHittedFrame = listAllFrames.IndexOf(lastHittedFrame);
            controlKeyDown = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) && controlKeyDown)
        {
            controlKeyDown = false;
            lastKeyRight = false;
            lastKeyLeft = false;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && controlKeyDown)
        {

            if (lastKeyLeft)
                nextFrame = listAllFrames[index_lastHittedFrame];
            else if (annotatedFramesToggle.isOn)
            {
                for (int f = index_lastHittedFrame; f < listAllFrames.Count; f++)
                {
                    if (f + 1 < listAllFrames.Count)
                    {
                        nextFrame = listAllFrames[f + 1];

                    }
                    else
                    {
                        nextFrame = listAllFrames[index_lastHittedFrame];
                    }
                    if (annotatedFrames.IndexOf(nextFrame) == -1)
                    {
                        break;
                    }
                }
            }

            else
            {
                if (index_lastHittedFrame + 1 < listAllFrames.Count)
                {
                    nextFrame = listAllFrames[index_lastHittedFrame + 1];
                }
                else
                {
                    nextFrame = listAllFrames[index_lastHittedFrame];
                }
            }

            lastKeyRight = true;
            lastKeyLeft = false;

            if (hittedObjects.IndexOf(nextFrame) == -1)
            {
                selectFrame(nextFrame);
            }
            else if (hittedObjects.IndexOf(nextFrame) != -1)
            {
                unselectFrame(nextFrame);
            }
            index_lastHittedFrame = listAllFrames.IndexOf(nextFrame);
            Vector3 frameRect;
            if (hittedObjects.Count > 0)
                frameRect = hittedObjects.LastOrDefault().GetComponent<RectTransform>().position;
            else
                frameRect = nextFrame.GetComponent<RectTransform>().position;

            Vector3 anchoredFrame = nextFrame.GetComponent<RectTransform>().anchoredPosition;
            scroll_mnger_script.SnapTo(frameRect, anchoredFrame);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && controlKeyDown)
        {
            if (lastKeyRight)
                nextFrame = listAllFrames[index_lastHittedFrame];
            else if (annotatedFramesToggle.isOn)
            {
                for (int f = index_lastHittedFrame; f < listAllFrames.Count; f--)
                {
                    if (f - 1 > 0 && f - 1 < listAllFrames.Count)
                    {
                        nextFrame = listAllFrames[f - 1];

                    }
                    else
                    {
                        nextFrame = listAllFrames[index_lastHittedFrame];
                    }
                    if (annotatedFrames.IndexOf(nextFrame) == -1)
                    {
                        break;
                    }
                }
            }


            else
            {
                nextFrame = listAllFrames[index_lastHittedFrame - 1 == -1 ? index_lastHittedFrame : index_lastHittedFrame - 1];
            }


            lastKeyRight = false;
            lastKeyLeft = true;
            if (hittedObjects.IndexOf(nextFrame) == -1)
            {
                selectFrame(nextFrame);
            }
            else if (hittedObjects.IndexOf(nextFrame) != -1)
                unselectFrame(nextFrame);
            index_lastHittedFrame = listAllFrames.IndexOf(nextFrame);
            Vector3 frameRect;
            if (hittedObjects.Count > 0)
                frameRect = hittedObjects.LastOrDefault().GetComponent<RectTransform>().position;
            else
                frameRect = nextFrame.GetComponent<RectTransform>().position;

            Vector3 anchoredFrame = nextFrame.GetComponent<RectTransform>().anchoredPosition;
            scroll_mnger_script.SnapTo(frameRect, anchoredFrame);
        }


        // one option for annotation of fixations is to click cotrol key and "V"/"R", then the legend for virtual/real AOIs is shown, AOI can be selected from there and by clicking the space key
        // the annotation is done
        if (Input.GetKeyUp(KeyCode.V) && !controlKeyDown && menuOpened)
        {
            if (virtual_menu != null && virtual_menu.activeSelf)
            {
                Destroy(virtual_menu);
                menuOpened = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.R) && !controlKeyDown && menuOpened)
        {
            if (real_menu != null && real_menu.activeSelf)
            {
                Destroy(real_menu);
                menuOpened = false;
            }
        }


        if (Input.GetKeyUp(KeyCode.V) && controlKeyDown & !menuOpened)
        {
            RectTransform canvas_rect = canvas.GetComponent<RectTransform>();

            RectTransform rect_frame = listAllFrames[index_lastHittedFrame].GetComponent<RectTransform>();

            virtual_menu = Instantiate(menu_virtualAOI);
            virtual_menu.name = "virtual_menu";
            RectTransform rectTransform = virtual_menu.GetComponent<RectTransform>();
            Vector3 screenPoint = camera.WorldToScreenPoint(listAllFrames[index_lastHittedFrame].GetComponent<RectTransform>().position);
            Vector2 result;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas_rect, screenPoint, camera, out result);

            rectTransform.anchoredPosition = new Vector2(100, -80); 
            virtual_menu.transform.SetParent(AOI_menu.transform, false);


            foreach (RectTransform g in virtual_menu.transform.GetChild(0).GetChild(0))
            {
                Toggle AOI_toggle = g.transform.GetComponent<Toggle>();
                AOI_toggle.group = virtual_menu.transform.GetChild(0).GetChild(0).transform.GetComponent<ToggleGroup>();
                g.transform.GetComponent<Toggle>().isOn = false;
            }

            ToggleOn = false;
            menuOpened = true;

        }

        if (Input.GetKeyUp(KeyCode.R) && controlKeyDown && !menuOpened)
        {
            RectTransform canvas_rect = canvas.GetComponent<RectTransform>();
            RectTransform rect_frame = listAllFrames[index_lastHittedFrame].GetComponent<RectTransform>();

            real_menu = Instantiate(menu_realAOI);
            real_menu.name = "real_menu";
            RectTransform rectTransform = real_menu.GetComponent<RectTransform>();

            Vector3 screenPoint = camera.WorldToScreenPoint(listAllFrames[index_lastHittedFrame].GetComponent<RectTransform>().position);
            Vector2 result;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas_rect, screenPoint, camera, out result);

            rectTransform.anchoredPosition = new Vector2(100, -80); 
            real_menu.transform.SetParent(AOI_menu.transform, false);

            foreach (RectTransform g in real_menu.transform.GetChild(0).GetChild(0))
            {
                Toggle AOI_toggle = g.transform.GetComponent<Toggle>();
                AOI_toggle.group = real_menu.transform.GetChild(0).GetChild(0).transform.GetComponent<ToggleGroup>();
                g.transform.GetComponent<Toggle>().isOn = false;
            }

            ToggleOn = false;
            menuOpened = true;
        }

        if (menuOpened && Input.GetKeyDown(KeyCode.Space))
        {
            if (real_menu != null && real_menu.gameObject.activeSelf)
            {
                activeGroup = real_menu.transform.GetChild(0).GetChild(0).transform.GetComponent<ToggleGroup>();
                ToggleOn = activeGroup.AnyTogglesOn();
            }
            else if (virtual_menu != null && virtual_menu.gameObject.activeSelf)
            {
                activeGroup = virtual_menu.transform.GetChild(0).GetChild(0).transform.GetComponent<ToggleGroup>();
                ToggleOn = activeGroup.AnyTogglesOn();

            }
            if (ToggleOn)
            {
                Toggle selectedToggle = activeGroup.ActiveToggles().FirstOrDefault();


                if (real_menu != null && real_menu.activeSelf)
                {
                    Destroy(real_menu);
                    annotate_real = true;
                }
                else
                {
                    Destroy(virtual_menu);
                    annotate_real = false;
                }
                menuOpened = false;
                annotateWithinCOntextMenu(selectedToggle.name, annotate_real);

            }

        }

        if (AOI_region != SceneDataHandler.AOI_region)
        {
            AOI_region = SceneDataHandler.AOI_region;
            AOI_collider = AOI_region.GetComponent<Collider>();
            assignAOIregionToFrames();
            AOI_handler_script.createAOI_Label_from_Replay();
            apply_definingAOIregion();
            SceneDataHandler.AOI_region_.Clear();

        }


    }

    void unselect_All_Frames()
    {
        unselect_allFrames();

        if (virtual_menu != null && virtual_menu.activeSelf)
        {
            Destroy(virtual_menu);
            menuOpened = false;
        }
        if (real_menu != null && real_menu.activeSelf)
        {
            Destroy(real_menu);
            menuOpened = false;
        }
    }


    void selectFrame(GameObject frame)
    {
        hittedObjects.Add(frame);
        highlightSelectedFrame(frame, true);
        findFrameInReplay(frame);
    }
    void unselectFrame(GameObject frame)
    {
        hittedObjects.Remove(frame);
        highlightSelectedFrame(frame, false);
        findFrameInReplay(frame);
    }




    public void unselect_allFrames()
    {
        if (hittedObjects.Count > 0)
        {
            foreach (GameObject frame in hittedObjects)
            {
                highlightSelectedFrame(frame, false);
            }
            findListOfFramesInReplay(hittedObjects);

        }
        hittedObjects.Clear();

    }


    //if you click remaining frames
    void selectMultipleFrames(List<GameObject> frames)
    {
        foreach (GameObject frame in frames)
        {
            if (!hittedObjects.Any(x => x == frame))
            {
                hittedObjects.Add(frame);
                highlightSelectedFrame(frame, true);
            }
        }
        findListOfFramesInReplay(frames);
    }

    void unselectMultipleFrames(List<GameObject> frames)
    {
        foreach (GameObject frame in frames)
        {
            hittedObjects.Remove(frame);
            highlightSelectedFrame(frame, false);
        }
        findListOfFramesInReplay(frames);
    }
    public void assignAOIregionToFrames()
    {
        List<GameObject> listFrames = AOI_handler_script.getFramesOfFilteredPart();
        Vector3 gazePoint = new Vector3(0, 0, 0);
        foreach (var frame in listFrames)
        {
            Frame frameScript = frame.GetComponent<Frame>();
            gazePoint = frameScript.gazePoint;
            if (AOI_collider.bounds.Contains(gazePoint))
            {
                hittedObjects.Add(frame);
            }
        }
    }


    public void numberSelectedFrames()
    {
        numberFrames.gameObject.SetActive(true);
        int number = hittedObjects.Count();
        numberFrames.text = "Fixations selected: " + number;
    }
    public void findFrameInReplay(GameObject hittedFrame)
    {
        Vector3 gazePoint = new Vector3(0, 0, 0);
        string ID = null;
        string participant = null;
        float timestamp = 0;

        foreach (var frame in listAllFrames)
        {
            if (frame == hittedFrame)
            {
                //if frame is selected
                Frame frameScript = frame.GetComponentInParent<Frame>();
                gazePoint = frameScript.gazePoint;
                ID = frameScript.ID;
                SceneDataHandler.gazePoint = gazePoint;
                SceneDataHandler.FrameClicked = true;
                SceneDataHandler.hittedFrame_ID = ID;
                break;
            }
        }
    }

    public void findListOfFramesInReplay(List<GameObject> hittedFrames)
    {
        Dictionary<string, Tuple<bool, Vector3>> findFrameInReplay = new Dictionary<string, Tuple<bool, Vector3>>();
        Vector3 gazePoint = new Vector3(0, 0, 0);
        string ID = null;
        string participant = null;
        float timestamp = 0;

        foreach (var hittedFrame in hittedFrames)
        {
            foreach (var frame in listAllFrames)
            {
                if (frame == hittedFrame)
                {
                    //if frame is selected
                    Frame frameScript = frame.GetComponentInParent<Frame>();
                    gazePoint = frameScript.gazePoint;
                    ID = frameScript.ID;
                    findFrameInReplay.Add(ID, new Tuple<bool, Vector3>(true, gazePoint));
                    break;
                }
            }
        }
        SceneDataHandler.FramesToShowInReplay = findFrameInReplay;
    }

    public void createFrameRegion(Vector3 gazePoint)
    {
        GameObject frameRegion = Instantiate(frameRegion_prefab);
        RectTransform frameRegionTransform = frameRegion.GetComponent<RectTransform>();
        frameRegionTransform.anchoredPosition = gazePoint;
    }

    public void apply_definingAOIregion()
    {
        string AOInameFromRegion;
        bool r_AOI_virtual = false;
        bool r_AOI_real = false;

        if (SceneDataHandler.AOI_region != null)
        {
            AOInameFromRegion = SceneDataHandler.AOI_region.name;
            r_AOI_virtual = SceneDataHandler.AOI_region_[SceneDataHandler.AOI_region].First();
            r_AOI_real = SceneDataHandler.AOI_region_[SceneDataHandler.AOI_region].Skip(1).First();

            if (AOInameFromRegion != null)
            {
                //write Annotation into the frame
                foreach (var frame in hittedObjects)
                {
                    if (AOInameFromRegion != null)
                    {
                        annotation = AOInameFromRegion;
                    }
                    Frame frameScript = frame.GetComponentInParent<Frame>();
                    frameScript.getAnnotation(annotation, r_AOI_real, r_AOI_virtual, true);
                    RawImage rawImage = frame.GetComponent<RawImage>();
                    rawImage.color = Color.white;
                    if (!annotatedFrames.Contains(frame))
                        annotatedFrames.Add(frame.transform.GetChild(0).gameObject);


                    //is annotatedFrames Toggle is on then remove frames which are annotated onClick of the annotateBttn
                    if (annotatedFramesToggle.isOn)
                    {
                        newAnnotatedFrame(frame.transform.GetChild(0).gameObject);

                    }

                    //if colorAOIs toggle is On then color frames which are annotated onClick of the annotateBttn
                    if (colorAOI_toggle.isOn && frame.transform.parent.gameObject.activeSelf)
                    
                    {
                        Frame frameDurScript = frame.GetComponentInParent<Frame>();
                        frameDurScript.colorFrame(true);


                    }

                    //if color virtual/real AOIs toggle is on then color frames which are annotated onClick of the annotateBttn
                    if (colorVirtual_RealAOI_toggle.isOn && frame.transform.parent.gameObject.activeSelf)
                    {
                        Frame frameDurScript = frame.GetComponentInParent<Frame>();
                        frameDurScript.colorFrameVirtualReal(true);
                    }

                    AOInameFromRegion = null;
                }

                SceneDataHandler.framesAnnotated = true;
                numberSelectedFrames();
                numberFrames.gameObject.SetActive(false);
                StartCoroutine(AOI_handler_script.writeAOI_percent());
                hittedObjects.Clear();
            }
        }
    }

    public void annotateWithinCOntextMenu(string selectedAOI, bool isRealAOI)
    {
        foreach (var frame in hittedObjects)
        {

            annotation = selectedAOI;

            Frame frameScript = frame.GetComponentInParent<Frame>();
            frameScript.getAnnotation(annotation, isRealAOI, !isRealAOI, true);
            RawImage rawImage = frame.GetComponent<RawImage>();
            rawImage.color = Color.white;
            if (!annotatedFrames.Contains(frame))
                annotatedFrames.Add(frame);
            //is annotatedFrames Toggle is on then remove frames which are annotated onClick of the annotateBttn
            if (annotatedFramesToggle.isOn)
            {
                newAnnotatedFrame(frame);

            }

            //if colorAOIs toggle is On then color frames which are annotated onClick of the annotateBttn
            if (colorAOI_toggle.isOn && frame.transform.parent.gameObject.activeSelf)
            {
                Frame frameDurScript = frame.GetComponentInParent<Frame>();
                frameDurScript.colorFrame(true);
            }

            //if color virtual/real AOIs toggle is on then color frames which are annotated onClick of the annotateBttn
            if (colorVirtual_RealAOI_toggle.isOn && frame.transform.parent.gameObject.activeSelf)
            {
                Frame frameDurScript = frame.GetComponentInParent<Frame>();
                frameDurScript.colorFrameVirtualReal(true);
            }
        }

        SceneDataHandler.framesAnnotated = true;
        numberSelectedFrames();
        numberFrames.gameObject.SetActive(false);
        StartCoroutine(AOI_handler_script.writeAOI_percent());
        hittedObjects.Clear();
        selectedFrames.Clear();
    }

    public void select_remainingFrames(bool toggle)
    {
        List<GameObject>  notAnnotatedFrames = new List<GameObject>();
        getAnnotatedFrames();

        if (toggle)
        {
            for (int i = 0; i < listAllFrames.Count; i++)
            {
                if (!listAllFrames[i].GetComponentInParent<Frame>().frames.frameData.is_annotated && !hittedObjects.Any(x => x == listAllFrames[i]))
                {
                    notAnnotatedFrames.Add(listAllFrames[i]);
                }
            }
            selectMultipleFrames(notAnnotatedFrames);

        }
        if (!toggle)
        {
            for (int i = 0; i < listAllFrames.Count; i++)
            {
                if (!listAllFrames[i].GetComponentInParent<Frame>().frames.frameData.is_annotated && hittedObjects.Any(x => x == listAllFrames[i]))

                {
                    notAnnotatedFrames.Add(listAllFrames[i]);
                }
            }
            unselectMultipleFrames(notAnnotatedFrames);

        }
    }


    public void ClickOnAnnotateBttn()
    {
        if (hittedObjects.Count != 0)
        {
            inputAOI = input.text;
            newAOIFromInput = input.text;
            if (newAOIFromInput != "")
                AOI_handler_script.createAOILabel(newAOIFromInput, virtualToggle.isOn, realToggle.isOn);
            AOI_handler_script.select_AOI_toggle(out string AOI_toggle);

            input.text = "";
            if (AOI_toggle != null || inputAOI != "")
            {
                //write Annotation into the frame
                foreach (var frame in hittedObjects)
                {
                    if (inputAOI != "")
                    {
                        annotation = inputAOI;
                    }
                    else if (AOI_toggle != null)
                    {
                        annotation = AOI_toggle;
                    }

                    Frame frameScript = frame.GetComponentInParent<Frame>();
                    frameScript.getAnnotation(annotation, realToggle.isOn, virtualToggle.isOn, true);
                    RawImage rawImage = frame.GetComponent<RawImage>();
                    rawImage.color = Color.white;
                    if (!annotatedFrames.Contains(frame))
                        annotatedFrames.Add(frame);
                    //is annotatedFrames Toggle is on then remove frames which are annotated onClick of the annotateBttn
                    if (annotatedFramesToggle.isOn)
                    {
                        newAnnotatedFrame(frame);

                    }

                    //if colorAOIs toggle is On then color frames which are annotated onClick of the annotateBttn
                    if (colorAOI_toggle.isOn && frame.transform.parent.gameObject.activeSelf)
                    {
                        Frame frameDurScript = frame.GetComponentInParent<Frame>();
                        frameDurScript.colorFrame(true);
                    }

                    //if color virtual/real AOIs toggle is on then color frames which are annotated onClick of the annotateBttn
                    if (colorVirtual_RealAOI_toggle.isOn && frame.transform.parent.gameObject.activeSelf)
                    {
                        Frame frameDurScript = frame.GetComponentInParent<Frame>();
                        frameDurScript.colorFrameVirtualReal(true);
                    }

                    AOI_toggle = null;
                }

                hittedObjects.Clear();
                selectedFrames.Clear();
                SceneDataHandler.framesAnnotated = true;
                numberSelectedFrames();
                numberFrames.gameObject.SetActive(false);

                StartCoroutine(AOI_handler_script.writeAOI_percent());
            }
        }
    }

    bool frameSelectionMissing = false;
    public void toggleVirtual(bool isToggle)
    {
        isToggle = virtualToggle.isOn;
        if ((isToggle || realToggle.isOn))
        {
            AnnotateBttn.interactable = true;
        }
        else if (isToggle || realToggle.isOn)
        {
            frameSelectionMissing = true;
        }
        else
        {
            AnnotateBttn.interactable = false;
        }
    }

    public void toggleReal(bool isToggle)
    {
        isToggle = realToggle.isOn;
        if ((isToggle || virtualToggle.isOn))
        {
            AnnotateBttn.interactable = true;
        }
        else if (isToggle || realToggle.isOn)
        {
            frameSelectionMissing = true;
        }
        else
        {
            AnnotateBttn.interactable = false;
        }
    }

    Color frameDefaultCol;

    public void highlightSelectedFrame(GameObject hittedFrame, bool select)
    {
     
        numberSelectedFrames();
        RawImage rawImage = hittedFrame.GetComponent<RawImage>();

        //if the hittedFrame is within the hittedObjects list:
        if (select) 
        {
            frameDefaultCol = rawImage.color;
            rawImage.color = Color.green;
        }
        else
        {
            rawImage.color = frameDefaultCol;
        }

    }

    List<GameObject> annotatedFrames = new List<GameObject>();
    public void getAnnotatedFrames()
    {
        List<GameObject> listFrames = AOI_handler_script.getFramesOfFilteredPart();
        for (int i = 0; i < listFrames.Count; i++)
        {
            if (listFrames[i].GetComponent<Frame>().frames.frameData.is_annotated)
            {
                GameObject frame = listFrames[i].transform.GetChild(0).gameObject;
                annotatedFrames.Add(frame);
            }
        }
    }


    //if annotated frames Toggle is on disable all already annotated frames
    public void dis_enableAnnotatedFramesToggle(bool toggle)
    {
        getAnnotatedFrames();
        for (int i = 0; i < annotatedFrames.Count; i++)
        {
            RawImage frameImage = annotatedFrames[i].GetComponent<RawImage>();
            GameObject frame = annotatedFrames[i].transform.gameObject;
            GameObject framedur = annotatedFrames[i].transform.parent.gameObject;
            RawImage frameDurImage = framedur.GetComponent<RawImage>();
            if (toggle)
            {
                frameImage.color = new Color(frameImage.color.r, frameImage.color.g, frameImage.color.b, 0.2f);
                frameDurImage.color = new Color(frameDurImage.color.r, frameDurImage.color.g, frameDurImage.color.b, 0.01f);
                frame.layer = ignoreRaycast_layer;
            }
            else
            {
                frameImage.color = new Color(frameImage.color.r, frameImage.color.g, frameImage.color.b, 1);
                frameDurImage.color = new Color(frameDurImage.color.r, frameDurImage.color.g, frameDurImage.color.b, 1f);
                frame.layer = selectObj_layer;
            }
        }
    }

    private void newAnnotatedFrame(GameObject frame)
    {
        RawImage frameImage = frame.GetComponent<RawImage>();
        GameObject framedur = frame.transform.parent.gameObject;
        RawImage frameDurImage = framedur.GetComponent<RawImage>();

        frameImage.color = new Color(frameImage.color.r, frameImage.color.g, frameImage.color.b, 0.2f);
        frameDurImage.color = new Color(frameDurImage.color.r, frameDurImage.color.g, frameDurImage.color.b, 0.01f);
        frame.layer = ignoreRaycast_layer;
    }
}