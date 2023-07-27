using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{

public class RedOrBlackLogic : MonoBehaviour 
{
    private IEnumerator MoveAndScaleCoroutine(Transform obj, float speed)
    {
        float startTime = Time.time;
        float dist = Vector3.Distance(obj.localPosition, Vector3.zero);
       
        float progress = 0;
        while (progress < dist)
        {
            progress = (Time.time - startTime) * speed;
            obj.localPosition = Vector3.Lerp(obj.localPosition, Vector3.zero, progress/dist);
            obj.localScale = Vector3.Lerp(obj.localScale, Vector3.one, progress/dist);

            yield return null;
        }

        obj.localPosition = Vector3.zero;
        obj.localScale = Vector3.one;
    }

    public void MoveAndScale(GameObject obj, GameObject target, float speed)
    {
        obj.transform.parent = target.transform;
        obj.GetComponent<MonoBehaviour>().StopAllCoroutines();
        obj.GetComponent<MonoBehaviour>().StartCoroutine(MoveAndScaleCoroutine(obj.transform, speed));
    }
}

}