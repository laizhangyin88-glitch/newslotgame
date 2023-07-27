using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Transform")]
public class SetAnchoredPosition : ActionTask
{
    public BBParameter<Transform> target;
    public BBParameter<TextAnchor> anchor;
    public BBParameter<Vector2> offset;

    protected override void OnExecute()
    {
        // var parent = target.value.parent as RectTransform;
        var child = target.value as RectTransform;

        Vector2 size = child.rect.size;
        Vector2 alignment = new Vector2(((int)anchor.value % 3) * 0.5f, ((int)anchor.value / 3) * 0.5f);
		Vector2 startOffset = Vector2.zero;
        startOffset.x = -alignment.x * size.x;
		startOffset.y = alignment.y * size.y;
		Vector2 newAnchor = new Vector2(alignment.x, 1.0f - alignment.y);

        child.anchorMin = child.anchorMax = newAnchor;
        child.anchoredPosition = offset.value;

        EndAction();
    }
}

}
