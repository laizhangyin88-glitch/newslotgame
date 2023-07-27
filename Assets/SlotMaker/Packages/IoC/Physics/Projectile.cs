using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.IoC
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour, IBody
    {
        public Rigidbody projectileRigidbody;

        public float area = 1f;
        public float density = 1f;

        public float aerodynamicDrag;

        private HashSet<IForceField> forceFields = new HashSet<IForceField>();

        public void AddForce(Vector3 force, ForceMode forceMode)
        {
            projectileRigidbody.AddForce(force, forceMode);
        }

        public Vector3 GetVelocity()
        {
            return projectileRigidbody.velocity;
        }

        public void SetVelocity(Vector3 velocity)
        {
            projectileRigidbody.velocity = velocity;
        }

        public float GetMass()
        {
            return projectileRigidbody.mass;
        }

        private void OnTriggerEnter(Collider other)
        {
            IForceField forceField = other.GetComponent<IForceField>();
            if (forceField != null)
            {
                forceFields.Add(forceField);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            IForceField forceField = other.GetComponent<IForceField>();
            if (forceField != null)
            {
                forceFields.Remove(forceField);
            }
        }

        private void FixedUpdate()
        {
            foreach (var forceField in forceFields)
            {
                forceField.PerformUpdate(this);
            }

            if (aerodynamicDrag > 0f)
            {
                Vector3 velocity = projectileRigidbody.velocity;
                Vector3 aerodynamicDragDirection = -velocity.normalized;
                float aerodynamicDragForce = 0.5f * density * area * aerodynamicDrag * velocity.sqrMagnitude;
                projectileRigidbody.AddForce(aerodynamicDragDirection * aerodynamicDragForce, ForceMode.Force);
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        ////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
            Validate();
        }

        private void Validate()
        {
            if (projectileRigidbody == null)
            {
                projectileRigidbody = GetComponent<Rigidbody>();
            }
        }
#endif
    }
}
