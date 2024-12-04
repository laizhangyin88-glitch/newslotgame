using BagelCode;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingAndTips : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _progressTextCom;
    [SerializeField] private TextMeshProUGUI _tipsTextCom;

    [SerializeField] private const float _speed = 50f;

    private float _targetProgress = 0f;
    private float _curProgress = -1f;

    public float TargetProgress => _targetProgress;

    void Start()
    {
        SetTips("");
        SetProgress(0f);
    }

    void Update()
    {
        if (_curProgress >= _targetProgress)
            return;

        _curProgress += Time.deltaTime * _speed;
        UpdateProgressText(_curProgress);
    }

    public void SetTips(string tips)
    {
        _tipsTextCom.text = tips;
    }

    public void SetProgress(float progress)
    {
        _targetProgress = progress;
    }

    public void AddProgress(float progress)
    {
        _targetProgress += progress;
    }

    protected void UpdateProgressText(float progressValue)
    {
        progressValue = Mathf.Clamp(progressValue, 0f, 100f);
        progressValue = Mathf.Round(progressValue);
        _progressTextCom.text = $"{progressValue}%";
    }
}
