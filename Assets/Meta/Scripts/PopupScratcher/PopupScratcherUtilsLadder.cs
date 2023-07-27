using System.Collections.Generic;
using SlotMaker;
using BagelCode.Scratcher;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using System.Linq;
using UnityEngine;
using System.Collections;

using static BagelCode.Scratcher.ScratcherCellPlayGroup;

using CellGroup = System.Collections.Generic.List<BagelCode.Scratcher.ScratcherCellController>;
using LadderExistRow = System.Collections.Generic.List<bool>;
using LadderObjRow = System.Collections.Generic.List<UnityEngine.GameObject>;

namespace BagelCode
{
    public static partial class PopupScratcherUtils
    {
        private static void InitScratcherPlayGroupsForLadder(List<ScratcherPlayGroup> scratcherPlayGroups, Blackboard scratcherInfo, CellGroup cellInstanceList, ContextElement agent, bool useBonus, bool useMulti)
        {
            var positionList = scratcherInfo.GetValue<List<bool>>("startPositionList");
            List<LadderExistRow> ladderRowList = scratcherInfo.GetValue<List<Blackboard>>("ladder_2dArray").Select(bb => bb.GetValue<List<bool>>("bars")).ToList();

            // Bonus
            var numInstantWin = scratcherInfo.GetVariable<int>("numInstantWin")?.value ?? 0;

            var bonusList = new List<LadderBonusBox>();
            var bonusInfoList = new List<BonusInfo>();

            void GetBonusList(string key)
            {
                var bonusInfoBbList = scratcherInfo.GetVariable<List<Blackboard>>(key)?.value;
                var getBonusInfoList = new List<BonusInfo>();
                bonusInfoBbList?.ForEach(bb =>
                {
                    var getBonusInfo = new BonusInfo()
                    {
                        isRow = bb.GetValue<bool>("isRow"),
                        xPos = bb.GetValue<int>("xPos"),
                        yPos = bb.GetValue<int>("yPos"),
                    };
                    if (!getBonusInfo.isRow) getBonusInfo.yPos++;
                    getBonusInfoList.Add(getBonusInfo);
                });

                bonusInfoList.AddRange(getBonusInfoList);
            }

            if (useBonus)
            {
                GetBonusList("bonusWinInfo");
                GetBonusList("bonusLoseInfo");
            }

            int row = positionList.Count;
            int column = ladderRowList.Count;

            /// Cell Grouping
            // cellInstanceList
            // PlayerCover, Prize, MiddleCover, BonusObj, PlayerObj
            int idx = 0;
            CellGroup playerCoverCellGroup = cellInstanceList.GetRange(0, row);
            idx += row;
            CellGroup prizeCellGroup = cellInstanceList.GetRange(idx, row);
            idx += row;
            var middleCoverCell = cellInstanceList[idx++];

            CellGroup bonusObjCellGroup = null;
            if (useBonus)
            {
                bonusObjCellGroup = cellInstanceList.GetRange(idx, bonusInfoList.Count);
                idx += bonusInfoList.Count;
            }

            ScratcherCellController multiplierCell = null;
            if (useMulti)
            {
                multiplierCell = cellInstanceList[idx++];
            }

            CellGroup playerObjCellGroup = cellInstanceList.GetRange(idx, row);
            ///

            var ladderPlayerList = new List<LadderPlayer>();
            var ladderObjRowList = new List<LadderObjRow>();

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string staticLadderAssetName = "Scratcher Ladder Static";

            // Set Static Ladder
            var staticArea = ContextUtils.FindElement(agent, string.Format("Horizontal Static Ladder Area"), CHILDREN);
            for (int i = 0; i < column; ++i)
            {
                var ladderRow = ladderRowList[i];
                var ladderObjRow = new LadderObjRow();
                for (int j = 0; j < row - 1; ++j)
                {
                    if (ladderRow[j])
                    {
                        var lineAnchor = ContextUtils.FindElement(staticArea, string.Format("Line {0}", j), CHILDREN);
                        var anchor = ContextUtils.FindElement(lineAnchor, i.ToString(), CHILDREN);
                        var obj = MetaObjectUtils.MakePrefab(bundle, staticLadderAssetName, anchor.transform);
                        ladderObjRow.Add(obj);
                    }
                }
                ladderObjRowList.Add(ladderObjRow);
            }

            // Make Player, Progress Ladders
            int playerCount = 0;
            for (int i = 0; i < row; ++i)
            {
                if (positionList[i])
                {
                    var ladderPlayer = new LadderPlayer(playerCount++, i, row, column);
                    ladderPlayer.SetPlayerObj(playerObjCellGroup[i]);
                    ladderPlayer.MakeProgressLadderRowList(ladderRowList, agent);
                    ladderPlayerList.Add(ladderPlayer);

                    playerCoverCellGroup[i].AddHighlightCellsSelf();
                }
            }

            // Make Bonus Box
            for (int i = 0; i < bonusInfoList.Count; ++i)
            {
                var bonus = bonusInfoList[i];
                var bonusCell = bonusObjCellGroup[i];

                LadderBonusBox bonusBox = new LadderBonusBox(bonus);
                bonusBox.SetBonusBox(agent, bonusCell);
                bonusList.Add(bonusBox);
            }
            var loseBonusList = bonusList;

            // Set PlayGroups

            // Open Prize Cell
            AddPlayGroup(scratcherPlayGroups, prizeCellGroup, NON_HIGHLIGHT_OPTION_SET);

            // Open Player Cell
            AddPlayGroup(scratcherPlayGroups, playerCoverCellGroup, DEFAULT_OPTION_SET);

            // Open Blind Cell
            var blindOpenPlayGroup = AddPlayGroup(scratcherPlayGroups, middleCoverCell, DEFAULT_OPTION_SET);
            blindOpenPlayGroup.isPlayNextInstantly = true;
            blindOpenPlayGroup.instantOpenDelay = 0.3f;

            // Show Player
            ladderPlayerList.ForEach(p =>
            {
                var showPlayerPlayGroup = AddPlayGroup(scratcherPlayGroups, () => p.ShowPlayerObj(agent), 0f);
                if (bonusList.Count > 0)
                    showPlayerPlayGroup.isPlayNextInstantly = true;
                else
                    showPlayerPlayGroup.isPlayNextInstantly = p != ladderPlayerList.Last();
                showPlayerPlayGroup.instantOpenDelay = 0f;
            });

            // Show Bonus
            bonusList.ForEach(b =>
            {
                var showBonusPlayGroup = AddPlayGroup(scratcherPlayGroups, () => b.ShowBonusBox(agent, row), 0f);
                showBonusPlayGroup.isPlayNextInstantly = b != bonusList.Last();
                showBonusPlayGroup.instantOpenDelay = 0f;
            });

            bool isLastPlayGroup = bonusList.Count == 0 && !useMulti;
            // Move Player
            foreach (var player in ladderPlayerList)
            {
                var arrivedPosX = player.progressLadderInfoList.Last().x;
                var winPrizeCell = prizeCellGroup[arrivedPosX];
                winPrizeCell.AddHighlightCellsSelf();

                // Make Play Group
                ScratcherPlayGroup playerMovementPlayGroup = AddPlayGroup(
                    scratcherPlayGroups,
                    () => agent.StartCoroutine(
                        player.MovementCoroutine(
                        agent, ladderPlayerList, bonusList, winPrizeCell, isLastPlayGroup)),
                    0f);

                player.currentPlayGroup = playerMovementPlayGroup;

                bool isLastPlayer = player == ladderPlayerList.Last();
                playerMovementPlayGroup.SetEventName(
                    isLastPlayer ? ScratcherPlayGroup.EventType.NONE : ScratcherPlayGroup.EventType.PLAY_NEXT);
            }

            // Lose Bonus
            bonusList.ForEach(b =>
            {
                var loseBonusPlayGroup = AddPlayGroup(scratcherPlayGroups, () => b.Lose(), 0f);
                loseBonusPlayGroup.isPlayNextInstantly = b != bonusList.Last();
                loseBonusPlayGroup.instantOpenDelay = 0f;
            });

            // Multiplier
            if (useMulti)
            {
                AddHighlightToCell(multiplierCell);
                AddPlayGroup(scratcherPlayGroups, multiplierCell, DEFAULT_OPTION_SET);
            }
        }
    }

