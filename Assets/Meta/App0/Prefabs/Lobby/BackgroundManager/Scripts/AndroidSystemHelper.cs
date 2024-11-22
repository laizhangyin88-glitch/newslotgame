using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

public class AndroidSystemHelper : MonoSingleton<AndroidSystemHelper>
{
    private AndroidJavaObject nativeObject;

    public void Init()
    {
        nativeObject = new AndroidJavaObject("com.cryfx.game.libserialport.SystemHelper");
        if (nativeObject == null) return;
        nativeObject.Call("Init");
    }

    public void SetSystemTime(int year, int month, int day, int hour, int minute, int second)
    {
        if (nativeObject == null)
            return;

        nativeObject.Call("SetSystemTime", year, month, day, hour, minute, second);
    }

    public void ScreenFlip()
    {
        if (nativeObject == null)
            return;

        nativeObject.Call("ScreenFlip");
    }

    private void Start()
    {
        Init();
    }
}
