using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace BagelCode
{
    public abstract class VipDealV2SelectMainControllerBase : EventMonoBehaviour
    {
        private const float WHEEL_PLAY_SPEED = 1f; // todo

        protected const int CELL_COUNT = 6;
        protected const int WHEEL_COUNT = 12;
        protected const int DEAL_COUNT = 3;

        protected ContextElement root;
        protected Blackboard bb;
        protected Animator anim;

        protected ContextElement wheelElement;
        protected Rigidbody wheelRigidbody;
        protected ContextElement highlightElement;
        protected ContextElement titleIconElement;
        protected Animator titleIconAnim;

        protected Blackboard vipInfo;

        protected bool isPayDeal;

        protected List<Blackboard> dealInfoList;
        protected List<long> wheelMultiplierNumeratorList;
        protected List<GameObject> selectCellObjectList = new List<GameObject>();

        protected Variable<int> dealSelectCount = null;
        protected GameObject targetItemCellObj = null;
        protected List<GameObject> itemCellObjectList = new List<GameObject>();

        private List<ContextElement> wheelCellElementList = new List<ContextElement>();
        private List<ContextElement> wheelHighlightElementList = new List<ContextElement>();

        private const string ON_READY = "OnReady";

        protected const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        protected const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        protected const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        protected abstract IEnumerator RequestSelectItemCoroutine();
        public abstract IEnumerator OnSelectCoroutine(int index);
        public abstract void Close();

        public virtual void Init()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            wheelElement = ContextUtils.FindElement(root, "Wheel", CHILDREN);
            wheelRigidbody = wheelElement.GetComponent<Rigidbody>();
            highlightElement = ContextUtils.FindElement(wheelElement, "Highlight", CHILDREN);
            titleIconElement = ContextUtils.FindElement(root, "VIP Deal Icon", CHILDREN);
            titleIconAnim = titleIconElement.GetComponent<Animator>();

            dealSelectCount = BlackboardUtils.FindVariable<int>(bb, "dealSelectCount");

            MetaContextElementUtils.SimpleSetClickable(root,
                "VIP Deal Icon/Full Cover Button", gameObject, EventSender.ON_CUSTOM_EVENT,
                ON_READY, false, true, FULL);

            anim.SetBool("Active", false);

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            RegisterHandleEventType(MetaEventDefine.ON_SYSTEM_EVENT);
        }

        public virtual IEnumerator AppearCoroutine()
        {
            anim.SetBool("Active", true);
            yield return new WaitForSeconds(0.2f);

            titleIconAnim.SetBool("IsPickGame", true);
            titleIconAnim.SetBool("Default", true);

            // EventSender.SendGlobalMetaEvent(MetaEventDefine.INACTIVE_META_UI); // 하는 이유 확인

            vipInfo = VipDealV2.Utils.GetActiveInfo();
            wheelMultiplierNumeratorList = VipDealV2.Utils.GetWheelMultiplierNumeratorList(isPayDeal);
            dealInfoList = VipDealV2.Utils.GetDealList(isPayDeal);

            InitCells();
            InitWheels();
            InitDeals();

            anim.SetInteger("State", 0);
            yield return new WaitForSeconds(1f);

            titleIconAnim.SetBool("IsReady", true);
            yield return new WaitForSeconds(0.2f);

            var readyTrigger = new EventTrigger(gameObject, ON_READY);
            var timerTrigger = new TimerTrigger(3f);
            yield return new WaitUntilTrigger(readyTrigger, timerTrigger);
        }

        public void ReadyPick()
        {
            titleIconAnim.SetTrigger("NextPick");
        }

        public IEnumerator SpinWheelCoroutine()
        {
            yield return new WaitForSeconds(0.1f);

            var currentDealInfo = dealInfoList[dealSelectCount.value - 1];
            float angle = BlackboardUtils.FindValue<float>(currentDealInfo, "wheelAngle");

            var wheelController = wheelElement.GetComponent<BigWheel>();
            wheelController.Simulation(0, angle, 1); // spin wheel

            yield return new WaitForSeconds(0.1f);

            int lastWheelIndex = 0;
            float threshhold = 0.05f;
            float speed = wheelRigidbody.angularVelocity.magnitude;
            while (speed > threshhold) // until stop wheel
            {
                float current = MetaContextElementUtils.GetFloatProperty(wheelElement);
                float targetangle = current + 360f + 6.5f;

                int wheelIndex = (((int)(targetangle / 30f)) % 12);
                if (lastWheelIndex != wheelIndex)
                {
                    GSManager.Instance.GetHandler(VipDealV2.Defines.SOUND_WHEEL_LOOP).Play();
                    lastWheelIndex = wheelIndex;
                }

                yield return new WaitForEndOfFrame();
                speed = wheelRigidbody.angularVelocity.magnitude;
            }

            GSManager.Instance.GetHandler(VipDealV2.Defines.SOUND_WHEEL_STOP).Play();

            var targetWheelAnim = wheelHighlightElementList[lastWheelIndex].GetComponent<Animator>();

            targetWheelAnim.SetBool("Win", true);
            anim.SetInteger("WheelState", 2);
            yield return new WaitForSeconds(1.5f);

            targetWheelAnim.SetBool("Win", false);
            anim.SetInteger("WheelState", 0);

            EventSender.SendEvent(targetItemCellObj, VipDealV2.Events.SET_MULTIPLIER);
        }

        private void InitCells()
        {
            for (int i = 0; i < CELL_COUNT; ++i)
            {
                string cellName = string.Format("Item {0:00}", i + 1);
                var cellElement = ContextUtils.FindElement(root, "VIP Deal Item Area/" + cellName, FULL);

                var cellBB = cellElement.GetComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue(cellBB, "index", i);
                MetaObjectUtils.SetCalleeCaller(cellElement.gameObject, gameObject);

                itemCellObjectList.Add(cellElement.gameObject);
            }
        }

        private void InitWheels()
        {
            for (int i = 0; i < wheelMultiplierNumeratorList.Count; ++i)
            {
                string wheelName = string.Format("{0:00}", i + 1);
                var wheelElement = ContextUtils.FindElement(root, "Wheel/Base/" + wheelName, FULL);
                var highlightElement = ContextUtils.FindElement(root, "Wheel/Highlight/" + wheelName, FULL);

                long multiplierNumerator = wheelMultiplierNumeratorList[i];
                double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplierNumerator);

                string text = StringTableUtils.GetString(GLOBAL, "VIP_DEAL_WHEEL_MULTIPLIER_TEXT", multiplier);
                MetaContextElementUtils.SimpleSetText(wheelElement, "Text", text, CHILDREN);

                wheelCellElementList.Add(wheelElement);
                wheelHighlightElementList.Add(highlightElement);
            }
        }

        private void InitDeals()
        {
            for (int i = 0; i < dealInfoList.Count; ++i)
            {
                var dealInfo = dealInfoList[i];
                long dealMultiplierNumerator = BlackboardUtils.FindValue<long>(dealInfo, "multiplierNumerator");
                int index = wheelMultiplierNumeratorList.IndexOf(dealMultiplierNumerator);

                float delta = 360f / (float)WHEEL_COUNT;
                float angle = delta * (float)index;
                BlackboardUtils.SetOrCreateValue(dealInfo, "wheelIndex", index);
                BlackboardUtils.SetOrCreateValue(dealInfo, "wheelAngle", angle);
            }
        }
    }
}
