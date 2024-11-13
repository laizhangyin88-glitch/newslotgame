using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchTestViewController : MonoBehaviour
{
    private RectTransform point;
    private Button closeButton;

    // Start is called before the first frame update
    void Start()
    {
        point = transform.Find("point").GetComponent<RectTransform>();
        point.gameObject.SetActive(false);
        closeButton = transform.Find("Button").GetComponent<Button>();
        closeButton.onClick.AddListener(() => { Destroy(gameObject); });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            point.gameObject.SetActive(true);
        }
        if (Input.GetMouseButton(0))
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var temp = transform.InverseTransformVector(pos);
            point.transform.localPosition = new Vector3(temp.x, temp.y, 0);
        }
        if (Input.GetMouseButtonUp(0))
        {
            point.gameObject.SetActive(false);
        }

    }
}
