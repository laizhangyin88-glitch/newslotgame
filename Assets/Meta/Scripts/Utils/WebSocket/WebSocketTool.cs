using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BagelCode
{
    public delegate void DelegateProcess(byte[] bytes);
    public class WebSocketTool
    {
        private static Dictionary<string, DelegateProcess> processDic = new Dictionary<string, DelegateProcess>();

        public static byte[] Serialize<T>(T t)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<T>(ms, t);
                byte[] result = ms.ToArray();
                return result;
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return ProtoBuf.Serializer.Deserialize<T>(ms);
            }
        }

        public static void RegisterReceiveHandler(string msgName, DelegateProcess process)
        {
            if (processDic.ContainsKey(msgName))
                processDic[msgName] += process;
            else
                processDic[msgName] = process;
        }

        public static void UnRegisterHandler(string msgName, DelegateProcess process)
        {
            if (processDic.ContainsKey(msgName))
            {
                processDic[msgName] -= process;
                if (processDic[msgName] == null)
                    processDic.Remove(msgName);
            }
        }

        public static void ClearHandler()
        {
            processDic.Clear();
        }

        public static void CheckReceiveMsg(string msgName, byte[] bytes)
        {
            if (processDic.ContainsKey(msgName))
            {
                //Debug.LogError("接到消息 ======> " + msgName);
                processDic[msgName](bytes);
            }
            else
                Debug.LogError("未注册的消息类型 ======> " + msgName);
        }
    }
}

