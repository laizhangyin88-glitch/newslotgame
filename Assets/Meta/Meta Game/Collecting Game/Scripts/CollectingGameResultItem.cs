using System;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Scratcher
{
    public class CollectingGameResultItem : MonoBehaviour
    {
        //private bool movable;
        //private Vector3 from;
        //private Vector3 to;
        //private Vector3 direction;
        //private float magnitude;
        
        //private float velocity;
        
        //private float time;
        //private float passedTime;

        //private bool destroyAtArrive;
        //private float delayBeforeStart;
        //private Action callback;

        //private void Awake()
        //{
        //    movable = false;
        //}

        //private void LateUpdate()
        //{
        //    if (movable)
        //    {
        //        passedTime += Time.deltaTime;

        //        if (passedTime < delayBeforeStart)
        //            return;
                
        //        if (passedTime >= time + delayBeforeStart)
        //        {
        //            movable = false;
        //            transform.position = to;
                    
        //            if (destroyAtArrive)
        //                GameObject.Destroy(gameObject);

        //            if (callback != null)
        //                callback();
                    
        //            return;
        //        }
                
        //        float x = (passedTime - delayBeforeStart) / time;
        //        float y = -x * x + 2 * x;
                
        //        transform.position = @from + direction * magnitude * y;
        //    }
        //}

        // use CoroutineUtils.ApplyMovement instead
        //public void StartMove(Transform from, Transform to, float time, bool destroyAtArrive, float delayBeforeStart, Action callback, Transform parent = null)
        //{
        //    passedTime = 0f;
        //    this.@from = from.position;
        //    this.@to = to.position;
        //    this.direction = this.to - this.@from;
        //    magnitude = direction.magnitude;
        //    direction.Normalize();

        //    transform.SetParent(parent != null ? parent : to);

        //    this.time = time;
        //    this.destroyAtArrive = destroyAtArrive;
        //    this.delayBeforeStart = delayBeforeStart;
        //    this.callback = callback;

        //    movable = true;
        //}

    }
}