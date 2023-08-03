using Com.TheFallenGames.OSA.Core;
using hall;
using NodeCanvas.Framework;
using SlotMaker;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BagelCode.OSA_Scroll
{
    public class OSA_Rooms : OSA<RoomsParams, RoomsItem>
    {
        private bool isInteractable = true;

        public void Refresh()
        {
            ClearVisibleItems();
            CreateRoomList();
        }

        private new void Awake()
        {
            
        }

        private void OnEnable()
        {
            isInteractable = PopupManager.Instance.popupCount == 1;
            ScrollPositionChanged += OnChangeScrollPosition;
        }

        private void OnDisable()
        {
            ScrollPositionChanged -= OnChangeScrollPosition;
        }

        public void OnChangeMetaPopupCount()
        {
            isInteractable = PopupManager.Instance.popupCount == 0;

            if (isInteractable == false)
            {
                StopMovement();
            }
        }

        private void OnChangeScrollPosition(double delta)
        {

        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (isInteractable)
            {
                base.OnDrag(eventData);
            }
        }


        protected override void Start()
        {
            FixPlatformScaler();
            base.Start();
            CreateRoomList();
        }

        protected override RoomsItem CreateViewsHolder(int itemIndex)
        {
            var item = new RoomsItem();
            if (item != null)
                item.Init(FindPrefab(0, itemIndex), itemIndex);
            return item;
        }

        protected override void UpdateViewsHolder(RoomsItem newOrRecycled)
        {
            var model = _Params.data[newOrRecycled.ItemIndex];
            newOrRecycled.UpdateViews(model);
        }

        protected override bool IsRecyclable(RoomsItem potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
        {
            return true;
        }

        protected override bool ShouldDestroyRecyclableItem(RoomsItem inRecycleBin, bool isInExcess)
        {
            return inRecycleBin.ShouldDestroyRecyclableItem();
        }

        void CreateRoomList()
        {
            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "fishModels");
            LoginHallRsp loginHallRsp = bb.GetValue<LoginHallRsp>("loginHallRsp");
            var roomInfoList = loginHallRsp.rooms;
            _Params.data.Clear();

            //fade roomInfoList
            //List<RoomStatusInfo> roomInfoList = new List<RoomStatusInfo>();
            //RoomStatusInfo roomStatusInfo = new RoomStatusInfo();
            //for (int i = 0; i < 3; i++)
            //{
            //    roomStatusInfo.enter_min = (ulong)(100 * (i + 1));
            //    roomInfoList.Add(roomStatusInfo);
            //}



            _Params.data.AddRange(roomInfoList);
            ResetItems(roomInfoList.Count);
        }

        GameObject FindPrefab(int id, int itemIndex)
        {
            if (_Params.prefabs[id] == null && _Params.sceneInfos[id] != null)
            {
                var go = SceneManager.LoadScene(transform, _Params.sceneInfos[id].GetSceneInfo());
                go.SetActive(false);
                _Params.prefabs[id] = go;
            }
            return _Params.prefabs[id];
        }
        void FixPlatformScaler()
        {
            var rectTransform = GetComponent<RectTransform>();
            float xScale = rectTransform.localScale.x;
            if (xScale != 0f && !Mathf.Approximately(xScale, 1f))
            {
                float screenWidth = rectTransform.root.GetComponent<RectTransform>().sizeDelta.x;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, screenWidth * (1f / xScale - 1f));
            }
        }
        public void ScrollToNextPage()
        {
            MovePage(1);
        }

        public void ScrollToPrevPage()
        {
            MovePage(-1);
        }

        public void ScrollToFirstPage()
        {
            SmoothScrollTo(0, 0.3f, 0.5f);

            Analytics.CustomEvent("client_click_lobby_reset", null);
        }

        void MovePage(int pageCount)
        {
            float offset = 0.075f;
            int itemIndex = GetItemViewsHolder(0).ItemIndex + pageCount * (VisibleItemsCount - 1);
            if (itemIndex < 0)
                offset = 0.5f;
            itemIndex = Mathf.Clamp(itemIndex, 0, _Params.data.Count - 1);
            SmoothScrollTo(itemIndex, 0.3f, offset);
        }

    }
    [Serializable]
    public class RoomsParams : BaseParams
    {
        public SceneInfoObject[] sceneInfos;
        public GameObject[] prefabs;

        public List<hall.RoomStatusInfo> data = new List<hall.RoomStatusInfo>();
    }

    public class RoomsItem : BaseItemViewsHolder
    {
        public virtual bool ShouldDestroyRecyclableItem() { return false; }
        public virtual void UpdateViews(hall.RoomStatusInfo roomStatusInfo)
        {
            var element = root.GetComponent<ContextElement>();
            root.gameObject.SetActive(true);
            var controller = root.GetComponent<RoomItemController>();
            controller.UpdateRoomInfo(roomStatusInfo);
        }
    }
}