    public class LadderPlayer
    {
        public const float V_MOVING_TIME_MAX = 0.8f;
        public const float V_MOVING_TIME_SHORT = V_MOVING_TIME_MAX * 2f / 3f;
        public const float H_MOVING_TIME_MAX = 2f;
        public const float DELAY_AFTER_MOVEMENT = 0.2f;

        public GameObject playerObj;
        public int startX;

        public bool isArrived = false;

        public List<LadderInfo> progressLadderInfoList = new List<LadderInfo>();
        public List<Transform> playerPositionList = new List<Transform>();

        public ScratcherPlayGroup currentPlayGroup;

        private int playerIndex;
        private int row;
        private int column;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public List<Color> ladderColorList = new List<Color>()
        {
            new Color(1f, 0.99f, 0.02f),
            new Color(1f, 0.73f, 0.02f),
            new Color(1f, 0.51f, 0.02f),
            new Color(1f, 0.28f, 0.09f),
            new Color(0.53f, 1f, 0.02f),
            new Color(0f, 0.94f, 0.15f),
            new Color(0f, 0.73f, 0.39f),
            new Color(0f, 0.87f, 1f),
        };

        public class LadderInfo
        {
            public LadderInfo(GameObject _ladderObj, int _x, int _y, bool _isRow, bool _isShort, bool _isLeftToRight)
            {
                ladderObj = _ladderObj;
                x = _x;
                y = _y;
                isRow = _isRow;
                isShort = _isShort;
                isLeftToRight = _isLeftToRight;
            }

