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
#define SUPPORT_OLD_CODE

using Hal;
using System.Data;
using UnityEngine;
using UnityEngine.Events;

namespace SBoxApi
{


    public class SBoxSandbox
    {
        public enum SBOX_SWITCH
        {
            // 以下为SwitchState函数返的状态值固定定义的bit，不同的硬件项目，对应的bit不一定有效，
            // 以下定义以外的bit，由项目情况再决定其作用
            SWITCH_UP = (1 << 0),
            SWITCH_DOWN = (1 << 1),
            SWITCH_LEFT = (1 << 2),
            SWITCH_RIGHT = (1 << 3),
            SWITCH_ROOT_SET = (1 << 4),
            SWITCH_SET = (1 << 5),
            SWITCH_DOOR_SWITCH = (1 << 6),
            SWITCH_PAYOUT = (1 << 7),
            SWITCH_ENTER = (1 << 8),
            SWITCH_ESC = (1 << 9),
            SWITCH_SWITCH = (1 << 10),
            SWITCH_SCORE_UP = (1 << 11),
            SWITCH_SCORE_DOWN = (1 << 12),
            SWITCH_RED = (1 << 13),     //SWITCH_BET1
            SWITCH_GREEN = (1 << 14),   //SWITCH_BET2
            SWITCH_YELLOW = (1 << 15),  //SWITCH_BET3
            SWITCH_BET4 = (1 << 16),
            SWITCH_BET5 = (1 << 17),
            SWITCH_AUTO = (1 << 18),
        };

        private class SBoxInfo
        {

#if SUPPORT_OLD_CODE
            public bool OldCode;
            public bool Ready;
            public bool req;
#endif
            public int DeviceId;
            public int[] NumberOfCoinOut = new int[2];
            public int[] NumberOfCoinIn = new int[4];

            public int[] CounterOfCoinOut = new int[2];
            public int[] CounterOfCoinIn = new int[4];

            public bool[] IsCoinOutTimeout = new bool[2];
            public int InStateTimeout;
            public ulong InState;
            public ulong InStateEx;

            public ulong OutStateMaskBk;
            public ulong OutStateMask;
            public ulong OutStateExMaskBk;
            public ulong OutStateExMask;
        }
        private static SBoxInfo sBoxInfo = new SBoxInfo();

        // --------------------------------------------------
        //
        //  init(); exit(); 两函数由本SDK调用，APP层禁止调用
        //
        // --------------------------------------------------
        /**
          *  @brief          
          *  @param          无
          *  @return         true or false
          *  @details        
          */
        public static void Init()
        {
            DataReset();

            SBoxIOEvent.AddListener(40001, MessageR);
#if SUPPORT_OLD_CODE
            // Old Code
            sBoxInfo.OldCode = false;           
            sBoxInfo.Ready = false;
            sBoxInfo.req = false;
            SBoxIOEvent.AddListener(1801, MessageR2);
#endif
        }

        /**
		 *  @brief          
		 *  @param          无
		 *  @return         无
		 *  @details        
		 */
        public static void Exit()
        {

        }

        /**
		 *  @brief          
		 *  @param          Millisecond 每个周期的时间差，毫秒为单位
		 *  @return         无
		 *  @details        
		 */
        public static void Exec(int Millisecond)
        {
            SwitchInStateExec(Millisecond);

            SwitchOutStateExec(Millisecond);
        }

        private static void SwitchInStateExec(int Millisecond)
        {
            if (sBoxInfo.InStateTimeout != 0)
            {
                if (sBoxInfo.InStateTimeout > Millisecond)
                {
                    sBoxInfo.InStateTimeout -= Millisecond;
                }
                else
                {
                    sBoxInfo.InStateTimeout = 0;
                }
                if (sBoxInfo.InStateTimeout == 0)
                {
                    sBoxInfo.InState = 0;
                    sBoxInfo.InStateEx = 0;
                }
            }
        }

