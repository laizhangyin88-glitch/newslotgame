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
using Hal;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Events;

namespace SBoxApi
{
    public class SBoxConfData
    {
        public int result;
        public int PwdType;                         // 0：无任何修改参数的权限，1：普通密码权限，2：管理员密码权限，3：超级管理员密码权限
        public int PlaceType;                       // 场地类型，0：普通，1：技巧，2：专家
        public int difficulty;                      // 难度，0~5
        public int odds;                            // 倍率，0：低倍率，1：高倍率，2：随机

        public int WinLock;                         // 盈利宕机
        public int MachineId;                       // 机台编号，8位有效十进制数
        public int LineId;                          // 线号，4位有效十进制数

        public int TicketMode;                      // 退票模式，0：即中即退，1：退票
        public int TicketValue;                     // 1票对应几分（彩票比例）
        public int scoreTicket;                     // 1分对应几票
        public int CoinValue;                       // 投币比例
        public int MaxBet;                          // 最大押注
        public int MinBet;                          // 最小押注
        public int CountDown;                       // 例计时
    }

    public class SBoxCoderData
    {
        public int result;

        public int TotalBets;                       // 总押分
        public int TotalWin;                        // 总得分
        public int MachineId;                       // 机台编号，8位有效十进制数
        public int CoderCount;                      // 打码次数
        public int CheckValue;                      // 校验码
        public int RemainMinute;                    // 当前剩余时间（分钟）
    }

    public class SBoxPrizeData
    {
        public int result;

        public int PersonId;                        // 基础游戏中奖人物
                                                    // 0	红唐僧
                                                    // 1	绿唐僧
                                                    // 2	黄唐僧
                                                    // 3	红孙悟空
                                                    // 4	绿孙悟空
                                                    // 5	黄孙悟空
                                                    // 6	红猪八戒
                                                    // 7	绿猪八戒
                                                    // 8	黄猪八戒
                                                    // 9	红沙和尚
                                                    // 10	绿沙和尚
                                                    // 11	黄沙和尚

        public int FeatureId;                       // 小游戏中奖类型
                                                    // 0	不开奖，笑脸
                                                    // 1	红龙
                                                    // 2	金龙
                                                    // 3	绿龙
                                                    // 4	加倍
                                                    // 5	翻倍
                                                    // 6	送灯
                                                    // 7	大满贯
                                                    // 8	彩金

        public int Jackpot;                         // 彩金值
        public int[] CoinToTable = new int[5];      // 落到桌面上各类型金币数量：
                                                    // [0]: 银币数量（1分）
                                                    // [1]: 金币数量（2分）
                                                    // [2]: 元宝数量（5分）
                                                    // [3]: 彩球数量（10分）
                                                    // [4]: 彩盘数量（20分）
        public int[] Data;                          // 小游戏相应参数，不同类型有不同的内容定义
                                                    // 4：[0~11]加倍值
                                                    // 5：[0]翻倍值
                                                    // 6：[0~11]每个人物是否送灯,0-不送，1-送
    }

    public class SBoxPrizePrepareData
    {
        public int result;

        public int state;                          // 算法卡状态，enum SBOX_IDEA_STATE
        public int rfu;                            // 保留
    }

    public class SBoxPrizePlayerData
    {
        public int result;

        public int PlayerId;                        // 玩家ID
        public int[] WinRole = new int[15];         // 本局15门赢分
        public int Win;                             // 本局总赢分
        public int WinJackpot;                      // 本局赢的彩金金额
        public int BoxPoints;                       // 宝箱累加积分
        public int OpenBoxID;                       // 本局要开的箱子编号
        public int WinBoxGame;                      // 本局箱子游戏的赢钱金额
        public int BoxPointsLength;                 // 宝箱进度条长度
        //public int BoxOpenFlag0;                    // 第0个箱子是否开过了
        //public int BoxOpenFlag1;                    // 第1个箱子是否开过了
        //public int BoxOpenFlag2;                    // 第2个箱子是否开过了
        //public int BoxOpenFlag3;                    // 第3个箱子是否开过了
        public int[] BoxOpenFlags = new int[4];
    }

