using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandboxApi;
using System;
using System.Collections.Concurrent;

namespace SboxSpace
{

    public enum MemType
    {
        Mem_Buffer,
        Mem_MsgInfo,
    }
    //public class GenericList<T>
    //{
    //    private List<T> list;

    //    public GenericList()
    //    {
    //        list = new List<T>();
    //    }

    //    public void Add(T item)
    //    {
    //        list.Add(item);
    //    }

    //    public T GetItem(int index)
    //    {
    //        return list[index];
    //    }
    //}
    //    public static class MemManager<T>
    //    {
    //        static Dictionary<MemType, List<T>> EventDic = new Dictionary<MemType, List<T>>();
    //        static Dictionary<MemType, List<bool>> EventDicuse = new Dictionary<MemType, List<bool>>();
    //        static object getLock = new object();//创建对象锁
    //        static object resetLock = new object();//创建对象锁

    //        public static int Count(MemType type, T Callback)
    //        {
    //            int count = 0;
    //            List<T> Callbacks = null;
    //            List<bool> useList = null;
    //            if (!EventDic.TryGetValue(type, out Callbacks))
    //            {
    //                if (EventDicuse.TryGetValue(type, out useList))
    //                {
    //                    for (int i = 0; i < useList.Count; i++)
    //                    {
    //                        if (useList[i] == false)
    //                            count++;
    //                    }
    //                }
    //            }
    //            return count;
    //        }
    //        public static bool Add(MemType type, T Callback)
    //        {
    //            List<T> Callbacks = null;
    //            List<bool> useList = null;
    //            bool unuse = false;
    //            if (!EventDic.TryGetValue(type, out Callbacks))
    //            {
    //                Callbacks = new List<T>();
    //                useList = new List<bool>();
    //                EventDic.Add(type, Callbacks);
    //                EventDicuse.Add(type, useList);
    //            }
    //            else
    //            {
    //                EventDicuse.TryGetValue(type, out useList);
    //            }
    //            Callbacks.Add(Callback);
    //            useList.Add(unuse);
    //            return true;
    //        }
    //        public static void Remove(MemType type, T listener)
    //        {

    //            List<T> listeners = null;
    //            if (EventDic.TryGetValue(type, out listeners))
    //            {
    //                listeners.Clear();
    //                if (listeners.Count <= 0)
    //                {
    //                    EventDic.Remove(type);
    //                }
    //            }
    //        }
    //        public static void Reset(MemType type, T listener )
    //        {
    //            lock (getLock)
    //            {
    //                List<T> listeners = null;
    //                if (EventDic.TryGetValue(type, out listeners))
    //                {
    //                    int i = 0;
    //                    for (i = 0; i < listeners.Count; i++)
    //                    {
    //                        if (listeners[i].Equals(listener))
    //                        {

    //                            break;
    //                        }
    //                    }
    //                    if (i < listeners.Count)
    //                    {
    //                        List<bool> uselist = null;
    //                        if (EventDicuse.TryGetValue(type, out uselist))
    //                        {
    //                            uselist[i] = false;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        public static T tryGet(MemType EventName)
    //        {
    //            if (!EventDic.ContainsKey(EventName)) return default(T);

    //            lock(getLock)
    //            {
    //                List<T> events = null;
    //                if (EventDic.TryGetValue(EventName, out events))
    //                {
    //                    int i = 0;
    //                    List<bool> uselist = null;
    //                    if (EventDicuse.TryGetValue(EventName, out uselist))
    //                    {
    //                        for (i = 0; i < uselist.Count; i++)
    //                        {
    //                            if (uselist[i] == false)
    //                            {
    //                                break;
    //                            }
    //                        }
    //                    }
    //                    if (i < uselist.Count)
    //                    {
    //                        uselist[i] = true;
    //                        return events[i];
    //                    }
    //                }
    //            }
    //            return default(T);
    //        }
    //    }
    //}
    public static class MemManager<T>
    {
        static Dictionary<MemType, ConcurrentQueue<T>> EnQueDic = new Dictionary<MemType, ConcurrentQueue<T>>();
        // static Dictionary<MemType, List<bool>> EventDicuse = new Dictionary<MemType, List<bool>>();
        static object getLock = new object();//创建对象锁
        static object resetLock = new object();//创建对象锁

        public static int Count(MemType type)
        {
            int count = 0;
            ConcurrentQueue<T> Callbacks = null;
            if (EnQueDic.TryGetValue(type, out Callbacks))
            {
                return Callbacks.Count;
            }
            return count;
        }
        public static bool Add(MemType type, T Callback)
        {
            ConcurrentQueue<T> Callbacks = null;
            if (!EnQueDic.TryGetValue(type, out Callbacks))
            {
                Callbacks = new ConcurrentQueue<T>();
                EnQueDic.Add(type, Callbacks);
            }
            Callbacks.Enqueue(Callback);
            return true;
        }
        //public static void Remove(MemType type, T listener)
        //{

        //    List<T> listeners = null;
        //    if (EnQueDic.TryGetValue(type, out listeners))
        //    {
        //        listeners.Clear();
        //        if (listeners.Count <= 0)
        //        {
        //            EventDic.Remove(type);
        //        }
        //    }
        //}
        public static void Reset(MemType type, T listener)
        {
            ConcurrentQueue<T> listeners = null;
            if (EnQueDic.TryGetValue(type, out listeners))
            {
                listeners.Enqueue(listener);
            }
        }
        public static T tryGet(MemType EventName)
        {
            if (!EnQueDic.ContainsKey(EventName)) return default(T);

            ConcurrentQueue<T> events = null;
            if (EnQueDic.TryGetValue(EventName, out events))
            {
                T tmp;
                events.TryDequeue(out tmp);
               
                return tmp;
            }
            return default(T);
        }
    }
}