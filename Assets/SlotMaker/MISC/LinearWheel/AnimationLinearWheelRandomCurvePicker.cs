using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    public class AnimationLinearWheelRandomCurvePicker : MonoBehaviour
    {
        [BoxGroup("Curve")]
        public AnimationCurve baseCurve = new AnimationCurve(
            new Keyframe(0.0f, 0.0f, 0.0f, 25.0f),
            new Keyframe(0.04f, 1.0f, 25.0f, -1.041666f),
            new Keyframe(1.0f, 0.0f, -1.041666f, 0f)
        );
        [BoxGroup("Curve")]
        [OnValueChanged("OnCurveListChanged")]
        public List<AnimationCurve> curves = new List<AnimationCurve>();
        public WeightRandomGeneratorBase randomGenerator;
        public AnimationLinearWheel linearWheel;

        public void Pick()
        {
            if (linearWheel == null)
            {
                Debug.LogError("[AnimationLinearWheelRandomCurvePicker] Linear Wheel is not assigned", gameObject);
                return;
            }

            linearWheel.curve = curves[randomGenerator.TakeOne()];
        }

#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private bool isBaseCurveApplied = false;
        private void OnCurveListChanged()
        {
            if (curves.Count == 0)
            {
                isBaseCurveApplied = false;
            }
        }

        [BoxGroup("Curve")]
        [Button]
        private void AddCurve()
        {
            curves.Add(
                new AnimationCurve(baseCurve.keys)
            );
        }

        [BoxGroup("Curve")]
        [Button]
        private void ApplyBaseCurve()
        {
            if (!isBaseCurveApplied)
            {
                PrependBaseCurve();
            }
            else
            {
                ReplaceCurveFrontWithBaseCurve();
            }
            isBaseCurveApplied = true;
        }

        private void ReplaceCurveFrontWithBaseCurve()
        {
            if (baseCurve == null || baseCurve.keys.Length == 0) return;

            float baseCurveStart = baseCurve.keys[0].time;
            Keyframe[] shiftedBaseCurveKeys = ShiftKeyframes(baseCurve.keys, 0.0f);
            for (int curveIndex = 0; curveIndex < curves.Count; curveIndex++)
            {
                Keyframe[] keys = shiftedBaseCurveKeys.Concat(curves[curveIndex].keys.Skip(baseCurve.keys.Length)).ToArray();
                curves[curveIndex].keys = keys;
            }
        }

        private void PrependBaseCurve()
        {
            if (baseCurve == null || baseCurve.keys.Length == 0) return;

            Keyframe[] shiftedBaseCurveKeys = ShiftKeyframes(baseCurve.keys, 0.0f);

            // The base curve part would be 0.0f ~ baseCurveLength, and the rest part of each curve would start from baseCurveLength.
            float baseCurveLength = baseCurve.keys[baseCurve.keys.Length - 1].time - baseCurve.keys[0].time;
            AlignStartOfCurves(baseCurveLength);

            for (int curveIndex = 0; curveIndex < curves.Count; curveIndex++)
            {
                // Remove first key of each curve to avoid two keyframes, end of the baseCurve and start of each curve,
                // are overlapped on the same time (= baseCurveLength).
                Keyframe[] keys = shiftedBaseCurveKeys.Concat(curves[curveIndex].keys.Skip(1)).ToArray();
                curves[curveIndex].keys = keys;
            }
        }

        private void AlignStartOfCurves(float startTime)
        {
            if (startTime < 0.0f) return;

            for (int curveIndex = 0; curveIndex < curves.Count; curveIndex++)
            {
                var shiftedKeys = ShiftKeyframes(curves[curveIndex].keys, startTime);
                curves[curveIndex].keys = shiftedKeys;
            }
        }

        private Keyframe[] ShiftKeyframes(Keyframe[] curveKeys, float startTime)
        {
            if (curveKeys.Length == 0) return curveKeys;

            float shiftAmount = startTime - curveKeys[0].time;

            // Since Keyframe is a struct, original keys from curveKeys is not modified by this
            Keyframe[] shiftedKeys = curveKeys.Select((key) => {
                key.time += shiftAmount;
                return key;
            }).ToArray();

            return shiftedKeys;
        }
#endif
    }
}
