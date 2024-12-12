using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_NetworkTips : MonoBehaviour
{
    [SerializeField] private Button _closeBtn;
    public bool IsOpen { get; private set; }

    public void Show()
    {
        gameObject.SetActive(true);
        IsOpen = true;
    }

    public void Start()
    {
        _closeBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            IsOpen = false;
        });
    }
}
