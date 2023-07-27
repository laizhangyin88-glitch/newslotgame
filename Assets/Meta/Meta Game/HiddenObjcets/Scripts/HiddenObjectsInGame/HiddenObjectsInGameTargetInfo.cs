using UnityEngine;
using SlotMaker;

namespace BagelCode.HiddenObjects
{
    public class HiddenObjectsInGameTargetInfo
    {
        public ContextElement element;
        public Animator anim;

        public int id = -1;
        public long updatedTime = long.MaxValue;
    }
}
