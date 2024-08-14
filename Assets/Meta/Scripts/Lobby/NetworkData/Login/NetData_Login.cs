using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using System;
using SimpleJSON;
using BagelCode.ClientModels;

namespace SlotMaker
{
    public class NetData_Login : NetDataBase<NetData_Login>
    {
        public const string Path_UserId = "/me/userId";
        public const string Path_UserName = "/me/name";
        public const string Path_UserCredit = "/me/credit";
        public const string Path_UserLevel = "/level";
        public const string Path_UserProfileUrl = "/profile_url";
        public const string Path_ProfilePictures = "/profile_pictures";

        /// <summary>
        /// 用户id
        /// </summary>
        public string NetData_UserId => GetNetDataValue<string>(Path_UserId);
        /// <summary>
        /// 用户名字
        /// </summary>
        public string NetData_UserName => GetNetDataValue<string>(Path_UserName);
        /// <summary>
        /// 用户资产(可靠的，实时更新的用户资产)
        /// </summary>
        public long NetData_UserCredit => GetNetDataValue<long>(Path_UserCredit);
        /// <summary>
        /// 用户等级
        /// </summary>
        public int UserLevel => GetNetDataValue<int>(Path_UserLevel);

        /// <summary>
        /// 头像数据
        /// </summary>
        public JSONNode ProfilePictures => GetNetDataValue<JSONNode>(Path_ProfilePictures);
        /// <summary>
        /// 当前用户等级可选的头像数据
        /// </summary>
        public List<Tuple<int, string, string>> ProfilePicturesWithCurrentLevel => GetProfilePicturesByLevel(UserLevel);
        /// <summary>
        /// 用户当前头像地址
        /// </summary>
        public string UserProfileUrl
        {
            get
            {
                string url = GetNetDataValue<string>(Path_UserProfileUrl);
                if (url == null)
                    return null;

                if (url != "null")
                    return url.Trim('"');

                var profilePictures = GetProfilePicturesByLevel(1);
                if(profilePictures == null)
                    return null;

                return profilePictures[0].Item2;
            }
        }

        /// <summary>
        /// 根据当前等级获取可选的头像列表
        /// </summary>
        /// <remarks>
        /// 已根据头像id排序
        /// </remarks>
        /// <param name="level"></param>
        /// <returns>元组：item1=>头像id, item2=>头像Url, item3=>类型male或female</returns>
        protected List<Tuple<int, string, string>> GetProfilePicturesByLevel(int level)
        {
            if (level < 0)
                return null;

            List<Tuple<int, string, string>> ret = new List<Tuple<int, string, string>>();

            foreach (var item in ProfilePictures)
            {
                int itemLevel = item.Value["level"].AsInt;
                if (itemLevel > level)
                    continue;

                int itemId = item.Value["id"].AsInt;
                string itemUrl = item.Value["image_url"].ToString().Trim('"');
                string itemType = item.Value["type"].ToString().Trim('"');

                ret.Add(new Tuple<int, string, string>(itemId, itemUrl, itemType));
            }

            if (ret.Count <= 0)
                return null;

            ret.Sort((item1, item2) => item1.Item1 - item2.Item1);
            return ret;
        }

    }
}
