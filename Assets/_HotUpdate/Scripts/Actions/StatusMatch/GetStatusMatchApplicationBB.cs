using System;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/StatusMatch")]

public class GetStatusMatchApplicationBB : ActionTask<Blackboard>
{
    public BBParameter<List<string>> vipProgramTextList;
    public BBParameter<int> vipProgramIndex;
    public BBParameter<List<string>> vipAppTextList;
    public BBParameter<int> vipAppIndex;
    public BBParameter<bool> vipAppInputBool;
    public BBParameter<bool> vipAppDropdownBool;
    public BBParameter<List<string>> vipStatusTextList;
    public BBParameter<int> vipStatusIndex;
    public BBParameter<bool> vipStatusInputBool;
    public BBParameter<bool> vipStatusDropdownBool;
    public BBParameter<string> vipIdText;
    public BBParameter<string> vipTipText;
    public BBParameter<string> vipCautionText;
    public BBParameter<bool> vipAppDropdownInteractiveBool;
    public BBParameter<bool> vipStatusDropdownInteractiveBool;


    protected override string info
    {
        get { return "Get StatusMatch Application BB"; }
    }

    protected override void OnExecute()
    {
        List<string> vipProgramList = new List<string>(6);
        List<string> vipAppList = new List<string>(5);
        List<string> vipStatusList = new List<string>(11);

        vipProgramList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_PROGRAM_TEXT"));

        StringBuilder stringTableKey = new StringBuilder();
        string LoadedString = "";

        // setup programList
        foreach (StatusMatchVipProgram program in GetEnumValues<StatusMatchVipProgram>().OrderByDescending(a => (int)a))
        {
            stringTableKey.Append("STATUSMATCH_APPLICATION_FORM_SELECT_VIP_PROGRAM_TEXT");

            if ((int)program > -1)
            {
                stringTableKey.Append("_").Append(program.ToString());
                LoadedString = StringTableUtils.GetString(StringTable.StringTableType.Global, stringTableKey.ToString());

                if ((int)program > 0)
                    vipProgramList.Insert(1, LoadedString);
                else
                    vipProgramList.Add(LoadedString);
            }

            stringTableKey.Length = 0;
        }

        // setup applist & status list
        // sorted index
        // unkwon = 0, playtika = 1, loyalty = 2, huge = 3, ss = 4, jj =5, other =6
        // ... incremental
        switch (vipProgramIndex.value)
        {
            case (int)StatusMatchVipProgram.PLAYTIKA_REWARDS:
                SetupInteractableUIState(StatusMatchVipProgram.PLAYTIKA_REWARDS);
                vipAppList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_APP_TEXT_CAESARS_CASINO"));
                vipAppList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_APP_TEXT_SLOTOMANIA"));
                vipAppList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_APP_TEXT_HOUSE_OF_FUN"));

                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_PLAYTIKA_REWARDS_PLATINUM"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_PLAYTIKA_REWARDS_DIAMOND"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_PLAYTIKA_REWARDS_ROYAL_DIAMOND"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_PLAYTIKA_REWARDS_BLACK_DIAMOND"));

                switch (vipAppIndex.value)
                {
                    // case (int)StatusMatchVipProgramApp.CAESARS_CASINO:
                    case 0:
                        vipIdText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_ID_TEXT");
                        vipTipText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_TIP_TEXT_CAESARS_CASINO");
                        vipCautionText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_ATTENTION_TEXT_CAESARS_CASINO");
                    break;
                    // case (int)StatusMatchVipProgramApp.SLOTOMANIA:
                    case 1:
                        vipIdText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_ID_TEXT_NOT_REQUIRED");
                        vipTipText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_TIP_TEXT_SLOTOMANIA");
                        vipCautionText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_ATTENTION_TEXT_SLOTOMANIA");
                    break;
                    // case (int)StatusMatchVipProgramApp.HOUSE_OF_FUN:
                    case 2:
                        vipIdText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_ID_TEXT");
                        vipTipText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_TIP_TEXT_HOUSE_OF_FUN");
                        vipCautionText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_ATTENTION_TEXT_HOUSE_OF_FUN");
                    break;
                }

