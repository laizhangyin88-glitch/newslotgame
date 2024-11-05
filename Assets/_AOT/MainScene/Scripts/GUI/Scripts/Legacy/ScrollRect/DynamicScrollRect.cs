using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class DynamicScrollRect : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement
{
    public bool interactable = true;
    public bool draggable = true;
    protected bool groupAllowInteraction = true;

	public RectTransform viewport;
	public RectTransform content;

	public bool horizontal;
	public bool vertical;

	public ScrollRect.MovementType movementType = ScrollRect.MovementType.Elastic;
	public float elasticity = 0.1f;
	public bool inertia = true;
	public float decelerationRate = 0.135f;
    public float scrollSensitivity = 27f;
    public float maximumVelocity = 100f;

	public UnityEvent onInitialized;
	public ScrollRect.ScrollRectEvent onValueChanged;
    public UnityFloatEvent onDisplacementChanged;

	private RectTransform _viewRect;
	protected RectTransform viewRect
	{
		get
		{
			if (_viewRect == null)
				_viewRect = viewport;
			if (_viewRect == null)
				_viewRect = (RectTransform)transform;
			return _viewRect;
		}
	}

	protected Vector2 velocity = Vector2.zero;
    protected float displacement;
    protected float maximumDisplacement;

	protected Vector2 prevContentPosition = Vector2.zero;
	protected Bounds prevContentBounds;
	protected Bounds contentBounds;
	protected Bounds prevViewportBounds;
	protected Bounds viewportBounds;
	protected Vector2 pointerStartLocalCursor = Vector2.zero;
	protected Vector2 contentStartPosition = Vector2.zero;

	protected readonly Vector3[] corners = new Vector3[4];

	protected bool dragging;

	protected bool initialized = false;

	protected bool hasRebuiltLayout = false;

	public virtual void Rebuild(CanvasUpdate executing)
	{
		if (executing == CanvasUpdate.PostLayout)
		{
			UpdateBounds();
			UpdatePrevData();

			hasRebuiltLayout = true;
		}
	}

	public virtual void LayoutComplete() {}

    public virtual void GraphicUpdateComplete() {}

	public override bool IsActive()
	{
		return base.IsActive() && content != null;
	}

    public virtual bool IsInteractable()
    {
        return groupAllowInteraction && interactable;
    }

	protected void EnsureLayoutHasRebuilt()
	{
		if (!hasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
            Canvas.ForceUpdateCanvases();
	}

	public virtual void StopMovement()
    {
        velocity = Vector2.zero;
    }

    protected override void OnEnable()
	{
		base.OnEnable();

		CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
	}

	protected override void OnDisable()
	{
		CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);

		hasRebuiltLayout = false;
		velocity = Vector2.zero;
        LayoutRebuilder.MarkLayoutForRebuild(content);

		base.OnDisable();
	}

    public virtual void OnScroll(PointerEventData eventData)
    {
        if (!IsActive() || !IsInteractable())
            return;

        UpdateBounds();

        Vector2 delta = eventData.scrollDelta;
        // Down is positive for scroll events, while in UI system up is positive.
        delta.y *= -1;
        if (horizontal && !vertical)
        {
            if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
                delta.x = delta.y;
            delta.y = 0;
        }
        else if (vertical && !horizontal)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                delta.y = delta.x;
            delta.x = 0;
        }
        delta *= scrollSensitivity;
        if (delta.magnitude > maximumVelocity)
            delta = delta.normalized * maximumVelocity;

        Vector2 position = content.anchoredPosition;
        position += delta;
        position += CalculateOffset(position - content.anchoredPosition);

        SetContentAnchoredPosition(position);
    }

    public virtual void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (!draggable) 
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        velocity = Vector2.zero;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!draggable)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!IsActive() || !IsInteractable())
            return;

        UpdateBounds();

        pointerStartLocalCursor = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out pointerStartLocalCursor);
        contentStartPosition = content.anchoredPosition;
        dragging = true;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!draggable)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        dragging = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!draggable)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!IsActive() || !IsInteractable())
            return;

        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, eventData.position, eventData.pressEventCamera, out localCursor))
            return;

        UpdateBounds();

        var pointerDelta = localCursor - pointerStartLocalCursor;
        Vector2 position = contentStartPosition + pointerDelta;

        Vector2 diff = position - prevContentPosition;
        if (horizontal && !vertical)
        {
            if (diff.x > maximumVelocity)
                position.x += maximumVelocity - diff.x;
            else if (diff.x < -maximumVelocity)
                position.x += -maximumVelocity - diff.x;
        }
        if (vertical && !horizontal)
        {
            if (diff.y > maximumVelocity)
                position.y += maximumVelocity - diff.y;
            else if (diff.y < -maximumVelocity)
                position.y += -maximumVelocity - diff.y;
        }

        // Offset to get content into place in the view.
        Vector2 offset = CalculateOffset(position - content.anchoredPosition);
        position += offset;
        if (movementType == ScrollRect.MovementType.Elastic)
        {
            if (offset.x != 0f)
                position.x = position.x - RubberDelta(offset.x, viewportBounds.size.x);
            if (offset.y != 0f)
                position.y = position.y - RubberDelta(offset.y, viewportBounds.size.y);
        }

        SetContentAnchoredPosition(position);
    }

    protected static float RubberDelta(float overStretching, float viewSize)
    {
        return (1f - (1f / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1f))) * viewSize * Mathf.Sign(overStretching);
    }

    protected override void OnRectTransformDimensionsChange()
    {
        SetDirty();
    }

    public Vector2 normalizedPosition
    {
        get
        {
            return new Vector2(horizontalNormalizedPosition, verticalNormalizedPosition);
        }
        set
        {
            SetHorizontalNormalizedPosition(value.x);
            SetVerticalNormalizedPosition(value.y);
        }
    }

    public float horizontalNormalizedPosition
    {
        get
        {
            UpdateBounds();
            if (contentBounds.size.x <= viewportBounds.size.x)
                return (viewportBounds.min.x > contentBounds.min.x) ? 1 : 0;
            return (viewportBounds.min.x - contentBounds.min.x) / (contentBounds.size.x - viewportBounds.size.x);
        }
        set
        {
            SetHorizontalNormalizedPosition(value);
        }
    }

    public float verticalNormalizedPosition
    {
        get
        {
            UpdateBounds();
            if (contentBounds.size.y <= viewportBounds.size.y)
                return (viewportBounds.min.y > contentBounds.min.y) ? 1 : 0;
            ;
            return (viewportBounds.min.y - contentBounds.min.y) / (contentBounds.size.y - viewportBounds.size.y);
        }
        set
        {
            SetVerticalNormalizedPosition(value);
        }
    }

    public void SetHorizontalNormalizedPosition(float value) { SetNormalizedPosition(value, 0); }
    public void SetVerticalNormalizedPosition(float value) { SetNormalizedPosition(value, 1); }

    protected void SetNormalizedPosition(float value, int axis)
    {
        EnsureLayoutHasRebuilt();
        UpdateBounds();
        // How much the content is larger than the view.
        float hiddenLength = contentBounds.size[axis] - viewportBounds.size[axis];
        // Where the position of the lower left corner of the content bounds should be, in the space of the view.
        float contentBoundsMinPosition = viewportBounds.min[axis] - value * hiddenLength;
        // The new content localPosition, in the space of the view.
        float newLocalPosition = content.localPosition[axis] + contentBoundsMinPosition - contentBounds.min[axis];

        Vector3 localPosition = content.localPosition;
        if (Mathf.Abs(localPosition[axis] - newLocalPosition) > 0.01f)
        {
            localPosition[axis] = newLocalPosition;
            content.localPosition = localPosition;
            velocity[axis] = 0;
            UpdateBounds();
        }
    }

    public void ApproximateMaximumDisplacement(float padding, float size, float spacing, int count, int maxBufferingCount, out int displayCount)
    {
        displayCount = 0;

        float contentSize = padding + (size * count) + (spacing * (count - 1));
        if (horizontal && !vertical)
        {
            contentSize = (contentSize * content.localScale.x) - viewportBounds.size.x;
            displayCount = (int)(viewportBounds.size.x / ((size + spacing) * content.localScale.x));
        }
        else if (vertical && !horizontal)
        {
            contentSize = (contentSize * content.localScale.y) - viewportBounds.size.y;
            displayCount = (int)(viewportBounds.size.y / ((size + spacing) * content.localScale.y));
        }
        
        maximumDisplacement = Mathf.Max(contentSize, maximumDisplacement);
    }

    public float normalizedDisplacement
    {
        get
        {
            if (maximumDisplacement == 0f)
                return 0f;
            
            return Mathf.Clamp01(displacement / maximumDisplacement);
        }
        set 
        {
            SetNormalizedDisplacement(value);
        }
    }

    protected void SetNormalizedDisplacement(float value)
    {
        float targetDisplacement = value * maximumDisplacement;
        float delta = targetDisplacement - displacement;
        if (delta > maximumVelocity)
            delta = maximumVelocity;
        else if (delta < -maximumVelocity)
            delta = -maximumVelocity;

        Vector2 position = content.anchoredPosition;
        if (horizontal && !vertical)
            position.x -= delta;
        else if (vertical && !horizontal)
            position.y += delta;
        
        position += CalculateOffset(position - content.anchoredPosition);

        SetContentAnchoredPosition(position);
    }

    public void RebuildContentBounds()
    {
    	viewportBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
        contentBounds = GetBounds();

        if (content == null)
            return;

        Vector2 offset = Vector2.zero;
    	if (horizontal)
    		offset.x = -(contentBounds.max.x - prevContentBounds.max.x) + (contentBounds.min.x - prevContentBounds.min.x);
    	if (vertical)
    		offset.y = -(contentBounds.min.y - prevContentBounds.min.y) + (contentBounds.max.y - prevContentBounds.max.y);

        contentStartPosition += offset;
        prevContentPosition += offset;

        Vector2 position = content.anchoredPosition;
        position += offset;

    	SetContentAnchoredPosition(position);
    }

    public void UpdateBounds()
    {
    	viewportBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
        contentBounds = GetBounds();

        // Make sure content bounds are at least as large as view by adding padding if not.
        // One might think at first that if the content is smaller than the view, scrolling should be allowed.
        // However, that's not how scroll views normally work.
        // Scrolling is *only* possible when content is *larger* than view.
        // We use the pivot of the content rect to decide in which directions the content bounds should be expanded.
        // E.g. if pivot is at top, bounds are expanded downwards.
        // This also works nicely when ContentSizeFitter is used on the content.
        Vector3 contentSize = contentBounds.size;
        Vector3 contentPos = contentBounds.center;
        Vector3 excess = viewportBounds.size - contentSize;
        if (excess.x > 0)
        {
            contentPos.x -= excess.x * (content.pivot.x - 0.5f);
            contentSize.x = viewportBounds.size.x;
        }
        if (excess.y > 0)
        {
            contentPos.y -= excess.y * (content.pivot.y - 0.5f);
            contentSize.y = viewportBounds.size.y;
        }

        contentBounds.size = contentSize;
        contentBounds.center = contentPos;
    }

    public void UpdatePrevData()
    {
        if (content == null)
            prevContentPosition = Vector2.zero;
        else
            prevContentPosition = content.anchoredPosition;
        prevViewportBounds = viewportBounds;
        prevContentBounds = contentBounds;
    }

    protected Bounds GetBounds()
    {
        if (content == null)
            return new Bounds();

        var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        var toLocal = viewRect.worldToLocalMatrix;
        content.GetWorldCorners(corners);
        for (int j = 0; j < 4; j++)
        {
            Vector3 v = toLocal.MultiplyPoint3x4(corners[j]);
            vMin = Vector3.Min(v, vMin);
            vMax = Vector3.Max(v, vMax);
        }

        var bounds = new Bounds(vMin, Vector3.zero);
        bounds.Encapsulate(vMax);
        return bounds;
    }

    protected virtual Vector2 CalculateOffset(Vector2 delta)
    {
        Vector2 offset = Vector2.zero;
        if (movementType == ScrollRect.MovementType.Unrestricted)
            return offset;

        Vector2 min = contentBounds.min;
        Vector2 max = contentBounds.max;

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

    public virtual void SetContentAnchoredPosition(Vector2 position)
    {
    	if (!horizontal)
            position.x = content.anchoredPosition.x;
        if (!vertical)
            position.y = content.anchoredPosition.y;

        if (position != content.anchoredPosition)
        {
            content.anchoredPosition = position;
            UpdateBounds();
        }
    }

    protected virtual void LateUpdate()
    {
    	if (!IsActive())
    		return;

    	EnsureLayoutHasRebuilt();

    	if (!initialized)
    	{
    		onInitialized.Invoke();
            UpdatePrevData();
            
            initialized = true;
    	}

    	UpdateBounds();

    	float deltaTime = Time.unscaledDeltaTime;
    	Vector2 offset = CalculateOffset(Vector2.zero);
    	if (!dragging && (offset != Vector2.zero || velocity != Vector2.zero))
    	{
    		Vector2 position = content.anchoredPosition;
            for (int axis = 0; axis < 2; axis++)
            {
                // Apply spring physics if movement is elastic and content has an offset from the view.
                if (movementType == ScrollRect.MovementType.Elastic && offset[axis] != 0)
                {
                    float speed = velocity[axis];
                    position[axis] = Mathf.SmoothDamp(content.anchoredPosition[axis], content.anchoredPosition[axis] + offset[axis], ref speed, elasticity, Mathf.Infinity, deltaTime);
                    if (Mathf.Abs(speed) < 1)
                        speed = 0;
                    velocity[axis] = speed;
                }
                // Else move content according to velocity with deceleration applied.
                else if (inertia)
                {
                    velocity[axis] *= Mathf.Pow(decelerationRate, deltaTime);
                    if (Mathf.Abs(velocity[axis]) < 1f)
                        velocity[axis] = 0;
                    position[axis] += velocity[axis] * deltaTime;
                }
                // If we have neither elaticity or friction, there shouldn't be any velocity.
                else
                {
                    velocity[axis] = 0;
                }
            }

            if (velocity != Vector2.zero)
            {
                if (movementType == ScrollRect.MovementType.Clamped)
                {
                    offset = CalculateOffset(position - content.anchoredPosition);
                    position += offset;
                }

                SetContentAnchoredPosition(position);
            }
    	}

    	if (dragging && inertia)
        {
            Vector3 newVelocity = (content.anchoredPosition - prevContentPosition) / deltaTime;
            velocity = Vector3.Lerp(velocity, newVelocity, deltaTime * 10f);
        }

        if (viewportBounds != prevViewportBounds || contentBounds != prevContentBounds || content.anchoredPosition != prevContentPosition)
        {
        	onValueChanged.Invoke(normalizedPosition);

            if (horizontal && !vertical)
                displacement -= content.anchoredPosition.x - prevContentPosition.x;
            else if (vertical && !horizontal)
                displacement += content.anchoredPosition.y - prevContentPosition.y;

            maximumDisplacement = Mathf.Max(displacement, maximumDisplacement);
            onDisplacementChanged.Invoke(normalizedDisplacement);

            UpdatePrevData();
        }
    }

    private readonly List<CanvasGroup> canvasGroupCache = new List<CanvasGroup>();
    protected override void OnCanvasGroupChanged()
    {
        bool _groupAllowInteraction = true;
        Transform t = transform;
        while (t != null)
        {
            t.GetComponents(canvasGroupCache);
            bool shouldBreak = false;
            for (int i = 0; i < canvasGroupCache.Count; ++i)
            {
                if (!canvasGroupCache[i].interactable)
                {
                    _groupAllowInteraction = false;
                    shouldBreak = true;
                }

                if (canvasGroupCache[i].ignoreParentGroups)
                    shouldBreak = true;
            }
            if (shouldBreak)
                break;

            t = t.parent;
        }

        groupAllowInteraction = _groupAllowInteraction;
    }

    protected void SetDirty()
    {
        if (!IsActive())
            return;

        LayoutRebuilder.MarkLayoutForRebuild(content);
    }

    protected virtual void SetDirtyCaching()
    {
        if (!IsActive())
            return;

        CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        LayoutRebuilder.MarkLayoutForRebuild(content);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        SetDirtyCaching();
    }
#endif
}

}
