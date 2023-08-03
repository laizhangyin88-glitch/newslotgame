using BagelCode.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    //public enum SERVICE
    //{
    //    NONE = 0,
    //    CLIENT = 1,
    //    APISERVER = 2,//网关,转发服务
    //}

    //public enum HALLCMD
    //{
    //    LOGINReq = 2100,
    //    LOGINRsp = 2101,

    //    HeartHeatReq = 2102,
    //    HeartHeatRsp = 2103,
    //}

    //[System.Serializable]
    //public partial class RpcPacket
    //{
    //    public UInt32 MsgId;
    //    public SERVICE ServiceType;
    //    public UInt32 SrcClusterId;    //源集群id,客户端不需要填
    //    public UInt32 DstClusterId;    //目标集群id,客户端不需要填
    //    public byte[] RpcBody;      //具体的消息体
    //}

    //[System.Serializable]
    //public partial class HeartHeat
    //{
    //    public ulong SendTime;
    //    public ulong ReceiveTime;
    //}

    //[System.Serializable]
    //public partial class LoginHallReq
    //{
    //    public string playerId;
    //}

    //[System.Serializable]
    //public partial class LoginHallRsp
    //{
    //    public int result;
    //    public List<RoomStatusInfo> rooms;
    //}

    //[System.Serializable]
    //public partial class RoomStatusInfo
    //{
    //    public int game_id;
    //    public int room_id;
    //    public string room_name;
    //    public long enter_min;
    //    public long enter_max;
    //    public int status;
    //}

    public class FishEnterGameRsp
    {
        public int result;
        public int chair_count;
        public int my_chair_id;
        public int game_id;
        public int room_id;
        public int game_bus_id;
    }

    public class FishCannonInfo
    {
        public int cannon_id;
        public int cannon_value;
        public int cannon_value_gun_id;
    }

    public class FishUserInfo
    {
        public int chair_id;
        public string user_name;
        public long user_money;
        public int cannon_id;
    }
}

