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
            //_scroll.NextPage(false);
            _scroll.ScrollNextPage(false);
        });

        _rightBtn.onClick.AddListener(() =>
        {
            //_scroll.NextPage(true);
            _scroll.ScrollNextPage(true);
        });

        _scroll.onPageChange += (page) =>
        {
            _pageInfoController.SetCur(page);
        };

        //_scroll.onPageChangedToNext.AddListener((isForward) =>
        //{
        //    _pageInfoController.SetCur(_scroll.pageIndex);
        //});
    }

    public void Init(List<Tuple<int, string, string>> profileDatas)
    {
        InitProfileData(profileDatas);
        InitPageInfo();
    }

    private void InitProfileData(List<Tuple<int, string, string>> profileDatas)
    {
        _scrollContentNode.RemoveAllChildren();

        if (profileDatas == null)
            return;

        foreach (var item in profileDatas)
        {
            CreateProfileSelectItem(item.Item2);
        }

        //_scroll.RebuildContentBounds();
        //_scroll.Rebuild(CanvasUpdate.PostLayout);
        //_sizeFitter.SetLayoutHorizontal();
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
