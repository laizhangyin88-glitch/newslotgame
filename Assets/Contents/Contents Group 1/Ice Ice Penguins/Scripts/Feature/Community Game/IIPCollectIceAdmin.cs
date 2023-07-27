using System.Collections;
using System.Collections.Generic;
using GameStudio.Slot.IIP.Utility;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using TMPro;
using UnityEngine;
namespace GameStudio.Slot.IIP.Feature
{
    public struct IIPIceFlyingInfo
    {
        public IIPCommunityGameTile targetTile;
        public bool isMine;
    }
    public class IIPCollectIceAdmin : FeatureController
    {

        protected override string ON_FEATURE_BEGIN_EVENT => "IIP_COLLECT_ICE";

        protected override string ON_FEATURE_END_EVENT => "IIP_FINISH_COLLECT_ICE";

        public ObjectPool iceFlyingObjectPool;
        public ObjectPool treasureFlyingObjectPool;
        public ObjectPool valueFlyingObjectPool;

        public IIPCommunityGameTileAdmin tileAdmin;
        public IIPCommunityGameUIAdmin uiAdmin;
        public IIPCommunityGameAdmin communityGameAdmin;


        [SerializeField] private List<SlotMachine> listSlotMachine;
        private List<IIPIceFlyingInfo> flyingInfoList = new List<IIPIceFlyingInfo>();

        //Treasure
        private int treasureMutliplier = 0;
        [SerializeField] private Transform treasureTargetPoint;
        private Transform treasureStartPoint;
        private GameObject treasureFlying;
        private IIPCommunityGameTile treasureTile;

        private bool playerValueFly = false;
        private bool treasureDirecting = false;
        private void Awake()
        {
            RegisterEvent("IIPIceTileCollect", OnTileFlyArrive);

            RegisterEvent("IIPTreasureArrive", OnTreasureArrive);
            RegisterEvent("IIPTreasureFlying", OnTreasureFlying);
            RegisterEvent("IIPValueArrive", OnValueArrive);
        }

        public void OnTreasureFlying(EventData eventData)
        {
            Transform from = treasureStartPoint;
            Transform to = treasureTargetPoint;

            treasureFlying = treasureFlyingObjectPool.GetObject(true).gameObject;
            treasureFlying.transform.SetParent(this.transform);
            treasureFlying.SetActive(false);
            treasureFlying.transform.position = from.position;

            var tresureFlyingBB = treasureFlying.GetComponent<Blackboard>();

            tresureFlyingBB.GetVariable<Transform>("from").SetValue(from);
            tresureFlyingBB.GetVariable<Transform>("to").SetValue(from);
            treasureFlying.GetComponentInChildren<TextMeshProUGUI>(true).text = $"X{treasureMutliplier}";

            treasureFlying.SetActive(true);

            StartCoroutine(FlyingTreasureValueCoroutine(treasureTile, treasureMutliplier, 0.125f));
        }


        public void OnValueArrive(EventData eventData)
        {
            if (treasureDirecting)
            {
                uiAdmin.treasureAnimator.SetTrigger("CollectTreasure");
            }

            communityGameAdmin.AddEarnCredit(communityGameAdmin.bonusBet);
            playerValueFly = false;
        }

        public void OnTreasureArrive(EventData eventData)
        {
            communityGameAdmin.OnPlayerCollectTreasure();
            treasureTile.DisableObject(TileObjectStatus.TREASURE);
        }

        public void OnTileFlyArrive(EventData eventData)
        {
            IIPCommunityGameTile targetTile = flyingInfoList[0].targetTile;
            bool isMine = flyingInfoList[0].isMine;

            targetTile.UpdateFloor(TileFloorStatus.ICE, false);

            IIPUtility.PlaySound("Ice Appear");

            if (targetTile.tileObjectStatus != TileObjectStatus.TREASURE && isMine)
            {
                StartCoroutine(FlyingValueCoroutine(targetTile));
            }

            if (targetTile.tileObjectStatus == TileObjectStatus.PENGUIN && targetTile.isObjectOpened)
                targetTile.penguin.Rescue();

            else if (targetTile.tileObjectStatus == TileObjectStatus.TREASURE && isMine)
            {
                treasureStartPoint = flyingInfoList[0].targetTile.transform;
                targetTile.EnableObject(TileObjectStatus.TREASURE);
                treasureTile = targetTile;
            }
            else if (targetTile.tileObjectStatus == TileObjectStatus.TREASURE && !isMine)
            {
                communityGameAdmin.OnOtherPlayerCollectTreasure();
            }
            flyingInfoList.RemoveAt(0);
        }