        private static void SwitchOutStateExec(int Millisecond)
        {
            ulong xor = 0;
            ulong OnMask = 0;
            ulong OffMask = 0;
            ulong OnExMask = 0;
            ulong OffExMask = 0;

            if (sBoxInfo.OutStateMask != sBoxInfo.OutStateMaskBk)
            {
                xor = sBoxInfo.OutStateMask ^ sBoxInfo.OutStateMaskBk;
                OnMask = xor & sBoxInfo.OutStateMask;
                OffMask = xor & sBoxInfo.OutStateMaskBk;
                sBoxInfo.OutStateMaskBk = sBoxInfo.OutStateMask;
            }

            if (sBoxInfo.OutStateExMask != sBoxInfo.OutStateExMaskBk)
            {
                xor = sBoxInfo.OutStateExMask ^ sBoxInfo.OutStateExMaskBk;
                OnExMask = xor & sBoxInfo.OutStateExMask;
                OffExMask = xor & sBoxInfo.OutStateExMaskBk;
                sBoxInfo.OutStateExMaskBk = sBoxInfo.OutStateExMask;
            }

            if ((OnMask != 0) || (OnExMask != 0))
            {
                SwitchOn(OnMask, OnExMask);
            }

            if ((OffMask != 0) || (OffExMask != 0))
            {
                SwitchOff(OffMask, OffExMask);
            }
        }

        private static void DataReset()
        {
            for(int i = 0;i < 2;i++)
            {
                sBoxInfo.NumberOfCoinOut[i] = 0;
            }
            for (int i = 0; i < 4; i++)
            {
                sBoxInfo.NumberOfCoinIn[i] = 0;
            }

            for(int i = 0;i < 2;i++)
            {
                sBoxInfo.CounterOfCoinOut[i] = 0;
            }
            for (int i = 0; i < 4; i++)
            {
                sBoxInfo.CounterOfCoinIn[i] = 0;
            }

            sBoxInfo.OutStateMask = 0;
            sBoxInfo.OutStateMaskBk = 0;
            sBoxInfo.OutStateExMask = 0;
            sBoxInfo.OutStateExMaskBk = 0;

            for(int i = 0;i < 2;i++)
            {
                sBoxInfo.IsCoinOutTimeout[i] = false;
            }
        }


