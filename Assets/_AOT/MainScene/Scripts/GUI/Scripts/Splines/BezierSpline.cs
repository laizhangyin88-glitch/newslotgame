using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    /// https://catlikecoding.com/unity/tutorials/curves-and-splines/#a-derivative
    /// https://github.com/yasirkula/UnityBezierSolution/blob/master/Assets/Plugins/BezierSolution/BezierSpline.cs
    /// http://www.habrador.com/tutorials/interpolation/3-move-along-curve/
    public class BezierSpline : MonoBehaviour
    {
        [SerializeField]
        private Vector3[] points;

        public enum BezierControlPointMode
        {
            Free,
            Aligned,
            Mirrored
        };
        [SerializeField]
        private BezierControlPointMode[] modes;

        [SerializeField]
        private bool loop;
        public bool Loop 
        {
            get { return loop; }
            set 
            {
                loop = value;
                if (value == true)
                {
                    modes[modes.Length - 1] = modes[0];
                    SetControlPoint(0, points[0]);    
                }
            }
        }

        public int CurveCount { get { return (points.Length - 1) / 3; } }

        public int ControlPointCount { get { return points.Length; } }

        public float Length { get { return GetLengthApproximately(0f, 1f, 50); } }

        private float POINT_UNIT = 1f;

        public Vector3 GetControlPoint(int index)
        {
            return points[index];
        }

        public void SetControlPoint(int index, Vector3 point)
        {
            if (index % 3 == 0)
            {
                Vector3 delta = point - points[index];
                if (loop)
                {
                    if (index == 0) 
                    {
                        points[1] += delta;
                        points[points.Length - 2] += delta;
                        points[points.Length - 1] = point;
                    }
                    else if (index == points.Length - 1)
                    {
                        points[0] = point;
                        points[1] += delta;
                        points[index - 1] += delta;
                    }
                    else 
                    {
                        points[index - 1] += delta;
                        points[index + 1] += delta;
                    }
                }
                else
                {
                    if (index > 0)
                        points[index - 1] += delta;
                    if (index + 1 < points.Length)
                        points[index + 1] += delta; 
                }
            }
            points[index] = point;
            EnforceMode(index);
        }

        public BezierControlPointMode GetControlPointMode(int index)
        {
            return modes[(index + 1) / 3];
        }

        public void SetControlPointMode(int index, BezierControlPointMode mode)
        {
            int modeIndex = (index + 1) / 3;
            modes[modeIndex] = mode;
            if (loop)
            {
                if (modeIndex == 0)
                    modes[modes.Length - 1] = mode;
                else if (modeIndex == modes.Length - 1)
                    modes[0] = mode;
            }
            EnforceMode(index);
        }

        private void EnforceMode(int index)
        {
            int modeIndex = (index + 1) / 3;
            var mode = modes[modeIndex];
            if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1))
                return;

            int middleIndex = modeIndex * 3;
            int fixedIndex, enforcedIndex;
            if (index <= middleIndex)
            {
                fixedIndex = middleIndex - 1;
                if (fixedIndex < 0)
                    fixedIndex = points.Length - 2;
                enforcedIndex = middleIndex + 1;
                if (enforcedIndex >= points.Length)
                    enforcedIndex = 1;
            }
            else 
            {
                fixedIndex = middleIndex + 1;
                if (fixedIndex >= points.Length)
                    fixedIndex = 1;
                enforcedIndex = middleIndex - 1;
                if (enforcedIndex < 0)
                    enforcedIndex = points.Length - 2;
            }

            Vector3 middle = points[middleIndex];
            Vector3 enforcedTangent = middle - points[fixedIndex];
            if (mode == BezierControlPointMode.Aligned)
                enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
            points[enforcedIndex] = middle + enforcedTangent;
        }

        public Vector3 GetPoint(float t)
        {
            int i = FindIndex(ref t);
            return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
        }

        public Vector3 GetVelocity(float t)
        {
            int i = FindIndex(ref t);
            return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
        }

        public int FindIndex(ref float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = points.Length - 4;
            }
            else 
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            return i;
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }

        public float GetLengthApproximately(float startT, float endT, int accuracy)
        {
            float step = 1f / accuracy * (endT - startT);
            float length = 0f;
            Vector3 lastPoint = GetPoint(startT);
            for (float i = startT + step; i < endT; i += step)
            {
                Vector3 point = GetPoint(i);
                length += Vector3.Distance(point, lastPoint);
                lastPoint = point;
            }
            length += Vector3.Distance(lastPoint, GetPoint(endT));
            return length;
        }

        public Vector3 FindNearestPointTo(Vector3 position, int accuracy)
        {
            float t;
            return FindNearestPointTo(position, out t, accuracy);
        }

        public Vector3 FindNearestPointTo(Vector3 position, out float t, int accuracy)
        {
            Vector3 result = Vector3.zero;
            t = -1f;
            float step = 1f / accuracy;
            float minDistance = Mathf.Infinity;
            for (float i = 0f; i < 1f; i += step)
            {
                Vector3 point = GetPoint(i);
                float distance = (position - point).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    result = point;
                    t = i;
                }
            }
            return result;
        }

        public float MoveAlong(float t, float displacement, int accuracy)
        {
            // Credit: https://gamedev.stackexchange.com/a/27138
            float _1OverCount = 1f / ControlPointCount;
            for (int i = 0; i < accuracy; ++i)
            {
                t += displacement * _1OverCount / ((float)accuracy * GetVelocity(t).magnitude);
            }
            return t;
        }

        public void AddCurve()
        {
            Vector3 point = points[points.Length - 1];
            Array.Resize(ref points, points.Length + 3);
            point.x += POINT_UNIT;
            points[points.Length - 3] = point;
            point.x += POINT_UNIT;
            points[points.Length - 2] = point;
            point.x += POINT_UNIT;
            points[points.Length - 1] = point;

            Array.Resize(ref modes, modes.Length + 1);
            modes[modes.Length - 1] = modes[modes.Length - 2];
            EnforceMode(points.Length - 4);

            if (loop)
            {
                points[points.Length - 1] = points[0];
                modes[modes.Length - 1] = modes[0];
                EnforceMode(0);
            }
        }

#if UNITY_EDITOR
        public void Reset()
        {
            POINT_UNIT = 1f / transform.lossyScale.x;

            points = new Vector3[] 
            {
                new Vector3(1f * POINT_UNIT, 0f, 0f),
                new Vector3(2f * POINT_UNIT, 0f, 0f),
                new Vector3(3f * POINT_UNIT, 0f, 0f),
                new Vector3(4f * POINT_UNIT, 0f, 0f)
            };

            modes = new BezierControlPointMode[] 
            {
                BezierControlPointMode.Free,
                BezierControlPointMode.Free
            };
        }

        // https://forum.unity.com/threads/sharing-a-variable-between-onscenegui-and-oninspectorgui.452764/
        private int selectedIndex = -1;
        public int SelectedIndex { get { return selectedIndex; } set { selectedIndex = value; } }
#endif
    }
}
