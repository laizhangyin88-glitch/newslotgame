using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CheckVersionTips : MonoBehaviour
{
    [SerializeField] private Button _okBtn;

    protected void Start()
    {
        _okBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
