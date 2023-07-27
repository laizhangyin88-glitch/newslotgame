using UnityEngine;
using SlotMaker;
using System.Collections.Generic;
using ParadoxNotion;
using BagelCode.ClientModels;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsGurusRankingBoardController : MonoBehaviour
    {
        private ContextElement root;

        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;

        private bool isInit = false;

        private void OnEnable()
        {
            if (isInit)
            {
                UpdateRanking();
            }
        }

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();

            root.UpdateContext(false);
            UpdateRanking();

            isInit = true;
        }

        public void UpdateRanking()
        {
            if (VegasDreams.Utils.GurusBuilding == null || PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.BUILD_DREAM_SEASON) == null) return;

            MetaContextElementUtils.SimpleSetActive(root, "Gurus Ranking Board/Loading", true, FULL);

            BagelCodeClientAPI.RequestVegasDreamGurusRankingList(
                (response) =>
                {
                    ClientAPI2Blackboard.Serialize(VegasDreams.Utils.MainSceneBlackboard, response);

                    var rankList = response.gurusRankList;
                    for (int i = 0; i < rankList.Count; i++)
                    {
                        MetaContextElementUtils.SimpleSetActive(root, $"Gurus Ranking Board/Cell Group/Cell Area {i + 1}", true, FULL);

                        var info = rankList[i];
                        var profile = rankList[i].profile;
                        MetaContextElementUtils.SimpleSetActive(root, $"Gurus Ranking Board/Cell Group/Cell Area {i + 1}/Gurus Ranking Cell/My Cell Glow", profile.userId == BlackboardQueryUtils.GetMyUserId(), FULL);
                        MetaContextElementUtils.SimpleSetText(root, $"Gurus Ranking Board/Cell Group/Cell Area {i + 1}/Gurus Ranking Cell/Text User Name", profile.name, FULL);
                        MetaContextElementUtils.SimpleSetText(root, $"Gurus Ranking Board/Cell Group/Cell Area {i + 1}/Gurus Ranking Cell/Text Rank", $"{info.rank}th", FULL);
                        MetaContextElementUtils.SimpleSetText(root, $"Gurus Ranking Board/Cell Group/Cell Area {i + 1}/Gurus Ranking Cell/Text Level", $"{info.level}", FULL);
                        MetaContextElementUtils.SimpleSetClickable(root, $"Gurus Ranking Board/Cell Group/Cell Area {i + 1}/Gurus Ranking Cell", () => {
                            var eventData = new EventData<string>("OnOpenProfile", profile.userId);
                            EventSender.SendGlobalEvent(eventData);
                        }, true, FULL);

                        MetaContextElementUtils.SimpleSetIntProperty(root, $"Gurus Ranking Board/Cell Group/Cell Area {i + 1}/Gurus Ranking Cell/Profile Picture Normal", MetaSystem.GetTierGroup(profile.tier), FULL);
                        MetaContextElementUtils.SimpleSetWebImage(root, $"Gurus Ranking Board/Cell Group/Cell Area {i + 1}/Gurus Ranking Cell/Profile Picture Normal/Image", profile.profileUrl, null, null, FULL);

                        MetaContextElementUtils.SimpleSetActive(root, $"Gurus Ranking Board/Cell Group/Cell Area {i + 1}/Gurus Ranking Cell/Icon 1st", info.rank == 1, FULL);
                        MetaContextElementUtils.SimpleSetActive(root, $"Gurus Ranking Board/Cell Group/Cell Area {i + 1}/Gurus Ranking Cell/Icon 2nd", info.rank == 2, FULL);
                        MetaContextElementUtils.SimpleSetActive(root, $"Gurus Ranking Board/Cell Group/Cell Area {i + 1}/Gurus Ranking Cell/Icon 3rd", info.rank == 3, FULL);
                        MetaContextElementUtils.SimpleSetActive(root, $"Gurus Ranking Board/Cell Group/Cell Area {i + 1}/Gurus Ranking Cell/Text Rank", info.rank >= 4, FULL);
                    }

                    MetaContextElementUtils.SimpleSetActive(root, "Gurus Ranking Board/Loading", false, FULL);
                },
                (error) =>
                {
                    GlobalErrorHandler.GlobalError(error);
                }
            );

        }
    }
}
