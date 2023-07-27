using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class LuckyFiveMoveContainerController : MetaGameEventButtonController
    {
        public List<Transform> targetList = new List<Transform>();
        public List<Transform> objectList = new List<Transform>();

        public float appearTime = 1f;
        public float disappearTime = 1f;
        public float moveTime = 1f;
        public float waitOpenTime = 1f;

        protected bool isProcess = false;

        public List<Transform> TargetList
        {
            get { return targetList; }
            set { targetList = value; }
        }

        public List<Transform> ObjectList
        {
            get { return objectList; }
            set { objectList = value; }
        }

        public virtual void RemoveFirstContainer(Transform firstContainer) { }
        public virtual void AddLastContainer(Transform lastContainer) { }
        public virtual void MoveComplete(Transform targetObject) { }
        public virtual void OpenComplete() { }

        public void InitPosition()
        {
            for (int i = 0; i < ObjectList.Count; ++i)
            {
                ObjectList[i].localPosition = TargetList[i].localPosition;
                ObjectList[i].rotation = TargetList[i].rotation;
                ObjectList[i].localScale = TargetList[i].localScale;
            }
        }

        public void Move()
        {
            if (!isProcess)
            {
                isProcess = true;
                StartCoroutine(MoveProcess());
            }
        }

        IEnumerator MoveProcess()
        {
            float totalDeltaTime = 0f;

            totalDeltaTime = 0f;

            while (true)
            {
                // Scale 1 -> 0
                // Disappear 0 Item
                if (totalDeltaTime < disappearTime)
                {
                    ObjectList[0].localScale = Vector3.Lerp(TargetList[0].localScale, Vector3.zero, totalDeltaTime / disappearTime);
                    yield return null;
                }
                else
                {
                    ObjectList[0].localScale = Vector3.zero;
                    break;
                }

                totalDeltaTime += Time.deltaTime;
            }

            ObjectList[0].localPosition = TargetList[TargetList.Count - 1].localPosition;
            ObjectList[0].rotation = TargetList[TargetList.Count - 1].rotation;
            ObjectList[0].SetAsLastSibling();
            // Set card back image
            // 0 item send event
            RemoveFirstContainer(ObjectList[0]);

            totalDeltaTime = 0f;

            while (true)
            {
                if (totalDeltaTime < moveTime)
                {
                    for (int i = 1; i < ObjectList.Count; ++i)
                    {
                        ObjectList[i].localPosition = Vector3.Lerp(TargetList[i].localPosition, TargetList[i - 1].localPosition, totalDeltaTime / moveTime);
                        ObjectList[i].rotation = Quaternion.Lerp(TargetList[i].rotation, TargetList[i - 1].rotation, totalDeltaTime / moveTime);
                        ObjectList[i].localScale = Vector3.Lerp(TargetList[i].localScale, TargetList[i - 1].localScale, totalDeltaTime / moveTime);
                    }
                    yield return null;
                }
                else
                {
                    for (int i = 1; i < ObjectList.Count; ++i)
                    {
                        ObjectList[i].localPosition = TargetList[i - 1].localPosition;
                        ObjectList[i].rotation = TargetList[i - 1].rotation;
                        ObjectList[i].localScale = TargetList[i - 1].localScale;
                    }

                    break;
                }

                totalDeltaTime += Time.deltaTime;
            }

            // Move Front to Rear Item.
            var moveObject = ObjectList[0];
            ObjectList.RemoveAt(0);
            ObjectList.Add(moveObject);

            AddLastContainer(ObjectList[ObjectList.Count - 1]);

            totalDeltaTime = 0f;

            while (true)
            {
                // Scale 0 -> 1
                // Appear 0 Item
                if (totalDeltaTime < appearTime)
                {
                    ObjectList[ObjectList.Count - 1].localScale = Vector3.Lerp(Vector3.zero, TargetList[TargetList.Count - 1].localScale, totalDeltaTime / appearTime);
                    yield return null;
                }
                else
                {
                    ObjectList[ObjectList.Count - 1].localScale = TargetList[TargetList.Count - 1].localScale;
                    break;
                }

                totalDeltaTime += Time.deltaTime;
            }

            MoveComplete(ObjectList[ObjectList.Count - 1]);

            yield return new WaitForSeconds(waitOpenTime);

            OpenComplete();

            isProcess = false;

        }
    }
}
