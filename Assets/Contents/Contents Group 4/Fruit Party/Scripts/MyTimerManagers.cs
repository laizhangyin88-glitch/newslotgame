using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

    class TimerItem
    {
        public int id;
        public float interval;
        public int loopTimes;
        private float _interval;

        public UnityEngine.Object callbackObject;
        public object[] args;

        public Action callback;
        public Action<UnityEngine.Object> callbackObjectEvent;
        public Action<UnityEngine.Object, object[]> callbackObjAndArgs;
        public bool isNotLimtied;
        public bool isStop;
        public bool isFree;
        public TimerItem(int id, float interval, int loopTimes, Action callback = null, Action<UnityEngine.Object> actionObject = null, Action<UnityEngine.Object, object[]> action = null, UnityEngine.Object obj = null, object[] args = null)
        {
            this.id = id;
            this.interval = interval;
            this._interval = interval;
            this.loopTimes = loopTimes;
            this.callback = callback;
            this.callbackObjectEvent = actionObject;
            this.callbackObjAndArgs = action;
            this.callbackObject = obj;
            this.args = args;
            isStop = false;
            isNotLimtied = loopTimes == -1;
            isFree = false;
        }
        public void Reset()
        {
            this.interval = this._interval;
            this.loopTimes--;
            if (loopTimes == 0)
            {
                isFree = true;
            }
        }
    }

public class MyTimerManagers : MonoBehaviour
{
    public static MyTimerManagers _timerManager;
    public static MyTimerManagers Instance
    {
        get
        {
            if (_timerManager == null)
            {
                GameObject go = new GameObject("TimerManagers");
                _timerManager = go.AddComponent<MyTimerManagers>();
            }
            return _timerManager;
        }
    }
    private Dictionary<int, TimerItem> _timerDicti;
    
    private int actionIndex;
    //public MyTimerManagers()
    //{

    //}

    private void Awake()
    {
        _timerDicti = new Dictionary<int, TimerItem>();
        actionIndex = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="interval">时间间隔</param>
    /// <param name="loopTimes">执行次数,小于0表示不限次数</param>
    /// <param name="callback">回调函数</param>
    /// <param name="actionObject"></param>
    /// <param name="action"></param>
    /// <param name="obj"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public int AddTimer(float interval, int loopTimes, Action callback, Action<UnityEngine.Object> actionObject = null, Action<UnityEngine.Object, object[]> action = null, UnityEngine.Object obj = null, object[] args = null)
    {
        TimerItem item = GetTimerItem(interval, loopTimes, callback, actionObject, action, obj, args);
        return item.id;
    }

    private void FixedUpdate()
    {
        OnUpdate(Time.deltaTime);
        CheckTimerItemIsFree();
    }

    private void CheckTimerItemIsFree()
    {
        for (int i = 0; i < _timerDicti.Count; i++)
        {
            KeyValuePair<int, TimerItem> item = _timerDicti.ElementAt(i);
            if (item.Value.isFree)
            {
                _timerDicti.Remove(item.Key);
            }
        }
    }

    public void OnUpdate(float intervalTime)
    {
        for (int i = 0; i < _timerDicti.Count; i++)
        {
            KeyValuePair<int, TimerItem> item = _timerDicti.ElementAt(i);

            if (!item.Value.isStop)
            {
                if (item.Value.isNotLimtied || item.Value.loopTimes > 0)
                {
                    item.Value.interval -= intervalTime;
                    if (item.Value.interval <= 0)
                    {
                        if (item.Value.callback != null)
                        {
                            item.Value.callback();
                        }
                        if (item.Value.callbackObjectEvent != null && item.Value.callbackObject != null)
                        {
                            item.Value.callbackObjectEvent(item.Value.callbackObject);
                        }
                        if (item.Value.callbackObjAndArgs != null && item.Value.callbackObject != null && item.Value.args != null)
                        {
                            item.Value.callbackObjAndArgs(item.Value.callbackObject, item.Value.args);
                        }
                        item.Value.Reset();
                    }
                }
            }
        }
    }

    public void StopTimerById(int id, bool isStop = true)
    {
        if (_timerDicti.ContainsKey(id))
        {
            _timerDicti[id].isStop = isStop;
        }
    }

    public void RemoveTimerById(int id)
    {
        if (_timerDicti.ContainsKey(id))
        {
            _timerDicti[id].isFree = true;
        }
    }

    private TimerItem GetTimerItem(float interval, int loopTimes, Action callback, Action<UnityEngine.Object> actionObject, Action<UnityEngine.Object, object[]> action = null, UnityEngine.Object obj = null, object[] args = null)
    {
        //if (_timerDicti.Count > 0)
        //{
        //    for (int i = 1; i < _timerDicti.Count; i++)
        //    {
        //        TimerItem item = _timerDicti[i];
        //        if (item.isFree)
        //        {
        //            item.isFree = false;
        //            item.interval = interval;
        //            item.loopTimes = loopTimes;
        //            item.callback = callback;
        //            item.callbackObjectEvent = actionObject;
        //            item.callbackObjAndArgs = action;
        //            item.args = args;
        //            return item;
        //        }
        //    }
        //}
        actionIndex++;
        TimerItem item1 = new TimerItem(actionIndex, interval, loopTimes, callback, actionObject, action, obj, args);
        item1.isFree = false;
        _timerDicti.Add(actionIndex, item1);
        return item1;
    }

    public void OnDestroy()
    {
        _timerDicti.Clear();
    }
}

