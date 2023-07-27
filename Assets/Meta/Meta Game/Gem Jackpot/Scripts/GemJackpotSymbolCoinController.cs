using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using TMPro;

namespace BagelCode.GemJackpot
{
    public class GemJackpotSymbolCoinController : MonoBehaviour, IFromToTransformPair
    {
        public Animator animator;

        public Transform from;
        public Transform to;

        public Transform From { get { return from; } }
        public Transform To { get { return to; } }

        public float amountTime = 0.0f;

        public void SetMoveTransform(Transform _from, Transform _to)
        {
            from = _from;
            to = _to;
        }

        public void SetTrigger()
        {
            if (animator != null)
                animator.SetTrigger("Disappear");
        }

        public void MoveCoin(GameObject obj)
        {
            if (from == null || to == null) return;
            StartCoroutine(StartMoveCoin(obj));
        }

        protected IEnumerator StartMoveCoin(GameObject obj)
        {
            yield return new WaitForSeconds(1.0f);
            if (obj != null) obj.SetActive(false);

            GSManager.Instance.GetHandler("Meta_Gemjackpot_Gagefly").Play();

            while (amountTime < 1.0f)
            {
                amountTime += Time.deltaTime * 5;

                transform.position = Vector3.Slerp(from.position, to.position, amountTime);

                yield return new WaitForFixedUpdate();
            }

            SetTrigger();
            yield return new WaitForSeconds(0.5f);

            Destroy(gameObject);
        }
    }
}