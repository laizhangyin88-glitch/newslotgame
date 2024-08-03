using BagelCode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyLeftController : MonoBehaviour
{
    private Toggle toggle;
    private Animator animator;
    private bool isHide;
    
    void Start()
    {
        toggle = transform.Find("Toggle").GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleChange);
    }

    void OnToggleChange(bool value)
    {
        toggle.enabled = false;
        isHide = !value;
        //AsyncActionUtils.ApplyAnchoredMovement(this,
        //    transform, )
        animator.SetInteger("show", value ? 0 : 1);
    }

    public void AniFinish()
    {
        //animator.SetInteger("show", -1);
        toggle.enabled = true;
    }
}
