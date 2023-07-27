using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Actions
{
    [Category("★ BagelCode/SlotList")]
    public class SetEnterGameInfo : ActionTask<Blackboard>
    {
        public BBParameter<string> gameIdValue;
        public BBParameter<string> targetRoomId;
        public BBParameter<string> bonusTicketId;
        public BBParameter<string> fromType;
        public BBParameter<string> noticeIdValue;
        public BBParameter<string> slbIdValue;
        public BBParameter<bool> isEarlyAccess;
        public BBParameter<string> contextIdValue;

        protected override string info
        {
            get { return string.Format("Set Game Info {0}", fromType); }
        }

        protected override void OnExecute()
        {
            var gameID = BlackboardUtils.FindVariable<int>(agent, gameIdValue.value);
            var roomId = BlackboardUtils.FindVariable<string>(agent, targetRoomId.value);
            var ticketId = BlackboardUtils.FindVariable<int>(agent, bonusTicketId.value);
            var noticeId = BlackboardUtils.FindVariable<int>(agent, noticeIdValue.value);
            var slbId = BlackboardUtils.FindVariable<string>(agent, slbIdValue.value);
            var contextId = BlackboardUtils.FindVariable<string>(agent, contextIdValue.value);

            string targetRoomID = null;
            if (roomId != null)
                targetRoomID = roomId.value;

            int targetTicketID = 0;
            if (ticketId != null)
                targetTicketID = ticketId.value;

            int fromNoticeID = 0;
            if (noticeId != null)
                fromNoticeID = noticeId.value;

            string fromSlbID = null;
            if (slbId != null)
                fromSlbID = slbId.value;

            string contextID = null;
            if (contextId != null)
                contextID = contextId.value;

            BlackboardQueryUtils.SetEnterGameInfo(gameID.value, "EnterGame", fromType.value, targetRoomID, fromNoticeID, fromSlbID, isEarlyAccess.value, targetTicketID, contextID);

            EndAction();
        }
    }
}