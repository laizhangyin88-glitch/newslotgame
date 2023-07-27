using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC.ForceField
{
    [RequireComponent(typeof(Collider))]
    public class WindZone : MonoBehaviour, IForceField
    {
        public Vector3 windForce;

        public void PerformUpdate(IBody body)
        {
            body.AddForce(windForce, ForceMode.Force);
        }
    }
}
