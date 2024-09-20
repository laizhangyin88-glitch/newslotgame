using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using SlotMaker.Layout;

public class Popup_ProfileChange_ScrollController : MonoBehaviour
{
    [SerializeField] private PageInfoController _pageInfoController;
    [SerializeField] private PageScrollView _scroll;
    [SerializeField] private ContentSizeFitter _sizeFitter;
    [SerializeField] private Button _leftBtn;
    [SerializeField] private Button _rightBtn;
    [SerializeField] private Popup_ProfileSelectItem _profileSelectItemRef;
    [SerializeField] private Transform _scrollContentNode;
    [SerializeField] private ToggleGroup _toggleGroup;

    public Popup_ProfileSelectItem CurrentSelect => FindCurrentSelect();

    private void Start()
    {
        _leftBtn.onClick.AddListener(() =>
        {
            _scroll.ScrollNextPage(false);
        });

        _rightBtn.onClick.AddListener(() =>
        {
            _scroll.ScrollNextPage(true);
        });

        _scroll.onPageChange += (page) =>
        {
            _pageInfoController.SetCur(page);
        };
    }

    public void Init(List<Tuple<int, string, string>> profileDatas)
    {
        InitProfileData(profileDatas);
        InitPageInfo();
        _scroll.ScrollPage(0);
    }

    private void InitProfileData(List<Tuple<int, string, string>> profileDatas)
    {
        _scrollContentNode.RemoveAllChildrenImmediate(true);

        if (profileDatas == null)
            return;

        foreach (var item in profileDatas)
        {
            CreateProfileSelectItem(item.Item2);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollContentNode as RectTransform);
        _scroll.Init();
    }

    private void InitPageInfo()
    {
        _pageInfoController.Init(_scroll.pageCount, _scroll.currentPage);
    }

    public void UpdateProfileDisplay(List<Tuple<int, string>> profileDatas)
    {
        if (profileDatas == null)
            return;

        foreach (var item in profileDatas)
        {
            CreateProfileSelectItem(item.Item2);
        }
    }

    private Popup_ProfileSelectItem CreateProfileSelectItem(string url)
    {
        var item = Instantiate(_profileSelectItemRef, _scrollContentNode);
        item.SetImageUrl(url, _toggleGroup);
        return item;
    }

    private Popup_ProfileSelectItem FindCurrentSelect()
    {
        Popup_ProfileSelectItem[] items = _scrollContentNode.GetComponentsInChildren<Popup_ProfileSelectItem>();
        foreach (var item in items)
        {
            if(item.IsSelected)
                return item;
        }

        return null;
    }
}
