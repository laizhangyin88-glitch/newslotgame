/**
 * @file    
 * @author  Huang Wen <Email:ww1383@163.com, QQ:214890094, WeChat:w18926268887>
 * @version 1.0
 *
 * @section LICENSE
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 * @section DESCRIPTION
 *
 * This file is ...
 */
using System;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SandboxApi
{

    /*
	 * SandboxPacket类
	 */
    [Serializable]
    public class SandboxPacket
    {
        public int cmd;
        public int source;
        public int target;
        public int[] data;
    }


    /*
	 * Sandbox类，只能存在一个全局对象
	 */
    public class Sandbox
    {

        /*
		 * 引入android plugin
		 */
        //private static AndroidJavaClass m_jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //private static AndroidJavaObject m_jo = m_jc.GetStatic<AndroidJavaObject>("currentActivity");
        //private static AndroidJavaObject m_jo = new AndroidJavaObject("com.unity3d.player.UnityPlayer");	

#if UNITY_EDITOR//在unity编辑模式下
        private static AndroidJavaClass m_jc = null;
        private static AndroidJavaObject m_jo = null;
#elif UNITY_ANDROID//ANDROID平台
        private static AndroidJavaClass m_jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        private static AndroidJavaObject m_jo = m_jc.GetStatic<AndroidJavaObject>("currentActivity");
#endif

        /**
		 *  @brief          初始化Sandbox模块
		 *  @param          无
		 *  @return         true or false
		 *  @details        
		 */
        public static bool init()
        {
            // 调用java函数sandboxInit初始化
            bool result = m_jo.Call<bool>("sandboxInit");
            return result;
        }

        /**
		 *  @brief          退出Sandbox模块
		 *  @param          无
		 *  @return         无
		 *  @details        
		 */
        public static void exit()
        {
            if (Application.platform != RuntimePlatform.Android) return;
            m_jo.Call("sandboxExit");
        }

        /**
		 *  @brief          获取sandbox服务模块的版本号
		 *  @param          无
		 *  @return         返回版本号字符串，如：1.0.0
		 *  @details        
		 */
        public static string version()
        {
            string version = m_jo.Call<string>("sandboxVersion");

            return version;
        }

        /**
		 *  @brief          设备是否已连接
		 *  @param[in]      address 设备地址
		 *  @return         已连接：true，未连接：false
		 *  @details        
		 */
        public static bool connected(int address)
        {
            bool result = m_jo.Call<bool>("sandboxConnected", address);

            return result;
        }

        /**
		 *  @brief          需要周期性调用
		 *  @param          无
		 *  @return         无
		 *  @details        
		 */
        public static void exec()
        {
            //m_jo.Call("sandboxExec");
        }

        /**
		 *  @brief          读取数据包
		 *  @param          无
		 *  @return         数据包对象或null
		 *  @details        
		 */
        public static SandboxPacket read()
        {
            string json = m_jo.Call<string>("sandboxRead");

            if (json != null)
            {
                return JsonUtility.FromJson<SandboxPacket>(json);
            }
            return null;
        }

        /**
		 *  @brief          发送数据包
		 *  @param          packet SandboxPacket对象
		 *  @return         true or false
		 *  @details        
		 */
        public static bool write(SandboxPacket packet)
        {
            string json = JsonUtility.ToJson(packet, false);
            bool result = m_jo.Call<bool>("sandboxWrite", json);

            return result;
        }
    }
}
