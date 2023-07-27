using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using SlotMaker.Json;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace SlotMaker
{
    public class PayLineEditor : OdinEditorWindow
    {
        [MenuItem("SlotMaker/Contents/PayLine Editor")]
        private static void OpenWindow()
        {
            GetWindow<PayLineEditor>().Show();
        }

        [ValidateInput("PayLinesMustBeNotNull", "", InfoMessageType.None)]
        public PayLines Target;
        [ValidateInput("ConfigMustBeNotNull", "", InfoMessageType.None)]
        public BasePayLineEditorConfig PayLineEditorConfig;

        [DisableIf("IsNotValidetedInput")]
        [Button(ButtonSizes.Large)]
        public void BuildLines()
        {
            PreBuild();
            Build();
        }

        [Space]
        [PropertyOrder(1), Toggle("IsActiveLineDebug")]
        public ToggleLineDebug LineDebug = new ToggleLineDebug();

        [Serializable]
        public class ToggleLineDebug
        {
            public bool IsActiveLineDebug;
            public bool TurnOffAllLines;
            public ushort WinLinesCount;

            [ShowInInspector, MinMaxSlider(0, "WinLinesCount", true)]
            public Vector2 DravingLinesRange
            {
                get
                {
                    return new Vector2(min, max);
                }
                set
                {
                    min = (int)value.x;
                    max = (int)value.y;
                }
            }

            [SerializeField, HideInInspector]
            private int min;
            [SerializeField, HideInInspector]
            private int max;
        }

        private bool validetedPayLinesInput;
        private bool validetedConfigInput;
        private bool IsNotValidetedInput
        {
            get
            {
                return !(validetedPayLinesInput && validetedConfigInput);
            }
        }
        private bool PayLinesMustBeNotNull(PayLines obj)
        {
            validetedPayLinesInput = obj != null;
            return obj != null;
        }
        private bool ConfigMustBeNotNull(BasePayLineEditorConfig obj)
        {
            validetedConfigInput = obj != null;
            return obj != null;
        }

        private float MiddleColumnMultiplier
        {
            get
            {
                return -((PayLineEditorConfig.totalColumn - 1) * 0.5f);
            }
        }
        private float MiddleRowMultiplier
        {
            get
            {
                return (PayLineEditorConfig.totalRow - 1) * 0.5f;
            }
        }

        private const float iconMiddleMultiplier = 0.5f;
        private const int extraPointsOffset = 2;

        private class DrawingInfo
        {
            public int count;
            public float pivot;
            public float margin;
            public float direction;

            public float CalculateOffset()
            {
                float offset = pivot + margin * ((count + 1) / 2) * direction;

                direction *= -1f;
                ++count;

                return offset;
            }
        }

        private void PreBuild()
        {
            Target.gameObject.DestroyImmediateChildren();
            Target.lines = new List<LineRenderer>();
        }

        private void Build()
        {
            dynamic payLineEditorConfig = this.PayLineEditorConfig as NonUniformPayLineEditorConfig;
            if (payLineEditorConfig == null)
            {
                payLineEditorConfig = this.PayLineEditorConfig as PayLineEditorConfig;
            }

            var payLines = SlotSimpleJson.DeserializeObject<List<List<int>>>(payLineEditorConfig.json.text);
            DrawingInfo[] drawingInfo = GetDrawingInfoList(payLineEditorConfig);

            for (int payLineNumber = 0; payLineNumber < payLines.Count; ++payLineNumber)
            {
                var payLine = payLines[payLineNumber];

                Vector3[] points = GetLinePoints(payLineEditorConfig, payLineNumber, payLine, drawingInfo);

                Target.lines.Add(BuildLineObject(payLineEditorConfig, payLineNumber, points));
            }
        }

        private Vector3[] GetLinePoints(NonUniformPayLineEditorConfig config, int payLineNumber, List<int> payLine, DrawingInfo[] drawingInfo)
        {
            int pivotRow = payLine[0];
            float offset = drawingInfo[pivotRow].CalculateOffset();

            var points = new Vector3[config.totalColumn + extraPointsOffset];
            int subPointExtraOffset = 0;
            for (int column = 0; column < config.totalColumn; ++column)
            {
                int row = payLine[column];

                var symbolPos = CalcSymbolPosition(column, row, config);

                if (column == 0)
                {
                    float firstXCoordinate = symbolPos.x - GetHalfSize(config.columnsPref[column].Width) - config.xOffset;
                    points[column + subPointExtraOffset] = GetLinePoint(firstXCoordinate, payLineNumber, offset, symbolPos);
                }

                points[column + 1 + subPointExtraOffset] = GetLinePoint(symbolPos.x, payLineNumber, offset, symbolPos);

                if (column == config.totalColumn - 1)
                {
                    float lastXCoordinate = symbolPos.x + GetHalfSize(config.columnsPref[column].Width) - config.xOffset;
                    points[column + 2 + subPointExtraOffset] = GetLinePoint(lastXCoordinate, payLineNumber, offset, symbolPos);
                }

                if (config.columnsPref[column].subPointsMask && column != config.totalColumn - 1)
                {
                    ArrayUtility.Add(ref points, new Vector3());
                    symbolPos = SubPointPosition(column, row, config);
                    float subPointXCoordinate = symbolPos.x + GetHalfSize(config.columnsPref[column].subPointsSize.Width) - config.xOffset;
                    points[column + 2 + subPointExtraOffset] = GetLinePoint(subPointXCoordinate, payLineNumber, offset, symbolPos);
                    subPointExtraOffset++;
                }
            }

            return points;
        }

        private Vector3[] GetLinePoints(PayLineEditorConfig config, int payLineNumber, List<int> payLine, DrawingInfo[] drawingInfo)
        {
            int pivotRow = payLine[0];
            float offset = drawingInfo[pivotRow].CalculateOffset();

            var points = new Vector3[config.totalColumn + extraPointsOffset];
            for (int column = 0; column < config.totalColumn; ++column)
            {
                int row = payLine[column];
                var symbolPos = CalcSymbolPosition(column, row, config);

                if (column == 0)
                {
                    float firstXCoordinate = symbolPos.x - GetHalfSize(config.symbolWidth) - config.xOffset;
                    points[column] = GetLinePoint(firstXCoordinate, payLineNumber, offset, symbolPos);

                }

                points[column + 1] = GetLinePoint(symbolPos.x, payLineNumber, offset, symbolPos);

                if (column == config.totalColumn - 1)
                {
                    float lastXCoordinate = symbolPos.x + GetHalfSize(config.symbolWidth) - config.xOffset;
                    points[column + 2] = GetLinePoint(lastXCoordinate, payLineNumber, offset, symbolPos);
                }
            }

            return points;
        }

        private DrawingInfo[] GetDrawingInfoList(PayLineEditorConfig config)
        {
            var drawingInfo = new DrawingInfo[config.totalRow];
            for (int row = 0; row < config.totalRow; ++row)
            {
                drawingInfo[row] = new DrawingInfo
                {
                    count = 0,
                    pivot = -config.yOffset + (config.symbolHeight * iconMiddleMultiplier),
                    margin = config.margin,
                    direction = row < (config.totalRow / 2) ? 1f : -1f
                };
            }

            return drawingInfo;
        }

        private DrawingInfo[] GetDrawingInfoList(NonUniformPayLineEditorConfig config)
        {
            var drawingInfo = new DrawingInfo[config.totalRow];
            for (int row = 0; row < config.totalRow; ++row)
            {
                drawingInfo[row] = new DrawingInfo
                {
                    count = 0,
                    pivot = -config.yOffset + (config.columnsPref[0].Height * iconMiddleMultiplier),
                    margin = config.margin,
                    direction = row < config.totalRow / 2 ? 1f : -1f
                };
            }

            return drawingInfo;
        }

        private LineRenderer BuildLineObject(BasePayLineEditorConfig config, int payLineNumber, Vector3[] linePoints)
        {
            var lineGameObject = CreateLineGameObject(config, payLineNumber);
            var lineRenderer = GetСonfiguredLineRenderer(config, payLineNumber, linePoints, lineGameObject);

            var lineAnimator = lineGameObject.AddComponent<Animator>();
            lineAnimator.runtimeAnimatorController = config.animator;

            bool lineNotInDebugRange = payLineNumber < LineDebug.DravingLinesRange.x || payLineNumber > LineDebug.DravingLinesRange.y;
            if (LineDebug.IsActiveLineDebug && (LineDebug.TurnOffAllLines ||  lineNotInDebugRange))
            {
                lineGameObject.gameObject.SetActive(false);
            }

            return lineRenderer;
        }

        private GameObject CreateLineGameObject(BasePayLineEditorConfig config, int payLineNumber)
        {
            var lineGameObject = new GameObject("Line" + payLineNumber)
            {
                layer = config.layer
            };

            lineGameObject.AddComponent<RectTransform>().SetParent(Target.transform, false);
            return lineGameObject;
        }

        private LineRenderer GetСonfiguredLineRenderer(BasePayLineEditorConfig config, int payLineNumber, Vector3[] points, GameObject lineGameObject)
        {
            var lineRenderer = lineGameObject.AddComponent<LineRenderer>();
            lineRenderer.material = config.material;
            lineRenderer.receiveShadows = false;
            lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
            lineRenderer.sortingLayerName = config.sortingLayer;
            lineRenderer.sortingOrder = config.sortingOrder;
            lineRenderer.startColor = config.colorTable.GetColor(payLineNumber);
            lineRenderer.endColor = config.colorTable.GetColor(payLineNumber);
            lineRenderer.widthMultiplier = config.lineWidth;
            lineRenderer.positionCount = points.Length;
            lineRenderer.textureMode = LineTextureMode.Stretch;
            lineRenderer.numCornerVertices = 90;
            lineRenderer.useWorldSpace = false;
            lineRenderer.SetPositions(points);
            return lineRenderer;
        }

        private Vector3 GetLinePoint(float xCoordinate, int payLineNumber, float yOffset, Vector3 symbolPos)
        {
            return new Vector3
            {
                x = xCoordinate,
                y = symbolPos.y + yOffset,
                z = -payLineNumber,
            };
        }

        private Vector3 CalcSymbolPosition(int column, int row, PayLineEditorConfig config)
        {
            Vector4 offset = GetCalcLayoutOffset(config);

            float x = offset.x + offset.z * column;
            float y = offset.y + offset.w * -row;
            float z = 0.0f;
            return new Vector3(x, y, z);
        }

        private Vector4 GetCalcLayoutOffset(PayLineEditorConfig config)
        {
            float width = config.symbolWidth + config.spacing.x;
            float height = config.symbolHeight + config.spacing.y;

            Vector4 offset = new Vector4
            (
                width * MiddleColumnMultiplier,
                height * MiddleRowMultiplier,
                width,
                height
            );

            return offset;
        }

        private Vector3 CalcSymbolPosition(int column, int row, NonUniformPayLineEditorConfig config)
        {
            Vector4 offset = GetCalcLayoutOffset(column, config);

            float x = offset.x + offset.z * column;
            float y = offset.y + offset.w * -row;
            float z = 0.0f;
            return new Vector3(x, y, z);
        }

        private Vector4 GetCalcLayoutOffset(int column, NonUniformPayLineEditorConfig config)
        {
            float width = config.columnsPref[column].Width + config.spacing.x;
            float height = config.columnsPref[column].Height + config.spacing.y;

            Vector4 offset = new Vector4
            (
                width * MiddleColumnMultiplier,
                height * MiddleRowMultiplier,
                width,
                height
            );

            return offset;
        }

        private Vector3 SubPointPosition(int column, int row, NonUniformPayLineEditorConfig config)
        {
            Vector4 offset = GetCalcSubPointLayoutOffset(column, config);

            float x = offset.x + offset.z * column;
            float y = offset.y + offset.w * -row;
            float z = 0.0f;
            return new Vector3(x, y, z);
        }
        private Vector4 GetCalcSubPointLayoutOffset(int column, NonUniformPayLineEditorConfig config)
        {
            float width = config.columnsPref[column].subPointsSize.Width + config.spacing.x;
            float height = config.columnsPref[column].subPointsSize.Height + config.spacing.y;

            Vector4 offset = new Vector4
            (
                width * MiddleColumnMultiplier,
                height * MiddleRowMultiplier,
                width,
                height
            );

            return offset;
        }

        private float GetHalfSize(float startSize)
        {
            return startSize * 0.5f;
        }
    }
}
