using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public enum PageScrollType
{
    Horizontal,
    Vertical,
}

[RequireComponent(typeof(ScrollRect))]
public class PageScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    protected ScrollRect rect;
    public int pageCount;//页面item个数
    protected float[] pages;//存滚动条进度

    //public int PageCount => pageCount;

    public float moveTime = 0.3f;
    private float timer = 0;
    private float startMovePos;//开始位置
    public int currentPage = 0;//当前页
    //是否在移动
    private bool isMoving = false;
    //是否开启自动滚动
    public bool IsAutoScroll;
    //是否正在拖拽
    private bool isDraging = false;
    public float AutoScrollTime = 2;
    private float AutoScrollTimer = 0;

    public PageScrollType pageScrollType = PageScrollType.Horizontal;

    public Action<int> onPageChange;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Init();
    }

    // Update is called once per frame
    protected virtual void Update()
    {

        ListenerMove();
        ListenerAutoScroll();
    }

    public void Init()
    {
        rect = transform.GetComponent<ScrollRect>();
        if (rect == null)
        {
            throw new System.Exception("未查询到scrollRect");
        }

        InitPage();
        //if (pageCount == 1) return;
        //pageCount = content.childCount;
        //pages = new float[pageCount];
        ////设好每个页面的刻度
        //for (int i = 0; i < pages.Length; i++)
        //{
        //    switch (pageScrollType)
        //    {
        //        case PageScrollType.Horizontal:
        //            pages[i] = i * (1 / (float)(pageCount - 1));
        //            break;
        //        case PageScrollType.Vertical:
        //            pages[i] = 1 - i * (1 / (float)(pageCount - 1));
        //            break;
        //    }

        //}

    }

    public void InitPage()
    {
        switch (pageScrollType)
        {
            case PageScrollType.Horizontal:
                float viewportWidth = rect.viewport.rect.width;
                float contentWidth = rect.content.rect.width;
                pageCount = (int)Math.Ceiling(contentWidth / viewportWidth);
                pages = new float[pageCount];
                //设好每个页面的刻度
                for (int i = 0; i < pages.Length; i++)
                {
                    pages[i] = (viewportWidth * i) / contentWidth;
                    //pages[i] = i * (1 / (float)(pageCount - 1));
                }
                break;
            case PageScrollType.Vertical:
                Debug.LogError("暂未实现垂直方向的分页滚动");
                break;
            default:
                Debug.LogError("暂未实现");
                break;
        }
    }

    //监听移动
    public void ListenerMove()
    {
        if (isMoving)
        {
            timer += Time.deltaTime * (1 / moveTime);
            switch (pageScrollType)
            {
                case PageScrollType.Horizontal:
                    rect.horizontalNormalizedPosition = Mathf.Lerp(startMovePos, pages[currentPage], timer);
                    break;
                case PageScrollType.Vertical:
                    rect.verticalNormalizedPosition = Mathf.Lerp(startMovePos, pages[currentPage], timer);
                    break;
                default:
                    break;
            }

            if (timer >= 1)
            {
                isMoving = false;
            }
        }
    }
    //监听自动滚动
    public void ListenerAutoScroll()
    {
        if (isDraging) return;
        if (IsAutoScroll)
        {
            AutoScrollTimer += Time.deltaTime;
            if (AutoScrollTimer >= AutoScrollTime)
            {
                AutoScrollTimer = 0;
                currentPage++;
                currentPage %= pageCount;
                ScrollPage(currentPage);
            }
        }
    }
    //计算//计算出离得最近的一页
    public int CaculateMinDistancePage()
    {
        int minPage = 0;
        //计算出离得最近的一页
        for (int i = 1; i < pages.Length; i++)
        {
            float curPage = 0;
            switch (pageScrollType)
            {
                case PageScrollType.Horizontal:
                    curPage = rect.horizontalNormalizedPosition;
                    break;
                case PageScrollType.Vertical:
                    curPage = rect.verticalNormalizedPosition;
                    break;

            }
            if (Mathf.Abs(pages[i] - curPage) < Mathf.Abs(pages[minPage] - curPage))
            {
                minPage = i;
            }
        }
        return minPage;
    }

    public void ScrollNextPage(bool forward)
    {
        int targetPage = forward ? currentPage + 1 : currentPage - 1;
        targetPage = Mathf.Clamp(targetPage, 0, pageCount - 1);
        ScrollPage(targetPage);
    }

    //滚到哪一页
    public void ScrollPage(int page)
    {
        isMoving = true;
        this.currentPage = page;
        timer = 0;
        switch (pageScrollType)
        {
            case PageScrollType.Horizontal:
                startMovePos = rect.horizontalNormalizedPosition;
                break;
            case PageScrollType.Vertical:
                startMovePos = rect.verticalNormalizedPosition;
                break;

        }
        onPageChange?.Invoke(this.currentPage);
    }
    //结束拖拽滑动
    public void OnEndDrag(PointerEventData eventData)
    {
        this.ScrollPage(CaculateMinDistancePage());
        isDraging = false;
        AutoScrollTimer = 0
;
    }
    //开始拖拽滑动
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDraging = true;
    }
}