            public GameObject ladderObj;
            public int x;
            public int y;
            public bool isRow;
            public bool isShort;
            public bool isLeftToRight;
        }

        public LadderPlayer(int _playerIndex, int _startX, int _row, int _column)
        {
            playerIndex = _playerIndex;
            startX = _startX;
            row = _row;
            column = _column;
        }

        public void ShowPlayerObj(MonoBehaviour agent)
        {
            Vector3 from = new Vector3(0.3f, 0.3f);
            Vector3 to = new Vector3(1f, 1f);

            float scaleFactor = 1f - System.Math.Max(1, row - 5) * 0.05f;
            from *= scaleFactor;
            to *= scaleFactor;

            playerObj.transform.localScale = from;
            AsyncActionUtils.ApplyScaling(agent, playerObj.transform, from, to, 0.2f, TweenUtils.VectorTweenOutSine);

            playerObj.SetActive(true);
        }

        public void SetPlayerObj(ScratcherCellController cell)
        {
            playerObj = cell.gameObject;
        }

        public void MakeProgressLadderRowList(List<LadderExistRow> ladderRowList, ContextElement agent)
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string vAssetName = "Scratcher Ladder Progress Vertical";
            string hAssetName = "Scratcher Ladder Progress Horizontal";

            var playerAnchor = ContextUtils.FindElement(agent, string.Format("Scratcher Area {0}", startX), FULL);
            var vAnchor = ContextUtils.FindElement(agent, string.Format("Vertical Ladder Area"), CHILDREN);
            var hAnchor = ContextUtils.FindElement(agent, string.Format("Horizontal Ladder Area"), CHILDREN);
            var ladderProgressArea = ContextUtils.FindElement(agent, "Ladder Object Area/Ladder Progress Area", FULL);
            var ladderPlayerPositionArea = ContextUtils.FindElement(agent, "Ladder Object Area/Player Position Area", FULL);

            Color ladderColor = ladderColorList[playerIndex];

            GameObject MakeProgressLadder(bool isRow, int _x, int _y, bool isLeftToRight)
            {
                var ladderAnchor = isRow ? hAnchor : vAnchor;
                string asset = isRow ? hAssetName : vAssetName;

                var lineAnchor = ContextUtils.FindElement(ladderAnchor, string.Format("Line {0}", _x), CHILDREN);
                var anchor = ContextUtils.FindElement(lineAnchor, _y.ToString(), CHILDREN);
                var progressAnchor = ContextUtils.FindElement(anchor, "Progress Area", CHILDREN);

                var obj = MetaObjectUtils.MakePrefab(bundle, asset, progressAnchor.transform);

                // Parent
                obj.transform.SetParent(ladderProgressArea.transform);

                // Color
                obj.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = ladderColor;

                if (isRow && !isLeftToRight) obj.transform.localEulerAngles = new Vector3(0f, 0f, 180f);

                return obj;
            }

