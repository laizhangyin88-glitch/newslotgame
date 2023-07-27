using UnityEngine;

namespace BagelCode
{
    public class LobbyVipFreeDealBehaviour : EventMonoBehaviour
    {
        private GameObject vipDealObj = null;

        private SimpleReserveTimer _reserveTimer;
        private SimpleReserveTimer reserveTimer
        {
            get
            {
                if (_reserveTimer == null)
                {
                    _reserveTimer = GetComponent<SimpleReserveTimer>();
                    if (_reserveTimer == null)
                        _reserveTimer = gameObject.AddComponent<SimpleReserveTimer>();
                }
                return _reserveTimer;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateVariables();
        }

        private void Start()
        {
            Register("RefreshTier", UpdateVariables);
        }

        public void UpdateVariables()
        {
            if (!VipDealV2.Utils.IsTierLock())
            {
                var vipDealInfo = VipDealV2.Utils.GetActiveInfo();
                if (vipDealInfo != null)
                {
                    var endTimestamp = vipDealInfo.GetValue<long>("endTimestamp");

                    ActivateVipDeal(endTimestamp);
                }
                else
                {
                    DeactivateVipDeal();
                }
            }
            else
            {
                DeactivateVipDeal();
            }
        }

        private void ActivateVipDeal(long endTimestamp)
        {
            // Make Icon
            if (vipDealObj == null)
            {
                vipDealObj = MetaObjectUtils.MakePrefab(
                    MetaStringDefine.LOBBY_BUNDLE_NAME, "Lobby Button VIP Free Deal", transform);
            }

            vipDealObj.SetActive(true);

            reserveTimer.SetReserveCallback(endTimestamp, () => { OnEndTimer(); });
        }

        private void DeactivateVipDeal()
        {
            if (vipDealObj != null)
                vipDealObj.SetActive(false);

            reserveTimer.Stop();
        }

        private void OnEndTimer()
        {
            UpdateVariables();
        }
    }
}
