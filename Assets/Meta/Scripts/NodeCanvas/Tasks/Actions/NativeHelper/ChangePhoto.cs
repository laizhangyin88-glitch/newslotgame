using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class ChangePhoto : ActionTask<ContextElement>
{
    public BBParameter<byte[]> saveAs;
    public BBParameter<int> width = (int)NativeHelper.PROFILE_IMAGE_MIN_SIDE;
    public BBParameter<int> height = (int)NativeHelper.PROFILE_IMAGE_MIN_SIDE;
    public BBParameter<bool> isCropable = true;
    protected override string info
    {
        get
        {
            return string.Format("Get Profile from device and save as {0}", saveAs);
        }
    }

    protected override void OnExecute()
    {
#if !UNITY_EDITOR
        Debug.Log("Change Photo");
        NativeHelper.Instance.GetProfileImage(width.value, height.value, isCropable.value,
            (byte[] bytes) =>
            {
                if (bytes.Length > 0)
                {
                    saveAs.value = bytes;
                    SendEvent("OnChangePhotoSuccess");
                    EndAction(true);
                }
                else
                {
                    SendEvent("OnChangePhotoError");
                    EndAction(false);
                }
            }
        );
#else
        SendEvent("OnChangePhotoError");
        EndAction(false);
#endif
    }
}

}
