using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class NetworkData_Login : Singleton<NetworkData_Login>
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string NetData_UserId => BlackboardUtils.FindVariable<string>("/me/userId")?.value ?? "";
        /// <summary>
        /// 用户名字
        /// </summary>
        public string NetData_UserName => BlackboardUtils.FindVariable<string>("/me/name")?.value ?? "";
        /// <summary>
        /// 用户资产
        /// </summary>
        public long NetData_UserCredit => BlackboardUtils.FindVariable<long>("/me/credit")?.value ?? default;

        public int UserLevel => BlackboardUtils.FindVariable<int>("/level")?.value ?? default;
        public string UserProfileUrl => BlackboardUtils.FindVariable<string>("/profile_url")?.value ?? default;
    }
}
