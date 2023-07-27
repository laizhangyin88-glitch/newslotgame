using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SlotMaker
{
    [CustomEditor(typeof(AnimationLinearWheel))]
    public class AnimationLinearWheelInspector : Editor
    {
    	private AnimationLinearWheel linearWheel;
    	private void OnEnable()
    	{
    		linearWheel = target as AnimationLinearWheel;
    	}

        private int targetIndex;

    	public override void OnInspectorGUI()
    	{
            if (linearWheel == null)
                linearWheel = target as AnimationLinearWheel;

            linearWheel.curve = EditorGUILayout.CurveField("Curve", linearWheel.curve);

            linearWheel.wheelPointerPosition = EditorGUILayout.FloatField("Pointer Position", linearWheel.wheelPointerPosition);

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("segmentList"), true);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("onSpinLinearWheel"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onStoppedLinearWheel"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onChangedSegment"), true);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Simulation", MessageType.Info, true);
            linearWheel.animationTime = EditorGUILayout.FloatField("Spin Time", linearWheel.animationTime);
            linearWheel.desiredDistance = EditorGUILayout.FloatField("Target Distance", linearWheel.desiredDistance);
            linearWheel.upward = EditorGUILayout.Toggle("Upward/Rightward", linearWheel.upward);
            if (GUILayout.Button("Simulate"))
            {
                linearWheel.Simulation();
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Set Target Segment Index", MessageType.Info, true);
            linearWheel.minimumSpinCount = EditorGUILayout.FloatField("Minimum Spin Count", linearWheel.minimumSpinCount);
            targetIndex = EditorGUILayout.IntField("Target Index", targetIndex);
            if (GUILayout.Button("Set Target"))
            {
                linearWheel.SetDesiredSegment(targetIndex);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Reset Wheel Position"))
            {
                linearWheel.ResetWheel();
            }
    	}
    }
}
