using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SlotMaker
{

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class PageScrollRect : DynamicScrollRect
{
    public bool usePageAutoChange = false;
    public float pageAutoChangeInterval = 2f;
    public bool pageAutoChangeForward = true;
    public bool pageAutoChangeLoop = true;

    public int _pageIndex = 0;
	public int pageIndex
    {
        get
        {
            return _pageIndex;
        }
        set
        {
            if (_pageIndex != value)
            {
                onPageChangedToNext.Invoke(value > _pageIndex ? true : false); 
                onPageChanged.Invoke(value);

                OnPageLoaded(value);
            }
            _pageIndex = value;
        }
    }

    private void OnPageLoaded(int value)
    {
        if (value == 0)
            if (onFirstPageLoaded != null) onFirstPageLoaded.Invoke();

        if (value == (pageCount - 1))
            if (onLastPageLoaded != null) onLastPageLoaded.Invoke();
    }

    public UnityBoolEvent onPageChangedToNext;
    public UnityIntEvent onPageChanged;
    public UnityEvent onFirstPageLoaded;
    public UnityEvent onLastPageLoaded;

	public int pageCount
    {
        get { return IsActive() ? content.childCount : 1; }
    }

	protected Bounds pageBounds;

	public void ForceUpdatePageIndex()
    {
    	pageIndex = Mathf.Clamp(pageIndex, 0, pageCount - 1);
    	if (horizontal)
    		horizontalNormalizedPosition = (float)pageIndex / (pageCount - 1);
    	else 
    		verticalNormalizedPosition = (float)pageIndex / (pageCount - 1);
    }

    public void NextPage(bool forward)
    {
		_pageIndex = Mathf.Clamp(forward ? ++pageIndex : --pageIndex, 0, pageCount - 1);
    }

    public void NextPageWithLoop(bool forward)
    {
        int newIndex = forward ? _pageIndex + 1 : _pageIndex - 1;

        if(newIndex > pageCount - 1)
            pageIndex = 0;
        else if(newIndex < 0)
            pageIndex = pageCount - 1;
        else
            pageIndex = newIndex;

        // _pageIndex = Mathf.Clamp(forward ? ++pageIndex : --pageIndex, 0, pageCount - 1);
    }

	public override void Rebuild(CanvasUpdate executing)
	{
		if (executing == CanvasUpdate.PostLayout)
		{
			UpdateBounds();
			UpdatePrevData();
			ForceUpdatePageIndex();

			hasRebuiltLayout = true;
		}
	}

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        if(usePageAutoChange && dragging)
            StopCoroutine("PageAutoChange");
    }

	public override void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        pageIndex = FindNearestPageIndex();

        if(usePageAutoChange && dragging)
            StartCoroutine("PageAutoChange");

        dragging = false;
    }

	protected override Vector2 CalculateOffset(Vector2 delta)
    {
    	if (dragging || pageCount == 0)
    		return base.CalculateOffset(delta);

        Vector2 offset = Vector2.zero;
        if (movementType == ScrollRect.MovementType.Unrestricted)
            return offset;

        pageIndex = Mathf.Clamp(pageIndex, 0, pageCount - 1);
        pageBounds.size = contentBounds.size / pageCount;
        pageBounds.center = contentBounds.min + pageBounds.size * ((float)pageIndex + 0.5f);

        Vector2 min = pageBounds.min;
        Vector2 max = pageBounds.max;

        if (horizontal)
        {
            min.x += delta.x;
            max.x += delta.x;
            if (min.x > viewportBounds.min.x)
                offset.x = viewportBounds.min.x - min.x;
            else if (max.x < viewportBounds.max.x)
                offset.x = viewportBounds.max.x - max.x;
        }

        if (vertical)
        {
            min.y += delta.y;
            max.y += delta.y;
            if (max.y < viewportBounds.max.y)
                offset.y = viewportBounds.max.y - max.y;
            else if (min.y > viewportBounds.min.y)
                offset.y = viewportBounds.min.y - min.y;
        }

        return offset;
    }

    protected int FindNearestPageIndex()
    {
    	float position = horizontal ? horizontalNormalizedPosition : verticalNormalizedPosition;
    	position = 0.5f + position * (pageCount - 1);
    	return Mathf.Clamp(Mathf.FloorToInt(position), 0, pageCount - 1);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        OnPageLoaded(pageIndex);

        if(usePageAutoChange)
            StartCoroutine("PageAutoChange");
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if(usePageAutoChange)
            StopCoroutine("PageAutoChange");
    }

    private IEnumerator PageAutoChange()
    {
        while(true)
        {
            yield return new WaitForSeconds(pageAutoChangeInterval);

            if(!dragging)
            {
                if(pageAutoChangeLoop)
                    NextPageWithLoop(pageAutoChangeForward);
                else
                    NextPage(pageAutoChangeForward);
            }
            
        }
    }
}

}
