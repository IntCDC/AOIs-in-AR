using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class part_panel_mnger : MonoBehaviour
{
    ScrollRect scrollRect;
    public GameObject scrollViewTimelinePanel;
    public GameObject part_content;
    RectTransform part_rect;

    public RectTransform contentPanel;
    public GameObject contentRect;
    public GameObject scrollRect2;
    public void SnapTo(Vector3 target, Vector3 anchoredTarget)
    {
        Canvas.ForceUpdateCanvases();

        contentPanel.anchoredPosition = new Vector2(((Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(target)).x + 500, contentPanel.anchoredPosition.y) ;
    }

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.verticalScrollbar.value = scrollViewTimelinePanel.GetComponent<ScrollRect>().verticalScrollbar.value;       
    }

    public void scrollAuto()
    {
        scrollRect.verticalScrollbar.value = scrollViewTimelinePanel.GetComponent<ScrollRect>().verticalScrollbar.value;

    }

}
