using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using System;

namespace BagelCode
{
    public class InGameController : EventMonoBehaviour
    {
        private Variable<bool> autoSpin;
        private ContextElement root;
        private Animator anim;

        private ContextElement vipLpBoosterTextElement;

        private GameObject vipLpBooster = null;

        protected virtual void Start()
        {
            autoSpin = BlackboardUtils.FindVariable<bool>("./autoSpin");

            if (BlackboardQueryUtils.IsPipTriggerButtonEnabled() && NativeHelper.Instance.GetAppSettingsPipModeAvailable())
            {
                autoSpin.onValueChanged += UpdatePipButton;
            }

            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            // todo
            InitProperty();
            UpdateLpBooster();
        }

        private void UpdatePipButton(string name, object value)
        {
            if (!BlackboardQueryUtils.IsPipModeSettingsEnabled()) return;
            
            anim.SetBool("PIP", (bool)value);
        }

        private void InitProperty()
        {
            if (vipLpBooster == null)
            {
                var currentOrientation = BlackboardUtils.FindVariable<Orientation>(MainBlackboard.Get(), "currentOrientation");
                if (currentOrientation == null)
                    return;

                string lpBoosterAssetName = "Ingame VIP Lounge Club LP Booster";
                string winElementName = "In Game Bottom/Win";
                if (currentOrientation.value == Orientation.PORTRAIT)
                {
                    if(OrientationUtils.Instance.PossibleChangeOrientation())
                    {
                        lpBoosterAssetName += " Vertical";
                        winElementName += " Vertical Mode";
                    }
                }
                ContextElement winElement = ContextUtils.FindElement(root, winElementName, ContextSearchingType.FullNameSearch);
                vipLpBooster = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, lpBoosterAssetName, winElement.transform);

                winElement.UpdateContext(false);
                vipLpBoosterTextElement = ContextUtils.FindElement(winElement, "Text LP Perceentage", ContextSearchingType.ChildrenSearch);
            }

            MetaContextElementUtils.SimpleSetClickable(root, "Menu In Game/Button PIP",
                () =>
                {
                    NativeHelper.Instance.MoveHomeScreen();
                }
                , true, ContextSearchingType.FullNameSearch
            );

            SetActiveLpBooster(false);
        }
#region LP BOOSTER
        private void SetActiveLpBooster(bool isActive)
        {
            if (vipLpBooster != null)
                vipLpBooster.SetActive(isActive);
        }

        public void UpdateLpBooster()
        {
            if (vipLpBooster == null)
                return;
            // Active VIP Lounge Benefit
            bool backupActive = vipLpBooster.activeSelf;
            bool isActive = GetActiveVipLpBooster();

            //// room player check
            //isActive = BlackboardQueryUtils.IsRoomInClubMember();

            if (isActive != backupActive)
            {
                SetActiveLpBooster(isActive);
                if (vipLpBoosterTextElement != null)
                    MetaContextElementUtils.SetTextGlobal(vipLpBoosterTextElement, "VIP_LOUNGE_INGAME_CLUB_LP_BOOSTER", NumberUtils.GetAdditionalPercent(BlackboardQueryUtils.GetVIPLoungeClubVegasRewardNumerator("LP_BOOST")));
            }
        }

        private bool GetActiveVipLpBooster()
        {
            if (BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.CLUB) || !ClubUtils.IsClubber())
                return false;
            return BlackboardQueryUtils.IsVipLoungeEnabled() && !VipLounge.VipLounge.Utils.IsEnded && !BlackboardQueryUtils.IsVipLoungeEndedTimestamp();
        }
#endregion
    }
}
