using BagelCode.ClientModels;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestThunderDragon : MonoBehaviour
{
    public Button dragonBtn;
    public Button fishBtn;
    public SkeletonAnimation thunderDragon;
    public SkeletonAnimation ship;

    // Start is called before the first frame update
    void Start()
    {
        thunderDragon.state.SetAnimation(0, "SWIM", true);
        thunderDragon.state.Complete += StateComplete;
        dragonBtn.onClick.AddListener(OnDragonClick);

        
        ship.state.SetAnimation(0, "PariteShip_ani", false);
        ship.state.Complete += State_Complete;
        fishBtn.onClick.AddListener(OnFishClick);
    }

    private void State_Complete(TrackEntry trackEntry)
    {
        ship.timeScale = 0;
    }

    private void StateComplete(Spine.TrackEntry trackEntry)
    {
        thunderDragon.timeScale = 0;
    }

    void OnDragonClick()
    {
        thunderDragon.timeScale = 0.3f;
        thunderDragon.state.SetAnimation(0, "SWIM", true);
    }

    void OnFishClick()
    {
        ship.timeScale = 1f;
        ship.state.SetAnimation(0, "PariteShip_ani", false);
    }

}
