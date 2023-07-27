using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using NodeCanvas.Framework;

namespace BagelCode.BossRaiders
{
    public class BossRaidersDealPopupTotalResultController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Blackboard rootBB;
        private Animator rootAnimator;

        private ContextElement topWinAreaElement;
        private ContextElement collectButtonElement;
        private ContextElement messageTextElement;

        private BossRaidersDealJackpotType jackpotType;

        private bool isInit = false;
        private bool isBigWin = true;

        public void OnInit()
        {
            if (isInit) return;

            InitProperty();
            InitData();

            rootAnimator?.SetBool("Active", true);
            PlayResultSound();
            isInit = true;
        }

        private void InitProperty()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootBB = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();

            topWinAreaElement = ContextUtils.FindElement(rootElement, "Top Win Area", ContextSearchingType.ChildrenSearch);
            messageTextElement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);
            collectButtonElement = ContextUtils.FindElement(rootElement, "Button Collect", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(collectButtonElement, OnClickCollect);
        }

        private void InitData()
        {
            // endInfo Data Check
            Blackboard endInfo = BlackboardUtils.GetOrCreateVariable<Blackboard>(rootBB, "endInfo").value;
            long earnCredit = BlackboardUtils.FindValue<long>(endInfo, "totalEarnCredit");
            long earnGem = BlackboardUtils.FindValue<long>(endInfo, "totalEarnGem");
            jackpotType = BlackboardUtils.FindValue<BossRaidersDealJackpotType>(endInfo, "jackpotType");
            string sharedBundleName = BossRaidersUtils.GetBossRaidersSharedBundleName(true);
            GameObject objWin = null;
            switch (jackpotType)
            {
                case BossRaidersDealJackpotType.EPIC:
                    objWin = MetaObjectUtils.MakePrefab(sharedBundleName, "Boss Raiders Deal Epic Win", topWinAreaElement.transform, null, "Epic Win");
                    break;
                case BossRaidersDealJackpotType.SUPER_MEGA:
                    objWin = MetaObjectUtils.MakePrefab(sharedBundleName, "Boss Raiders Deal Super Mega Win", topWinAreaElement.transform, null, "Super Mega Win");
                    break;
                case BossRaidersDealJackpotType.MEGA:
                    objWin = MetaObjectUtils.MakePrefab(sharedBundleName, "Boss Raiders Deal Mega Win", topWinAreaElement.transform, null, "Mega Win");
                    break;
                case BossRaidersDealJackpotType.SUPER_BIG:
                    objWin = MetaObjectUtils.MakePrefab(sharedBundleName, "Boss Raiders Deal Super Big Win", topWinAreaElement.transform, null, "Super Big Win");
                    break;
                case BossRaidersDealJackpotType.BIG:
                    objWin = MetaObjectUtils.MakePrefab(sharedBundleName, "Boss Raiders Deal Big Win", topWinAreaElement.transform, null, "Big Win");
                    break;
            }

            if (objWin == null)
                isBigWin = false;
            rootAnimator?.SetBool("isWin", isBigWin);

            MetaContextElementUtils.SimpleSetTextGlobal(collectButtonElement, "Text", "POPUP_BOSS_RAIDERS_DEAL_TOTAL_RESULT_BUTTON", ContextSearchingType.ChildrenSearch);
            string resultText = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_BOSS_RAIDERS_DEAL_TOTAL_RESULT_TEXT_0", earnCredit);
            if (earnGem > 0)
                resultText += StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_BOSS_RAIDERS_DEAL_TOTAL_RESULT_TEXT_1", earnGem);
            MetaContextElementUtils.SetText(messageTextElement, resultText);
        }

        private void OnClickCollect()
        {
            EventSender.SendCalleeCallback(gameObject, "OnClose");
            PopupManager.Instance.Close(gameObject);
            rootAnimator.SetTrigger("Close");
        }

        private void PlayResultSound()
        {
            if (isBigWin)
            {
                switch (jackpotType)
                {
                    case BossRaidersDealJackpotType.EPIC:
                        GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_DEAL_POPUP_EPIC).Play();
                        break;
                    case BossRaidersDealJackpotType.SUPER_MEGA:
                        GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_DEAL_POPUP_SUPER_MEGA).Play();
                        break;
                    case BossRaidersDealJackpotType.MEGA:
                        GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_DEAL_POPUP_MEGA).Play();
                        break;
                    case BossRaidersDealJackpotType.SUPER_BIG:
                        GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_DEAL_POPUP_SUPER_BIG).Play();
                        break;
                    case BossRaidersDealJackpotType.BIG:
                        GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_DEAL_POPUP_BIG).Play();
                        break;
                }
                GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_DEAL_POPUP_EFFECT).Play();
            }
            else
                GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_DEAL_POPUP_DEFAULT).Play();
        }
    }
}