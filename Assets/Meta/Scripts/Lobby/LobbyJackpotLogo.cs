using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyJackpotLogo : MonoBehaviour
{

    Coroutine coroutine;
    SkeletonGraphic skeletonGraphic;

    private void Awake()
    {
        skeletonGraphic = GetComponent<SkeletonGraphic>();
    }

    private void Start()
    {
        skeletonGraphic.AnimationState.SetAnimation(0, "ide", false);
        coroutine = StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.5f);
            skeletonGraphic.AnimationState.SetAnimation(0, "ide", false);
        }
    }

    private void OnDestroy()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
}