            Transform MakePointArea(Vector2 _pos)
            {
                string asset = "Scratcher Ladder Point Movement Anchor";
                var obj = MetaObjectUtils.MakePrefab(bundle, asset, ladderPlayerPositionArea.transform);
                obj.transform.position = _pos;

                return obj.transform;
            }

            // ex)
            // I-I-I-I
            // I-I-I-I
            // I-I-I-I
            // I-I-I-I
            // I I I I

            Vector2 pos = playerAnchor.gameObject.transform.position;
            playerPositionList.Add(MakePointArea(pos));

            int x = startX;
            bool ladderExistLeft = false;
            bool ladderExistRight = false;

            for (int y = 0; y < column; ++y)
            {
                {
                    var ladderObj = MakeProgressLadder(false, x, y, false);
                    var ladder = new LadderInfo(ladderObj, x, y, false, false, false);
                    progressLadderInfoList.Add(ladder);

                    if (ladderExistLeft || ladderExistRight)
                    {
                        pos.x = ladderObj.transform.position.x;
                        playerPositionList.Add(MakePointArea(pos));
                    }

                    pos.y = ladderObj.transform.position.y;
                    playerPositionList.Add(MakePointArea(pos));
                }

                LadderExistRow ladderRow = ladderRowList[y];
                ladderExistLeft = (x > 0) && ladderRow[x - 1];
                ladderExistRight = (x < row - 1) && ladderRow[x];

                // Move Left
                if (ladderExistLeft)
                {
                    --x;

                    var ladderObj = MakeProgressLadder(true, x, y, false);
                    var ladder = new LadderInfo(ladderObj, x, y, true, false, false);
                    progressLadderInfoList.Add(ladder);
                }
                // Move Right
                else if (ladderExistRight)
                {
                    var ladderObj = MakeProgressLadder(true, x, y, true);
                    var ladder = new LadderInfo(ladderObj, x, y, true, false, true);
                    progressLadderInfoList.Add(ladder);

                    ++x;
                }

                // Arrive
                if (y == column - 1)
                {
                    ++y;

                    var ladderObj = MakeProgressLadder(false, x, y, false);
                    var ladder = new LadderInfo(ladderObj, x, y, false, true, false);
                    progressLadderInfoList.Add(ladder);

                    if (ladderExistLeft || ladderExistRight)
                    {
                        pos.x = ladderObj.transform.position.x;
                        playerPositionList.Add(MakePointArea(pos));
                    }

                    pos.y = ladderObj.transform.position.y;
                    playerPositionList.Add(MakePointArea(pos));
                }
            }
        }

