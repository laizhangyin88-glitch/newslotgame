using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SlotMaker
{
    class WheelSetupTools : OdinEditorWindow
    {
        [MenuItem("SlotMaker/Tools/Wheel Setup Tools", false, 204)]
        private static void OpenWindow()
        {
            GetWindow<WheelSetupTools>().Show();
        }

        public Transform wheelAnchor;
        public List<GameObject> segmentPrefabs;

        [Min(1)]
        public int segmentsCount = 1;
        public int radius = 1;
        public float angleZ = 0;

        private bool InValidInput { get { return !(wheelAnchor && segmentPrefabs.Count > 0); } }
        [DisableIf("InValidInput")]
        [Button(ButtonSizes.Large)]
        public void CreateWheelSectors()
        {
            DestroyAllChildren();
            CreateSectors(segmentPrefabs);
        }

        private void DestroyAllChildren()
        {
            int childCount = wheelAnchor.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(wheelAnchor.GetChild(i).gameObject);
            }
        }

        private void CreateSectors(List<GameObject> segmentPrefabs)
        {
            float oneSectorAngle = 360 / segmentsCount;
            for (int sectorIndex = 0; sectorIndex < segmentsCount; sectorIndex++)
            {
                int listElementIndex = sectorIndex % segmentPrefabs.Count;
                GameObject go = PrefabUtility.InstantiatePrefab(segmentPrefabs[listElementIndex], wheelAnchor) as GameObject;
                float angle = oneSectorAngle * sectorIndex;
                go.transform.localPosition = new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * angle), radius * Mathf.Sin(Mathf.Deg2Rad * angle), 0);
                go.transform.Rotate(new Vector3(0, 0, angle + angleZ));
                go.name += " " + sectorIndex;
            }
        }
    }
}
