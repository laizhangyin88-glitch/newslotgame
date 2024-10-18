using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginMaskController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;

    private void Start()
    {
        if (_canvasGroup == null)
            return;

        _canvasGroup.blocksRaycasts = !TestDisplayManager.Instance.IsGM;
    }
}
