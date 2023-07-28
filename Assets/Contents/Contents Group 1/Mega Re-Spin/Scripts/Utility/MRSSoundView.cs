
using System.Collections.Generic;
using GameStudio.Slot.MRS.Utility;
using UnityEngine;

namespace GameStudio.Slot.MRS.Global
{
    public class MRSSoundView : MonoBehaviour
    {
        [SerializeField] private List<string> spinBGMSoundList;
        public int lastSpinBGMIndex = -1;

        public void StartRandomSpinBGMSound()
        {
            MRSUtility.ChangeSnapShot("Content_Main");
            int randomNumber;
            do randomNumber = Random.Range(0, 3);
            while (randomNumber == lastSpinBGMIndex);
            MRSUtility.PlaySound(spinBGMSoundList[randomNumber]);
            lastSpinBGMIndex = randomNumber;
        }

        public void StopSpinBGMSound()
        {
            MRSUtility.StopSound(spinBGMSoundList[0]);
            MRSUtility.StopSound(spinBGMSoundList[1]);
            MRSUtility.StopSound(spinBGMSoundList[2]);
            MRSUtility.PlaySound("Base Spin BGM END");
        }
    }
}