            break;
            case (int)StatusMatchVipProgram.LOYALTY_LOUNGE:
                SetupInteractableUIState(StatusMatchVipProgram.LOYALTY_LOUNGE);
                vipAppList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_APP_TEXT_HIT_IT_RICH"));
                vipAppList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_APP_TEXT_WIZARD_OF_OZ"));
                vipAppList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_APP_TEXT_WILLK_WONKA_SLOTS"));
                vipAppList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_APP_TEXT_BLACK_DIAMOND"));

                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_LOYALTY_LOUNGE_PLATINUM"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_LOYALTY_LOUNGE_RUBY"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_LOYALTY_LOUNGE_DIAMOND"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_LOYALTY_LOUNGE_BLACK_DIAMOND"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_LOYALTY_LOUNGE_YELLOW_DIAMOND"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_LOYALTY_LOUNGE_BLUE_DIAMOND"));

                switch (vipAppIndex.value)
                {
                    // case (int)StatusMatchVipProgramApp.HIT_IT_RICH:
                    case 0:
                        vipIdText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_ID_TEXT");
                        vipTipText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_TIP_TEXT_HIT_IT_RICH");
                        vipCautionText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_ATTENTION_TEXT_HIT_IT_RICH");
                    break;
                    // case (int)StatusMatchVipProgramApp.WIZARD_OF_OZ:
                    case 1:
                        vipIdText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_ID_TEXT");
                        vipTipText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_TIP_TEXT_WIZARD_OF_OZ");
                        vipCautionText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_ATTENTION_TEXT_WIZARD_OF_OZ");
                    break;
                    // case (int)StatusMatchVipProgramApp.WILLK_WONKA_SLOTS:
                    case 2:
                        vipIdText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_ID_TEXT");
                        vipTipText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_TIP_TEXT_WILLK_WONKA_SLOTS");
                        vipCautionText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_ATTENTION_TEXT_WILLK_WONKA_SLOTS");
                    break;
                    // case (int)StatusMatchVipProgramApp.BLACK_DIAMOND:
                    case 3:
                        vipIdText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_ID_TEXT");
                        vipTipText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_TIP_TEXT_BLACK_DIAMOND");
                        vipCautionText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_ATTENTION_TEXT_BLACK_DIAMOND");
                    break;
                }

            break;
            case (int)StatusMatchVipProgram.HUUGE_CASINO:
                SetupInteractableUIState(StatusMatchVipProgram.HUUGE_CASINO);
                vipAppList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_APP_TEXT_HUUGE_CASINO"));

                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_HUUGE_CASINO_GOLD_I"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_HUUGE_CASINO_GOLD_II"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_HUUGE_CASINO_GOLD_III"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_HUUGE_CASINO_PLATINUM_I"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_HUUGE_CASINO_PLATINUM_II"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_HUUGE_CASINO_PLATINUM_III"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_HUUGE_CASINO_DIAMOND_I"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_HUUGE_CASINO_DIAMOND_II"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_HUUGE_CASINO_DIAMOND_III"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_HUUGE_CASINO_MASTER"));

                switch (vipAppIndex.value)
                {
                    // case (int)StatusMatchVipProgramApp.HUUGE_CASINO:
                    case 0:
                        vipIdText.value  = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_ID_TEXT");
                        vipTipText.value  = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_TIP_TEXT_HUUGE_CASINO");
                        vipCautionText.value  = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_ATTENTION_TEXT_HUUGE_CASINO");
                    break;
                }

            break;
            case (int)StatusMatchVipProgram.STAR_SPINS_VIP:
                SetupInteractableUIState(StatusMatchVipProgram.STAR_SPINS_VIP);
                vipAppList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_APP_TEXT_STAR_SPINS"));

                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_STAR_SPINS_VIP_GOLD"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_STAR_SPINS_VIP_PLATINUM"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_STAR_SPINS_VIP_SAPPHIRE"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_STAR_SPINS_VIP_EMERALD"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_STAR_SPINS_VIP_DIAMOND"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_STAR_SPINS_VIP_ELITE"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_STAR_SPINS_VIP_ROYAL"));

                switch (vipAppIndex.value)
                {
                    // case (int)StatusMatchVipProgramApp.STAR_SPINS:
                    case 0:
                        vipIdText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_ID_TEXT");
                        vipTipText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_TIP_TEXT_STAR_SPINS");
                        vipCautionText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_ATTENTION_TEXT_STAR_SPINS");
                    break;
                }

            break;
            case (int)StatusMatchVipProgram.JACKPOTJOY_REWARDS:
                SetupInteractableUIState(StatusMatchVipProgram.JACKPOTJOY_REWARDS);
                vipAppList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_APP_TEXT_JACKPOTJOY_SLOTS"));

                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_JACKPOTJOY_REWARDS_GOLD"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_JACKPOTJOY_REWARDS_PLATINUM"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_JACKPOTJOY_REWARDS_SAPPHIRE"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_JACKPOTJOY_REWARDS_EMERALD"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_JACKPOTJOY_REWARDS_DIAMOND"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_JACKPOTJOY_REWARDS_ELITE"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT_JACKPOTJOY_REWARDS_ROYAL"));

                switch (vipAppIndex.value)
                {
                    // case (int)StatusMatchVipProgramApp.JACKPOTJOY_SLOTS:
                    case 0:
                        vipIdText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_ID_TEXT");
                        vipTipText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_TIP_TEXT_JACKPOTJOY_SLOTS");
                        vipCautionText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_ATTENTION_TEXT_STAR_SPINS");
                    break;
                }

            break;
            case (int)StatusMatchVipProgram.OTHER: // default case. because of UI sorting
                SetupInteractableUIState(StatusMatchVipProgram.UNKNOWN);

                vipAppList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_APP_TEXT"));
                vipStatusList.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_STATUS_TEXT"));

                vipIdText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_ID_TEXT");
                vipTipText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_TIP_TEXT");
                vipCautionText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_ATTENTION_TEXT");
            break;

            default: // OTHER case. because of UI sorting
                SetupInteractableUIState(StatusMatchVipProgram.OTHER);

                vipIdText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_ID_TEXT");
                vipTipText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_SELECT_VIP_TIP_TEXT_OTHER");
                vipCautionText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "STATUSMATCH_APPLICATION_FORM_ATTENTION_TEXT_OTHER");
            break;

        }

        vipProgramTextList.value = vipProgramList;
        vipAppTextList.value = vipAppList;
        vipStatusTextList.value = vipStatusList;

        EndAction();
    }

    private void SetupInteractableUIState(StatusMatchVipProgram program)
    {
        if (program == StatusMatchVipProgram.OTHER)
        {
            vipAppInputBool.value       = true;
            vipAppDropdownBool.value    = false;
            vipStatusInputBool.value    = true;
            vipStatusDropdownBool.value = false;
            vipAppDropdownInteractiveBool.value = true;
            vipStatusDropdownInteractiveBool.value = true;
        }
        else if (program == StatusMatchVipProgram.UNKNOWN) // default case
        {
            vipAppInputBool.value       = false;
            vipAppDropdownBool.value    = true;
            vipStatusInputBool.value    = false;
            vipStatusDropdownBool.value = true;
            vipAppDropdownInteractiveBool.value = false;
            vipStatusDropdownInteractiveBool.value = false;
        }
        else
        {
            vipAppInputBool.value       = false;
            vipAppDropdownBool.value    = true;
            vipStatusInputBool.value    = false;
            vipStatusDropdownBool.value = true;
            vipAppDropdownInteractiveBool.value = true;
            vipStatusDropdownInteractiveBool.value = true;
        }
    }

    private IEnumerable<T> GetEnumValues<T>()
    {
        return (T[])Enum.GetValues(typeof(T));
    }


}

}
