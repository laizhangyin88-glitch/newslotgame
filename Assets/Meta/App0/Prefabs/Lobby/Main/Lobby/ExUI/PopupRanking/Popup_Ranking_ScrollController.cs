using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Ranking_ScrollController : MonoBehaviour
{
    [SerializeField] private PageInfoController _pageInfoController;
    [SerializeField] private PageScrollView _scroll;
    [SerializeField] private ContentSizeFitter _sizeFitter;
    [SerializeField] private Button _leftBtn;
    [SerializeField] private Button _rightBtn;
    [SerializeField] private Popup_ProfileSelectItem _profileSelectItemRef;
    [SerializeField] private Transform _scrollContentNode;
    [SerializeField] private ToggleGroup _toggleGroup;

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

        Init(null);
    }

    public void Init(List<Tuple<int, string, string>> profileDatas)
    {
        InitProfileData(profileDatas);
        InitPageInfo();
        _scroll.ScrollPage(0);
    }

    private void InitProfileData(List<Tuple<int, string, string>> profileDatas)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollContentNode as RectTransform);
        _scroll.Init();
    }

    private void InitPageInfo()
    {
        _pageInfoController.Init(_scroll.currentPage);
    }
}
