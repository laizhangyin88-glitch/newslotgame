using System.Collections;
using System.Collections.Generic;
using GameStudio.Slot.IIP.Utility;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
namespace GameStudio.Slot.IIP.Feature
{
    public class IIPMovePenguinsAdmin : FeatureController
    {
        protected override string ON_FEATURE_BEGIN_EVENT => "IIP_MOVE_PENGUINS";
        protected override string ON_FEATURE_END_EVENT => "IIP_FINISH_MOVE_PENGUINS";

        public ObjectPool penguinFlyingPool;
        [SerializeField] private Transform penguinFlyingTarget;
        public IIPCommunityGameTileAdmin tileAdmin;
        public IIPCommunityGameUIAdmin uiAdmin;
        public IIPCommunityGameAdmin communityGameAdmin;
        public List<IIPCommunityGamePenguin> penguinList = new List<IIPCommunityGamePenguin>();

        public int savedPenguinCount { get; private set; }
        private int currentTurnArrivePenguinCount = 0;
        private void Awake()
        {
            RegisterEvent("IIPInitializeCommunityGame", InitialzieBonusGame);
            RegisterEvent("IIPPenguinFlyingArrive", OnArrivePenguinFlying);
        }

        private void InitialzieBonusGame(EventData eventData)
        {
            penguinList.Clear();
            savedPenguinCount = 0;
            currentTurnArrivePenguinCount = 0;
        }

        private void OnArrivePenguinFlying(EventData eventData)
        {
            uiAdmin.penguinGaugeAdmin.ChargeGaugeAmount(1f);
            IIPUtility.PlaySound("Penguin Collect");
            currentTurnArrivePenguinCount++;
        }

        private void OnArrivePenguin(IIPCommunityGamePenguin arrivedPenguin)
        {
            arrivedPenguin.Arrive();
            savedPenguinCount++;
            FlyingPenguin(arrivedPenguin.transform);
        }


        private void FlyingPenguin(Transform from)
        {
            Transform to = penguinFlyingTarget;

            GameObject treasureFlying = penguinFlyingPool.GetObject(true).gameObject;
            treasureFlying.SetActive(false);
            treasureFlying.transform.position = from.position;

            var penguinFlyingBB = treasureFlying.GetComponent<Blackboard>();

            penguinFlyingBB.GetVariable<Transform>("from").SetValue(from);
            penguinFlyingBB.GetVariable<Transform>("to").SetValue(to);

            treasureFlying.SetActive(true);
        }

