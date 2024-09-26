using BagelCode;
using ParadoxNotion;
using SimpleJSON;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDisplayManager : MonoBehaviour
{
    public const string LOGINISGM = "is_gm";

    [SerializeField] private GameObject _testObj;

    void Start()
    {
        //默认显示指令页面和按钮指令页面
        TestManager.Instance.SetMaxPageIndex(2);

        //如果是展示包，则关闭AutoUrl
#if MARS_FORTUNE_REALSE || K3K_REALSE
        TestManager.Instance.SetAutoUrlEnable(false);
        SetTestManagerEnable(false);
#else
        TestManager.Instance.SetAutoUrlEnable(true);
        SetTestManagerEnable(true);
#endif
    }

    public void SetTestManagerEnable(bool enable)
    {
        //如果是展示包，则允许启用/禁用TestManager功能
        //默认启用TestManager
#if MARS_FORTUNE_REALSE || K3K_REALSE
        _testObj.SetActive(enable);
#endif
    }

    private void OnEnable()
    {
        MessageDispatcher.Register(RPCName.login, OnReceiveLoginHandle);
        MessageDispatcher.Register(MetaEventDefine.ON_SYSTEM_EVENT, OnReceiveSystemEventHandle);
    }

    private void OnDisable()
    {
        MessageDispatcher.UnRegister(RPCName.login, OnReceiveLoginHandle);
        MessageDispatcher.UnRegister(MetaEventDefine.ON_SYSTEM_EVENT, OnReceiveSystemEventHandle);
    }

    private void OnReceiveLoginHandle(EventData eventData)
    {
        EventData<JSONNode> jsonEventData = eventData as EventData<JSONNode>;
        bool isGM = false;
        if(jsonEventData.value.HasKey(LOGINISGM))
        {
            if(jsonEventData.value[LOGINISGM].AsInt == 1)
                isGM = true;
        }

        SetTestManagerEnable(isGM);
    }
    private void OnReceiveSystemEventHandle(EventData eventData)
    {
        if (eventData.name != MetaEventDefine.SYSTEM_RESET)
            return;

        //接收到系统重置事件
        SetTestManagerEnable(false);
    }
}
