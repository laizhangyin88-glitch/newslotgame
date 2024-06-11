using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [ExecuteInEditMode]
    public class GlobalReelStrips : MonoWeakSingleton<GlobalReelStrips>, IGlobalReelStrips
    {
        private int _index;

        public int index
        { get { return _index; } set { _index = value; } }

        public int stripsCount
        { get { return stripsList.Count; } }

        public List<ReelStrips> stripsList;

        public ReelStrips GetReelStrips(int stripsIndex)
        {
            int sI;
            if (stripsIndex < 0 || stripsIndex > stripsList.Count)
            {
                sI = 0;
            }
            else
            {
                sI = stripsIndex;
            }
            return stripsList[sI];
        }

        public ReelStrips GetReelStrips()
        {
            return GetReelStrips(index);
        }
    }
}
