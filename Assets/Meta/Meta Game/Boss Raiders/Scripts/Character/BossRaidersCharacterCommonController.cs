using SlotMaker;
using UnityEngine;

namespace BagelCode.BossRaiders
{
    public class BossRaidersCharacterCommonController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Animator rootAnimator;

        private ContextElement characterAreaElement;
        private ContextElement appearEnergyTextElement;
        private ContextElement powerTextElement;

        private BossRaidersCharacterBarBase characterBarController;
        private BossRaidersCharacterBase chracterBaseController;

        private bool isMeta = true;
        private bool isInit = false;

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();

            characterAreaElement = ContextUtils.FindElement(rootElement, "Character Area", ContextSearchingType.ChildrenSearch);

            isInit = true;
        }

        private bool InitCharacter(string bundle)
        {
            bool isCreate = false;
            GameObject obj = MetaObjectUtils.MakePrefab(bundle, "Boss Raiders Character", characterAreaElement.transform, null, "Character");
            if (obj != null)
            {
                chracterBaseController = obj.GetComponent<BossRaidersCharacterBase>();
                if (chracterBaseController == null)
                {
                    Destroy(obj);
                    return false;
                }
                chracterBaseController.InitData();
                return true;
            }
            return isCreate;
        }

        private void InitMetaInfo()
        {
            appearEnergyTextElement = ContextUtils.FindElement(rootElement, "Text Appear Energy", ContextSearchingType.ChildrenSearch);

            ContextElement myClubNameEmelent = ContextUtils.FindElement(rootElement, "Text Club Name", ContextSearchingType.ChildrenSearch);
            ContextElement myClubSymbolAreaElement = ContextUtils.FindElement(rootElement, "Club Symbol Area", ContextSearchingType.ChildrenSearch);

            ContextElement energySliderElement = ContextUtils.FindElement(rootElement, "Progress Bar", ContextSearchingType.ChildrenSearch);
            ContextElement textElement = ContextUtils.FindElement(rootElement, "Text Energy", ContextSearchingType.ChildrenSearch);
            characterBarController?.OnInit(rootAnimator, energySliderElement, textElement);

            MetaIconUtils.MakeClubSymbolIconObject(BlackboardUtils.FindValue<string>(null, "/clubInfo/symbol"), myClubSymbolAreaElement.transform, null);

            MetaContextElementUtils.SetText(myClubNameEmelent, BlackboardUtils.FindValue<string>(null, "/clubInfo/name"));
        }

        private void InitDealInfo()
        {
            powerTextElement = ContextUtils.FindElement(rootElement, "Text Power", ContextSearchingType.ChildrenSearch);
            characterBarController?.OnInit(rootAnimator, null, powerTextElement);
        }

        public bool InitData(BossRaidersCharacterBarBase barController, string bundle, bool _isMeta = true)
        {
            bool isSuccess = false;

            characterBarController = barController;
            isMeta = _isMeta;

            InitProperty();
            isSuccess = InitCharacter(bundle);
            if (isMeta)
                InitMetaInfo();
            else
                InitDealInfo();

            chracterBaseController?.SetAnimator("Active", true);

            return isSuccess;
        }

        public void Attack(bool isAttack)
        {
            chracterBaseController?.Attack(isAttack);
        }

        public void SetAnimator(string paramName, bool isActive)
        {
            // animator paramName is "Win"
            if (chracterBaseController != null)
                chracterBaseController.SetAnimator(paramName, isActive);
        }

        public void SetEnergyAnimator(bool isActive)
        {
            characterBarController?.SetEnergyAnimator(isActive);
        }

        public void SetEnergy(long energy)
        {
            characterBarController?.SetEnergy(energy);
        }

        public void SetAppearEnergy(int energy)
        {
            if (appearEnergyTextElement != null)
                MetaContextElementUtils.SetTextGlobal(appearEnergyTextElement, "BOSS_RAIDERS_CHARACTER_APPEAR_ENERGY_TEXT", energy);
        }

        public void SetTextPower(int power)
        {
            if (powerTextElement != null)
                MetaContextElementUtils.SetText(powerTextElement, power.ToString());
        }
    }
}