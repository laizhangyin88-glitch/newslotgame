using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace BagelCode
{
    public abstract class VipDealV2SelectItemControllerBase : EventMonoBehaviour
    {
        protected ContextElement root;
        protected Blackboard bb;
        protected Animator anim;

        private ContextElement multiplierElement;

        protected const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        protected const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        protected const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        protected abstract string GetSelectCellText(Blackboard dealInfo);
        protected abstract long GetDealTotalCredit(Blackboard dealInfo);

        public void Init()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            multiplierElement = ContextUtils.FindElement(root, "Multiplier", CHILDREN);

            MetaContextElementUtils.SetClickable(root, OnClick);

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
        }

        public void WaitReady()
        {
            anim.SetBool("Closed Idle", false);
            MetaContextElementUtils.SetBooleanProperty(root, false);
        }

        public void Idle()
        {
            anim.SetBool("Closed Idle", true);
            MetaContextElementUtils.SetBooleanProperty(root, true);
        }

        public IEnumerator OpenCardCoroutine()
        {
            MetaContextElementUtils.SetBooleanProperty(root, false);

            var dealInfo = BlackboardUtils.FindValue<Blackboard>(bb, "dealInfo");

            string cellValueText = GetSelectCellText(dealInfo);
            MetaContextElementUtils.SimpleSetText(root, "Text", cellValueText, CHILDREN);

            long multiplierNumerator = BlackboardUtils.FindValue<long>(dealInfo, "multiplierNumerator");
            double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplierNumerator);
            MetaContextElementUtils.SimpleSetTextGlobal(
                multiplierElement, "Text", "VIP_DEAL_WHEEL_MULTIPLIER_TEXT", CHILDREN, multiplier);

            long totalCredit = GetDealTotalCredit(dealInfo);
            int coinLevel = totalCredit > VipDealV2.Defines.COIN_GRADE_SEPARATOR_2 ? 2 :
                totalCredit > VipDealV2.Defines.COIN_GRADE_SEPARATOR_1 ? 1 : 0;
            anim.SetInteger("Coin", coinLevel);

            anim.SetBool("Open", true);

            yield return new WaitForSeconds(1f);

            // to select scene
            EventSender.SendCalleeCallback(gameObject, VipDealV2.Events.COMPLETE_OPEN_CARD);
        }

        public IEnumerator SetMultiplierCoroutine()
        {
            bool isMax = BlackboardUtils.FindValue<bool>(bb, "isMax");
            var multiplierAnim = multiplierElement.GetComponent<Animator>();
            multiplierAnim.SetInteger("On", isMax ? 3 : 1);

            anim.SetBool("Opened", true);

            GSManager.Instance.GetHandler(VipDealV2.Defines.SOUND_MULTIPLIER_DECIED).Play();

            yield return new WaitForSeconds(0.5f);

            EventSender.SendCalleeCallback(gameObject, VipDealV2.Events.COMPLETE_SET_MULTIPLIER);
        }

        private void OnClick()
        {
            // to select scene
            int index = BlackboardUtils.FindValue<int>(bb, "index");
            var eventData = new EventData<int>(VipDealV2.Events.ON_SELECT_ITEM, index);
            EventSender.SendCalleeCallback(gameObject, eventData);
        }
    }
}
