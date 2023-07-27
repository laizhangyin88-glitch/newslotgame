using SlotMaker;
using NodeCanvas.Framework;
using System.Collections.Generic;

namespace BagelCode
{
    public abstract class ClubNewsFeedCellController : EventMonoBehaviour
    {
        protected ContextElement root;
        protected Blackboard bb;

        protected Blackboard feedInfo;

        protected const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        protected const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        protected const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        private bool isInit = false;

        private void Start()
        {
            if (!isInit)
                InitProperty();

            isInit = true;

            UpdateCellData();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (isInit)
                UpdateCellData();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // UnRegister All Events
            InitHandleEventType(true);
            UnRegisterAll();
            UnRegisterHandlingEventAll();
            UnRegisterSchedulingEventAll();
        }

        protected virtual void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);
        }

        protected virtual void UpdateCellData()
        {
            InitEvents();

            feedInfo = bb.GetValue<Blackboard>("feedInfo");

            // Time
            var timeTextElement = ContextUtils.FindElement(root, "Text Time", CHILDREN);
            var createdTimestamp = feedInfo.GetValue<long>("createdTimestamp");
            var timeText = ClubUtils.GetClubFeedLeftTimeText(TimeUtils.GetTimeStamp() - createdTimestamp);
            MetaContextElementUtils.SetText(timeTextElement, timeText);

            UpdateBG();
        }

        //

        protected virtual void InitEvents()
        {
            UnRegisterAll();
            UnRegisterHandlingEventAll();
        }

        private void UpdateBG()
        {
            int cellIndex = bb.GetValue<int>("cellIndex");
            int cellStyle = cellIndex % 2;

            var bgList = new List<ContextElement>();
            bgList.Add(ContextUtils.FindElement(root, "Base 01", CHILDREN));
            bgList.Add(ContextUtils.FindElement(root, "Base 02", CHILDREN));

            for (int i = 0; i < bgList.Count; ++i)
            {
                bgList[i].gameObject.SetActive(i == cellStyle);
            }
        }
    }
}
