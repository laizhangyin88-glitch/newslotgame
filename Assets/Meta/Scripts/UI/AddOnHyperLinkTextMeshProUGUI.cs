using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;
using System.Runtime.InteropServices;
using BagelCode;

public class AddOnHyperLinkTextMeshProUGUI : MonoBehaviour, 
#if UNITY_WEBGL && !UNITY_EDITOR
    IPointerDownHandler
#else
    IPointerClickHandler
#endif
{
    public TextMeshProUGUI textUGUI = null;

    private void Awake()
    {
        textUGUI = GetComponent<TextMeshProUGUI>();

        if(textUGUI != null)
        {
            Graphic[] graphicComponents = textUGUI.GetComponentsInChildren<Graphic>();
            for (int i = 0; i < graphicComponents.Length; ++i)
            {
                graphicComponents[i].raycastTarget = true;
            }
        }
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    public void OnPointerDown(PointerEventData eventData)
#else
    public void OnPointerClick(PointerEventData eventData)
#endif
    {
        if( textUGUI!= null )
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(textUGUI, eventData.position, eventData.pressEventCamera);

            if(linkIndex > -1)
            {
                TMP_LinkInfo linkInfo = textUGUI.textInfo.linkInfo[linkIndex];
                
#if DEV
                Debug.Log(linkInfo.GetLinkID());
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
                NativeHelper.Instance.OpenUrl(linkInfo.GetLinkID());
#else
                Application.OpenURL(linkInfo.GetLinkID());
#endif
            }
        }
    }
}
