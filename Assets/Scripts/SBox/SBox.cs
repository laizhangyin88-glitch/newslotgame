/**
 * SourceTree Test
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
using Hal;
using UnityEngine;

namespace SBoxApi
{
    public class SBoxEventHandle
    {
        // IDEA
        public const string SBOX_RESET = "SBOX_RESET";
        public const string SBOX_READ_CONF = "SBOX_READ_CONF";
        public const string SBOX_WRITE_CONF = "SBOX_WRITE_CONF";
        public const string SBOX_CHECK_PASSWORD = "SBOX_CHECK_PASSWORD";
        public const string SBOX_CHANGE_PASSWORD = "SBOX_CHANGE_PASSWORD";
        public const string SBOX_REQUEST_CODER = "SBOX_REQUEST_CODER";
        public const string SBOX_CODER = "SBOX_CODER";
        public const string SBOX_GET_ODDS = "SBOX_GET_ODDS";
        public const string SBOX_GET_PRIZE_PREPARE = "SBOX_GET_PRIZE_PREPARE";
        public const string SBOX_GET_PRIZE = "SBOX_GET_PRIZE";
        public const string SBOX_GET_PRIZE_PLAYER = "SBOX_GET_PRIZE_PLAYER";
        public const string SBOX_SET_PLAYER_BETS = "SBOX_SET_PLAYER_BETS";
        public const string SBOX_SET_COIN_TO_HOLE_COUNT = "SBOX_SET_COIN_TO_HOLE_COUNT";

        // SANDBOX
        public const string SBOX_SADNBOX_RESET = "SBOX_SADNBOX_RESET";
        public const string SBOX_SADNBOX_COIN_OUT_START = "SBOX_SADNBOX_COIN_OUT_START";
        public const string SBOX_SADNBOX_COIN_OUT_STOP = "SBOX_SADNBOX_COIN_OUT_STOP";
        public const string SBOX_SADNBOX_METER_SET = "SBOX_SADNBOX_METER_SET";

    }


    public class SBoxBaseData
    {
        public int[] value;
        public SBoxBaseData(int[] value)
        {
            this.value = value;
        }
    }

    public class SBox : MonoBehaviour
    {
        private static bool bInit = false;

        public static void Init()
        {
            Debug.LogError("Call Init");
            bInit = SBoxIOStream.Init();
            if (bInit)
            {
                SBoxIdea.Init();

                SBoxSandbox.Init();
            }
        }

        public static void Exit()
        {
            SBoxSandbox.Exit();

            SBoxIdea.Exit();

            SBoxIOStream.Exit();

            bInit = false;
        }


        /**
          *  @brief          
          *  @param          无
          *  @return         无
          *  @details        
          */
        private void Update()
        {
            if(bInit == true)
            {
                int counter = 0;
                int millisecond = (int)(Time.deltaTime * 1000);

                SBoxIOStream.Exec();

                SBoxSandbox.Exec(millisecond);

                SBoxIdea.Exec(millisecond);

                counter = 0;

                //while (bExecRun == true)
                while (counter++ < 50)
                {
                    SBoxPacket packet = SBoxIOStream.Read();
                    if (packet != null)
                    {
                        SBoxIOEvent.SendEvent(packet.cmd, packet);
                    }
                    else
                    {
                        break;
                        //Thread.Sleep(5);
                    }
                }
            }
        }
    }
}