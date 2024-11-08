using BagelCode;
using ParadoxNotion;
using SimpleJSON;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDisplayManager : MonoSingleton<TestDisplayManager>
{
    public const string LOGINISGM = "is_gm";

    [SerializeField] private GameObject _testObj;

    /// <summary>
    /// 用户是否是管理员
    /// </summary>
    public bool IsGM
    {
        get => isGM;
        set
        {
            isGM = value;
            PlayerPrefs.SetInt("IsGM", isGM ? 1 : 0);
        }
    }

    private bool isGM = false;

    void Start()
    {
        //默认显示指令页面和按钮指令页面
        TestManager.Instance.SetMaxPageIndex(2);

        //如果不是测试包并且不是编辑器环境，则关闭AutoUrl
#if !MARS_FORTUNE_TEST && !K3K_TEST && !UNITY_EDITOR
        TestManager.Instance.SetAutoUrlEnable(false);
        SetTestManagerEnable(false);
#else
        TestManager.Instance.SetAutoUrlEnable(true);
        SetTestManagerEnable(true);
#endif

        Debug.Log("!!!!!热更测试预留位置");
    }

    public void SetTestManagerEnable(bool enable)
    {
        //如果是展示包，则允许启用/禁用TestManager功能
        //默认启用TestManager
#if !MARS_FORTUNE_TEST && !K3K_TEST && !UNITY_EDITOR
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

        IsGM = isGM;
        SetTestManagerEnable(isGM);
    }
    private void OnReceiveSystemEventHandle(EventData eventData)
    {
        if (eventData.name != MetaEventDefine.SYSTEM_RESET)
            return;

        //接收到系统重置事件
        SetTestManagerEnable(false);
        IsGM = false;
    }
}
