using ParadoxNotion;
using SlotMaker;
using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class FgBgManager : MonoSingleton<FgBgManager>
{

    private int _isPause = -1;
    private int _isFocus = -1;
    private long lastUnix = -1;

    private bool _isForground = true;

    protected System.Timers.Timer eventTimer = null;

    private bool isRun = false;
    private Queue<Action> taskQueue = new Queue<Action>();



    public void Init()
    {
        _isPause = -1;
        _isFocus = -1;
        _isForground = true;
    }

    private void Update()
    {
        if (!isRun)
        {
            isRun = true;
            while (taskQueue.Count > 0)
            {
                var task = taskQueue.Dequeue();
                task.Invoke();
            }
            isRun = false;
        }
    }



    protected override void OnDestroy()
    {

        _isPause = -1;
        _isFocus = -1;
        _isForground = true;
        isRun = false;

        if (this.eventTimer != null)
        {
            this.eventTimer.Stop();
            this.eventTimer.Dispose();
            this.eventTimer = null;
        }
        base.OnDestroy();   
    }


    //游戏失去焦点
    protected void OnApplicationFocus(bool hasFocus)
    {
        //Debug.LogWarning($"@游戏焦点");
        isFocus = hasFocus;
    }

    //游戏暂停
    protected void OnApplicationPause(bool pauseStatus)
    {
        //Debug.LogWarning($"@游戏暂停");
        isPause = pauseStatus;
    }

    private bool isPause
    {
        set
        {
            _isPause = true == value ? 1 : 0;
            long curUnix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            Debug.LogWarning($"@游戏暂停 _isPause = {_isPause} - nowtime = {curUnix} - lasttime = {lastUnix}");

            //if (lastUnix != curUnix)
            if (curUnix - lastUnix > 100) // 安卓是在100ms内
            {
                lastUnix = curUnix;
                _isFocus = -1;
                return;
            }
            OnResumeOrPause();
        }
    }
    private bool isFocus
    {
        set
        {
            _isFocus = true == value ? 1 : 0;
            long curUnix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            Debug.LogWarning($"@游戏焦点  _isFocus = {_isFocus} - nowtime = {curUnix} - lasttime = {lastUnix}");

            //if (lastUnix != curUnix)
            if (curUnix - lastUnix > 100) // 安卓是在100ms内
            {
                lastUnix = curUnix;
                _isPause = -1;
                return;
            }
            OnResumeOrPause();
        }
    }

   /* private void Emit()
    {
        if (this.eventTimer != null)
        {
            this.eventTimer.Stop();
            this.eventTimer.Dispose();
            this.eventTimer = null;
        }

        this.eventTimer = new System.Timers.Timer(800); //避免重复触发
        this.eventTimer.AutoReset = false; // 是否重复执行
        this.eventTimer.Elapsed += (object sender, ElapsedEventArgs e) => {
            taskQueue.Enqueue(() =>
            {
                Debug.LogWarning($" 触发消息 FgBg : {_isForground}");
                var eventData = new EventData<bool>("FgBg", _isForground);
                //EventSender.SendGlobalEvent(eventData);
                MessageDispatcher.Dispatch("OnFgBg", eventData);
            });
        };
        this.eventTimer.Start();
    }*/

    private void OnResumeOrPause()
    {
//#if UNITY_IPHONE || UNITY_ANDROID

        Debug.Log($"@调用 OnResumeOrPause  _isFocus = {_isFocus} _isPause = {_isPause} ");

        if (_isPause == 1 && _isFocus == 0)//进入后台
        {
            Debug.Log($"@ 进入后台");

            _isForground = false;
            _isPause = -1;
            _isFocus = -1;

            var eventData = new EventData<bool>("FgBg", _isForground);
            //EventSender.SendGlobalEvent(eventData);
            MessageDispatcher.Dispatch("OnFgBg", eventData);

            // Emit(); 进入后台后，所有函数会被停止运行，不能使用Emit()延迟
        }
        else if (_isPause == 0 && _isFocus == 1)//进入前台
        {
            Debug.Log($"@ 进入前台");

            _isForground = true;
            _isPause = -1;
            _isFocus = -1;

            var eventData = new EventData<bool>("FgBg", _isForground);
            //EventSender.SendGlobalEvent(eventData);
            MessageDispatcher.Dispatch("OnFgBg", eventData);

            //  Emit();
        }

//#endif
    }


}