        public IEnumerator MovementCoroutine(ContextElement agent, List<LadderPlayer> ladderPlayerList, List<LadderBonusBox> bonusList, ScratcherCellController prizeCell, bool isLastPlayGroup)
        {
            Vector3 TO_SCALE = new Vector3(1f, 1f);

            float V_MOVING_TIME = V_MOVING_TIME_MAX / column;
            float V_MOVING_TIME_HALF = V_MOVING_TIME * 0.5f;
            float H_MOVING_TIME = H_MOVING_TIME_MAX / (row - 1);
            float BONUS_OPEN_TIMING = 0.5f;

            RectTransform playerTransform = (RectTransform)playerObj.transform;
            var playerMovingLadderList = progressLadderInfoList;

            Transform ladderBaseTransform;
            bool isShort, isRow;
            int x, y;

            GSManager.Instance.GetHandler("Collecting_Game_Scratcher_Roller_Start").Play();

            int moveCount = playerMovingLadderList.Count;
            for (int i = 0; i < moveCount; ++i)
            {
                var movingLadder = playerMovingLadderList[i];
                var pos = playerPositionList[i];
                var toPos = playerPositionList[i + 1];

                isShort = movingLadder.isShort;
                isRow = movingLadder.isRow;
                x = movingLadder.x;
                y = movingLadder.y;

                ladderBaseTransform = movingLadder.ladderObj.transform.GetChild(0);

                // Scaling
                Vector3 baseScale = ladderBaseTransform.localScale;
                float movingTime = isRow ? H_MOVING_TIME : isShort ? V_MOVING_TIME_HALF : V_MOVING_TIME;
                float startDelay = i == 0 ? 0.2f : 0f;
                AsyncActionUtils.ApplyScaling(
                    agent, ladderBaseTransform, baseScale, TO_SCALE, movingTime, TweenUtils.VectorTweenLinear, startDelay);

                // Bonus
                foreach (var bonus in bonusList)
                {
                    if (!bonus.opened)
                    {
                        var bonusInfo = bonus.bonusInfo;
                        if (bonusInfo.isRow == isRow && x == bonusInfo.xPos && y == bonusInfo.yPos)
                        {
                            // Open
                            AsyncActionUtils.DelayedAction(agent, movingTime * BONUS_OPEN_TIMING, bonus.Open);

                            bonus.opened = true;
                            break;
                        }
                    }
                }

                // Move
                yield return AsyncActionUtils.MoveCoroutine(
                        agent, playerTransform, pos, toPos, movingTime, TweenUtils.VectorTweenLinear, 0f, null);
            }

            yield return new WaitForSeconds(DELAY_AFTER_MOVEMENT);

            var playerElement = playerObj.GetComponent<ContextElement>();
            playerElement.UpdateContext();

            var particleAnchorElement = ContextUtils.FindElement(playerElement, "FX Trail Anchor", ContextSearchingType.ChildrenSearch);
            particleAnchorElement.gameObject.SetActive(false);

            yield return agent.StartCoroutine(prizeCell.Open(true, ScratcherCellPlayGroup.ONLY_HIGHLIGHT_OPTION_SET));

            isArrived = true;

            ScratcherPlayController scratcherPlayController = agent.GetComponent<ScratcherPlayController>();
            int arrivedCount = ladderPlayerList.Count(p => p.isArrived);
            bool isLastPlayer = arrivedCount == ladderPlayerList.Count;
            if (isLastPlayer)
            {
                if (isLastPlayGroup)
                {
                    currentPlayGroup.SetEventName(ScratcherPlayGroup.EventType.FINISH_SCRATCHER);
                }
                else
                {
                    currentPlayGroup.SetEventName(ScratcherPlayGroup.EventType.PLAY_NEXT);
                }

                currentPlayGroup.SendEvent(0f, scratcherPlayController);
            }
        }
    }

    public class LadderBonusBox
    {
        public GameObject bonusObj;
        public BonusInfo bonusInfo;

        public bool opened = false;

        private Animator bonusAnim;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        public LadderBonusBox(BonusInfo _bonusInfo)
        {
            bonusInfo = _bonusInfo;
        }

        public void Open()
        {
            GSManager.Instance.GetHandler("Collecting_Game_Scratcher_Highlight").Play();
            bonusAnim.SetBool("Active", true);

            opened = true;
        }

        public void Lose()
        {
            if (!opened)
                bonusAnim.SetBool("ActiveNon", true);
        }

        public void ShowBonusBox(MonoBehaviour agent, int row)
        {
            Vector3 from = new Vector3(0.3f, 0.3f);
            Vector3 to = new Vector3(1f, 1f);

            float scaleFactor = 1f - System.Math.Max(1, row - 5) * 0.1f;
            from *= scaleFactor;
            to *= scaleFactor;

            bonusObj.transform.localScale = from;
            AsyncActionUtils.ApplyScaling(agent, bonusObj.transform, from, to, 0.3f, TweenUtils.VectorTweenOutSine);

            bonusObj.SetActive(true);
        }

        public void SetBonusBox(ContextElement agent, ScratcherCellController cell)
        {
            bonusObj = cell.gameObject;

            var ladderAnchor = bonusInfo.isRow ?
             ContextUtils.FindElement(agent, string.Format("Horizontal Ladder Area"), CHILDREN) :
             ContextUtils.FindElement(agent, string.Format("Vertical Ladder Area"), CHILDREN);

            ContextElement lineAnchor = ContextUtils.FindElement(ladderAnchor, string.Format("Line {0}", bonusInfo.xPos), CHILDREN);
            var anchor = ContextUtils.FindElement(lineAnchor, bonusInfo.yPos.ToString(), CHILDREN);
            var bonusAnchor = ContextUtils.FindElement(anchor, "Bonus Area", CHILDREN);

            bonusAnim = bonusObj.GetComponent<Animator>();
            bonusObj.transform.position = bonusAnchor.transform.position;
        }
    }
}
