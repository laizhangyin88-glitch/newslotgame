using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public class FishTipsContentItem
    {
        public GameObject gameObject;
        public int UID;
        public FishFishBase targetObjIns;
        public bool isShowTips;
        public float showTime;
        public float currentTime;
        public bool isCanDestroy;
        public Transform trans;
        public GameObject bg_gameObject;
        public Text tipsText;
        public FishTipsContentItem(GameObject obj)
        {
            gameObject = obj;
            UID = 0;
            isShowTips = false;
            showTime = 5;
            currentTime = 0;
            isCanDestroy = false;
            FindView();
        }

        public void FindView()
        {
            trans = gameObject.transform;
            bg_gameObject = trans.Find("bg").gameObject;
            tipsText = trans.Find("bg/Text").GetComponent<Text>();
        }

        public void ResetState(FishFishBase tempIns, int uID)
        {
            targetObjIns = tempIns;
            showTime = tempIns.fishVo.fishCfg.tCShowTime;
            SetUID(uID);
            isShowTips = true;
            SetCurrentPos();
            SetShowText(tempIns.fishVo.fishCfg.TipsContentInfo);
            IsShowText(true);
            currentTime = 0;
            isCanDestroy = false;
            SetTargetObjInsTipsContentState(true);
        }

        public void SetShowText(string content)
        {
            if (tipsText != null)
            {
                tipsText.text = content;
            }
        }

        public void IsShowText(bool isShow)
        {
            bg_gameObject.SetActive(isShow);
            tipsText.gameObject.SetActive(isShow);
        }

        public void SetUID(int uid)
        {
            UID = uid;
        }

        public void SetCurrentPos()
        {
            if (targetObjIns != null)
            {
                trans.localPosition = targetObjIns.transform.localPosition;
            }
        }

        public void SetTargetObjInsTipsContentState(bool isShow)
        {
            if (targetObjIns != null)
            {
                targetObjIns.SetTipsContentState(isShow);
            }
        }

        public void Update()
        {
            if (isShowTips)
            {
                SetCurrentPos();
                currentTime += Time.deltaTime;
                if (currentTime >= showTime || targetObjIns.GetIsDie())
                {
                    isShowTips = false;
                    currentTime = 0;
                    isCanDestroy = true;
                }
            }
        }

        public void Destroy()
        {
            SetTargetObjInsTipsContentState(false);
            targetObjIns = null;
            isShowTips = false;
            IsShowText(false);
            isCanDestroy = false;
        }
    }
}
