using UnityEngine;

namespace SlotMaker.IoC
{
    public interface IBody
    {
        void AddForce(Vector3 force, ForceMode forceMode);

        Vector3 GetVelocity();

        void SetVelocity(Vector3 velocity);

        float GetMass();
    }
}
