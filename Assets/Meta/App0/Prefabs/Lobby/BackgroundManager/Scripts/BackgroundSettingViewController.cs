using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundSettingViewController : MonoBehaviour
{
    private Button closeBtn;
    private Transform btnBg;
    private Button BtnMachine;
    private Button BtnDevices;
    private int pageIndex = 0;
    private List<Transform> pageList = new List<Transform>();

    private void Start()
    {
        closeBtn = transform.Find("content/ButtonClose").GetComponent<Button>();
        
        BtnMachine = transform.Find("content/Pages/ButtonMachine").GetComponent<Button>();
        BtnDevices = transform.Find("content/Pages/ButtonDevices").GetComponent<Button>();
        BtnMachine.onClick.AddListener(OnClickBtnMachine);
        BtnDevices.onClick.AddListener(OnClickBtnDevices);
        closeBtn.onClick.AddListener(OnClickClose);
        btnBg = transform.Find("content/Pages/btnBg");
        for (int i = 0; i < 2; i++)
        {
            Transform temp = transform.Find("content/Pages/page" + (i + 1));
            if (temp != null)
            {
                pageList.Add(temp);
            }
            temp.gameObject.SetActive(false);
        }
        OnClickBtnMachine();
    }

    private void OnClickClose()
    {
        Destroy(gameObject);
    }

    private void SetBtnBgParent(Transform parent)
    {
        if(btnBg != null)
        {
            btnBg.transform.SetParent(parent);
            btnBg.transform.localPosition = Vector3.zero;
            btnBg.transform.localScale = Vector3.one;
            btnBg.transform.SetAsFirstSibling();
        }
    }

    private void OnClickBtnMachine()
    {
        SetBtnBgParent(BtnMachine.transform);
        ShowPage(0);
    }

    private void OnClickBtnDevices()
    {
        SetBtnBgParent(BtnDevices.transform);
        ShowPage(1);
    }

    private void ShowPage(int index)
    {
        pageList[pageIndex].gameObject.SetActive(false);
        pageIndex = index;
        pageList[pageIndex].gameObject.SetActive(true);
    }
}
