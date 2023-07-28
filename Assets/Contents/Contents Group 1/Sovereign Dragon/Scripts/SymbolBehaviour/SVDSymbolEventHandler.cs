using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace GameStudio.Slot.SVD
{
    public class SVDSymbolEventHandler : DefaultSymbolEventHandler
    {
        [SerializeField] private Blackboard _symbolBB;

        public void ChangeSate(string animationName)
        {
            EnterState(animationName);
        }

        public Blackboard GetSymbolBlackboard()
        {
            return _symbolBB;
        }
    }
}