        private IEnumerator FlyingTreasureValueCoroutine(IIPCommunityGameTile fromTile, int count, float delay)
        {
            Coroutine lastCoroutine = null;
            for (int i = 0; i < count; i++)
            {
                lastCoroutine = StartCoroutine(FlyingValueCoroutine(fromTile));
                yield return new WaitForSeconds(delay);
            }
            yield return lastCoroutine;
            yield return new WaitForSeconds(2f);
            treasureDirecting = false;
        }

        private IEnumerator FlyingValueCoroutine(IIPCommunityGameTile fromTile)
        {
            GameObject valueFlying = valueFlyingObjectPool.GetObject(true).gameObject;
            valueFlying.transform.position = fromTile.transform.position;
            Variable<Transform> from = BlackboardUtils.FindVariable<Transform>(valueFlying.GetComponent<Blackboard>(), "from");
            Variable<Transform> to = BlackboardUtils.FindVariable<Transform>(valueFlying.GetComponent<Blackboard>(), "to");
            from.SetValue(fromTile.transform);
            to.SetValue(treasureTargetPoint);
            valueFlying.GetComponentInChildren<TextMeshProUGUI>().text = FormatUtility.SimpleNumberFormat(communityGameAdmin.bonusBet);
            yield return new WaitForSeconds(1f);
            valueFlying.GetComponent<Animator>().SetTrigger("Fly");

        }

        protected override IEnumerator OnPlayCoroutine()
        {
            List<Blackboard> userGameList = BlackboardUtils.FindVariable<List<Blackboard>>("./bonus/response/userGameResultList").value;
            int spinIndex = BlackboardUtils.FindVariable<int>("./bonus/spinCount").value - 1;
            for (int userIndex = 0; userIndex < userGameList.Count; userIndex++)
            {
                List<Blackboard> spinResultList = userGameList[userIndex].GetVariable<List<Blackboard>>("spinResultList").value;
                Blackboard currentSpinResult = spinResultList[spinIndex];
                Blackboard iceData = currentSpinResult.GetVariable<Blackboard>("placeIceData")?.value;

                if (iceData != null)
                {
                    Blackboard treasureData = currentSpinResult.GetVariable<Blackboard>("treasureData")?.value;
                    Blackboard cell = iceData.GetVariable<Blackboard>("cell").value;

                    BaseSymbol symbol = listSlotMachine[userIndex].GetSymbol(0, 0);
                    Vector3 fromPosition = symbol.transform.position;

                    GameObject iceFlying = iceFlyingObjectPool.GetObject(true).gameObject;
                    iceFlying.transform.SetParent(this.transform);
                    iceFlying.SetActive(false);
                    iceFlying.transform.position = fromPosition;

                    Variable<Transform> from = BlackboardUtils.FindVariable<Transform>(iceFlying.GetComponent<Blackboard>(), "from");
                    from.SetValue(symbol.transform);
                    Variable<Transform> to = BlackboardUtils.FindVariable<Transform>(iceFlying.GetComponent<Blackboard>(), "to");
                    var tile = tileAdmin.GetTileByAvailablePos(cell.GetVariable<int>("x").value, cell.GetVariable<int>("y").value);
                    flyingInfoList.Add(new IIPIceFlyingInfo { targetTile = tile, isMine = userIndex == 0 });
                    to.SetValue(tile.transform);
                    iceFlying.SetActive(true);
                    symbol.Play("Hide");
                    if (userIndex == 0)
                    {
                        iceFlying.GetComponent<Animator>().SetBool("Value", true);
                        yield return null;
                        playerValueFly = true;
                        iceFlying.GetComponentInChildren<TextMeshProUGUI>().text = FormatUtility.SimpleNumberFormat(communityGameAdmin.bonusBet);
                    }
                    else
                    {
                        iceFlying.GetComponent<Animator>().SetBool("Value", false);
                    }


                    if (treasureData != null)
                    {
                        if (userIndex == 0)
                        {
                            treasureMutliplier = treasureData.GetVariable<int>("multiplier").value;
                            treasureDirecting = true;
                            yield return new WaitUntil(() => treasureDirecting == false);
                            yield return new WaitForSeconds(0.5f);
                        }

                    }
                    else
                        yield return new WaitForSeconds(0.15f);
                }
            }

            yield return new WaitUntil(() => flyingInfoList.Count == 0 && playerValueFly == false);
            yield return new WaitForSeconds(0.5f);
        }
    }
}