        /**
          *  @brief          底板主动上发的消息
          *  @param          sBoxPacket
          *  @return         无
          *  @details        
          */
        private static void MessageR(SBoxPacket sBoxPacket)
        {
            if (sBoxPacket.data.Length != 24)
            {
                return;
            }
            ulong tmp64 = 0;
            int tmp = 0;

            sBoxInfo.req = true;

            tmp = sBoxPacket.data[0];
            // 退币机状态1
            if ((tmp & (1 << 5)) != 0)
            {
                sBoxInfo.IsCoinOutTimeout[0] = true;
            }
            // 退币机状态2
            if ((tmp & (1 << 6)) != 0)
            {
                sBoxInfo.IsCoinOutTimeout[1] = true;
            }

            // 设备ID
            sBoxInfo.DeviceId = sBoxPacket.data[4];

            // 
            tmp64 = (uint)sBoxPacket.data[11];
            tmp64 <<= 32;
            tmp64 |= (uint)sBoxPacket.data[10];
            sBoxInfo.InState = tmp64;
            // 
            tmp64 = (uint)sBoxPacket.data[13];
            tmp64 <<= 32;
            tmp64 |= (uint)sBoxPacket.data[12];
            sBoxInfo.InStateEx = tmp64;
            sBoxInfo.InStateTimeout = 1000;


            // 退币计数器：0~255
            for (int i = 0; i < 2; i++)
            {
                tmp = sBoxPacket.data[16 + i];
                if (tmp > sBoxInfo.CounterOfCoinOut[i])
                {
                    sBoxInfo.NumberOfCoinOut[i] += (tmp - sBoxInfo.CounterOfCoinOut[i]);
                }
                else if (tmp < sBoxInfo.CounterOfCoinOut[i])
                {
                    sBoxInfo.NumberOfCoinOut[i] += (256 - sBoxInfo.CounterOfCoinOut[i] + tmp);
                }
                sBoxInfo.CounterOfCoinOut[i] = tmp;
            }

            // 投币计数器：0~255
            for (int i = 0; i < 4; i++)
            {
                tmp = sBoxPacket.data[18 + i];
                if (tmp > sBoxInfo.CounterOfCoinIn[i])
                {
                    sBoxInfo.NumberOfCoinIn[i] += (tmp - sBoxInfo.CounterOfCoinIn[i]);
                }
                else if (tmp < sBoxInfo.CounterOfCoinIn[i])
                {
                    sBoxInfo.NumberOfCoinIn[i] += (256 - sBoxInfo.CounterOfCoinIn[i] + tmp);
                }
                sBoxInfo.CounterOfCoinIn[i] = tmp;
            }

        }

#if SUPPORT_OLD_CODE
        private static bool OldCode()
        {
            return sBoxInfo.OldCode;
        }
        private static void MessageR2(SBoxPacket sBoxPacket)
        {
            if (sBoxPacket.data.Length != 9)
            {
                return;
            }
            ulong tmp64 = 0;
            ulong tmp64t = 0;
            int tmp = 0;

            sBoxInfo.req = true;

            sBoxInfo.OldCode = true;

            // 退币机状态
            tmp = sBoxPacket.data[2];
            if ((tmp & (1 << 3)) != 0)
            {
                sBoxInfo.IsCoinOutTimeout[0] = true;
            }

            // ------------------------------------------
            tmp64t = (uint)sBoxPacket.data[1];
            tmp64t <<= 32;
            tmp64t |= (uint)sBoxPacket.data[0];

            sBoxInfo.DeviceId = (int)(tmp64t & 0x0ff);
            tmp64t >>= 32;

            if((tmp64t & (1 << 0)) != 0)
            {
                tmp64 |= (1 << 16);
            }
            if ((tmp64t & (1 << 1)) != 0)
            {
                tmp64 |= (1 << 11);
            }
            if ((tmp64t & (1 << 2)) != 0)
            {
                tmp64 |= (1 << 17);
            }
            if ((tmp64t & (1 << 3)) != 0)
            {
                tmp64 |= (1 << 12);
            }
            if ((tmp64t & (1 << 4)) != 0)
            {
                tmp64 |= (1 << 18);
            }
            if ((tmp64t & (1 << 5)) != 0)
            {
                tmp64 |= (1 << 7);
            }
            //if ((tmp64t & (1 << 6)) != 0)
            //{
            //    tmp64 |= (1 << 0);
            //}
            //if ((tmp64t & (1 << 7)) != 0)
            //{
            //    tmp64 |= (1 << 0);
            //}
            if ((tmp64t & (1 << 8)) != 0)
            {
                tmp64 |= (1 << 8);
            }
            if ((tmp64t & (1 << 9)) != 0)
            {
                tmp64 |= (1 << 9);
            }

            if ((tmp64t & (1 << 11)) != 0)
            {
                tmp64 |= (1 << 1);
            }
            if ((tmp64t & (1 << 10)) != 0)
            {
                tmp64 |= (1 << 0);
            }
            if ((tmp64t & (1 << 12)) != 0)
            {
                tmp64 |= (1 << 2);
            }
            if ((tmp64t & (1 << 13)) != 0)
            {
                tmp64 |= (1 << 3);
            }

            //if ((tmp64t & (1 << 10)) != 0)
            //{
            //    tmp64 |= (1 << 2);
            //}
            //if ((tmp64t & (1 << 11)) != 0)
            //{
            //    tmp64 |= (1 << 3);
            //}
            //if ((tmp64t & (1 << 12)) != 0)
            //{
            //    tmp64 |= (1 << 1);
            //}
            //if ((tmp64t & (1 << 13)) != 0)
            //{
            //    tmp64 |= (1 << 0);
            //}

            if ((tmp64t & (1 << 14)) != 0)
            {
                tmp64 |= (1 << 19);
            }
            if ((tmp64t & (1 << 15)) != 0)
            {
                tmp64 |= (1 << 20);
            }
            if ((tmp64t & (1 << 16)) != 0)
            {
                tmp64 |= (1 << 21);
            }

            // ------------------------------------------
            sBoxInfo.InState = tmp64;
            sBoxInfo.InStateTimeout = 1000;


            // 退币计数器：0~255
            for (int i = 0; i < 1; i++)
            {
                tmp = sBoxPacket.data[8 + i];
                if (tmp > sBoxInfo.CounterOfCoinOut[i])
                {
                    sBoxInfo.NumberOfCoinOut[i] += (tmp - sBoxInfo.CounterOfCoinOut[i]);
                }
                else if (tmp < sBoxInfo.CounterOfCoinOut[i])
                {
                    sBoxInfo.NumberOfCoinOut[i] += (256 - sBoxInfo.CounterOfCoinOut[i] + tmp);
                }
                sBoxInfo.CounterOfCoinOut[i] = tmp;
            }

            // 投币计数器：0~255
            for (int i = 0; i < 1; i++)
            {
                tmp = sBoxPacket.data[7 + i];
                if (tmp > sBoxInfo.CounterOfCoinIn[i])
                {
                    sBoxInfo.NumberOfCoinIn[i] += (tmp - sBoxInfo.CounterOfCoinIn[i]);
                }
                else if (tmp < sBoxInfo.CounterOfCoinIn[i])
                {
                    sBoxInfo.NumberOfCoinIn[i] += (256 - sBoxInfo.CounterOfCoinIn[i] + tmp);
                }
                sBoxInfo.CounterOfCoinIn[i] = tmp;
            }

        }
#endif
        /**
          *  @brief          检查底板是否连接
          *  @param          无
          *  @return         true or false
          *  @details        
          */
        public static bool Ready()
        {
            bool bResult = SBoxIOStream.Connected((int)SBoxIOStream.SBoxIODevice.SBOX_DEVICE_SANDBOX);
            Debug.LogError($"IsSBoxReady:{bResult}");
#if SUPPORT_OLD_CODE
            if (bResult != sBoxInfo.Ready)
            {
                if(bResult && sBoxInfo.req)
                {
                    sBoxInfo.Ready = bResult;
                }
                else
                {
                    bResult = false;
                }
            }
#endif

            return bResult;
        }

