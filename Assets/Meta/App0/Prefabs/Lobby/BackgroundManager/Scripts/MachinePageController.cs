using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MachinePageController : MonoBehaviour
{
    private Button screenFilpBtn;

    private bool isInit = false;
    private void OnEnable()
    {
        if (!isInit)
        {
            isInit = true;
            screenFilpBtn = transform.Find("ScreenFlip").GetComponent<Button>();
            screenFilpBtn.onClick.AddListener(OnClickScreenFilpBtn);    
        }
    }

    private void OnClickScreenFilpBtn()
    {
        AndroidSystemHelper.Instance.ScreenFlip();
    }
}
