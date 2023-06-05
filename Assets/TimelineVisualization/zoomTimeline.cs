using UnityEngine;
using UnityEngine.EventSystems;

public class zoomTimeline : MonoBehaviour //, IScrollHandler
{
    private Vector3 initialScale;
    private Vector3 initialScale_participants;
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private float minZoom = 0.0001f;

    public GameObject participant_panel;
    private void Awake()
    {
        initialScale = transform.localScale;
        initialScale_participants = participant_panel.transform.localScale;
    }

    public void ScrollWithSliderHorizontal(float sliderValue)
    {
        Vector3 v = new Vector3(1, 0, 0);
        var delta_hor = sliderValue * zoomSpeed;
        var delta = new Vector3(delta_hor, transform.localScale.y, 1);
        var desiredScale = Vector3.Scale(initialScale, delta);

      //  desiredScale = ClampDesiredScale(desiredScale);
        transform.localScale = desiredScale;

    }

    public void ScrollWithSliderVertical(float sliderValue)
    {
        var delta_vert = sliderValue * zoomSpeed;
        var delta = new Vector3(transform.localScale.x, delta_vert, 1);
        var desiredScale = Vector3.Scale(initialScale, delta);

        desiredScale = ClampDesiredScale(desiredScale);
        transform.localScale = desiredScale;
        Vector3 v = new Vector3(1, desiredScale.y, 1);
        participant_panel.transform.localScale = Vector3.Scale(initialScale_participants, v);
        participant_panel.transform.parent.InverseTransformPoint(Vector3.Scale(initialScale_participants, v));
    }
    private Vector3 ClampDesiredScale(Vector3 desiredScale)
    {
        desiredScale = Vector3.Max(initialScale * minZoom, desiredScale);
        desiredScale = Vector3.Min(initialScale * maxZoom, desiredScale);
        return desiredScale;
    }

}