        /**
          *  @brief          设备ID号
          *  @param          无
          *  @return         设备ID号
          *  @details        
          */
        public static int DeviceId()
        {
            return sBoxInfo.DeviceId;
        }

        /**
          *  @brief          读取外部输入端口的状态位，1：ON, 0：OFF
          *                  部分bit的定义，参考SBOX_SWITCH
          *  @param          无
          *  @return         设备ID号
          *  @details        
          */
        public static ulong SwitchInState()
        {
            return sBoxInfo.InState;
        }

        /**
          *  @brief          读取外部拓展输入端口的状态位，1：ON, 0：OFF
          *  @param          无
          *  @return         设备ID号
          *  @details        
          */
        public static ulong SwitchInStateEx()
        {
            return sBoxInfo.InStateEx;
        }

        /**
		 *  @brief          输出端口开启，参数中每个bit代表着一个端口，对应bit为1时，开启端口，对应bit为0时，不起作用
		 *  @param          state 端口状态位
		 *  @return         无
		 *  @details        
		 */
        public static void SwitchOutStateOn(ulong state)
        {
            sBoxInfo.OutStateMask |= state;
        }

        /**
		 *  @brief          输出端口关闭，参数中每个bit代表着一个端口，对应bit为1时，开启端口，对应bit为0时，不起作用
		 *  @param          state 端口状态位
		 *  @return         无
		 *  @details        
		 */
        public static void SwitchOutStateOff(ulong state)
        {
            sBoxInfo.OutStateMask &= (~state);
        }

