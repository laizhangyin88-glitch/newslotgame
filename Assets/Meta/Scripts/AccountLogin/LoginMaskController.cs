using BagelCode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginMaskController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _tips;

    private Slider slider;

    private float currentValue;

    private float totalValue;
    private void Start()
    {
        slider = transform.Find("Anchor/Slider").GetComponentInChildren<Slider>();
        slider.value = 0;
        totalValue = 0;

        _tips.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            Debug.Log("解除异常...");
            NetManager.Instance.Post(RPCName.clearAbormalStatus, null,
                (data) =>
                {
                    LastFreeGameManager.Instance.StopLastFreeGame();
                    EventSender.SendGlobalEvent(MetaEventDefine.ON_SYSTEM_EVENT, MetaEventDefine.SYSTEM_RESET);
                },
                (err) =>
                {
                    LastFreeGameManager.Instance.StopLastFreeGame();
                    EventSender.SendGlobalEvent(MetaEventDefine.ON_SYSTEM_EVENT, MetaEventDefine.SYSTEM_RESET);
                });
        });
        SetTipsActive(false);

        if (_canvasGroup == null)
            return;
        
        _canvasGroup.blocksRaycasts = !TestDisplayManager.Instance.IsGM;
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //        DelayShowTips(0f);
    //}

    public void SetSliderTotal(float total)
    {
        if (totalValue <= 0)
        {
            totalValue = total;
            currentValue = 0;
        }
    }

    public void AddSliderValue()
    {
        DelayShowTips(6f);
        currentValue++;
        slider.value = currentValue / totalValue;
    }

    /// <summary>
    /// 延迟显示Tips
    /// </summary>
    /// <param name="delay">延迟时间，单位s</param>
    public void DelayShowTips(float delay)
    {
        SetTipsActive(false);
        DelayedCall(delay, () =>
        {
            SetTipsActive(true);
        });
    }

    public void SetTipsActive(bool state)
    {
        if (_tips.activeSelf == state)
            return;

        _tips.SetActive(state);
    }

    /// <summary>
    /// 不受<see cref="Time.timeScale"/>影响的延迟调用
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="action"></param>
    public void DelayedCall(float delay, System.Action action)
    {
        StopAllCoroutines();
        StartCoroutine(DelayedCallCoroutine(delay, action));
    }

    private IEnumerator DelayedCallCoroutine(float delay, System.Action action)
    {
        float elapsed = 0f;

        while (elapsed < delay)
        {
            elapsed += Time.unscaledDeltaTime; // 使用unscaledDeltaTime
            yield return null; // 等待下一帧
        }

        action?.Invoke(); // 执行回调
    }
}
