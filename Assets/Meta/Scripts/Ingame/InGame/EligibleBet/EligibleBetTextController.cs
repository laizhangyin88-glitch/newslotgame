using System.Collections.Generic;
using System.Collections;
using SlotMaker;
using UnityEngine;
using ParadoxNotion;

namespace BagelCode
{
    public class EligibleBetTextController : EventMonoBehaviour
    {
        private ContextElement root;
        private ContextElement eligibleBetAreaElement;

        private const float DISPLAY_TIME = 1.8f;
        private const float FIRST_DISPLAY_TIME = 6f;

        private float remaining = 0f;
        private bool isFirst = true;

        public List<EligibleBetItem> itemList = new List<EligibleBetItem>();
        public EligibleBetItem currentEligibleItem = null;

        private void Start()
        {
            StartCoroutine(InitCoroutine());
        }

        private IEnumerator InitCoroutine()
        {
            root = GetComponent<ContextElement>();

            root.UpdateContext(false);

            itemList.Clear();

            // Item List
            eligibleBetAreaElement = ContextUtils.FindElement(root, "Eligible Bet Balloon Area", ContextSearchingType.ChildrenSearch);
            int itemCount = eligibleBetAreaElement.transform.childCount;

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            RegisterHandleEventType(MetaEventDefine.ON_CREDIT_EVENT);

            Register(MetaEventDefine.ON_CREDIT_EVENT, "UpdatedTotalBetCredit", OnChangeBet);
            Register(MetaEventDefine.ON_CREDIT_EVENT, "UpdateExtraBetRatioIndex", OnExtraBet);
            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_LEVEL_UP, OnLevelUp);
            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_LOAD_SUCCESS_BUNDLE, OnLoadSuccessBundle);

            // OnChangeBet();

            yield return new WaitUntil(() => BlackboardUtils.FindVariable<bool>("./initializedContent")?.value ?? false);

            // 하이어라키 Area 하위에서 정렬된 순서가 위일 수록 우선순위 높음
            for (int i = 0; i < itemCount; ++i)
            {
                var itemObjs = eligibleBetAreaElement.transform.GetChild(i);
                var item = itemObjs.GetComponent<EligibleBetItem>();
                if (item != null)
                {
                    if(item.Init())
                        itemList.Add(item);
                }
            }

            OnChangeBet();
        }

        private void OnLoadSuccessBundle(EventData eventData)
        {
            switch ((MetaGameType)eventData.value)
            {
                case MetaGameType.BUILD_DREAM_SEASON:
                    {
                        var obj = MetaObjectUtils.MakePrefab(VegasDreams.VegasDreams.Defines.COMMON_BUNDLE, "Vegas Dreams Bet Progress", eligibleBetAreaElement.transform);
                        var item = obj.GetComponent<EligibleBetItem>();
                        if (item.Init())
                            itemList.Add(item);
                    }
                    break;
            }
        }

        private void OnExtraBet(EventData eventData) // on change bet 이 항상 먼저 호출됨
        {
            int index = (int)eventData.value;
            itemList.ForEach(i => i.UpdateExtraBetIndex(index));
        }

        private void OnChangeBet()
        {
            long totalBet = BlackboardQueryUtils.GetTotalBet();
            itemList.ForEach(i => i.UpdateBet(totalBet));

            var eligibleItem = GetEligibleItem();
            if(eligibleItem == null) return;

            if(currentEligibleItem != null && eligibleItem != currentEligibleItem)
                currentEligibleItem.Disappear();

            currentEligibleItem = eligibleItem;

            StopAllCoroutines();
            StartCoroutine(UpdateDisplayTextRemainingCoroutine());
        }

        private void OnLevelUp()
        {
            OnChangeBet();
        }

        private EligibleBetItem GetEligibleItem()
        {
#if DEV
            // item 설정 변경에 따라 DevEditorSettingsGeneral 의 ELIGIBLE_BET 세팅 수정 필요
            // enable 상관없이 항상 표시
            int setting = PlayerPrefs.GetInt("DEBUG_ELIGIBLE_BET");
            EligibleBetItem settingItem = null;
            // setting == 0 Default
            if (setting == 1) // LP
            {
                settingItem = itemList.Find(i => i is EligibleBetClubLeaguePoint);
            }
            else if(setting == 2) // HOG
            {
                settingItem = itemList.Find(i => i is EligibleBetHiddenUniverseFinder);
            }

            if (settingItem != null)
            {
                return settingItem;
            }
#endif

            foreach (var item in itemList)
            {
                // lp가 hog보다 활성화 뱃이 낮은경우
                // hog의 off뱃 미만일때 Available으로 설정되기 떄문에 항상 hog off상태만 보임.
                // 따라서 on이 가장 우선순위가 되어야함.
                if ( item.IsAvailable() )
                {
                    return item;
                }
            }

            return null;
        }

        private List<EligibleBetItem> GetAnotherEligibleItemList()
        {
            var result = new List<EligibleBetItem>(itemList);
            var current = GetEligibleItem();

            if (current != null)
                result.Remove(current);

            return result;
        }

        private IEnumerator UpdateDisplayTextRemainingCoroutine()
        {
            if (isFirst)
            {
                isFirst = false;
                remaining = FIRST_DISPLAY_TIME;
            }
            else
            {
                remaining = DISPLAY_TIME;
            }

            foreach(var item in GetAnotherEligibleItemList())
            {
                item.Disappear();
            }

            currentEligibleItem.Appear();
            yield return new WaitForSeconds(remaining);
            currentEligibleItem.Disappear();
        }
    }
}
