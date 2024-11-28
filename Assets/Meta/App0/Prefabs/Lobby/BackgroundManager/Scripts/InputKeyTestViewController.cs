using BlizzEvent;
using ParadoxNotion;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SBoxApi.SBoxSandbox;

public class InputKeyTestViewController : MonoBehaviour
{
    private Image ImageRightBig;
    private bool isDownRightBig;

    private Image ImageLeftBig;
    private bool isDownLeftBig;

    private Image ImageRight1;
    private bool isDownRight1;

    private Image ImageLeft1;
    private bool isDownLeft1;

    private Image ImageRight2;
    private bool isDownRight2;

    private Image ImageLeft2;
    private bool isDownLeft2;

    private Image ImageRight3;
    private bool isDownRight3;

    private Image ImageLeft3;
    private bool isDownLeft3;

    private Image ImageRight4;
    private bool isDownRight4;

    private Image ImageLeft4;
    private bool isDownLeft4;



    private Button closeBtn;

    private void Start()
    {
        ImageRightBig = transform.Find("content/RightBig").GetComponent<Image>();
        ImageLeftBig = transform.Find("content/LeftBig").GetComponent<Image>();

        ImageLeft1 = transform.Find("content/Left/Left1").GetComponent<Image>();
        ImageLeft2 = transform.Find("content/Left/Left2").GetComponent<Image>();
        ImageLeft3 = transform.Find("content/Left/Left3").GetComponent<Image>();
        ImageLeft4 = transform.Find("content/Left/Left4").GetComponent<Image>();

        ImageRight1 = transform.Find("content/Right/Right1").GetComponent<Image>();
        ImageRight2 = transform.Find("content/Right/Right2").GetComponent<Image>();
        ImageRight3 = transform.Find("content/Right/Right3").GetComponent<Image>();
        ImageRight4 = transform.Find("content/Right/Right4").GetComponent<Image>();

        closeBtn = transform.Find("content/ButtonClose").GetComponent<Button>();
        closeBtn.onClick.AddListener(OnClickBtnClose);

        AddButtonEvent();
    }

    private void AddButtonEvent()
    {
#if UNITY_EDITOR
        EventCenter.Instance.AddEventListener<SBOX_SWITCH>(EventHandle.HARDWARE_KEY_DOWN, OnKeyDown);
        EventCenter.Instance.AddEventListener<SBOX_SWITCH>(EventHandle.HARDWARE_KEY_UP, OnKeyUp);
#else
        if (ApplicationSettings.Instance.isMachine){

            foreach (SBOX_SWITCH value in Enum.GetValues(typeof(SBOX_SWITCH)))
            {
                SBoxSandboxListener.Instance.AddButtonDown(value, () => {
                    OnKeyDown(value);
                });
                SBoxSandboxListener.Instance.AddButtonUp(value, () => {
                    OnKeyUp(value);
                });
            }
        }
#endif
    }

    private void Update()
    {
        if(isDownRightBig)
        {
            ImageRightBig.color = Color.red;
        }
        else
        {
            ImageRightBig.color = Color.white;
        }

        if (isDownLeftBig)
        {
            ImageLeftBig.color = Color.red;
        }
        else
        {
            ImageLeftBig.color = Color.white;
        }

        if (isDownRight1)
        {
            ImageRight1.color = Color.red;
        }
        else
        {
            ImageRight1.color= Color.white;
        }

        if (isDownRight2)
        {
            ImageRight2.color = Color.red;
        }
        else
        {
            ImageRight2.color = Color.white;
        }

        if (isDownRight3)
        {
            ImageRight3.color = Color.red;
        }
        else
        {
            ImageRight3.color = Color.white;
        }

        if (isDownRight4)
        {
            ImageRight4.color = Color.red;
        }
        else
        {
            ImageRight4.color = Color.white;
        }

        if (isDownLeft1)
        {
            ImageLeft1.color = Color.red;
        }
        else
        {
            ImageLeft1.color= Color.white;
        }

        if (isDownLeft1)
        {
            ImageLeft1.color = Color.red;
        }
        else
        {
            ImageLeft1.color = Color.white;
        }


        if (isDownLeft2)
        {
            ImageLeft2.color = Color.red;
        }
        else
        {
            ImageLeft2.color = Color.white;
        }

        if (isDownLeft3)
        {
            ImageLeft3.color = Color.red;
        }
        else
        {
            ImageLeft3.color = Color.white;
        }

        if (isDownLeft4)
        {
            ImageLeft4.color = Color.red;
        }
        else
        {
            ImageLeft4.color = Color.white;
        }
    }

