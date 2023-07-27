using SlotMaker;
using BagelCode.BossRaiders;
using BagelCode.MetaGame;
using NodeCanvas.Framework;

namespace BagelCode.BossRaiders.Deal
{
    public class BossRaidersDealWheelController : BossRaidersWheelControllerBase
    {
        private Blackboard dealBB;

        private ContextElement spinButtonElement;

        private const int WHEEL_ITEM_COUNT = 12;

        public override void OnInit(ContextElement caller)
        {
            dealBB = BlackboardUtils.GetOrCreateVariable<Blackboard>(MainBlackboard.Get(), BossRaidersUtils.BOSS_RAIDERS_DEAL_INFO).value;
            base.OnInit(caller);
        }

        protected override void InitProperty()
        {
            spinButtonElement = ContextUtils.FindElement(callerElement, "Button Spin", ContextSearchingType.ChildrenSearch);
        }

        protected override void InitWheelController()
        {
            ContextElement wheelElement = ContextUtils.FindElement(rootElement, "Wheel", ContextSearchingType.ChildrenSearch);

            wheelController = new BossRaidersDealWheel();
            wheelController.OnInit(wheelElement, rootAnimator, WHEEL_ITEM_COUNT);
            wheelController.InitInactiveElement(ContextUtils.FindElement(rootElement, "Wheel Inactive", ContextSearchingType.ChildrenSearch));
        }

        protected override void InitSpinButtonController()
        {
            spinButtonController = spinButtonElement.GetComponent<MetaGameSpinButton>();
        }

        protected override void InitWheelData()
        {
            SetWheelData(dealBB.GetValue<int>("spinCount"));
        }

        public override void ChangeWheelData(long changeValue)
        {
            wheelController?.SetWheelData(changeValue);
        }
    }
}