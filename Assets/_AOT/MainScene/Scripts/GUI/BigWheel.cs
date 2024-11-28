using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SlotMaker
{

/// <summary>
/// References: http://digitalopus.ca/site/pd-controllers/
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BigWheel : MonoBehaviour
{
    private Rigidbody __rigidbody;
	protected Rigidbody _rigidbody
    {
        get
        {
            if (__rigidbody == null)
                __rigidbody = GetComponent<Rigidbody>();
            return __rigidbody;
        }
    }

    public bool clockwise = true;

    public float direction { get { return clockwise ? -1f : 1f; } }

    [SerializeField]
    private float _frequency = 1f;
	public float frequency
    {
        get { return _frequency; }
        set
        {
            _frequency = value;
            UpdateCoefficient();
        }
    }

    [SerializeField]
    private float _damping = 1f;
    public float damping
    {
        get { return _damping; }
        set
        {
            _damping = value;
            UpdateCoefficient();
        }
    }

    public float maximumVelocityRatio = 1f;
    public float sleepThreshold = 0.01f;
    public float angleThreshold = 0.01f;

    public Vector3 localRotationAxis = new Vector3(0, 0, 1.0f);

    public float ks;
    public float kd;
    public float dt;
    public float g;
    public float ksg;
    public float kdg;

    public UnityEvent onSpinBigWheel;
    public UnityEvent onStoppedBigWheel;

    protected void UpdateCoefficient()
    {
        ks = (6f * frequency) * (6f * frequency) * 0.25f;
        kd = 4.5f * frequency * damping;
        dt = Time.fixedDeltaTime;
	    g = 1f / (1f + kd * dt + ks * dt * dt);
	    ksg = ks * g;
	    kdg = (kd + ks * dt) * g;
    }

    protected bool simulation;
    protected float angleLength;
    protected Quaternion oldRotation;
    protected Quaternion desiredRotation;

    public void Simulation(float initialTorque, float desiredAngle, int additionalRotationCount)
    {
        _rigidbody.AddRelativeTorque(localRotationAxis * initialTorque, ForceMode.Impulse);

        Vector3 desiredEulerAngle = desiredAngle * localRotationAxis;
        desiredRotation = Quaternion.Euler(desiredEulerAngle.x, desiredEulerAngle.y, desiredEulerAngle.z);
        Quaternion localRotation = GetLocalRotation();

        float angle, axis;
        CalcAngleAxis(localRotation, desiredRotation, out angle, out axis);
        angle *= direction;
        if ((axis * direction) < 0f)
            angle = 360f * direction - angle;

        angleLength = 360f * additionalRotationCount * direction + angle;
        angleLength *= Mathf.Deg2Rad;

        oldRotation = localRotation;

        simulation = true;

        onSpinBigWheel.Invoke();
    }

    public void SkipSimulation()
    {
        if(simulation)
        {
            transform.localRotation = desiredRotation;
            _rigidbody.Sleep();
            simulation = false;
            onStoppedBigWheel.Invoke();
        }
    }

    private void FixedUpdate()
    {
        if (simulation)
        {
            Quaternion localRotation = GetLocalRotation();
            float angle, axis;
            CalcAngleAxis(oldRotation, localRotation, out angle, out axis);
            angle *= Mathf.Sign(axis);
            angleLength -= angle * Mathf.Deg2Rad;

            float dir = Mathf.Sign(angleLength);
            float magnitude = Mathf.Min(Mathf.Abs(angleLength), 360f * maximumVelocityRatio * Mathf.Deg2Rad);
            Vector3 localAngularVelocity = Quaternion.Inverse(_rigidbody.rotation) * _rigidbody.angularVelocity;
            Vector3 torque = ksg * dir * magnitude * localRotationAxis - kdg * localAngularVelocity;

            if ((magnitude < angleThreshold) && (torque.magnitude < sleepThreshold))
            {
                _rigidbody.Sleep();
                simulation = false;
                onStoppedBigWheel.Invoke();
                return;
            }

            Quaternion rotInertia2World = _rigidbody.inertiaTensorRotation * localRotation;
            torque = Quaternion.Inverse(rotInertia2World) * torque;
            torque.Scale(_rigidbody.inertiaTensor);
            torque = rotInertia2World * torque;
            _rigidbody.AddRelativeTorque(torque);

            oldRotation = localRotation;
        }
    }

    protected void CalcAngleAxis(Quaternion oldRotation, Quaternion newRotation, out float angle, out float axis)
    {
        Vector3 orthoNormalVector = new Vector3(1.0f, 1.0f, 1.0f);
        Vector3.OrthoNormalize(ref localRotationAxis, ref orthoNormalVector);

        Vector3 oldPoint = oldRotation * orthoNormalVector;
        Vector3 newPoint = newRotation * orthoNormalVector;

        Vector3 axisVec = Vector3.Cross(oldPoint.normalized, newPoint.normalized);
        axis = Vector3.Dot(localRotationAxis, axisVec);
        angle = Vector3.Angle(oldPoint, newPoint);
    }

    protected Quaternion GetLocalRotation()
    {
        //return Quaternion.Inverse(transform.parent.rotation) * _rigidbody.rotation;
        return transform.localRotation;
    }
}

}
