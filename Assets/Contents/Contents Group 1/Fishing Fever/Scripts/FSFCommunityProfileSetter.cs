using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using UnityEngine.UI;

namespace GameStudio.Slot.FSF
{
    public class FSFCommunityProfileSetter : FeatureController
    {
        [SerializeField]
        private List<Image> userProfileImageList;
        [SerializeField]
        private Sprite botProfileSprite;

        protected override string ON_FEATURE_BEGIN_EVENT { get => "LoadUserProfilePictures"; }
        protected override string ON_FEATURE_END_EVENT { get => "EndLoadUserProfilePictures"; }

        protected override IEnumerator OnPlayCoroutine()
        {
            var userCommunityGameResultList = BlackboardUtils.FindValue<List<Blackboard>>(null, "./bonus/response/userGameResultList");

            // exclude this client's info , eg 0st element
            for (int i = 1; i < userCommunityGameResultList.Count; i++)
                if (userCommunityGameResultList[i].GetValue<bool>("isBot"))
                {
                    userProfileImageList[i-1].sprite = botProfileSprite;
                }
                else
                {
                    Sprite profilePic = null;
                    bool hadLoadedProile = false;
                    WebImageDownloader.Instance.LoadWebImage(
                         userCommunityGameResultList[i].GetValue<string>("profileUrl"),
                         CacheType.MemCache,
                         false,
                         null,
                         delegate (Sprite img)
                         {
                             hadLoadedProile = true;
                             profilePic = img;
                         }
                     );
                    yield return new WaitUntil(() => hadLoadedProile == true);
                    userProfileImageList[i-1].sprite = profilePic;
                }

            yield return new WaitForSeconds(0.1f);
        }

        protected override void OnFinish()
        {
        }

        protected override void OnStart()
        {
        }
    }
}