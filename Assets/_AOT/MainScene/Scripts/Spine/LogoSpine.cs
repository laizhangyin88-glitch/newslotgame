using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoSpine : MonoBehaviour
{
    private SkeletonGraphic skeletonGraphic;

    private void Awake()
    {
        skeletonGraphic = GetComponent<SkeletonGraphic>();
    }
    void Start()
    {
        skeletonGraphic.AnimationState.AddAnimation(0, "idle", true, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
