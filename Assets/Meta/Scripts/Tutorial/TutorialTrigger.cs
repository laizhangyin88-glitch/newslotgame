using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{

[RequireComponent(typeof(RectTransform))]
public class TutorialTrigger : MonoBehaviour
{
    public string key;

    private RectTransform _rectTransform;
    public RectTransform rectTransform { get { return _rectTransform ?? (_rectTransform = GetComponent<RectTransform>()); } }

    private void Start()
    {
        TutorialManager.TriggerEvent(key, rectTransform);
    }

    private void OnDestroy()
    {
        TutorialManager.TriggerEvent(key, null);
    }
}

}
