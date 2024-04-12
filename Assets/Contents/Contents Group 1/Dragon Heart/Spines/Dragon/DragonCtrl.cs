using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;

public class DragonCtrl : MonoBehaviour
{
    public SkeletonAnimation skeleton;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DragonAnimEvent_Idle(int isLoop)
    {
        skeleton.AnimationState.SetAnimation(0,"1", Convert.ToBoolean(isLoop));
    }

    public void DragonAnimEvent_Attack(int isLoop )
    {
        skeleton.AnimationState.SetAnimation(0, "2", Convert.ToBoolean(isLoop));
    }

    public void DragonAnimEvent_Appear(int isLoop)
    {
        skeleton.AnimationState.SetAnimation(0, "3", Convert.ToBoolean(isLoop));
    }
}
