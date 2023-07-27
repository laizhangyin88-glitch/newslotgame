using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SlotMaker
{
    [CustomEditor(typeof(BigWheel))]
    public class BigWheelInspector : Editor
    {
    	private BigWheel bigWheel;

        private float initialTorque;
        private float desiredAngle;
        private int additionalRotationCount;

    	private void OnEnable()
    	{
    		bigWheel = target as BigWheel;
    	}

    	public override void OnInspectorGUI()
    	{
            bigWheel.clockwise = EditorGUILayout.Toggle("Clockwise", bigWheel.clockwise);
    		bigWheel.frequency = EditorGUILayout.FloatField("Frequency", bigWheel.frequency);
    		bigWheel.damping = EditorGUILayout.FloatField("Damping", bigWheel.damping);
            bigWheel.maximumVelocityRatio = EditorGUILayout.FloatField("Maximum Velocity Ratio", bigWheel.maximumVelocityRatio);
    		bigWheel.sleepThreshold = EditorGUILayout.FloatField("Sleep Threshold", bigWheel.sleepThreshold);
    		bigWheel.angleThreshold = EditorGUILayout.FloatField("Angle Threshold", bigWheel.angleThreshold);
    		bigWheel.localRotationAxis = EditorGUILayout.Vector3Field("Local Rotation Axis", bigWheel.localRotationAxis);

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onSpinBigWheel"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onStoppedBigWheel"), true);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Simulation", MessageType.Info, true);
            initialTorque = EditorGUILayout.FloatField("Initial Torque", initialTorque);
            desiredAngle = EditorGUILayout.FloatField("Desired Angle", desiredAngle);
            additionalRotationCount = EditorGUILayout.IntField("Additional Rotation Count", additionalRotationCount);
            if (GUILayout.Button("Simulate"))
            {
                bigWheel.Simulation(initialTorque, desiredAngle, additionalRotationCount);
            }
    	}
    }
}