        /**
		 *  @brief          拓屏输出端口开启，参数中每个bit代表着一个端口，对应bit为1时，开启端口，对应bit为0时，不起作用
		 *  @param          state 端口状态位
		 *  @return         无
		 *  @details        
		 */
        public static void SwitchOutStateExOn(ulong state)
        {
            sBoxInfo.OutStateExMask |= state;
        }

        /**
		 *  @brief          拓展输出端口关闭，参数中每个bit代表着一个端口，对应bit为1时，开启端口，对应bit为0时，不起作用
		 *  @param          state 端口状态位
		 *  @return         无
		 *  @details        
		 */
        public static void SwitchOutStateExOff(ulong state)
        {
            sBoxInfo.OutStateExMask &= (~state);
        }

        /**
          *  @brief          检查退币是否超时，画面上，可以实现提示退币超时几秒钟后自动取消提示
          *                  注意，当超时返回true时，只返回一次true，再次读取时状态已自动还原为false
          *  @param          id 退币器编号：0~1
          *  @return         true：超时，false：正常
          *  @details        
          */
        public static bool IsCoinOutTimeout(int id)
        {
            if(id > 1)
            {
                return false;
            }
            bool bIsCoinOutTimeout = sBoxInfo.IsCoinOutTimeout[id];
            sBoxInfo.IsCoinOutTimeout[id] = false;
            return bIsCoinOutTimeout;
        }

        /**
          *  @brief          获取相对于上次读取后的退币数量
          *  @param          id 退币器编号，<= 1，正常用用id=0即可，其它只在一些特别的项目用
          *  @return         退币数量
          *  @details        
          */
        public static int NumberOfCoinOut(int id)
        {
            if (id <= 1)
            {
                int result = sBoxInfo.NumberOfCoinOut[id];
                sBoxInfo.NumberOfCoinOut[id] = 0;
                return result;
            }
            else
            {
                return 0;
            }

        }

        /**
          *  @brief          获取相对于上次读取后的投币数量
          *  @param          id 投币器编号，<= 3，正常用用id=0即可，其它只在一些特别的项目用
          *  @return         投币数量
          *  @details        
          */
        public static int NumberOfCoinIn(int id)
        {
            if (id <= 3)
            {
                int result = sBoxInfo.NumberOfCoinIn[id];
                sBoxInfo.NumberOfCoinIn[id] = 0;
                return result;
            }
            else
            {
                return 0;
            }
        }


