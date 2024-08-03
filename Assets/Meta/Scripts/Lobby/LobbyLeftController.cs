using BagelCode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyLeftController : MonoBehaviour
{
    private Toggle toggle;
    private Animator animator;
    private bool isHide;
    Vector2 originalPos;
    Vector2 hidePos;
    
    void Start()
    {
        toggle = transform.Find("Toggle").GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleChange);
        originalPos = GetComponent<RectTransform>().anchoredPosition;
        hidePos = new Vector2(originalPos.x * -1, originalPos.y);
    }

    void OnToggleChange(bool value)
    {
        toggle.enabled = false;
        isHide = !value;
        if (!isHide)
            AsyncActionUtils.ApplyAnchoredMovement(this,
            transform, originalPos, hidePos, 0.5f, TweenUtils.VectorTweenLinear, onComplete: AniFinish);
        else
            AsyncActionUtils.ApplyAnchoredMovement(this,
            transform, hidePos, originalPos, 0.5f, TweenUtils.VectorTweenLinear, onComplete: AniFinish);


        animator.SetInteger("show", value ? 0 : 1);
    }

    public void AniFinish()
    {
        //animator.SetInteger("show", -1);
        toggle.enabled = true;
    }
}
