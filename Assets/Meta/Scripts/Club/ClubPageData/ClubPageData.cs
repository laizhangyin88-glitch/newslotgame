using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public abstract class ClubPageData
    {
        public ClubPageData(GameObject _obj)
        {
            obj = _obj;
            root = obj.GetComponent<ContextElement>();
            anim = obj.GetComponent<Animator>();
        }

        public GameObject obj;
        public ContextElement root;
        public Animator anim;

    }
}