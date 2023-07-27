using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [Serializable]
    public class Expectation
    {
        public List<bool> expectations;
        public List<List<Cell>> expectationSpots;

        public int expectationCount
        {
            get 
            {
                int count = 0;
                foreach (bool exp in expectations)
                {
                    if (exp)
                        ++count;
                }
                return count;
            }
        }
    }
}
