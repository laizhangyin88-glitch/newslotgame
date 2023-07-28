using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Slots.TRR.Utillity
{
    public class TRRProfileManager : MonoWeakSingleton<TRRProfileManager>
    {
        Dictionary<string, Sprite> userProfilePool;
        HashSet<string> loadingTask;

        public bool IsCachedProfile(string userID) => userProfilePool.ContainsKey(userID);
        public Sprite GetUserProfileImage(string userID) => userProfilePool[userID];

        private void Awake()
        {
            userProfilePool = new Dictionary<string, Sprite>();
            loadingTask = new HashSet<string>();
        }
        public bool LoadFinish() => loadingTask.Count == 0;
        public bool IsLoading(string userID) => loadingTask.Contains(userID);

        public Coroutine LoadUserProfileCoroutine(List<string> userIDList, Action<Sprite> callback = null) => StartCoroutine(_LoadUserProfileAsync(userIDList, callback));
        IEnumerator _LoadUserProfileAsync(List<string> userIDList, Action<Sprite> callback = null)
        {
            List<string> requiredLoadIdList = new List<string>();

            foreach (var each in userIDList)
            {
                //if each id was already loaded or in loading, that will be ignored. 
                if (IsLoading(each) == false && IsCachedProfile(each) == false)
                    requiredLoadIdList.Add(each);
            }

            // if count of required load id list is zero, end this function. 
            if (requiredLoadIdList.Count == 0) yield break;

            foreach (var each in requiredLoadIdList)
            {
                loadingTask.Add(each);
            }

            bool loadedProfileInfo = false;
            bool isSuccess = false;
            UserProfileListResponse loadedResponse = null;
            BagelCodeClientAPI.GetUserProfileList(requiredLoadIdList,
            (UserProfileListResponse response) =>
            {
                loadedProfileInfo = true;
                loadedResponse = response;
                isSuccess = true;
            },
            (BagelCodeHTTPError errorInfo) =>
            {
                loadedProfileInfo = true;
                Debug.LogError($"Failed Load UserInfo\nError : {errorInfo.error}\nError Detail : {errorInfo.errorDetailInfo}");
                isSuccess = false;
            });

            yield return new WaitUntil(() => loadedProfileInfo == true);

            if (isSuccess == false) yield break;

            var userProfileList = loadedResponse.userProfileList;

            foreach (var each in userProfileList)
            {
                string profileUrl = each.profileUrl;

                bool isLoadedProfileImage = false;
                Sprite loadedSprite = null;
                WebImageDownloader.Instance.LoadWebImage(
                     profileUrl,
                     CacheType.MemCache,
                     false,
                     null,
                     delegate (Sprite img)
                     {
                         isLoadedProfileImage = true;
                         loadedSprite = img;
                     }
                 );

                yield return new WaitUntil(() => isLoadedProfileImage == true);
                userProfilePool.Add(each.userId, loadedSprite);
                callback?.Invoke(loadedSprite);
                loadingTask.Remove(each.userId);
            }
        }
    }
}