        /**
		 *  @brief          app重启动后，重置SANDBOX本次启动的运行数据
		 *                  用于SANDBOX程序做一些必要的初始化操作
		 *  @param          无
		 *  @return         result = 0：成功
		 *                  result < 0：发送参数错误
		 *                  result > 0：状态码
		 *  @details        
		 */
        public static void Reset()
        {
            SBoxPacket sBoxPacket;
#if SUPPORT_OLD_CODE
            if (OldCode())
            {
                sBoxPacket = new SBoxPacket(cmd: 1800, source: 1, target: 4, size: 2);
            }
            else
#endif
            {
                sBoxPacket = new SBoxPacket(cmd: 40000, source: 1, target: 4, size: 2);
            }
            
            DataReset();

            sBoxPacket.data[0] = 0;
            sBoxPacket.data[1] = 0;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, ResetR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void ResetR(SBoxPacket sBoxPacket)
        {
            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_RESET, sBoxPacket.data[0]);
        }


#if SUPPORT_OLD_CODE
        private static ulong SwitchMapIO(ulong state)
        {
            ulong result = 0;

            if ((state & (1 << 0)) != 0)
            {
                result |= (1 << 10);
            }
            if ((state & (1 << 8)) != 0)
            {
                result |= (1 << 8);
            }
            if ((state & (1 << 9)) != 0)
            {
                result |= (1 << 9);
            }
            if ((state & (1 << 13)) != 0)
            {
                result |= (1 << 1);
            }
            if ((state & (1 << 14)) != 0)
            {
                result |= (1 << 2);
            }
            if ((state & (1 << 15)) != 0)
            {
                result |= (1 << 3);
            }
            if ((state & (1 << 16)) != 0)
            {
                result |= (1 << 4);
            }
            if ((state & (1 << 17)) != 0)
            {
                result |= (1 << 5);
            }
            if ((state & (1 << 18)) != 0)
            {
                result |= (1 << 6);
            }
            if ((state & (1 << 19)) != 0)
            {
                result |= (1 << 7);
            }
            if ((state & (1 << 20)) != 0)
            {
                result |= (1 << 11);
            }
            if ((state & (1 << 21)) != 0)
            {
                result |= (1 << 12);
            }
            if ((state & (1 << 22)) != 0)
            {
                result |= (1 << 13);
            }

            return result;
        }
#endif
        /**
		 *  @brief          输出端口开启，参数中每个bit代表着一个端口，对应bit为1时，开启端口，对应bit为0时，不起作用
		 *                  对于有多个bit需要设定时，请一起处理好参数中的所有bit后，统一调用一次本函数，以免反复多点调用
		 *  @param          state 端口状态位
		 *  @param          stateex 端口状态位拓展
		 *  @return         无
		 *  @details        
		 */
        private static void SwitchOn(ulong state, ulong stateex)
        {
            SBoxPacket sBoxPacket;
#if SUPPORT_OLD_CODE
            if (OldCode())
            {
                sBoxPacket = new SBoxPacket(cmd: 1802, source: 1, target: 4, size: 2);

                state = SwitchMapIO(state);

                sBoxPacket.data[0] = (int)(state & 0x0ffffffff);
                state >>= 32;
                sBoxPacket.data[1] = (int)(state & 0x0ffffffff);
            }
            else
#endif
            {
                sBoxPacket = new SBoxPacket(cmd: 40002, source: 1, target: 4, size: 6);

                sBoxPacket.data[0] = (int)(state & 0x0ffffffff);
                state >>= 32;
                sBoxPacket.data[1] = (int)(state & 0x0ffffffff);

                sBoxPacket.data[2] = (int)(stateex & 0x0ffffffff);
                stateex >>= 32;
                sBoxPacket.data[3] = (int)(stateex & 0x0ffffffff);

                sBoxPacket.data[4] = 0;
                sBoxPacket.data[5] = 0;
            }

            //SBoxIOEvent.AddListener(sBoxPacket.cmd, SwitchOnR);
            SBoxIOStream.Write(sBoxPacket);
        }
        //private static void SwitchOnR(SBoxPacket sBoxPacket)
        //{
        //    EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_SWITCH_ON, sBoxPacket.data[0]);
        //}

        /**
		 *  @brief          输出端口关闭，参数中每个bit代表着一个端口，对应bit为1时，开启端口，对应bit为0时，不起作用
		 *                  对于有多个bit需要设定时，请一起处理好参数中的所有bit后，统一调用一次本函数，以免反复多点调用
		 *  @param          state 端口状态位
		 *  @param          stateex 端口状态位拓展
		 *  @return         无
		 *  @details        
		 */
        private static void SwitchOff(ulong state, ulong stateex)
        {
            SBoxPacket sBoxPacket;
#if SUPPORT_OLD_CODE
            if (OldCode())
            {
                sBoxPacket = new SBoxPacket(cmd: 1803, source: 1, target: 4, size: 2);

                state = SwitchMapIO(state);

                sBoxPacket.data[0] = (int)(state & 0x0ffffffff);
                state >>= 32;
                sBoxPacket.data[1] = (int)(state & 0x0ffffffff);
            }
            else
#endif
            {
                sBoxPacket = new SBoxPacket(cmd: 40003, source: 1, target: 4, size: 6);

                sBoxPacket.data[0] = (int)(state & 0x0ffffffff);
                state >>= 32;
                sBoxPacket.data[1] = (int)(state & 0x0ffffffff);

                sBoxPacket.data[2] = (int)(stateex & 0x0ffffffff);
                stateex >>= 32;
                sBoxPacket.data[3] = (int)(stateex & 0x0ffffffff);

                sBoxPacket.data[4] = 0;
                sBoxPacket.data[5] = 0;
            }

            //SBoxIOEvent.AddListener(sBoxPacket.cmd, SwitchOffR);
            SBoxIOStream.Write(sBoxPacket);
        }
        //private static void SwitchOffR(SBoxPacket sBoxPacket)
        //{
        //    EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_SWITCH_OFF, sBoxPacket.data[0]);
        //}

