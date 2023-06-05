using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(ScrollRect))]
public class AutoScrollRect : MonoBehaviour
{
    public GameObject Scrollbar_object;
    Scrollbar scrollbar;
    public float factor = .001f;
    public Camera cam;
    Vector3 view_point;
    private void Start()
    {
        scrollbar = Scrollbar_object.GetComponent<Scrollbar>();
    }

    void Update()
    {
        view_point = (cam.ScreenToViewportPoint(Input.mousePosition));
        if (view_point.x > 0.1 && view_point.y < 0.5)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                scrollbar.value += factor;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                scrollbar.value -= factor;
            }
        }
    }
}