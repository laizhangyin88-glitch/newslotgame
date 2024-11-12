using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{

public class NoticeList : MonoBehaviour 
{
    public   SceneInfoObject    noticeItemScene;

    private  GameObject         prefab = null;
    private  Stack<GameObject>  noticeStack  = new Stack<GameObject>();

    private const string ON_META_UI_EVENT = "OnMetaUIEvent";
    private const string ON_NEXT_NOTICE = "OnNextNotice";
    private const string ON_CLOSE_NOTICE = "OnCloseNotice";

    public void Initialize()
    {
        List<Blackboard> noticeList = BlackboardQueryUtils.GetPopupNoticeList();
        if (noticeList.Count == 0)
        {
            MessageDispatcher.Dispatch(ON_META_UI_EVENT, new EventData(ON_CLOSE_NOTICE));
            return;
        }

        SetNoticeList(noticeList);
        MessageDispatcher.Register(ON_META_UI_EVENT, OnClick);
    }

    public void OnClick(EventData eventData)
    {
        if (eventData.name.Equals(ON_NEXT_NOTICE, StringComparison.Ordinal))
        {
            Destroy(noticeStack.Pop());
            if (noticeStack.Count == 0)
            {
                MessageDispatcher.Dispatch(ON_META_UI_EVENT, new EventData(ON_CLOSE_NOTICE));
            }
            else
            {
                noticeStack.Peek().SetActive(true);
            }
        }
    }

    private GameObject InstantiateItem()
    {
        if (prefab == null)
        {
            prefab = SceneManager.LoadScene(gameObject.transform, noticeItemScene.GetSceneInfo());
            return prefab;
        }
        return Instantiate(prefab) as GameObject;
    }

    private void SetNoticeList(List<Blackboard> noticeList)
    {
        for (int i = noticeList.Count - 1; 0 <= i; --i)
        {
            long endTimestamp = BlackboardQueryUtils.GetNoticeEndTimestamp(noticeList[i]);
            long currentTimeStamp = TimeUtils.GetTimeStamp();
            if (endTimestamp == 0 || currentTimeStamp < endTimestamp)
            {
                var go = InstantiateItem();
                go.name = "Notice Item";
                go.transform.SetParent(gameObject.transform, false);

                Blackboard bb = go.GetComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue<Blackboard>(bb, "noticeInfo", noticeList[i]);
                BlackboardUtils.SetOrCreateValue<long>(bb, "endTimestamp", endTimestamp);

                noticeStack.Push(go);
            }
        }
        noticeStack.Peek().SetActive(true);
    }
}

}
