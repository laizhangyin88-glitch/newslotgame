using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public interface IPlayableObject
    {
        void StartPlayableObject();
        void StopPlayableObject();
        void UpdatePlayableObject();
    }
}