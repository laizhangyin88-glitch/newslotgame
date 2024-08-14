using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageInfoController : MonoBehaviour
{
    [SerializeField] private ToggleGroup _toggleGroupCom;
    [SerializeField] private Toggle _pageInfoItemRef;

    public int PageCount { get; private set; }
    public int CurPage {  get; private set; }

    public void Init(int count, int cur)
    {
        transform.RemoveAllChildrenImmediate();

        PageCount = count;
        CurPage = cur;

        for (int i = 0; i < count; i++)
        {
            Toggle toggle = Instantiate(_pageInfoItemRef, transform);
            toggle.group = _toggleGroupCom;
        }

        SetCur(cur);
    }

    public void SetCur(int cur)
    {
        Debug.Log($"<color=green>@==</color>{cur}/{PageCount}");
        if (cur < 0 || cur >= PageCount)
            return;

        transform.GetChild(cur).GetComponent<Toggle>().isOn = true;
    }
}
