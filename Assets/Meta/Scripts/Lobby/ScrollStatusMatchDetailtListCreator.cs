using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using BagelCode;
using BagelCode.ClientModels;

namespace SlotMaker
{

public class ScrollStatusMatchDetailtListCreator : MonoBehaviour
{
    public struct DetailCellInfo
    {
        public string textLeft;
        public string textRight;
        public string textComment;
        public string tierText;
        public int index;
        public bool hasButton;


        public DetailCellInfo(string textLeft, string textRight, string textComment, string tierText, int index, bool hasButton)
        {
            this.textLeft = textLeft;
            this.textRight = textRight;
            this.textComment = textComment;
            this.index = index;
            this.tierText = tierText;
            this.hasButton = hasButton;
        }
    }
    public GameObject rootContent;

    private const string STATUS_TEXT = "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_";
    private const string PROGRAM_TEXT = "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_PROGRAM_TEXT_";
    private const string STYLE_BRWON_TAG_TEXT = "<style=brown>";
    private const string TIER_STYLE_TEXT = "TEXT_TIER_STYLE";

    private List<DetailCellInfo>    cellInfoList;
    public  ObjectPool       cellPool;
    public  ObjectPool       benefitsPool;
    private int backIndex;

    public void Start()
    {
        backIndex = 0;
        SetupList();
        for (int i = 0; i < cellInfoList.Count; ++i)
        {
            PushBack();
        }
    }

    private void InitCellItem(int index, Blackboard bb)
    {
        bb.SetValue("Text Left", cellInfoList[index].textLeft);
        bb.SetValue("Text Right", cellInfoList[index].textRight);
        bb.SetValue("Text Comment", cellInfoList[index].textComment);
        bb.SetValue("Cell Animator Index", cellInfoList[index].index);
        bb.SetValue("Button Active", cellInfoList[index].hasButton);
    }

    private void SetupList()
    {
        cellInfoList = new List<DetailCellInfo>();

        Dictionary<StatusMatchVipProgram, List<int>> tierMap = GetOtherTierMap();

        StringBuilder stringTableKey = new StringBuilder();

        string savedToken = "";
        int programIndex = 0;
        int statusIndex = 0;

        string BLANK_TEXT = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_DETAIL_POPUP_CELL_BLANK_TEXT");
        string CLUBVEGAS_TEXT = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_DETAIL_POPUP_CELL_CLUBVEGAS_TEXT");
        string BENEFITS_TEXT = "STATUSMATCH_DETAIL_POPUP_CELL_BENEFITS_TEXT";

        // make celllist header
        string TITLE_TEXT = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_DETAIL_POPUP_CELL_HEADER_TITLE_TEXT");
        cellInfoList.Add(new DetailCellInfo(BLANK_TEXT, BLANK_TEXT, TITLE_TEXT, BLANK_TEXT, 2, false));

        string HEADER_TEXT = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_DETAIL_POPUP_CELL_HEADER_CONTENT_TEXT");
        cellInfoList.Add(new DetailCellInfo(BLANK_TEXT, BLANK_TEXT, HEADER_TEXT, BLANK_TEXT, 2, false));
        cellInfoList.Add(new DetailCellInfo(BLANK_TEXT, BLANK_TEXT, BLANK_TEXT, BLANK_TEXT, -1, false));
        cellInfoList.Add(new DetailCellInfo(BLANK_TEXT, BLANK_TEXT, BLANK_TEXT, BLANK_TEXT, -1, false));

        foreach (StatusMatchVipProgramStatus status in GetEnumValues<StatusMatchVipProgramStatus>().OrderBy(a => (int)a))
        {
            if ((int)status > 0)
            {
                string textLeft;
                string textRight;
                string textTierBenefits;

                string[] words = status.ToString().Split('_');
                if (!savedToken.Equals(words[0]))
                {
                    savedToken = words[0];
                    programIndex++;
                    statusIndex = 0;
                    stringTableKey.Append(PROGRAM_TEXT);
                    stringTableKey.Append(((StatusMatchVipProgram)programIndex).ToString());

                    textLeft = STYLE_BRWON_TAG_TEXT + StringTableUtils.GetString(StringTable.StringTableType.Global, stringTableKey.ToString());
                    textRight = STYLE_BRWON_TAG_TEXT + CLUBVEGAS_TEXT;

                    // divider for program, except first header
                    if ((int)status > 1)
                        cellInfoList.Add(new DetailCellInfo(BLANK_TEXT, BLANK_TEXT, BLANK_TEXT, BLANK_TEXT, -1, false));

                    cellInfoList.Add(new DetailCellInfo(textLeft, textRight, BLANK_TEXT, BLANK_TEXT, (int)status % 2, false));
                    stringTableKey.Length = 0;
                }

                int tier = tierMap[(StatusMatchVipProgram)programIndex][statusIndex++];
                // int tierGroup = TierUtils.GetTierGroup(tier);
                stringTableKey.Append(STATUS_TEXT);
                stringTableKey.Append(status.ToString());

                textLeft = StringTableUtils.GetString(StringTable.StringTableType.Global, stringTableKey.ToString());
                textRight = StringTableUtils.GetString(StringTable.StringTableType.Global, TIER_STYLE_TEXT, tier);

                double dailyBonus = TierUtils.GetTierMultiplier(tier);
                long timeBonus = TierUtils.GetTimeBonusCoins(tier);
                double purchaseBonus = TierUtils.GetTierMultiplier(tier);

                textTierBenefits = StringTableUtils.GetString(StringTable.StringTableType.Global, BENEFITS_TEXT, dailyBonus, timeBonus, purchaseBonus);

                cellInfoList.Add(new DetailCellInfo(textLeft, textRight, BLANK_TEXT, textTierBenefits, (int)status % 2, true));
                stringTableKey.Length = 0;
            }
        }

        string commentText = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_DETAIL_POPUP_CELL_COMMENT_TEXT");

        cellInfoList.Add(new DetailCellInfo(BLANK_TEXT, BLANK_TEXT, commentText, BLANK_TEXT, 2, false));
    }