        /**
		 *  @brief          启动退币
		 *  @param          id 退币器编号，0~1
		 *  @param          counts 退币数量
		 *  @param          type 退币类型，0：退币，1：脉冲打印
		 *  @return         result = 0：成功
		 *                  result< 0：发送参数错误
		 *                  result > 0：状态码
		 *  @details        
		 */
        public static void CoinOutStart(int id, int counts, int type)
        {
            SBoxPacket sBoxPacket;
            if (id > 1)
            {
                return;
            }

#if SUPPORT_OLD_CODE
            if (OldCode())
            {
                sBoxPacket = new SBoxPacket(cmd: 1805, source: 1, target: 4, size: 2);
                sBoxPacket.data[0] = counts;
                sBoxPacket.data[1] = type;
            }
            else
#endif
            {
                sBoxPacket = new SBoxPacket(cmd: 40005, source: 1, target: 4, size: 3);
                sBoxPacket.data[0] = id;
                sBoxPacket.data[1] = counts;
                sBoxPacket.data[2] = type;
            }

            SBoxIOEvent.AddListener(sBoxPacket.cmd, CoinOutStartR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void CoinOutStartR(SBoxPacket sBoxPacket)
        {
            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_COIN_OUT_START, sBoxPacket.data[0]);
        }

        /**
		 *  @brief          中止退币
		 *  @param          id 退币器编号，0~1
		 *  @return         result = 0：成功
		 *                  result< 0：发送参数错误
		 *                  result > 0：状态码
		 *  @details        
		 */
        public static void CoinOutStop(int id)
        {
            SBoxPacket sBoxPacket;
            if (id > 1)
            {
                return;
            }
#if SUPPORT_OLD_CODE
            if (OldCode())
            {
                sBoxPacket = new SBoxPacket(cmd: 1806, source: 1, target: 4, size: 1);
            }
            else
#endif
            {
                sBoxPacket = new SBoxPacket(cmd: 40006, source: 1, target: 4, size: 1);
            }

            sBoxPacket.data[0] = id;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, CoinOutStopR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void CoinOutStopR(SBoxPacket sBoxPacket)
        {
            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_COIN_OUT_STOP, sBoxPacket.data[0]);
        }

        /**
		 *  @brief          走码表
		 *  @param          id 码表编号，0：投币码表，1：退币码表，2：上分码表，3：下分码表
		 *  @param          counts 码表走数
		 *  @param          type 走数类型，0：无䇅，1：counts为绝对值，2：counts为追加值，3：中止走数，
		 *  @return         result = 0：成功
		 *                  result< 0：发送参数错误
		 *                  result > 0：状态码
		 *  @details        
		 */
        public static void MeterSet(int id, int counts, int type)
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 40007, source: 1, target: 4, size: 18);

            if (id > 3)
            {
                return;
            }
            for (int i = 0; i < sBoxPacket.data.Length; i++)
            {
                sBoxPacket.data[i] = 0;
            }
            sBoxPacket.data[id * 2 + 0] = type;
            sBoxPacket.data[id * 2 + 1] = counts;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, MeterSetR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void MeterSetR(SBoxPacket sBoxPacket)
        {
            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SADNBOX_METER_SET, sBoxPacket.data[0]);
        }
    }
}
