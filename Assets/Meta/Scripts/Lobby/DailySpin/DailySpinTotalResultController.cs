using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;

namespace BagelCode
{
    public class DailySpinTotalResultController : MonoBehaviour
    {
        private Blackboard bb;

        //

        private void Initproperty()
        {
            bb = GetComponent<Blackboard>();

            var creditList = bb.GetValue<List<long>>("creditList");
            var isJackpotList = bb.GetValue<List<bool>>("isJackpotList");


        }


    }
}