using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class LobbyBottomShopDealIconController : MonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private const string IAM_KEY = "IAMTimer:{0}";
        private const string IAM_DEAL_KEY = "IAMDeal_ID";

        private SimpleReserveTimer _reserveTimer;
        private SimpleReserveTimer reserveTimer
        {
            get
            {
                if (_reserveTimer == null)
                {
                    _reserveTimer = GetComponent<SimpleReserveTimer>();
                    if (_reserveTimer == null)
                        _reserveTimer = gameObject.AddComponent<SimpleReserveTimer>();
                }
                return _reserveTimer;
            }
        }

        private bool isInit = false;

        private void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            InitContext();

            isInit = true;
        }

        private void InitContext()
        {
            var eventTimerArea = ContextUtils.FindElement(root, "Event Tag Area", CHILDREN);

            var iamIdValue = PlayerPrefs.GetString(IAM_DEAL_KEY, "0");
            
            int iamID = System.Convert.ToInt32(iamIdValue);
            var iamBB = BlackboardQueryUtils.GetIAMBlackboard(iamID);
            MetaContextElementUtils.SetActive(eventTimerArea, iamBB != null);

            if(iamBB != null && IAMRouter.Instance.IsValidIAMasDeal(iamBB))
            {
                long currentTimestamp = TimeUtils.GetTimeStamp();
                long endTimestamp = GetEndTimestamp(iamBB);
                var eventTagObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Event Tag Without Text", eventTimerArea.transform);
                var eventTagController = eventTagObject.GetComponent<EventTagController>();

                eventTagController.Initialize(endTimestamp,
                                                "TIME_FORMAT_HHMMSS_TOTALHOUR",
                                                null,
                                                "Ended",
                                                true,
                                                null,
                                                null
                );

                reserveTimer.SetReserveCallback(endTimestamp,
                    () => 
                    {
                        gameObject.SetActive(false);
                    }
                );

            }
            else
            {
                PlayerPrefs.DeleteKey(IAM_DEAL_KEY);
            }

            MetaContextElementUtils.SetClickable(root, () => {
                    EventSender.SendGlobalEvent("TriggerDeal");
            });
        }

        private void EndEvent()
        {
            var eventTimerArea = ContextUtils.FindElement(root, "Event Timer Area", CHILDREN);
            MetaContextElementUtils.SetActive(eventTimerArea, false);
        }

        private long GetEndTimestamp(Blackboard iamInfoBB)
        {
            long currentTimestamp = BagelCode.TimeUtils.GetTimeStamp();

            long endTimestamp = BlackboardUtils.FindVariable<long>(iamInfoBB, "endTimestamp").value;
            bool useUserTimer = BlackboardUtils.FindVariable<bool>(iamInfoBB, "useUserTimer").value;
            int userTimerMin = BlackboardUtils.FindVariable<int>(iamInfoBB, "userTimerMin").value;

            string id = BlackboardUtils.FindVariable<int>(iamInfoBB, "id").value.ToString();
            string iamKey = string.Format(IAM_KEY, id);
            string userStartTimeText = PlayerPrefs.GetString(iamKey, "0");
            long userStartTimestamp = System.Convert.ToInt64(userStartTimeText);
            long userEndTimestamp = userStartTimestamp + (System.Convert.ToInt64(userTimerMin) * 60000L);

            if(useUserTimer && userTimerMin > 0)
            {
                if(endTimestamp == 0L)
                {
                    endTimestamp = userEndTimestamp;
                }
                else
                {
                    if(userEndTimestamp < endTimestamp)
                    {
                        endTimestamp = userEndTimestamp;
                    }
                }
            }

            if(endTimestamp == 0L || (endTimestamp > 0 && currentTimestamp < endTimestamp))
            {
                return endTimestamp;
            }
            else
            {
                return -1L;
            }
        }

        #if UNITY_EDITOR
        [Button]
        private void TestEndEvent()
        {
            EndEvent();
        }
        #endif
    }
}