    private GameObject PushBack(ObjectPool pool)
    {
        var go = pool.GetObject(false).gameObject;
        go.transform.SetParent(rootContent.transform, false);
        go.transform.SetAsLastSibling();
        return go;
    }

    protected void PushBack()
    {
        var go = PushBack(cellPool);
        InitCellItem(backIndex, go.GetComponent<Blackboard>());
        go.SetActive(true);

        var beneObj = PushBack(benefitsPool);
        beneObj.GetComponent<Blackboard>().SetValue("Text", cellInfoList[backIndex].tierText);
        go.GetComponent<Blackboard>().SetValue("Benefits Cell", beneObj);
        beneObj.SetActive(false);

        backIndex++;
    }

    private IEnumerable<T> GetEnumValues<T>()
    {
        return (T[])Enum.GetValues(typeof(T));
    }

    private Dictionary<StatusMatchVipProgram, List<int>> GetOtherTierMap()
    {
        Dictionary<StatusMatchVipProgram, List<int>> tierMap = new Dictionary<StatusMatchVipProgram, List<int>>();
        tierMap.Add(StatusMatchVipProgram.PLAYTIKA_REWARDS, new List<int>() {8, 15, 19, 20});
        tierMap.Add(StatusMatchVipProgram.LOYALTY_LOUNGE, new List<int>() {6, 10, 14, 17, 20, 20});
        tierMap.Add(StatusMatchVipProgram.HUUGE_CASINO, new List<int>() {6, 6, 6, 12, 12, 12, 15, 15, 15, 18});
        tierMap.Add(StatusMatchVipProgram.STAR_SPINS_VIP, new List<int>() {6, 10, 13, 15, 17, 19, 20});
        tierMap.Add(StatusMatchVipProgram.JACKPOTJOY_REWARDS, new List<int>() {6, 10, 13, 15, 17, 19, 20});
        tierMap.Add(StatusMatchVipProgram.OTHER, new List<int>() {-1});
        tierMap.Add(StatusMatchVipProgram.UNKNOWN, new List<int>() {-1});

        return tierMap;
    }

}

}

