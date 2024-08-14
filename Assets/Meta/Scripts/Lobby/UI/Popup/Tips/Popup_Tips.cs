using BagelCode;
using SlotMaker;
using SlotMaker.IoC.Tween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SlotMaker.Tasks.Actions;
using Unity.Collections;

public class Popup_Tips : MonoBehaviour
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private TextMeshProUGUI _textMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        var coroutine = AsyncActionUtils.ScalingCoroutine(this, _content, _content.localScale, Vector3.zero, 0.4f, TweenUtils.VectorTweenInQuint, 1f, OnTweenCompleteHandle);
        StartCoroutine(coroutine);
    }

    public void UpdateTextDisplay(string text)
    {
        _textMeshPro.text = text;
    }

    private void OnTweenCompleteHandle()
    {
        PopupManager.Instance.Close(this.gameObject);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 打开Tips弹窗
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="text"></param>
    public static void OpenTips(GameObject agent, string text)
    {
        GameObject obj = OpenLobbyPopup.LoadAndOpenLobbyPopup("lobby", "Popup_Tips", true, agent);
        obj.GetComponent<Popup_Tips>().UpdateTextDisplay(text);
    }
}
