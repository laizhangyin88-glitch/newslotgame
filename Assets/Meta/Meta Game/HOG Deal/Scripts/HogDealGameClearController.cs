using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class HogDealGameClearController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private ContextElement resultBaseElement;

        private long totalPrize;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private void Start()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            // Play Clear Sound
            GSManager.Instance.GetHandler(HiddenObjects.HiddenObjects.Defines.SOUNDS_STAGE_CLEAR).Play();

            var jackpotType = bb.GetValue<HogDealJackpotType>("jackpotType");
            totalPrize = bb.GetValue<long>("totalPrize");
            int remainingCount = bb.GetValue<int>("remainingCount");

#if DEV
            int playerPrefsWin = PlayerPrefs.GetInt(HogDeal.Defines.PLAYER_PREFS_WIN);
            if (playerPrefsWin > 0)
            {
                jackpotType = (HogDealJackpotType)playerPrefsWin;
            }
#endif

            bool isBigWin = jackpotType != HogDealJackpotType.UNKNOWN && jackpotType != HogDealJackpotType.NONE;
            anim.SetBool("isWin", isBigWin);

            resultBaseElement = ContextUtils.FindElement(root, "Result Base", CHILDREN);

            // Big Win
            if (isBigWin)
            {
                string winTypeText = TextDecoUtils.EnumTypeToText<HogDealJackpotType>((int)jackpotType, TextDecoUtils.TextFormat.PASCAL_CASE, " ");
                string bundle = HogDeal.Defines.CONTENTS_BUNDLE;
                string asset = string.Format("Hog Deal {0} Win", winTypeText);
                Transform parent = ContextUtils.FindElement(root, "Top Win Area", CHILDREN).transform;
                MetaObjectUtils.MakePrefab(bundle, asset, parent);

                // Play Sound
                string soundKey = "";
                switch (jackpotType)
                {
                    case HogDealJackpotType.BIG:
                    case HogDealJackpotType.SUPER_BIG:
                        soundKey = "Meta_HOG_Mini_Win";
                        break;
                    case HogDealJackpotType.MEGA:
                    case HogDealJackpotType.SUPER_MEGA:
                        soundKey = "Meta_HOG_Major_Win";
                        break;
                    case HogDealJackpotType.EPIC:
                        soundKey = "Meta_HOG_Mega_Win";
                        break;
                }
                GSManager.Instance.GetHandler(soundKey).Play();
            }

            // Prize
            MetaContextElementUtils.SimpleSetTextGlobal(resultBaseElement,
                "Text Rewards", "POPUP_HOG_DEAL_IN_GAME_CLEAR_WON", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(resultBaseElement,
                "Text Coin Rewards", "POPUP_HOG_DEAL_IN_GAME_CLEAR_COIN", CHILDREN, totalPrize);

            // Remaining
            MetaContextElementUtils.SimpleSetTextGlobal(root,
                "Remaining Area/Text Remaining Title", "POPUP_HOG_DEAL_IN_GAME_CLEAR_REMAINING", FULL);
            MetaContextElementUtils.SimpleSetText(root,
                "Remaining Area/Text Remaining", remainingCount.ToString(), FULL);

            // Collect
            var collectButtonElement = ContextUtils.FindElement(resultBaseElement, "Button Collect", FULL);
            MetaContextElementUtils.SimpleSetTextGlobal(collectButtonElement, "Text", "POPUP_HOG_DEAL_IN_GAME_CLEAR_COLLECT", CHILDREN);
            MetaContextElementUtils.SetClickable(collectButtonElement, Collect);

            // Next
            var nextButtonElement = ContextUtils.FindElement(resultBaseElement, "Button Next", FULL);
            if (remainingCount <= 0)
            {
                MetaContextElementUtils.SetActive(nextButtonElement, false);
            }
            else
            {
                MetaContextElementUtils.SimpleSetTextGlobal(nextButtonElement, "Text", "POPUP_HOG_DEAL_IN_GAME_CLEAR_NEXT", CHILDREN);
                MetaContextElementUtils.SetClickable(nextButtonElement, Next);
            }
        }

        public void Collect()
        {
            BlackboardQueryUtils.AddCoins(totalPrize);
            BlackboardQueryUtils.ApplyUserSyncInfo();

            EventSender.SendCalleeCallback(gameObject, HogDeal.Events.ON_CLOSE);
            MetaPopupUtils.ClosePopup(gameObject);
        }

        public void Next()
        {
            EventSender.SendCalleeCallback(gameObject, HogDeal.Events.ON_NEXT);
            MetaPopupUtils.ClosePopup(gameObject);
        }
    }
}