    public class SBoxPlayerBetsData
    {
        public int PlayerId;                        // 玩家ID
        public int balance;                         // 当前余分
        public int rfu;                             // 保留
        public int[] Bets = new int[15];            // 本局15门押分
    }

    public class SBoxCoinCount
    {
        public int result;

    }

    public class SBoxPermissionsData
    {
        public int result;
        public int permissions;                     // 0：无任何修改参数的权限，1：普通密码权限，2：管理员密码权限，3：超级管理员密码权限
    }

    public class SBoxIdea
    {
        public enum SBOX_IDEA_STATE
        {
            STATE_INACTIVE = (1 << 0),      // 未激活状态，需要激活
            STATE_WINLOCK = (1 << 1),       // 盈利宕机
        };

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

        }

        /**
          *  @brief          检查算法卡是否连接
          *  @param          无
          *  @return         true or false
          *  @details        
          */
        public static bool Ready()
        {
            bool bResult = SBoxIOStream.Connected((int)SBoxIOStream.SBoxIODevice.SBOX_DEVICE_IDEA);
            return bResult;
        }

        /**
		 *  @brief          app重启动后，重置算法卡本次启动的运行数据
		 *                  用于IDEA程序做一些必要的初始化操作
		 *  @param          无
		 *  @return         SBoxBaseData.value[0] = 0：成功
		 *                  SBoxBaseData.value[0] < 0：发送参数错误
		 *                  SBoxBaseData.value[0] > 0：状态码
		 *                  SBoxBaseData.value[1]：保留
		 *  @details        
		 */
        public static void Reset()
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20000, source: 1, target: 2, size: 2);

            sBoxPacket.data[0] = 0;
            sBoxPacket.data[1] = 0;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, ResetR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void ResetR(SBoxPacket sBoxPacket)
        {
            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_RESET, sBoxPacket.data[0]);
        }

        /**
		 *  @brief          读取游戏配置数据
		 *  @param          无
		 *  @return         SBoxConfData.result = 0：成功
		 *                  SBoxConfData.result < 0：发送参数错误
		 *                  SBoxConfData.result > 0：状态码
		 *  @details        
		 */
        public static void ReadConf()
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20001, source: 1, target: 2, size: 2);

            sBoxPacket.data[0] = 0;
            sBoxPacket.data[1] = 0;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, ReadConfR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void ReadConfR(SBoxPacket sBoxPacket)
        {
            SBoxConfData sBoxConfData = new SBoxConfData();
            sBoxConfData.result = sBoxPacket.data[0];
            sBoxConfData.PwdType = sBoxPacket.data[1];
            sBoxConfData.PlaceType = sBoxPacket.data[2];
            sBoxConfData.difficulty = sBoxPacket.data[3];
            sBoxConfData.odds = sBoxPacket.data[4];
            sBoxConfData.WinLock = sBoxPacket.data[5];
            sBoxConfData.MachineId = sBoxPacket.data[6];
            sBoxConfData.LineId = sBoxPacket.data[7];
            sBoxConfData.TicketMode = sBoxPacket.data[8];
            sBoxConfData.TicketValue = sBoxPacket.data[9];
            sBoxConfData.scoreTicket = sBoxPacket.data[10];
            sBoxConfData.CoinValue = sBoxPacket.data[11];
            sBoxConfData.MaxBet = sBoxPacket.data[12];
            sBoxConfData.MinBet = sBoxPacket.data[13];
            sBoxConfData.CountDown = sBoxPacket.data[14];

            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_READ_CONF, sBoxConfData);
        }

        /**
		 *  @brief          设定游戏配置数据
		 *  @param          sBoxIdeaConfData 游戏参数 
		 *  @return         SBoxBaseData.value[0] = 0：成功
		 *                  SBoxBaseData.value[0] < 0：发送参数错误
		 *                  SBoxBaseData.value[0] = 1：需验证密码
		 *                  SBoxBaseData.value[1] = 0：无任何修改参数的权限
		 *                  SBoxBaseData.value[1] = 1：普通密码权限
		 *                  SBoxBaseData.value[1] = 2：管理员密码权限
		 *                  SBoxBaseData.value[1] = 3：超级管理员密码权限
		 *  @details        
		 */
        public static void WriteConf(SBoxConfData sBoxConfData)
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20002, source: 1, target: 2, size: 20);

            sBoxPacket.data[0] = sBoxConfData.PlaceType;
            sBoxPacket.data[1] = sBoxConfData.difficulty;
            sBoxPacket.data[2] = sBoxConfData.odds;
            sBoxPacket.data[3] = sBoxConfData.WinLock;
            sBoxPacket.data[4] = sBoxConfData.MachineId;
            sBoxPacket.data[5] = sBoxConfData.LineId;
            sBoxPacket.data[6] = sBoxConfData.TicketMode;
            sBoxPacket.data[7] = sBoxConfData.TicketValue;
            sBoxPacket.data[8] = sBoxConfData.scoreTicket;
            sBoxPacket.data[9] = sBoxConfData.CoinValue;
            sBoxPacket.data[10] = sBoxConfData.MaxBet;
            sBoxPacket.data[11] = sBoxConfData.MinBet;
            sBoxPacket.data[12] = sBoxConfData.CountDown;
            sBoxPacket.data[13] = 0;
            sBoxPacket.data[14] = 0;
            sBoxPacket.data[15] = 0;
            sBoxPacket.data[16] = 0;
            sBoxPacket.data[17] = 0;
            sBoxPacket.data[18] = 0;
            sBoxPacket.data[19] = 0;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, WriteConfR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void WriteConfR(SBoxPacket sBoxPacket)
        {
            SBoxPermissionsData sBoxPermissionsData = new SBoxPermissionsData()
            {
                result = sBoxPacket.data[0],
                permissions = sBoxPacket.data[1]
            };
            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_WRITE_CONF, sBoxPermissionsData);
        }

        /**
		 *  @brief          密码验证
		 *  @param[in]      password 密码值（十进制），
		 *                  6位有效数字：普通密码，
		 *                  8位有效数字：管理员密码，
		 *                  9位有效数字：超级管理员密码
		 *  @return         SBoxBaseData.value[0] = 0：成功
		 *                  SBoxBaseData.value[0] < 0：发送参数错误
		 *                  SBoxBaseData.value[0] = 1：需验证密码
		 *                  SBoxBaseData.value[1] = 0：无任何修改参数的权限
		 *                  SBoxBaseData.value[1] = 1：普通密码权限
		 *                  SBoxBaseData.value[1] = 2：管理员密码权限
		 *                  SBoxBaseData.value[1] = 3：超级管理员密码权限
		 *  @details        1、IDEA需实现防止穷举的功能，连续5次失败后，就要等机器连续运行30分钟后才能再次验证密码。
         *                  2、需要验证密码的数据操作，一定要在通过了密码验证后才可以操作，并有20分钟限制，超时失效，需再次验证。
		 */
        public static void CheckPassword(int password)
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20003, source: 1, target: 2, size: 2);

            sBoxPacket.data[0] = password;
            sBoxPacket.data[1] = 0;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, CheckPasswordR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void CheckPasswordR(SBoxPacket sBoxPacket)
        {
            SBoxPermissionsData sBoxPermissionsData = new SBoxPermissionsData()
            {
                result = sBoxPacket.data[0],
                permissions = sBoxPacket.data[1]
            };
            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_CHECK_PASSWORD, sBoxPermissionsData);
        }

        /**
		 *  @brief          修改密码
		 *  @param[in]      password 密码值（十进制），
		 *                  6位有效数字：普通密码，
		 *                  8位有效数字：管理员密码，
		 *                  9位有效数字：超级管理员密码
		 *  @return         SBoxBaseData.value[0] = 0：成功
		 *                  SBoxBaseData.value[0] < 0：发送参数错误
		 *                  SBoxBaseData.value[0] = 1：需验证密码
		 *                  SBoxBaseData.value[1] = 0：无任何修改参数的权限
		 *                  SBoxBaseData.value[1] = 1：普通密码权限
		 *                  SBoxBaseData.value[1] = 2：管理员密码权限
		 *                  SBoxBaseData.value[1] = 3：超级管理员密码权限
		 *  @details        1、IDEA需实现防止穷举的功能，连续5次失败后，就要等机器连续运行30分钟后才能再次验证密码。
         *                  2、需要验证密码的数据操作，一定要在通过了密码验证后才可以操作，并有20分钟限制，超时失效，需再次验证。
		 */
        public static void ChangePassword(int password)
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20004, source: 1, target: 2, size: 2);

            sBoxPacket.data[0] = password;
            sBoxPacket.data[1] = 0;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, ChangePasswordR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void ChangePasswordR(SBoxPacket sBoxPacket)
        {
            SBoxPermissionsData sBoxPermissionsData = new SBoxPermissionsData()
            {
                result = sBoxPacket.data[0],
                permissions = sBoxPacket.data[1]
            };
            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_CHANGE_PASSWORD, sBoxPermissionsData);
        }

        /**
		 *  @brief          请求打码
		 *  @param          flag 打码的类似
		 *  @return         SBoxCoderData.result = 0：成功
		 *                  SBoxCoderData.result < 0：发送参数错误
		 *                  SBoxCoderData.result = 1：需验证密码
		 *  @details        
		 */
        public static void RequestCoder(int flag)
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20005, source: 1, target: 2, size: 2);

            sBoxPacket.data[0] = flag;
            sBoxPacket.data[1] = 0;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, RequestCoderR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void RequestCoderR(SBoxPacket sBoxPacket)
        {
            SBoxCoderData sBoxCoderData = new SBoxCoderData();
            sBoxCoderData.result = sBoxPacket.data[0];
            sBoxCoderData.TotalBets = sBoxPacket.data[1];
            sBoxCoderData.TotalWin = sBoxPacket.data[2];
            sBoxCoderData.MachineId = sBoxPacket.data[3];
            sBoxCoderData.CoderCount = sBoxPacket.data[4];
            sBoxCoderData.CheckValue = sBoxPacket.data[5];
            sBoxCoderData.RemainMinute = sBoxPacket.data[6];

            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_REQUEST_CODER, sBoxCoderData);
        }

        /**
		 *  @brief          打码
		 *  @param[in]      flag 打码的类似
		 *  @param[in]      Code 无符号64位整型数据码
		 *  @return         SBoxBaseData.value[0] = 0：成功
		 *                  SBoxBaseData.value[0] < 0：发送参数错误
		 *                  SBoxBaseData.value[0] = 1：需验证密码
		 *                  SBoxBaseData.value[1] = 0：无任何修改参数的权限
		 *                  SBoxBaseData.value[1] = 1：普通密码权限
		 *                  SBoxBaseData.value[1] = 2：管理员密码权限
		 *                  SBoxBaseData.value[1] = 3：超级管理员密码权限
		 *  @details        
		 */
        public static void Coder(int flag, ulong Code)
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20006, source: 1, target: 2, size: 4);
            uint tmp = (uint)Code;

            sBoxPacket.data[0] = flag;
            sBoxPacket.data[1] = (int)tmp;
            Code >>= 32;
            tmp = (uint)Code;
            sBoxPacket.data[2] = (int)tmp;
            sBoxPacket.data[3] = 0;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, CoderR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void CoderR(SBoxPacket sBoxPacket)
        {
            SBoxPermissionsData sBoxPermissionsData = new SBoxPermissionsData()
            {
                result = sBoxPacket.data[0],
                permissions = sBoxPacket.data[1]
            };
            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_CODER, sBoxPermissionsData);
        }

        /**
		 *  @brief          获取每一门的倍率
		 *  @param          flag 打码的类似
		 *  @return         SBoxBaseData.value[0] = 0：成功
		 *                  SBoxBaseData.value[0] < 0：发送参数错误
		 *                  SBoxBaseData.value[0] > 0：状态码
		 *  @details        
		 */
        public static void GetOdds()
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20007, source: 1, target: 2, size: 2);

            sBoxPacket.data[0] = 0;
            sBoxPacket.data[1] = 0;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, GetOddsR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void GetOddsR(SBoxPacket sBoxPacket)
        {
            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_GET_ODDS, sBoxPacket.data);
        }

        /**
		 *  @brief          预开奖，开始押注前调用，算法卡会告知一些特定的信息，如算法卡的状态，需要在押注过程中有所表现的小游戏，比如：明牌等等
		 *  @param          无
		 *  @return         SBoxBaseData.value[0] = 0：成功
		 *                  SBoxBaseData.value[0] < 0：发送参数错误
		 *                  SBoxBaseData.value[0] > 0：状态码
		 *  @details        
		 */
        public static void GetPrizePrepare()
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20008, source: 1, target: 2, size: 4);

            sBoxPacket.data[0] = 0;
            sBoxPacket.data[1] = 0;
            sBoxPacket.data[2] = 0;
            sBoxPacket.data[3] = 0;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, GetPrizePrepareR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void GetPrizePrepareR(SBoxPacket sBoxPacket)
        {
            SBoxPrizePrepareData sBoxPrizePrepareData = new SBoxPrizePrepareData()
            {
                result = sBoxPacket.data[0],
                state = sBoxPacket.data[1],
            };
            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_GET_PRIZE_PREPARE, sBoxPrizePrepareData);
        }

        /**
		 *  @brief          开奖
		 *  @return         SBoxPrizeData.result = 0：成功
		 *                  SBoxPrizeData.result < 0：发送参数错误
		 *                  SBoxPrizeData.result > 0：状态码
		 *  @details        
		 */
        public static void GetPrize()
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20009, source: 1, target: 2, size: 2);

            sBoxPacket.data[0] = 0;
            sBoxPacket.data[1] = 0;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, GetPrizeR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void GetPrizeR(SBoxPacket sBoxPacket)
        {
            SBoxPrizeData sBoxPrizeData = new SBoxPrizeData();

            if(sBoxPacket.data.Length > 10)
            {
                sBoxPrizeData.result = sBoxPacket.data[0];
                sBoxPrizeData.PersonId = sBoxPacket.data[1];
                sBoxPrizeData.FeatureId = sBoxPacket.data[2];
                sBoxPrizeData.Jackpot = sBoxPacket.data[3];

                sBoxPrizeData.CoinToTable[0] = sBoxPacket.data[4];
                sBoxPrizeData.CoinToTable[1] = sBoxPacket.data[5];
                sBoxPrizeData.CoinToTable[2] = sBoxPacket.data[6];
                sBoxPrizeData.CoinToTable[3] = sBoxPacket.data[7];
                sBoxPrizeData.CoinToTable[4] = sBoxPacket.data[8];

                sBoxPrizeData.Data = new int[sBoxPacket.data.Length - 10];
                for(int i = 10;i < sBoxPrizeData.Data.Length;i++)
                {
                    sBoxPrizeData.Data[i - 10] = sBoxPacket.data[i];
                }
            }
            else
            {
                sBoxPrizeData.result = -1;
            }
            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_GET_PRIZE, sBoxPrizeData);
        }

        /**
		 *  @brief          读取指定玩家的中奖数据
		 *  @param          无
		 *  @return         SBoxPrizePlayerData.result = 0：成功
		 *                  SBoxPrizePlayerData.result < 0：发送参数错误
		 *                  SBoxPrizePlayerData.result > 0：状态码
		 *  @details        
		 */
        public static void GetPrizePlayer(int PlayerId)
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20010, source: 1, target: 2, size: 2);

            sBoxPacket.data[0] = PlayerId;
            sBoxPacket.data[1] = 0;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, GetPrizePlayerR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void GetPrizePlayerR(SBoxPacket sBoxPacket)
        {
            SBoxPrizePlayerData sBoxPrizePlayerData = new SBoxPrizePlayerData();

            sBoxPrizePlayerData.result = sBoxPacket.data[0];
            sBoxPrizePlayerData.PlayerId = sBoxPacket.data[1];

            for(int i = 2; i < (15 + 2); i++)
            {
                sBoxPrizePlayerData.WinRole[i - 2] = sBoxPacket.data[i];
            }
            sBoxPrizePlayerData.Win = sBoxPacket.data[17];
            sBoxPrizePlayerData.WinJackpot = sBoxPacket.data[18];
            sBoxPrizePlayerData.BoxPoints = sBoxPacket.data[19];
            sBoxPrizePlayerData.OpenBoxID = sBoxPacket.data[20];
            sBoxPrizePlayerData.WinBoxGame = sBoxPacket.data[21];
            sBoxPrizePlayerData.BoxPointsLength = sBoxPacket.data[22];
            sBoxPrizePlayerData.BoxOpenFlags[0] = sBoxPacket.data[23];
            sBoxPrizePlayerData.BoxOpenFlags[1] = sBoxPacket.data[24];
            sBoxPrizePlayerData.BoxOpenFlags[2] = sBoxPacket.data[25];
            sBoxPrizePlayerData.BoxOpenFlags[3] = sBoxPacket.data[26];

            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_GET_PRIZE_PLAYER, sBoxPrizePlayerData);
        }

        /**
		 *  @brief          设定玩家押分数据
		 *  @param          sBoxPlayerBetsData 玩家的押注数据
		 *  @return         SBoxBaseData.value[0] = 0：成功
		 *                  SBoxBaseData.value[0] < 0：发送参数错误
		 *                  SBoxBaseData.value[0] > 0：状态码
		 *  @details        
		 */
        public static void SetPlayerBets(SBoxPlayerBetsData sBoxPlayerBetsData)
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20011, source: 1, target: 2, size: sBoxPlayerBetsData.Bets.Length + 3);

            sBoxPacket.data[0] = sBoxPlayerBetsData.PlayerId;
            sBoxPacket.data[1] = sBoxPlayerBetsData.balance;
            sBoxPacket.data[2] = sBoxPlayerBetsData.rfu;
            for(int i = 3; i < sBoxPacket.data.Length;i++)
            {
                sBoxPacket.data[i] = sBoxPlayerBetsData.Bets[i - 3];
            }

            SBoxIOEvent.AddListener(sBoxPacket.cmd, SetPlayerBetsR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void SetPlayerBetsR(SBoxPacket sBoxPacket)
        {
            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SET_PLAYER_BETS, sBoxPacket.data[0]);
        }

        /**
		 *  @brief          设定掉到坑里的各种金币的数量
		 *  @param          coinToHoleCount 掉到坑里的各种金币的数量
		 *                  [0]: 银币数量-1分
		 *                  [1]: 金币数量-2分
		 *                  [2]: 元宝数量-5分
		 *                  [3]: 彩球数量-10分
		 *                  [4]: 彩盘数量-20分
		 *  @return         SBoxBaseData.value[0] = 0：成功
		 *                  SBoxBaseData.value[0] < 0：发送参数错误
		 *                  SBoxBaseData.value[0] > 0：状态码
		 *  @details        
		 */
        public static void SetCoinToHoleCount(int[] coinToHoleCount)
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20012, source: 1, target: 2, size: 5);

            sBoxPacket.data[0] = coinToHoleCount[0];
            sBoxPacket.data[1] = coinToHoleCount[1];
            sBoxPacket.data[2] = coinToHoleCount[2];
            sBoxPacket.data[3] = coinToHoleCount[3];
            sBoxPacket.data[4] = coinToHoleCount[4];

            SBoxIOEvent.AddListener(sBoxPacket.cmd, SetCoinToHoleCountR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void SetCoinToHoleCountR(SBoxPacket sBoxPacket)
        {
            BlizzEvent.EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_SET_COIN_TO_HOLE_COUNT, sBoxPacket.data[0]);
        }
    }
}
