using BSS.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchTestViewController : MonoBehaviour
{
    private RectTransform point;
    private Button closeButton;
    private LineRenderer lineRenderer;
    private int index = 0;
    private List<Vector3> posList = new List<Vector3>();

    void Start()
    {
        point = transform.Find("point").GetComponent<RectTransform>();
        lineRenderer = transform.Find("point").GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        point.gameObject.SetActive(false);
        closeButton = transform.Find("Button").GetComponent<Button>();
        closeButton.onClick.AddListener(() => { Destroy(gameObject); });
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            point.gameObject.SetActive(true);
        }
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
#if UNITY_EDITOR
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#else
             var pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
#endif
            var temp = transform.InverseTransformVector(pos);
            var addPos = new Vector3(temp.x, temp.y, 0); 
            posList.Add(addPos);
            lineRenderer.positionCount = posList.Count;
            lineRenderer.SetPositions(posList.ToArray());
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0) || Input.touchCount > 0)
#else
        if (Input.touchCount <= 0) 
#endif
        {
            index = 0;
            lineRenderer.positionCount = 0;
            posList.Clear();
            point.gameObject.SetActive(false);
        }
    }
}
