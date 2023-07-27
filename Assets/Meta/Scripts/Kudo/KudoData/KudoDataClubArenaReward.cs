using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;

using static BagelCode.KudoEventManager;

namespace BagelCode
{
    public class KudoDataClubArenaReward : KudoData
    {
        private ContextElement fromPosElement;
        private ContextElement kudoTextElement;
        private ContextElement multiBetElement;
        private ContextElement flyBetTextElement;
        private ContextElement jackpotTextElement;
        private ContentJackpotCredit jackpotCredit;

        private bool isMulti = false;
        private long resultValue = 0;
        private string formatString = "";
        private float elapsedTime = 0.75f;

        protected override string GetKudoSceneName()
        {
            return "Kudo Club Arena In Game Scene";
        }

        protected override void InitProperty()
        {
            root = controller.GetComponent<ContextElement>();
            bb = controller.GetComponent<Blackboard>();
            anim = controller.GetComponent<Animator>();

            root.UpdateContext(false);

            fromPosElement = ContextUtils.FindElement(root, "From Position", CHILDREN);
            ContextElement kudoTextAreaElement = ContextUtils.FindElement(root, "Kudo Text Area", CHILDREN);
            kudoTextElement = ContextUtils.FindElement(kudoTextAreaElement, "Text", CHILDREN);
            jackpotTextElement = ContextUtils.FindElement(kudoTextAreaElement, "Text Jackpot", CHILDREN);

            ContextElement flyBetButtonElement = ContextUtils.FindElement(root, "Bet Button", CHILDREN);
            flyBetTextElement = ContextUtils.FindElement(flyBetButtonElement, "Text", CHILDREN);

            multiBetElement = ContextUtils.FindElement(root, "Multi Bet Area", CHILDREN);
            multiBetElement.gameObject.SetActive(false);

            jackpotCredit = controller.GetComponent<ContentJackpotCredit>();

            ContextUtils.FindElement(root, "Button Revenge Area", CHILDREN)?.gameObject.SetActive(false);
        }

        protected override IEnumerator UpdateKudoData(Blackboard info)
        {
            elapsedTime = 0.75f;
            long baseValue = SetRewardText();
            // Active Animator
            anim.SetBool("IsActive", true);
            if (isMulti)
            {
                yield return new WaitForSeconds(0.2f);
                anim.SetBool("isMulti", true);
                GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_BET_MULTI_KUDO).Play();
                yield return new WaitForSeconds(0.5f);
                SetRewardMultiText(baseValue);
                while (elapsedTime > 0)
                {
                    elapsedTime -= Time.deltaTime;
                    MetaContextElementUtils.SetTextGlobal(kudoTextElement, formatString, MetaContextElementUtils.GetText(jackpotTextElement, "0"));
                    yield return new WaitForEndOfFrame();
                }
            }
            MetaContextElementUtils.SetTextGlobal(kudoTextElement, formatString, FormatUtility.CommaNumberFormat(resultValue));

            // Wait | Skip
            var timerTrigger = new TimerTrigger(1.0f);
            yield return new WaitUntilTrigger(timerTrigger);
            EventData<GameObject> e = new EventData<GameObject>("OnClubArenaKudoRewardClose", fromPosElement.gameObject);
            EventSender.SendGlobalEvent(e);
            //yield return new WaitForSeconds(0.5f);
            // Disappear
            anim.SetBool("isMulti", false);
            yield return controller.StartCoroutine(DisappearCoroutine());
        }

        private long SetRewardText()
        {
            Blackboard rewardBB = ClubArenaUtils.WheelResultInfo;
            ClubArenaSpinResultType resultType = rewardBB.GetValue<ClubArenaSpinResultType>("type");
            ClubArenaDebugSpinResultType debugSpinType = ClubArenaUtils.GetResultDebugSpinType();
            long rewardMultiplier = ClubArenaUtils.CurrentBetMultiplyNumerator;
            isMulti = rewardMultiplier != ClubArenaUtils.BaseBetMultiplyNumerator;

            MetaContextElementUtils.SetTextGlobal(flyBetTextElement, "CLUB_ARENA_WHEEL_BET_BUTTON", NumberUtils.GetMultiplierFromNumerator(rewardMultiplier));

            long baseValue = 0;
            resultValue = 0;
            switch (debugSpinType)
            {
                case ClubArenaDebugSpinResultType.POINT:
                    baseValue = ClubArenaUtils.WheelBaseCandidateList[resultType];
                    resultValue = rewardBB.GetValue<long>("point");
                    formatString = "CLUB_ARENA_KUDO_REWARD_GET_POINT_TEXT";
                    MetaContextElementUtils.SetTextGlobal(kudoTextElement, formatString, FormatUtility.CommaNumberFormat(baseValue));
                    break;
                case ClubArenaDebugSpinResultType.SHIELD:
                    baseValue = ClubArenaUtils.WheelBaseCandidateList[resultType];
                    resultValue = (long)NumberUtils.GetMultiplierFromNumerator(rewardMultiplier);
                    formatString = "CLUB_ARENA_KUDO_REWARD_GET_SHIELD_TEXT";
                    MetaContextElementUtils.SetTextGlobal(kudoTextElement, formatString, FormatUtility.CommaNumberFormat(baseValue));
                    break;
                case ClubArenaDebugSpinResultType.STEAL:
                    baseValue = ClubArenaUtils.GetOpponentBasePoint();
                    resultValue = ClubArenaUtils.AddedPoint;
                    formatString = "CLUB_ARENA_KUDO_REWARD_GET_STEAL_TEXT";
                    MetaContextElementUtils.SetTextGlobal(kudoTextElement, formatString, FormatUtility.CommaNumberFormat(baseValue));
                    break;
                case ClubArenaDebugSpinResultType.ATTACK:
                    baseValue = ClubArenaUtils.GetOpponentBasePoint();
                    resultValue = ClubArenaUtils.AddedPoint;
                    formatString = "CLUB_ARENA_KUDO_REWARD_GET_ATTACK_TEXT";
                    MetaContextElementUtils.SetTextGlobal(kudoTextElement, formatString, FormatUtility.CommaNumberFormat(baseValue));
                    break;
            }


            return baseValue;
        }

        private void SetRewardMultiText(long baseValue)
        {
            if (isMulti)
            {
                double multiValue = NumberUtils.GetMultiplierFromNumerator(ClubArenaUtils.CurrentBetMultiplyNumerator);
                MetaContextElementUtils.SimpleSetTextGlobal(multiBetElement, "Text", "CLUB_ARENA_KUDO_REWARD_MULTI_BET_TEXT", CHILDREN, multiValue);
                jackpotCredit.Reset(jackpotTextElement as IContextText, formatString, baseValue, resultValue, elapsedTime, 1, false);
                multiBetElement.gameObject.SetActive(true);
            }
            else
                multiBetElement.gameObject.SetActive(false);
        }
    }
}