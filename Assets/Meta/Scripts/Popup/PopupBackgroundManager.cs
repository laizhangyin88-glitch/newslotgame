using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;

namespace BagelCode
{
    public class PopupBackgroundManager : MonoWeakSingleton<PopupBackgroundManager>
    {
        public GameObject backgroundObject;

        public List<Transform> listenParentsList;

        private const string ON_CHANGED_POPUP_COUNT_EVENT = "ChangedPopupCount";

        private IEnumerator coroutine;

        private void OnEnable()
        {
            MessageDispatcher.Register(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);
        }

        private void OnDisable()
        {
            MessageDispatcher.UnRegister(MetaEventDefine.ON_META_UI_EVENT, OnMetaUIEvent);
        }

        private void OnMetaUIEvent(EventData eventData)
        {
            if (ON_CHANGED_POPUP_COUNT_EVENT == eventData.name)
            {
                ShowBG((int)eventData.value > 0);
            }
        }

        private void ShowBG(bool isActive)
        {
            backgroundObject.SetActive(false);

            if(isActive)
            {
                for(int i=0; i < listenParentsList.Count; ++i)
                {
                    if(listenParentsList[i].childCount > 0)
                    {
                        backgroundObject.SetActive(true);

                        // if(coroutine != null)
                        //     StopCoroutine(coroutine);

                        if(coroutine == null)
                        {
                            coroutine = WatchChildCount();
                            StartCoroutine(coroutine);
                        }
                        
                        break;
                    }
                }
            }
        }

        private IEnumerator WatchChildCount()
        {
            // Debug.LogError(string.Format("Start Watch {0}", Time.time));

            int childCount = 0;
            for(int i=0; i < listenParentsList.Count; ++i)
                childCount += listenParentsList[i].childCount;

            while(childCount > 0)
            {
                yield return new WaitForSeconds(0.1f);

                childCount = 0;
                for(int i=0; i < listenParentsList.Count; ++i)
                    childCount += listenParentsList[i].childCount;

                if(childCount == 0)
                    break;
            }

            backgroundObject.SetActive(false);

            // Debug.LogError(string.Format("End Watch {0}", Time.time));
            coroutine = null;
        }
    }

}
