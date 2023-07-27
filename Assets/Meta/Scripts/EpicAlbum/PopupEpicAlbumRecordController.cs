using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
// using Sirenix.OdinInspector;

namespace BagelCode.EpicAlbum
{
    public class PopupEpicAlbumRecordController : MonoBehaviour
    {
        public float gaugeEffectDelta = 0.5f;

        private int gameID = -1;

        private ContextElement gaugeElement;
        private GameSoundPlayer soundPlayer;

        private int prevStarCount;
        private int prevPivotEpicWinCount;
        private int prevRequiredEpicWinCount;
        private int prevEpicWinCount;
        private float prevProgress;

        private int targetEpicWinCount;

        private List<ContextElement> starElementList = new List<ContextElement>();

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private const int STAR_COUNT = 3;
        // private bool actionTest = false;
        private bool isInit = false;

        public void Init(long earnCredit)
        {
            if(isInit) return;
            isInit = true;

            soundPlayer = gameObject.GetComponent<GameSoundPlayer>();
            if(soundPlayer == null)
                soundPlayer = gameObject.AddComponent<GameSoundPlayer>();

            var agentElement = GetComponent<ContextElement>();
            agentElement.UpdateContext();

            gameID = BlackboardUtils.GetOrCreateVariable<int>(null, "./game/gameId").value;
            bool newEpicRecord = BlackboardQueryUtils.CheckIfNewEpicRecord(gameID, earnCredit);

            var titleText = StringTableUtils.GetString(tableType, newEpicRecord ? "POPUP_EPIC_RECORD_NEW_TITLE_TEXT" : "POPUP_EPIC_RECORD_DEFAULT_TITLE_TEXT");
            MetaContextElementUtils.SimpleSetText(agentElement, "Title Area/Title Text", titleText, ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SimpleSetText(agentElement, "Title Area/Text", StringTableUtils.GetString(tableType, "POPUP_EPIC_RECORD_WIN_COUNT_TEXT"), ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SimpleSetText(agentElement, "Sub Text", StringTableUtils.GetString(tableType, "POPUP_EPIC_RECORD_BOTTOM_TEXT"), ContextSearchingType.ChildrenSearch);

            var maxCoin = BlackboardUtils.FindVariable<long>(null, "/values/misc/FACEBOOK_SHARE_REWARD_MAX_CREDIT");
            var buttonText = StringTableUtils.GetString(tableType, "BUTTON_SHARE_GET_MYSTERY_NEW", maxCoin.value);
            MetaContextElementUtils.SimpleSetText(agentElement, "Button Share/Text", buttonText, ContextSearchingType.FullNameSearch);

            DateTime dateTime = TimeUtils.GetCurrentDateTime();
            string dateFormat = StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_ALBUM_DETAIL_POPUP_DATE_FORMAT");
            string dateString = string.Format("{0}", dateTime.ToString(dateFormat, CultureInfo.CreateSpecificCulture("en-us")));
            MetaContextElementUtils.SimpleSetText(agentElement, "Day Tag Base/Text", dateString, ContextSearchingType.FullNameSearch);

            for(int i=0; i<STAR_COUNT; ++i)
            {
                starElementList.Add(ContextUtils.FindElement(agentElement, string.Format("Title Area/Star Base/Star {0} On", i), ContextSearchingType.FullNameSearch));
                starElementList[i].gameObject.GetComponent<Animator>().SetBool("IsActive", false);
            }

            gaugeElement = ContextUtils.FindElement(agentElement, "Title Area/Gauge", ContextSearchingType.FullNameSearch);

            Blackboard woeInfo = gameID != -1 ? BlackboardQueryUtils.GetWOEInfo(gameID) : null;
            if(woeInfo != null)
            {
                prevStarCount = woeInfo.GetValue<int>("starCount");
                prevPivotEpicWinCount = woeInfo.GetValue<int>("prevEpicWinCount");
                prevRequiredEpicWinCount = woeInfo.GetValue<int>("nextRequiredEpicWinCount");
                prevEpicWinCount = woeInfo.GetValue<int>("epicWinCount");

                var starCount = prevStarCount;
                if(starCount < STAR_COUNT)
                {
                    prevProgress = 0f;
                    float oneStepDelta = 1f/(float)STAR_COUNT;

                    if(starCount > 0)
                        prevProgress = starCount * oneStepDelta;

                    if(prevEpicWinCount > 0)
                        prevProgress += ((float)(prevEpicWinCount-prevPivotEpicWinCount)/(float)(prevRequiredEpicWinCount-prevPivotEpicWinCount))/(float)STAR_COUNT;

                    prevProgress = Mathf.Clamp(prevProgress, 0f, 1f);

                    MetaContextElementUtils.SetFloatProperty(gaugeElement, prevProgress);
                }
                else
                {
                    prevProgress = 1f;
                    MetaContextElementUtils.SetFloatProperty(gaugeElement, 1f);
                }
            }
            else
            {
                prevProgress = 0f;
                MetaContextElementUtils.SetFloatProperty(gaugeElement, 0f);
            }
        }

        // private void Update()
        // {
        //     if(actionTest)
        //     {
        //         for(int i=0; i<starElementList.Count; ++i)
        //         {
        //             starElementList[i].gameObject.SetActive(false);
        //             starElementList[i].gameObject.SetActive(true);
        //         }
        //         var starCount = prevStarCount;
        //         if(starCount < STAR_COUNT)
        //         {
        //             prevProgress = 0f;
        //             float oneStepDelta = 1f/(float)STAR_COUNT;

        //             if(starCount > 0)
        //                 prevProgress = starCount * oneStepDelta;

        //             if(prevEpicWinCount > 0)
        //                 prevProgress += ((float)(prevEpicWinCount-prevPivotEpicWinCount)/(float)(prevRequiredEpicWinCount-prevPivotEpicWinCount))/(float)STAR_COUNT;

        //             prevProgress = Mathf.Clamp(prevProgress, 0f, 1f);

        //             MetaContextElementUtils.SetFloatProperty(gaugeElement, prevProgress);
        //         }
        //         else
        //         {
        //             prevProgress = 1f;
        //             MetaContextElementUtils.SetFloatProperty(gaugeElement, prevProgress);
        //         }

        //         InitStar();
        //         EncreaseGaugeEffect();
        //         actionTest = false;
        //     }
        // }

        // public void TestAction()
        // {
        //     EncreaseGaugeEffect();
        // }

        public void InitStar()
        {
            var starCount = prevStarCount;
            for(int i=0; i<starElementList.Count; ++i)
            {
                starElementList[i].gameObject.GetComponent<Animator>().SetBool("IsActive", starCount > i);
            }
        }

        public void EncreaseGaugeEffect()
        {
            Blackboard woeInfo = gameID != -1 ? BlackboardQueryUtils.GetWOEInfo(gameID) : null;
            if(woeInfo != null)
            {
                 targetEpicWinCount = woeInfo.GetValue<int>("epicWinCount");
                 if(targetEpicWinCount > prevEpicWinCount)
                 {
                    StartCoroutine("EncreaseProgress", woeInfo);
                 }
            }
        }

        private IEnumerator EncreaseProgress(Blackboard woeInfo)
        {
            int starCount = woeInfo.GetValue<int>("starCount");
            int pivotEpicWinCount = woeInfo.GetValue<int>("prevEpicWinCount");
            int requiredEpicWinCount = woeInfo.GetValue<int>("nextRequiredEpicWinCount");
            int epicWinCount = woeInfo.GetValue<int>("epicWinCount");

            float targetProgress = 0f;
            float oneStepDelta = 1f/(float)STAR_COUNT;

            if(starCount > 0)
                targetProgress = starCount * oneStepDelta;

            if(epicWinCount > 0)
                targetProgress += ((float)(epicWinCount-pivotEpicWinCount)/(float)(requiredEpicWinCount-pivotEpicWinCount))/(float)STAR_COUNT;

            targetProgress = Mathf.Clamp(targetProgress, 0f, 1f);

            if(targetProgress > prevProgress)
            {
                soundPlayer.PlayGameSound("UI_Epic_Album_Gauge");
                MetaContextElementUtils.SetFloatProperty(gaugeElement, prevProgress);
                // Gauge effect
                float totalDelta = 0f;
                while(totalDelta < gaugeEffectDelta)
                {
                    totalDelta += Time.deltaTime;
                    float rate = totalDelta/gaugeEffectDelta;
                    MetaContextElementUtils.SetFloatProperty(gaugeElement, Mathf.Lerp(prevProgress, targetProgress, rate));
                    
                    yield return null;
                }

                MetaContextElementUtils.SetFloatProperty(gaugeElement, targetProgress);
            }

            if(starCount > prevStarCount)
            {
                for(int i=0; i<starElementList.Count; ++i)
                {
                    if(starCount < i) break;
                    if(prevStarCount > i) continue;
                    if(starCount > i)
                    {
                        soundPlayer.PlayGameSound("UI_Epic_Album_Star");
                        starElementList[i].gameObject.GetComponent<Animator>().SetTrigger("Appear");
                    }
                }
            }

            yield return null;
        }

        // public int simulateIndex = 0;
        // [Button]
        // private void SimulateStart()
        // {
        //     for(int i=0; i<starElementList.Count; ++i)
        //     {
        //         starElementList[i].gameObject.SetActive(false);
        //         starElementList[i].gameObject.SetActive(true);
        //         starElementList[i].gameObject.GetComponent<Animator>().SetBool("IsActive", simulateIndex > i);
        //     }

        //     soundPlayer.PlayGameSound("UI_Epic_Album_Star");
        //     starElementList[simulateIndex].gameObject.GetComponent<Animator>().SetTrigger("Appear");
        // }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}
