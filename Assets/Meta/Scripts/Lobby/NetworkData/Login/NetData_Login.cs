using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using System;

namespace SlotMaker
{
    public class NetData_Login : NetDataBase<NetData_Login>
    {
        public const string Path_UserId = "/me/userId";
        public const string Path_UserName = "/me/name";
        public const string Path_UserCredit = "/me/credit";
        public const string Path_UserLevel = "/level";
        public const string Path_UserProfileUrl = "/profile_url";

        /// <summary>
        /// 用户id
        /// </summary>
        public string NetData_UserId => GetNetDataValue<string>(Path_UserId);
        /// <summary>
        /// 用户名字
        /// </summary>
        public string NetData_UserName => GetNetDataValue<string>(Path_UserName);
        /// <summary>
        /// 用户资产
        /// </summary>
        public long NetData_UserCredit => GetNetDataValue<long>(Path_UserCredit);

        public int UserLevel => GetNetDataValue<int>(Path_UserLevel);
        public string UserProfileUrl => GetNetDataValue<string>(Path_UserProfileUrl);
    }
}
