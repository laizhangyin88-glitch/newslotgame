using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using TMPro;
using ParadoxNotion;

namespace GameStudio.Slot.FSF
{
    public class FSFSpinCountUIAdmin : FeatureController
    {
        [SerializeField]
        private TextMeshProUGUI spinCountUI;

        public const string EXTRA_SPINS_TEXT_FORMAT = "{0} SPINS";
        public const string EXTRA_SPIN_TEXT_FORMAT = "{0} SPIN";


        protected override string ON_FEATURE_BEGIN_EVENT { get => "RefreshSpinCount"; }
        protected override string ON_FEATURE_END_EVENT { get => "EndRefreshSpinCount"; }

        protected override IEnumerator OnPlayCoroutine()
        {
            var remainSpinCount = BlackboardUtils.FindVariable<int>(null, "./bonus/totalSpinCount").value - BlackboardUtils.FindVariable<int>(null, "./bonus/spinCount").value;
            if (remainSpinCount > 1)
                spinCountUI.text = string.Format(EXTRA_SPINS_TEXT_FORMAT, remainSpinCount);
            else
                spinCountUI.text = string.Format(EXTRA_SPIN_TEXT_FORMAT, remainSpinCount);
            yield return new WaitForSeconds(0.1f);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            var communityUserResultList = BlackboardUtils.FindValue<List<Blackboard>>(null, "./bonus/response/userGameResultList");
            spinCountUI.text = string.Format(EXTRA_SPINS_TEXT_FORMAT,communityUserResultList[0].GetValue<int>("baseSpinCount"));
        }

        protected override void OnFinish()
        {
        }

        protected override void OnStart()
        {
        }
    }
}