        protected override IEnumerator OnPlayCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            List<Blackboard> userGameList = BlackboardUtils.FindVariable<List<Blackboard>>("./bonus/response/userGameResultList").value;
            int spinIndex = BlackboardUtils.FindVariable<int>("./bonus/spinCount").value - 1;
            List<Blackboard> penguinMoveData = null;
            currentTurnArrivePenguinCount = 0;
            //Find Latest Data
            for (int userIndex = 0; userIndex < userGameList.Count; userIndex++)
            {
                List<Blackboard> spinResultList = userGameList[userIndex].GetVariable<List<Blackboard>>("spinResultList").value;
                Blackboard currentSpinResult = spinResultList[spinIndex];
                var userPenguinMoveData = currentSpinResult.GetVariable<List<Blackboard>>("penguinLinkData")?.value;
                if (userPenguinMoveData == null) break;
                if (penguinMoveData == null || userPenguinMoveData.Count > penguinMoveData.Count) penguinMoveData = userPenguinMoveData;
            }
            if (penguinMoveData != null)
            {
                penguinMoveData.Sort((Blackboard prev, Blackboard next) => prev.GetVariable<List<Blackboard>>("value").value.Count.CompareTo(next.GetVariable<List<Blackboard>>("value").value.Count));
                int startPenguinCount = savedPenguinCount;
                for (int i = 0; i < penguinMoveData.Count; i++)
                {
                    List<Blackboard> path = penguinMoveData[i].GetVariable<List<Blackboard>>("value").value;
                    IIPCommunityGameTile targetPenguinTile = tileAdmin.GetTileByAvailablePos(path[0].GetVariable<int>("x").value, path[0].GetVariable<int>("y").value);
                    penguinList.Add(targetPenguinTile.penguin);
                    StartCoroutine(MovePegnuinByPath(targetPenguinTile.penguin, path));
                    if (startPenguinCount + i + 1 >= uiAdmin.penguinGaugeAdmin.goalPointCondition[communityGameAdmin.currentLevel])
                    {
                        int index = uiAdmin.penguinGaugeAdmin.currentLevelIndex;
                        yield return new WaitUntil(() => startPenguinCount + currentTurnArrivePenguinCount == uiAdmin.penguinGaugeAdmin.goalPointCondition[index]);
                        yield return new WaitForSeconds(1f);
                        yield return communityGameAdmin.UpdateLevelCoroutine();
                    }
                    else yield return new WaitForSeconds(0.75f);
                }
                yield return new WaitUntil(() => { return currentTurnArrivePenguinCount == penguinMoveData.Count; });
                currentTurnArrivePenguinCount = 0;
                penguinMoveData.Clear();
                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator MovePegnuinByPath(IIPCommunityGamePenguin targetPenguin, List<Blackboard> path)
        {
            float penguinMoveDurationPerTile = 0.23333f;
            foreach (var targetTile in path)
            {
                tileAdmin.GetTileByAvailablePos(targetTile.GetVariable<int>("x").value, targetTile.GetVariable<int>("y").value).AddGlowStatus(1);
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.4f);
            tileAdmin.GetTileByAvailablePos(path[0].GetVariable<int>("x").value, path[0].GetVariable<int>("y").value).AddGlowStatus(-1);
            path.RemoveAt(0);
            foreach (var targetTile in path)
            {
                yield return StartCoroutine(MovePenguinToTile(targetPenguin, tileAdmin.GetTileByAvailablePos(targetTile.GetVariable<int>("x").value, targetTile.GetVariable<int>("y").value).penguinAnchor.position, penguinMoveDurationPerTile));
                tileAdmin.GetTileByAvailablePos(targetTile.GetVariable<int>("x").value, targetTile.GetVariable<int>("y").value).AddGlowStatus(-1);
                yield return new WaitForSeconds(0.05f);
            }
            targetPenguin.SetIsWalking(false);
            targetPenguin.SetDirection(0);
            yield return new WaitForSeconds(0.1f);
            OnArrivePenguin(targetPenguin);
        }

        private IEnumerator MovePenguinToTile(IIPCommunityGamePenguin targetPenguin, Vector3 to, float duration)
        {
            Vector3 from = targetPenguin.transform.position;
            to.z = 0;
            from.z = 0;

            Vector3 direction = to - from;
            direction.Normalize();
            float degree = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            int normalizedDirection = 0;
            if (degree < -120 || degree > 120) normalizedDirection = -1;
            else if (degree < 60 && degree > -60) normalizedDirection = 1;

            bool isFront = true;
            if (degree > 0) isFront = false;

            targetPenguin.SetDirection(normalizedDirection);
            targetPenguin.SetIsFront(isFront);
            targetPenguin.SetIsWalking(true);
            targetPenguin.Jump();

            yield return new WaitForSeconds(0.033f);
            float startTime = Time.time;

            // jumpCurve
            while (Time.time - startTime < duration)
            {
                Vector3 xyz = Vector3.Lerp(from, to, (Time.time - startTime) / duration);
                targetPenguin.transform.position = new Vector3(xyz.x, xyz.y, targetPenguin.transform.position.z);
                yield return null;
            }
            targetPenguin.transform.position = new Vector3(to.x, to.y, targetPenguin.transform.position.z);
        }
    }
}