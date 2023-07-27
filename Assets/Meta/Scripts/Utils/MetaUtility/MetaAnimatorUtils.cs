using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public static class MetaAnimatorUtils
    {
        public static void SetAnimSafty(MonoBehaviour behaviour, Animator anim, string param, bool value)
        {
            if (behaviour != null)
                behaviour.StartCoroutine(SetAnimSaftyCoroutine(anim, param, value));
        }

        private static IEnumerator SetAnimSaftyCoroutine(Animator anim, string param, bool value)
        {
            yield return new WaitUntil(() => anim.isActiveAndEnabled);

            anim.SetBool(param, value);
        }
    }
}
