using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class UploadProfilePicture : ActionTask
{
	public BBParameter<byte[]> imageBytes;
    public BBParameter<bool> isUpdateProfile;
    public BBParameter<long> rewardCoin;

	protected override string info 
	{ 
		get 
		{ 
			return string.Format("Upload user pic " + imageBytes);
		} 
	}
	protected override void OnExecute()
	{
        rewardCoin.value = 0;

#if !UNITY_EDITOR
        Debug.Log("UploadProfilePicture");

        // Image Resize
        Texture2D tex = new Texture2D(156, 156, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Trilinear;
        tex.anisoLevel = 4;
        tex.wrapMode = TextureWrapMode.Clamp;

        tex.LoadImage(imageBytes.value);

        BagelCodeClientAPI.UploadProfile(imageBytes.value, isUpdateProfile.value,
        (response) =>
        {
            Blackboard meBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "/me").value;

            meBB.SetValue("profileUrl", response.profileUrl);
            meBB.SetValue("profileHighResolutionUrl", response.profileHighResolutionUrl);

            BlackboardQueryUtils.AddCoins(response.rewardCredit);
            rewardCoin.value = response.rewardCredit;
            
            EndAction(true);
        },
        (error) =>
        {
        	Debug.Log("Upload Profile Fail " + error);
            EndAction(true);
        });
#endif
    }
}

}
