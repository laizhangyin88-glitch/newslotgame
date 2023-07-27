using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine.UI;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Scroll")]
public class ScrollTo : ActionTask<Transform>
{
    public enum ScrollToAxis {
        x,
        y
    };
    public BBParameter<ContextElement> scroll;
    public ScrollToAxis axis;
    public BBParameter<float> delta;

	protected override string info
	{
		get 
        {
            return string.Format("ScrollRect.ScrollTo({0}, {1})", axis, delta);
        }
	}

	protected override void OnExecute()
	{
        var scrollRect = scroll.value.GetComponent<ScrollRect>();
        var contentRectTransform = (RectTransform)scrollRect.content.transform;
        var viewportRectTransform = (RectTransform)scrollRect.viewport.transform;
        var tempPosition = contentRectTransform.anchoredPosition;
        if (axis == ScrollToAxis.x) {
            var limitX = contentRectTransform.rect.width - viewportRectTransform.rect.width;
            tempPosition.x = Mathf.Min(delta.value, limitX);
        } else {
            var limitY = contentRectTransform.rect.height - viewportRectTransform.rect.height;
            tempPosition.y = Mathf.Min(delta.value, limitY);
        }
        contentRectTransform.anchoredPosition = tempPosition;
		EndAction();
	}
}

}
