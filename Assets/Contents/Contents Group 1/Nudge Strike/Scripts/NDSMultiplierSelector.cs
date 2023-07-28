using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.NDS
{
    public class NDSMultiplierSelector : MonoBehaviour
    {
        public NDSMultiplierSubSymbolController baseController;
        public NDSSuperBonusMultiplierSubSymbolController superBonusController;
        public void Apply(BaseSymbol symbol)
        {
            bool isSuperBonus = BlackboardUtils.FindValue<bool>(null, "./game/isSuperBonus");
            if (isSuperBonus)
            {
                superBonusController.Apply(symbol);
            }
            else
            {
                baseController.Apply(symbol);
            }
        }

        public void Win()
        {
            bool isSuperBonus = BlackboardUtils.FindValue<bool>(null, "./game/isSuperBonus");
            if (isSuperBonus)
            {
                superBonusController.Win();
            }
            else
            {
                baseController.Win();
            }
        }
        public void Skip()
        {
            bool isSuperBonus = BlackboardUtils.FindValue<bool>(null, "./game/isSuperBonus");
            if (isSuperBonus)
            {
                superBonusController.Skip();
            }
            else
            {
                baseController.Skip();
            }
        }
    }
}