    private void OnKeyDown(SBOX_SWITCH sBOX_SWITCH)
    {
#if UNITY_EDITOR
        Debug.LogError("KeyDown " + sBOX_SWITCH);
#endif

        switch (sBOX_SWITCH)
        {
            case SBOX_SWITCH.SWITCH_UP:
                break;
            case SBOX_SWITCH.SWITCH_DOWN:
                break;
            case SBOX_SWITCH.SWITCH_LEFT:
                break;
            case SBOX_SWITCH.SWITCH_RIGHT:
                break;
            case SBOX_SWITCH.SWITCH_ROOT_SET:
                break;
            case SBOX_SWITCH.SWITCH_SET:
                break;
            case SBOX_SWITCH.SWITCH_DOOR_SWITCH:
                break;
            case SBOX_SWITCH.SWITCH_PAYOUT: 
                isDownLeftBig = true;
                break;
            case SBOX_SWITCH.SWITCH_ENTER:
                isDownRightBig = true;
                break;
            case SBOX_SWITCH.SWITCH_ESC:
                isDownLeft3 = true;
                break;
            case SBOX_SWITCH.SWITCH_SWITCH:
                isDownRight3 = true;
                break;
            case SBOX_SWITCH.SWITCH_SCORE_UP:
                break;
            case SBOX_SWITCH.SWITCH_SCORE_DOWN:
                break;
            case SBOX_SWITCH.SWITCH_RED:
                isDownLeft1 = true;
                break;
            case SBOX_SWITCH.SWITCH_GREEN:
                isDownLeft4 = true;
                break;
            case SBOX_SWITCH.SWITCH_YELLOW:
                isDownRight4 = true;
                break;
            case SBOX_SWITCH.SWITCH_BET4:
                isDownRight1 = true;
                break;
            case SBOX_SWITCH.SWITCH_BET5:
                isDownRight2 = true;
                break;
            case SBOX_SWITCH.SWITCH_AUTO:
                isDownLeft2 = true;
                break;
            default:
                break;
        }
    }

    private void OnKeyUp(SBOX_SWITCH sBOX_SWITCH)
    {
#if UNITY_EDITOR
        //Debug.LogError("KeyUp " + sBOX_SWITCH);
        Debug.LogError("KeyUp " + sBOX_SWITCH);
#endif
        switch (sBOX_SWITCH)
        {
            case SBOX_SWITCH.SWITCH_UP:
                break;
            case SBOX_SWITCH.SWITCH_DOWN:
                break;
            case SBOX_SWITCH.SWITCH_LEFT:
                break;
            case SBOX_SWITCH.SWITCH_RIGHT:
                break;
            case SBOX_SWITCH.SWITCH_ROOT_SET:
                break;
            case SBOX_SWITCH.SWITCH_SET:
                break;
            case SBOX_SWITCH.SWITCH_DOOR_SWITCH:
                break;
            case SBOX_SWITCH.SWITCH_PAYOUT:
                isDownLeftBig = false;
                break;
            case SBOX_SWITCH.SWITCH_ENTER:
                isDownRightBig = false;
                break;
            case SBOX_SWITCH.SWITCH_ESC:
                isDownLeft3 = false;
                break;
            case SBOX_SWITCH.SWITCH_SWITCH:
                isDownRight3 = false;
                break;
            case SBOX_SWITCH.SWITCH_SCORE_UP:
                break;
            case SBOX_SWITCH.SWITCH_SCORE_DOWN:
                break;
            case SBOX_SWITCH.SWITCH_RED:
                isDownLeft1 = false;
                break;
            case SBOX_SWITCH.SWITCH_GREEN:
                isDownLeft4 = false;
                break;
            case SBOX_SWITCH.SWITCH_YELLOW:
                isDownRight4 = false;
                break;
            case SBOX_SWITCH.SWITCH_BET4:
                isDownRight1 = false;
                break;
            case SBOX_SWITCH.SWITCH_BET5:
                isDownRight2 = false;
                break;
            case SBOX_SWITCH.SWITCH_AUTO:
                isDownLeft2 = false;
                break;
            default:
                break;
        }
    }

    private void OnClickBtnClose()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        RemoveButtonEvent();
        SBoxSanboxController.Instance.isEnterBackgroundTestView = false;
    }

    private void RemoveButtonEvent()
    {
#if UNITY_EDITOR
        EventCenter.Instance.AddEventListener<SBOX_SWITCH>(EventHandle.HARDWARE_KEY_DOWN, OnKeyDown);
        EventCenter.Instance.AddEventListener<SBOX_SWITCH>(EventHandle.HARDWARE_KEY_UP, OnKeyUp);
#else
        if (ApplicationSettings.Instance.isMachine)
        {

            foreach (SBOX_SWITCH value in Enum.GetValues(typeof(SBOX_SWITCH)))
            {
                SBoxSandboxListener.Instance.RemoveButtonDown(value, () => {
                    OnKeyDown(value);
                });
                SBoxSandboxListener.Instance.RemoveButtonUp(value, () => {
                    OnKeyUp(value);
                });
            }
        }
#endif
    }
}
