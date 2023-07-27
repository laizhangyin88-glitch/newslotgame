using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.OSA_Scroll;

namespace BagelCode.EpicPass
{
    public class EpicPassV2RewardLevelProgressController : MonoBehaviour
    {
        private ContextSlider sliderElement;

        private bool isInit = false;

        public float areaWidth = 0.0f;
        public float itemWidth = 0.0f;
        public float halfItemSize = 0.0f;

        public void OnInit(OSA_EpicPassV2Rewards osaRewards)
        {
            if (isInit)
                return;

            sliderElement = gameObject.GetComponent<ContextSlider>();

            if (osaRewards != null)
            {
                RectTransform rt = osaRewards.GetComponent<RectTransform>();
                areaWidth = rt.rect.width;
                itemWidth = osaRewards.Parameters.DefaultItemSize;
                halfItemSize = itemWidth * 0.5f;
            }

            SetSliderValue(0.0f);

            isInit = true;
        }

        public void UpdateProgress(List<EpicPassV2RewardItemsController> osaItemList)
        {
            if (!isInit)
                return;

            if (osaItemList == null || osaItemList.Count <= 0)
            {
                SetSliderValue(0.0f);
                return;
            }

            osaItemList.Sort((a, b) => a.RewardLevel.CompareTo(b.RewardLevel));

            int epicPassLevel = EpicPassUtilsV2.Level;
            int lastLevel = epicPassLevel;
            float lastPos = 0.0f;
            for (int i = 0; i < osaItemList.Count; ++i)
            {
                int rewardLevel = osaItemList[i].RewardLevel;
                float pos = osaItemList[i].transform.localPosition.x;
                if (i == 0)
                {
                    lastLevel = rewardLevel;
                    lastPos = pos;
                }

                if (epicPassLevel >= rewardLevel)
                {
                    lastLevel = rewardLevel;
                    lastPos = pos;
                }
                else
                    break;
            }
            CalculatePositionSlider(epicPassLevel, lastLevel, lastPos);
        }

        private void SetSliderValue(float value)
        {
            sliderElement?.SetFloatProperty(value);
        }

        private void CalculatePositionSlider(int epicPassLevel, int lastLevel, float lastPos)
        {
            float calcSliderValue = 0.0f;
            if (epicPassLevel >= lastLevel)
            {
                float exp = 0.0f;
                if (epicPassLevel >= EpicPassUtilsV2.MaxLevel)
                    calcSliderValue = 1.0f; // exp = (lastLevel == EpicPassUtilsV2.MaxLevel) ? 1.0f : 1.0f;
                else
                {
                    exp = GetExp();

                    float calcPos = lastPos + (exp * itemWidth);
                    calcSliderValue = Mathf.Clamp(calcPos / areaWidth, 0.0f, 1.0f);
                }
            }
            else
            {
                if (epicPassLevel + 1 == lastLevel)
                {
                    float exp = GetExp();
                    float calcPos = lastPos - ((1.0f - exp) * (epicPassLevel == 0 ? halfItemSize : itemWidth));
                    calcSliderValue = Mathf.Clamp(calcPos / areaWidth, 0.0f, 1.0f);
                }
            }
            SetSliderValue(calcSliderValue);
        }

        private float GetExp()
        {
            long point = EpicPassUtilsV2.Point;
            long requredPoint = EpicPassUtilsV2.RequiredPoint;
            return (float)point / (float)requredPoint;
        }
    }
}