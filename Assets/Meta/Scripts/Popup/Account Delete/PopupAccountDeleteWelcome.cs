using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class PopupAccountDeleteWelcome : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private bool isContinue = false;

        private void Start()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            int remainingSec = BlackboardUtils.GetOrCreateVariable<int>(bb, "removalRemainingSec")?.value ?? 0;
            int days = TimeUtils.SecondsToDays(remainingSec);

            if(days <= 0) days = 1;

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Text", "POPUP_DELETE_ACCOUNT_WELCOME_BACK_TITLE", FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Delete Account Welcome Back Text", "POPUP_DELETE_ACCOUNT_WELCOME_BACK_INFO", CHILDREN, days);
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Button Continue/Text", "BUTTON_CONTINUE", FULL);

            MetaContextElementUtils.SimpleSetClickable(root, "Button Continue", OnContinue);
        }

        private void OnContinue()
        {
            if (!isContinue)
            {
                isContinue = true;
                string actionType = BlackboardUtils.GetOrCreateVariable<string>(MainBlackboard.Get(), "actionType")?.value;
                if (string.IsNullOrEmpty(actionType))
                {
                    actionType = "device_login";
                }

                BagelCodeClientAPI.RequestDeleteAccountWithdraw(actionType,
                    (response) =>
                    {
                        if (ApplicationSettings.LogTest())
                            UnityEngine.Debug.Log("RequestDeleteAccountWithdraw called. actionType:" + actionType);

                        isContinue = false;
                        EventSender.SendCalleeCallback(gameObject);
                        MetaPopupUtils.ClosePopup(gameObject);
                    },
                    (error) =>
                    {
                        isContinue = false;
                    });
            }
        }
    }
}
