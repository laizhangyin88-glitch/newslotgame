using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class VipFreeDealSelectItemController : VipDealV2SelectItemControllerBase
    {
        public IEnumerator PlayFreeEffectCoroutine()
        {
            anim.SetTrigger("FreeChange");

            yield return new WaitForSeconds(0.5f); // 기존에는 Anim으로 이벤트 받았음

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Text", "VIP_DEAL_ITEM_FREE", CHILDREN);

            yield return new WaitForSeconds(1f);

            anim.SetBool("FreeSelectOn", true);
            GSManager.Instance.GetHandler(VipDealV2.Defines.SOUND_FREE_ITEM_DECIDED).Play();

            EventSender.SendCalleeCallback(gameObject, VipDealV2.Events.ON_SET_FREE_COMPLETE);
        }

        protected override string GetSelectCellText(Blackboard dealInfo)
        {
            long originCredit = BlackboardUtils.FindValue<long>(dealInfo, "creditAmount");
            long earnCredit = VipDealV2.Utils.GetFreeDealCreditMultiplierValue(originCredit);
            return StringTableUtils.GetString(GLOBAL, "VIP_DEAL_V2_FREE_SELECT_CELL_TEXT_FORMAT", earnCredit);
        }

        protected override long GetDealTotalCredit(Blackboard dealInfo)
        {
            long originCredit = BlackboardUtils.FindValue<long>(dealInfo, "creditAmount");
            long earnCredit = VipDealV2.Utils.GetFreeDealCreditMultiplierValue(originCredit);

            long multiplierNumerator = BlackboardUtils.FindValue<long>(dealInfo, "multiplierNumerator");
            return NumberUtils.GetMultiplierNumeratorValue(earnCredit, multiplierNumerator);
        }
    }
}
