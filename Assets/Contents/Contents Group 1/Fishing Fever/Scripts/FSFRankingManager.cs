using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using System;
using TMPro;
using UnityEngine.UI;
using NodeCanvas.Framework;


namespace GameStudio.Slot.FSF
{
    public class FSFRankingManager : FeatureController
    {
        [SerializeField]
        private List<FSFRankUI> userUIList;
        [SerializeField]
        private Sprite botProfileSprite;


        protected override string ON_FEATURE_BEGIN_EVENT => "SetUpRankingUI";
        protected override string ON_FEATURE_END_EVENT => "SetUpRankingUIDone";

        protected override IEnumerator OnPlayCoroutine()
        {
            var userCommunityResultList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./bonus/response/userGameResultList").value;
            userCommunityResultList.Sort(new BlackboardSorterByEarnCredit());

            for(int i = 0; i < userUIList.Count; i++)
            {
                var rankUI = userUIList[i];
                if (userCommunityResultList[i].GetValue<bool>("isBot"))
                {
                    rankUI.CoinText.text = 0L.ToString(); // FormatUtility.CommaNumberFormat(userCommunityResultList[i].GetValue<long>("totalEarnCredit"));
                    rankUI.UserNameText.text = String.Empty; //userCommunityResultList[i].GetValue<string>("userName");
                }
                else
                {
                    rankUI.CoinText.text = FormatUtility.CommaNumberFormat(userCommunityResultList[i].GetValue<long>("totalEarnCredit"));
                    rankUI.UserNameText.text = userCommunityResultList[i].GetValue<string>("userName");
                }
            }

            for(int i = 0; i < userUIList.Count; i++)
            {
                var rankUI = userUIList[i];
                 rankUI.Profile.sprite = botProfileSprite;
                if (userCommunityResultList[i].GetValue<bool>("isBot") == false)
                {
                    WebImageDownloader.Instance.LoadWebImage(
                         userCommunityResultList[i].GetValue<string>("profileUrl"),
                         CacheType.MemCache,
                         false,
                         null,
                         delegate (Sprite img)
                         {
                             rankUI.Profile.sprite = img;
                         }
                     );
                }
            }

            yield break;
        }

        protected override void OnStart() { }


        protected override void OnFinish() { }
    }

    public class BlackboardSorterByEarnCredit : IComparer<IBlackboard>
    {
        public int Compare(IBlackboard bb0, IBlackboard bb1)
        {
            int compareResult = -1;
            var gap = bb1.GetValue<long>("totalEarnCredit") - bb0.GetValue<long>("totalEarnCredit");
            compareResult = gap == 0 ? 0 : ( gap > 0 ? 1 : -1);

            return compareResult;
        }
    }

    [Serializable]
    public class FSFRankUI
    {
        [SerializeField]
        private TextMeshProUGUI coinText;
        [SerializeField]
        private TextMeshProUGUI userNameText;
        [SerializeField]
        private Image profile;

        public TextMeshProUGUI CoinText { get => coinText; }
        public TextMeshProUGUI UserNameText { get => userNameText; }
        public Image Profile { get => profile; }
    }
}
