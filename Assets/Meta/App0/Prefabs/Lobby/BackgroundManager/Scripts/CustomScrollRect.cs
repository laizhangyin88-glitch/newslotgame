using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomScrollRect : ScrollRect
{
    //父CustomScrollRect对象
    private CustomScrollRect m_Parent;
    private GameHistroyRecordScrollView m_ScrollView;

    public enum Direction
    {
        Horizontal,
        Vertical
    }
    //滑动方向
    private Direction m_Direction = Direction.Horizontal;
    //当前操作方向
    private Direction m_BeginDragDirection = Direction.Horizontal;

    protected override void Awake()
    {
        base.Awake();
        //找到父对象
        Transform parent = transform.parent;
        if (parent)
        {
            m_Parent = parent.GetComponentInParent<CustomScrollRect>();
            if(m_Parent != null)
            {
                Debug.LogError(m_Parent.name);
            }
        }
        m_ScrollView = transform.GetComponentInParent<GameHistroyRecordScrollView>();
        m_Direction = this.horizontal ? Direction.Horizontal : Direction.Vertical;
    }


    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (m_Parent)
        {
            m_BeginDragDirection = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y) ? Direction.Horizontal : Direction.Vertical;
            Debug.LogError("current direction " + m_BeginDragDirection);
            if (m_BeginDragDirection != m_Direction)
            {
                //当前操作方向不等于滑动方向，将事件传给父对象
                //ExecuteEvents.Execute(m_Parent.gameObject, eventData, ExecuteEvents.beginDragHandler);
                if(m_ScrollView != null)
                {
                    m_ScrollView.OnBeginDrag(eventData);
                }
                return;
            }
        }

        base.OnBeginDrag(eventData);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (m_Parent)
        {
            Debug.LogError("current direction " + m_BeginDragDirection + " " + "m_Direction:" + m_Direction);
            if (m_BeginDragDirection != m_Direction)
            {
                //当前操作方向不等于滑动方向，将事件传给父对象
                ExecuteEvents.Execute(m_Parent.gameObject, eventData, ExecuteEvents.dragHandler);
                if (m_ScrollView != null)
                {
                    m_ScrollView.OnDrag(eventData);
                }
                return;
            }
        }
        base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (m_Parent)
        {
            if (m_BeginDragDirection != m_Direction)
            {
                //当前操作方向不等于滑动方向，将事件传给父对象
                //ExecuteEvents.Execute(m_Parent.gameObject, eventData, ExecuteEvents.endDragHandler);
                if (m_ScrollView != null)
                {
                    m_ScrollView.OnEndDrag(eventData);
                }
                return;
            }
        }
        base.OnEndDrag(eventData);
    }

    public override void OnScroll(PointerEventData data)
    {
        if (m_Parent)
        {
            if (m_BeginDragDirection != m_Direction)
            {
                //当前操作方向不等于滑动方向，将事件传给父对象 
                ExecuteEvents.Execute(m_Parent.gameObject, data, ExecuteEvents.scrollHandler);
                //if (m_ScrollView != null)
                //{
                //    m_ScrollView.OnScroll(data);
                //}
                return;
            }
        }
        base.OnScroll(data);
    }
}

