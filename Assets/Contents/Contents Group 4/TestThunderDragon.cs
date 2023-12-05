using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestThunderDragon : MonoBehaviour
{
    public Button testButton;
    public Button testButton1;
    public SkeletonAnimation thunderDragon;

    // Start is called before the first frame update
    void Start()
    {
        thunderDragon.state.SetAnimation(0, "SWIM", false);
        thunderDragon.state.Complete += StateComplete;
        testButton.onClick.AddListener(OnClick);
        testButton1.onClick.AddListener(OnClick1);
    }

    private void StateComplete(Spine.TrackEntry trackEntry)
    {
        //thunderDragon.state.SetEmptyAnimation(0, 0);
        thunderDragon.timeScale = 0;
        gameObject.SetActive(false);
    }

    void OnClick()
    {
        gameObject.SetActive(true);
    }

    void OnClick1()
    {
        thunderDragon.skeleton.SetToSetupPose();
        thunderDragon.timeScale = 0.3f;
        thunderDragon.state.SetAnimation(0, "SWIM", false);
    }

}
