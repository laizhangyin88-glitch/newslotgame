using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_NetworkTips2 : MonoBehaviour
{
    [SerializeField] private Button _closeBtn;

    void Start()
    {
        _closeBtn.onClick.AddListener(() => Application.Quit());
    }
}
