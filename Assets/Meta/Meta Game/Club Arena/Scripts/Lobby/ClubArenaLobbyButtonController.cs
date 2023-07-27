using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using Sirenix.OdinInspector;

namespace BagelCode.ClubArena
{
    public class ClubArenaLobbyButtonController : ClubArenaButtonBaseController
    {
        public override void InitProperty()
        {
            base.InitProperty();

            InitProperty(root);

            MetaContextElementUtils.SetClickable(
                root,
                "OnEnterClubArena",
                root,
                null
            );
        }

        protected override void InitIconController()
        {
            base.InitIconController();

            iconController?.SetLocked(false);
        }

        public void ReturnSceneToLobby()
        {
            UpdateBadge();
            iconController.UpdateExpGauge();
        }

#if UNITY_EDITOR

        [Button]
        private void TestClubNoticePopup()
        {
            ClubArenaRankingPopupInfo clubArenaRankingPopupInfo = new ClubArenaRankingPopupInfo();
            clubArenaRankingPopupInfo.backgroundImageUrl = "https://cdn.bagelgames.com/SLOTS1/images/club_arena/tinify/Club Arena End Notice.png";
            clubArenaRankingPopupInfo.clubRanking = new List<ClientModels.ClubArenaClubRanking>();
            for (int i = 0; i < 20; ++i)
            {
                ClientModels.ClubArenaClubRanking rank = new ClientModels.ClubArenaClubRanking();
                rank.clubId = 1270;
                rank.clubName = string.Format("Dummy Name {0}", i + 1);
                rank.clubSymbol = "Club Symbol 15";
                rank.ranking = i + 1;
                rank.totalPoint = 100000 - (i * 10);
                clubArenaRankingPopupInfo.clubRanking.Add(rank);
            }

            ClientAPI2Blackboard.Serialize(BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "clubArenaRankingPopupInfo"), clubArenaRankingPopupInfo);

            StartCoroutine(OpenClubArenaNoticePopupCoroutine());
        }

        private IEnumerator OpenClubArenaNoticePopupCoroutine()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Club Arena End Notice Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject popupObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => popupObj = sceneLoadOperation.GetScene()));

            MetaPopupUtils.OpenPopup(popupObj);
        }
#endif
    }
}
