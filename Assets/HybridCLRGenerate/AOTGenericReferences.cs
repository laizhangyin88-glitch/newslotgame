using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

    // {{ AOT assemblies
    public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
    {
        "MainScene.dll",
        "Newtonsoft.Json.dll",
        "OSA.Core.dll",
        "System.Core.dll",
        "System.dll",
        "UnityEngine.AndroidJNIModule.dll",
        "UnityEngine.CoreModule.dll",
        "UnityEngine.JSONSerializeModule.dll",
        "mscorlib.dll",
        "zxing.unity.dll",
    };
    // }}

    // {{ constraint implement type
    // }} 

    // {{ AOT generic types
    // BagelCode.Internal.BagelCodeHTTP.<>c__DisplayClass24_0<object>
    // BagelCode.Internal.BagelCodeHTTP.<>c__DisplayClass26_0<object,object>
    // BagelCode.Internal.BagelCodeHTTP.<>c__DisplayClass27_0<object,object>
    // BagelCode.Protobuf.IResponse<int,object>
    // BagelCode.Task.Condition.CheckContentEvent<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckContentEvent<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckContentEvent<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckContentEvent<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckContentEvent<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckContentEvent<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckContentEvent<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckContentEvent<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckContentEvent<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckContentEvent<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckContentEvent<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckContentEvent<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckContentEvent<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckContentEvent<byte>
    // BagelCode.Task.Condition.CheckContentEvent<double>
    // BagelCode.Task.Condition.CheckContentEvent<float>
    // BagelCode.Task.Condition.CheckContentEvent<int>
    // BagelCode.Task.Condition.CheckContentEvent<long>
    // BagelCode.Task.Condition.CheckContentEvent<object>
    // BagelCode.Task.Condition.CheckContentEvent<uint>
    // BagelCode.Task.Condition.CheckContentEvent<ulong>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<byte>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<double>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<float>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<int>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<long>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<object>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<uint>
    // BagelCode.Task.Condition.CheckContentEventBlackboardValue<ulong>
    // BagelCode.Task.Condition.CheckContentEventValue<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckContentEventValue<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckContentEventValue<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckContentEventValue<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckContentEventValue<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckContentEventValue<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckContentEventValue<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckContentEventValue<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckContentEventValue<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckContentEventValue<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckContentEventValue<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckContentEventValue<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckContentEventValue<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckContentEventValue<byte>
    // BagelCode.Task.Condition.CheckContentEventValue<double>
    // BagelCode.Task.Condition.CheckContentEventValue<float>
    // BagelCode.Task.Condition.CheckContentEventValue<int>
    // BagelCode.Task.Condition.CheckContentEventValue<long>
    // BagelCode.Task.Condition.CheckContentEventValue<object>
    // BagelCode.Task.Condition.CheckContentEventValue<uint>
    // BagelCode.Task.Condition.CheckContentEventValue<ulong>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<byte>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<double>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<float>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<int>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<long>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<object>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<uint>
    // BagelCode.Task.Condition.CheckContentUIDetailEvent<ulong>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<byte>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<double>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<float>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<int>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<long>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<object>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<uint>
    // BagelCode.Task.Condition.CheckContentUIDetailEventValue<ulong>
    // BagelCode.Task.Condition.CheckContentUIEvent<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckContentUIEvent<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckContentUIEvent<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckContentUIEvent<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckContentUIEvent<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckContentUIEvent<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckContentUIEvent<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckContentUIEvent<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckContentUIEvent<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckContentUIEvent<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckContentUIEvent<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckContentUIEvent<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckContentUIEvent<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckContentUIEvent<byte>
    // BagelCode.Task.Condition.CheckContentUIEvent<double>
    // BagelCode.Task.Condition.CheckContentUIEvent<float>
    // BagelCode.Task.Condition.CheckContentUIEvent<int>
    // BagelCode.Task.Condition.CheckContentUIEvent<long>
    // BagelCode.Task.Condition.CheckContentUIEvent<object>
    // BagelCode.Task.Condition.CheckContentUIEvent<uint>
    // BagelCode.Task.Condition.CheckContentUIEvent<ulong>
    // BagelCode.Task.Condition.CheckContentUIEventValue<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckContentUIEventValue<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckContentUIEventValue<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckContentUIEventValue<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckContentUIEventValue<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckContentUIEventValue<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckContentUIEventValue<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckContentUIEventValue<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckContentUIEventValue<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckContentUIEventValue<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckContentUIEventValue<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckContentUIEventValue<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckContentUIEventValue<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckContentUIEventValue<byte>
    // BagelCode.Task.Condition.CheckContentUIEventValue<double>
    // BagelCode.Task.Condition.CheckContentUIEventValue<float>
    // BagelCode.Task.Condition.CheckContentUIEventValue<int>
    // BagelCode.Task.Condition.CheckContentUIEventValue<long>
    // BagelCode.Task.Condition.CheckContentUIEventValue<object>
    // BagelCode.Task.Condition.CheckContentUIEventValue<uint>
    // BagelCode.Task.Condition.CheckContentUIEventValue<ulong>
    // BagelCode.Task.Condition.CheckCreditEvent<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckCreditEvent<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckCreditEvent<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckCreditEvent<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckCreditEvent<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckCreditEvent<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckCreditEvent<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckCreditEvent<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckCreditEvent<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckCreditEvent<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckCreditEvent<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckCreditEvent<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckCreditEvent<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckCreditEvent<byte>
    // BagelCode.Task.Condition.CheckCreditEvent<double>
    // BagelCode.Task.Condition.CheckCreditEvent<float>
    // BagelCode.Task.Condition.CheckCreditEvent<int>
    // BagelCode.Task.Condition.CheckCreditEvent<long>
    // BagelCode.Task.Condition.CheckCreditEvent<object>
    // BagelCode.Task.Condition.CheckCreditEvent<uint>
    // BagelCode.Task.Condition.CheckCreditEvent<ulong>
    // BagelCode.Task.Condition.CheckCreditEventValue<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckCreditEventValue<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckCreditEventValue<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckCreditEventValue<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckCreditEventValue<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckCreditEventValue<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckCreditEventValue<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckCreditEventValue<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckCreditEventValue<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckCreditEventValue<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckCreditEventValue<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckCreditEventValue<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckCreditEventValue<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckCreditEventValue<byte>
    // BagelCode.Task.Condition.CheckCreditEventValue<double>
    // BagelCode.Task.Condition.CheckCreditEventValue<float>
    // BagelCode.Task.Condition.CheckCreditEventValue<int>
    // BagelCode.Task.Condition.CheckCreditEventValue<long>
    // BagelCode.Task.Condition.CheckCreditEventValue<object>
    // BagelCode.Task.Condition.CheckCreditEventValue<uint>
    // BagelCode.Task.Condition.CheckCreditEventValue<ulong>
    // BagelCode.Task.Condition.CheckMetaUIEvent<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckMetaUIEvent<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckMetaUIEvent<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckMetaUIEvent<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckMetaUIEvent<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckMetaUIEvent<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckMetaUIEvent<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckMetaUIEvent<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckMetaUIEvent<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckMetaUIEvent<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckMetaUIEvent<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckMetaUIEvent<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckMetaUIEvent<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckMetaUIEvent<byte>
    // BagelCode.Task.Condition.CheckMetaUIEvent<double>
    // BagelCode.Task.Condition.CheckMetaUIEvent<float>
    // BagelCode.Task.Condition.CheckMetaUIEvent<int>
    // BagelCode.Task.Condition.CheckMetaUIEvent<long>
    // BagelCode.Task.Condition.CheckMetaUIEvent<object>
    // BagelCode.Task.Condition.CheckMetaUIEvent<uint>
    // BagelCode.Task.Condition.CheckMetaUIEvent<ulong>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<byte>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<double>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<float>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<int>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<long>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<object>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<uint>
    // BagelCode.Task.Condition.CheckMetaUIEventValue<ulong>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<byte>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<double>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<float>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<int>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<long>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<object>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<uint>
    // BagelCode.Task.Condition.CheckSlotDetailEvent<ulong>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<byte>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<double>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<float>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<int>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<long>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<object>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<uint>
    // BagelCode.Task.Condition.CheckSlotDetailEventValue<ulong>
    // BagelCode.Task.Condition.CheckSlotEvent<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckSlotEvent<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckSlotEvent<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckSlotEvent<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckSlotEvent<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckSlotEvent<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckSlotEvent<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckSlotEvent<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckSlotEvent<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckSlotEvent<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckSlotEvent<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckSlotEvent<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckSlotEvent<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckSlotEvent<byte>
    // BagelCode.Task.Condition.CheckSlotEvent<double>
    // BagelCode.Task.Condition.CheckSlotEvent<float>
    // BagelCode.Task.Condition.CheckSlotEvent<int>
    // BagelCode.Task.Condition.CheckSlotEvent<long>
    // BagelCode.Task.Condition.CheckSlotEvent<object>
    // BagelCode.Task.Condition.CheckSlotEvent<uint>
    // BagelCode.Task.Condition.CheckSlotEvent<ulong>
    // BagelCode.Task.Condition.CheckSlotEventValue<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckSlotEventValue<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckSlotEventValue<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckSlotEventValue<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckSlotEventValue<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckSlotEventValue<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckSlotEventValue<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckSlotEventValue<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckSlotEventValue<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckSlotEventValue<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckSlotEventValue<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckSlotEventValue<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckSlotEventValue<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckSlotEventValue<byte>
    // BagelCode.Task.Condition.CheckSlotEventValue<double>
    // BagelCode.Task.Condition.CheckSlotEventValue<float>
    // BagelCode.Task.Condition.CheckSlotEventValue<int>
    // BagelCode.Task.Condition.CheckSlotEventValue<long>
    // BagelCode.Task.Condition.CheckSlotEventValue<object>
    // BagelCode.Task.Condition.CheckSlotEventValue<uint>
    // BagelCode.Task.Condition.CheckSlotEventValue<ulong>
    // BagelCode.Task.Condition.CheckSoundEvent<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckSoundEvent<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckSoundEvent<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckSoundEvent<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckSoundEvent<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckSoundEvent<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckSoundEvent<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckSoundEvent<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckSoundEvent<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckSoundEvent<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckSoundEvent<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckSoundEvent<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckSoundEvent<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckSoundEvent<byte>
    // BagelCode.Task.Condition.CheckSoundEvent<double>
    // BagelCode.Task.Condition.CheckSoundEvent<float>
    // BagelCode.Task.Condition.CheckSoundEvent<int>
    // BagelCode.Task.Condition.CheckSoundEvent<long>
    // BagelCode.Task.Condition.CheckSoundEvent<object>
    // BagelCode.Task.Condition.CheckSoundEvent<uint>
    // BagelCode.Task.Condition.CheckSoundEvent<ulong>
    // BagelCode.Task.Condition.CheckSoundEventValue<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckSoundEventValue<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckSoundEventValue<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckSoundEventValue<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckSoundEventValue<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckSoundEventValue<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckSoundEventValue<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckSoundEventValue<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckSoundEventValue<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckSoundEventValue<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckSoundEventValue<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckSoundEventValue<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckSoundEventValue<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckSoundEventValue<byte>
    // BagelCode.Task.Condition.CheckSoundEventValue<double>
    // BagelCode.Task.Condition.CheckSoundEventValue<float>
    // BagelCode.Task.Condition.CheckSoundEventValue<int>
    // BagelCode.Task.Condition.CheckSoundEventValue<long>
    // BagelCode.Task.Condition.CheckSoundEventValue<object>
    // BagelCode.Task.Condition.CheckSoundEventValue<uint>
    // BagelCode.Task.Condition.CheckSoundEventValue<ulong>
    // BagelCode.Task.Condition.CheckSymbolEvent<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckSymbolEvent<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckSymbolEvent<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckSymbolEvent<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckSymbolEvent<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckSymbolEvent<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckSymbolEvent<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckSymbolEvent<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckSymbolEvent<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckSymbolEvent<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckSymbolEvent<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckSymbolEvent<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckSymbolEvent<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckSymbolEvent<byte>
    // BagelCode.Task.Condition.CheckSymbolEvent<double>
    // BagelCode.Task.Condition.CheckSymbolEvent<float>
    // BagelCode.Task.Condition.CheckSymbolEvent<int>
    // BagelCode.Task.Condition.CheckSymbolEvent<long>
    // BagelCode.Task.Condition.CheckSymbolEvent<object>
    // BagelCode.Task.Condition.CheckSymbolEvent<uint>
    // BagelCode.Task.Condition.CheckSymbolEvent<ulong>
    // BagelCode.Task.Condition.CheckSymbolEventValue<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckSymbolEventValue<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckSymbolEventValue<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckSymbolEventValue<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckSymbolEventValue<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckSymbolEventValue<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckSymbolEventValue<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckSymbolEventValue<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckSymbolEventValue<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckSymbolEventValue<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckSymbolEventValue<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckSymbolEventValue<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckSymbolEventValue<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckSymbolEventValue<byte>
    // BagelCode.Task.Condition.CheckSymbolEventValue<double>
    // BagelCode.Task.Condition.CheckSymbolEventValue<float>
    // BagelCode.Task.Condition.CheckSymbolEventValue<int>
    // BagelCode.Task.Condition.CheckSymbolEventValue<long>
    // BagelCode.Task.Condition.CheckSymbolEventValue<object>
    // BagelCode.Task.Condition.CheckSymbolEventValue<uint>
    // BagelCode.Task.Condition.CheckSymbolEventValue<ulong>
    // BagelCode.Task.Condition.CheckSystemEvent<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckSystemEvent<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckSystemEvent<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckSystemEvent<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckSystemEvent<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckSystemEvent<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckSystemEvent<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckSystemEvent<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckSystemEvent<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckSystemEvent<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckSystemEvent<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckSystemEvent<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckSystemEvent<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckSystemEvent<byte>
    // BagelCode.Task.Condition.CheckSystemEvent<double>
    // BagelCode.Task.Condition.CheckSystemEvent<float>
    // BagelCode.Task.Condition.CheckSystemEvent<int>
    // BagelCode.Task.Condition.CheckSystemEvent<long>
    // BagelCode.Task.Condition.CheckSystemEvent<object>
    // BagelCode.Task.Condition.CheckSystemEvent<uint>
    // BagelCode.Task.Condition.CheckSystemEvent<ulong>
    // BagelCode.Task.Condition.CheckSystemEventValue<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckSystemEventValue<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckSystemEventValue<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckSystemEventValue<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckSystemEventValue<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckSystemEventValue<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckSystemEventValue<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckSystemEventValue<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckSystemEventValue<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckSystemEventValue<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckSystemEventValue<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckSystemEventValue<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckSystemEventValue<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckSystemEventValue<byte>
    // BagelCode.Task.Condition.CheckSystemEventValue<double>
    // BagelCode.Task.Condition.CheckSystemEventValue<float>
    // BagelCode.Task.Condition.CheckSystemEventValue<int>
    // BagelCode.Task.Condition.CheckSystemEventValue<long>
    // BagelCode.Task.Condition.CheckSystemEventValue<object>
    // BagelCode.Task.Condition.CheckSystemEventValue<uint>
    // BagelCode.Task.Condition.CheckSystemEventValue<ulong>
    // BagelCode.Task.Condition.CheckWinEvent<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckWinEvent<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckWinEvent<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckWinEvent<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckWinEvent<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckWinEvent<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckWinEvent<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckWinEvent<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckWinEvent<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckWinEvent<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckWinEvent<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckWinEvent<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckWinEvent<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckWinEvent<byte>
    // BagelCode.Task.Condition.CheckWinEvent<double>
    // BagelCode.Task.Condition.CheckWinEvent<float>
    // BagelCode.Task.Condition.CheckWinEvent<int>
    // BagelCode.Task.Condition.CheckWinEvent<long>
    // BagelCode.Task.Condition.CheckWinEvent<object>
    // BagelCode.Task.Condition.CheckWinEvent<uint>
    // BagelCode.Task.Condition.CheckWinEvent<ulong>
    // BagelCode.Task.Condition.CheckWinEventList<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckWinEventList<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckWinEventList<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckWinEventList<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckWinEventList<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckWinEventList<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckWinEventList<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckWinEventList<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckWinEventList<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckWinEventList<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckWinEventList<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckWinEventList<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckWinEventList<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckWinEventList<byte>
    // BagelCode.Task.Condition.CheckWinEventList<double>
    // BagelCode.Task.Condition.CheckWinEventList<float>
    // BagelCode.Task.Condition.CheckWinEventList<int>
    // BagelCode.Task.Condition.CheckWinEventList<long>
    // BagelCode.Task.Condition.CheckWinEventList<object>
    // BagelCode.Task.Condition.CheckWinEventList<uint>
    // BagelCode.Task.Condition.CheckWinEventList<ulong>
    // BagelCode.Task.Condition.CheckWinEventValue<UnityEngine.Bounds>
    // BagelCode.Task.Condition.CheckWinEventValue<UnityEngine.Color>
    // BagelCode.Task.Condition.CheckWinEventValue<UnityEngine.ContactPoint2D>
    // BagelCode.Task.Condition.CheckWinEventValue<UnityEngine.ContactPoint>
    // BagelCode.Task.Condition.CheckWinEventValue<UnityEngine.Keyframe>
    // BagelCode.Task.Condition.CheckWinEventValue<UnityEngine.Quaternion>
    // BagelCode.Task.Condition.CheckWinEventValue<UnityEngine.Ray>
    // BagelCode.Task.Condition.CheckWinEventValue<UnityEngine.RaycastHit2D>
    // BagelCode.Task.Condition.CheckWinEventValue<UnityEngine.RaycastHit>
    // BagelCode.Task.Condition.CheckWinEventValue<UnityEngine.Rect>
    // BagelCode.Task.Condition.CheckWinEventValue<UnityEngine.Vector2>
    // BagelCode.Task.Condition.CheckWinEventValue<UnityEngine.Vector3>
    // BagelCode.Task.Condition.CheckWinEventValue<UnityEngine.Vector4>
    // BagelCode.Task.Condition.CheckWinEventValue<byte>
    // BagelCode.Task.Condition.CheckWinEventValue<double>
    // BagelCode.Task.Condition.CheckWinEventValue<float>
    // BagelCode.Task.Condition.CheckWinEventValue<int>
    // BagelCode.Task.Condition.CheckWinEventValue<long>
    // BagelCode.Task.Condition.CheckWinEventValue<object>
    // BagelCode.Task.Condition.CheckWinEventValue<uint>
    // BagelCode.Task.Condition.CheckWinEventValue<ulong>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<UnityEngine.Bounds>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<UnityEngine.Color>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<UnityEngine.ContactPoint2D>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<UnityEngine.ContactPoint>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<UnityEngine.Keyframe>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<UnityEngine.Quaternion>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<UnityEngine.Ray>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<UnityEngine.RaycastHit2D>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<UnityEngine.RaycastHit>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<UnityEngine.Rect>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<UnityEngine.Vector2>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<UnityEngine.Vector3>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<UnityEngine.Vector4>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<byte>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<double>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<float>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<int>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<long>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<object>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<uint>
    // BagelCode.Tasks.Actions.SendMetaUIEvent<ulong>
    // Com.ForbiddenByte.OSA.Core.OSA.<>c__DisplayClass199_0<object,object>
    // Com.ForbiddenByte.OSA.Core.OSA.<SmoothScrollProgressCoroutine>d__199<object,object>
    // Com.ForbiddenByte.OSA.Core.OSA<object,object>
    // Com.ForbiddenByte.OSA.Core.SubComponents.ComputeVisibilityManager<object,object>
    // Com.ForbiddenByte.OSA.Core.SubComponents.InternalState<object>
    // Com.ForbiddenByte.OSA.Core.SubComponents.List.ListNavigationManager<object,object>
    // Com.ForbiddenByte.OSA.Core.SubComponents.NavigationManager<object,object>
    // Com.ForbiddenByte.OSA.Core.SubComponents.NestingManager<object,object>
    // Com.ForbiddenByte.OSA.Core.SubComponents.ReleaseFromPullManager<object,object>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<UnityEngine.Bounds>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<UnityEngine.Color>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<UnityEngine.ContactPoint2D>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<UnityEngine.ContactPoint>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<UnityEngine.Keyframe>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<UnityEngine.Quaternion>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<UnityEngine.Ray>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<UnityEngine.RaycastHit2D>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<UnityEngine.RaycastHit>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<UnityEngine.Rect>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<UnityEngine.Vector2>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<UnityEngine.Vector3>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<UnityEngine.Vector4>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<byte>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<double>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<float>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<int>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<long>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<object>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<uint>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass14_0<ulong>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<UnityEngine.Bounds>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<UnityEngine.Color>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<UnityEngine.ContactPoint2D>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<UnityEngine.ContactPoint>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<UnityEngine.Keyframe>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<UnityEngine.Quaternion>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<UnityEngine.Ray>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<UnityEngine.RaycastHit2D>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<UnityEngine.RaycastHit>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<UnityEngine.Rect>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<UnityEngine.Vector2>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<UnityEngine.Vector3>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<UnityEngine.Vector4>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<byte>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<double>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<float>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<int>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<long>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<object>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<uint>
    // NodeCanvas.Framework.BBParameter.<>c__DisplayClass15_0<ulong>
    // NodeCanvas.Framework.BBParameter<UnityEngine.Bounds>
    // NodeCanvas.Framework.BBParameter<UnityEngine.Color>
    // NodeCanvas.Framework.BBParameter<UnityEngine.ContactPoint2D>
    // NodeCanvas.Framework.BBParameter<UnityEngine.ContactPoint>
    // NodeCanvas.Framework.BBParameter<UnityEngine.Keyframe>
    // NodeCanvas.Framework.BBParameter<UnityEngine.Quaternion>
    // NodeCanvas.Framework.BBParameter<UnityEngine.Ray>
    // NodeCanvas.Framework.BBParameter<UnityEngine.RaycastHit2D>
    // NodeCanvas.Framework.BBParameter<UnityEngine.RaycastHit>
    // NodeCanvas.Framework.BBParameter<UnityEngine.Rect>
    // NodeCanvas.Framework.BBParameter<UnityEngine.Vector2>
    // NodeCanvas.Framework.BBParameter<UnityEngine.Vector3>
    // NodeCanvas.Framework.BBParameter<UnityEngine.Vector4>
    // NodeCanvas.Framework.BBParameter<byte>
    // NodeCanvas.Framework.BBParameter<double>
    // NodeCanvas.Framework.BBParameter<float>
    // NodeCanvas.Framework.BBParameter<int>
    // NodeCanvas.Framework.BBParameter<long>
    // NodeCanvas.Framework.BBParameter<object>
    // NodeCanvas.Framework.BBParameter<uint>
    // NodeCanvas.Framework.BBParameter<ulong>
    // NodeCanvas.Framework.ITaskAssignable<object>
    // NodeCanvas.Framework.Internal.ReflectedAction<UnityEngine.Bounds>
    // NodeCanvas.Framework.Internal.ReflectedAction<UnityEngine.Color>
    // NodeCanvas.Framework.Internal.ReflectedAction<UnityEngine.ContactPoint2D>
    // NodeCanvas.Framework.Internal.ReflectedAction<UnityEngine.ContactPoint>
    // NodeCanvas.Framework.Internal.ReflectedAction<UnityEngine.Keyframe>
    // NodeCanvas.Framework.Internal.ReflectedAction<UnityEngine.Quaternion>
    // NodeCanvas.Framework.Internal.ReflectedAction<UnityEngine.Ray>
    // NodeCanvas.Framework.Internal.ReflectedAction<UnityEngine.RaycastHit2D>
    // NodeCanvas.Framework.Internal.ReflectedAction<UnityEngine.RaycastHit>
    // NodeCanvas.Framework.Internal.ReflectedAction<UnityEngine.Rect>
    // NodeCanvas.Framework.Internal.ReflectedAction<UnityEngine.Vector2>
    // NodeCanvas.Framework.Internal.ReflectedAction<UnityEngine.Vector3>
    // NodeCanvas.Framework.Internal.ReflectedAction<UnityEngine.Vector4>
    // NodeCanvas.Framework.Internal.ReflectedAction<byte>
    // NodeCanvas.Framework.Internal.ReflectedAction<double>
    // NodeCanvas.Framework.Internal.ReflectedAction<float>
    // NodeCanvas.Framework.Internal.ReflectedAction<int>
    // NodeCanvas.Framework.Internal.ReflectedAction<long>
    // NodeCanvas.Framework.Internal.ReflectedAction<object>
    // NodeCanvas.Framework.Internal.ReflectedAction<uint>
    // NodeCanvas.Framework.Internal.ReflectedAction<ulong>
    // NodeCanvas.Framework.Internal.ReflectedFunction<UnityEngine.Bounds>
    // NodeCanvas.Framework.Internal.ReflectedFunction<UnityEngine.Color>
    // NodeCanvas.Framework.Internal.ReflectedFunction<UnityEngine.ContactPoint2D>
    // NodeCanvas.Framework.Internal.ReflectedFunction<UnityEngine.ContactPoint>
    // NodeCanvas.Framework.Internal.ReflectedFunction<UnityEngine.Keyframe>
    // NodeCanvas.Framework.Internal.ReflectedFunction<UnityEngine.Quaternion>
    // NodeCanvas.Framework.Internal.ReflectedFunction<UnityEngine.Ray>
    // NodeCanvas.Framework.Internal.ReflectedFunction<UnityEngine.RaycastHit2D>
    // NodeCanvas.Framework.Internal.ReflectedFunction<UnityEngine.RaycastHit>
    // NodeCanvas.Framework.Internal.ReflectedFunction<UnityEngine.Rect>
    // NodeCanvas.Framework.Internal.ReflectedFunction<UnityEngine.Vector2>
    // NodeCanvas.Framework.Internal.ReflectedFunction<UnityEngine.Vector3>
    // NodeCanvas.Framework.Internal.ReflectedFunction<UnityEngine.Vector4>
    // NodeCanvas.Framework.Internal.ReflectedFunction<byte>
    // NodeCanvas.Framework.Internal.ReflectedFunction<double>
    // NodeCanvas.Framework.Internal.ReflectedFunction<float>
    // NodeCanvas.Framework.Internal.ReflectedFunction<int>
    // NodeCanvas.Framework.Internal.ReflectedFunction<long>
    // NodeCanvas.Framework.Internal.ReflectedFunction<object>
    // NodeCanvas.Framework.Internal.ReflectedFunction<uint>
    // NodeCanvas.Framework.Internal.ReflectedFunction<ulong>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<UnityEngine.Bounds>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<UnityEngine.Color>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<UnityEngine.ContactPoint2D>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<UnityEngine.ContactPoint>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<UnityEngine.Keyframe>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<UnityEngine.Quaternion>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<UnityEngine.Ray>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<UnityEngine.RaycastHit2D>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<UnityEngine.RaycastHit>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<UnityEngine.Rect>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<UnityEngine.Vector2>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<UnityEngine.Vector3>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<UnityEngine.Vector4>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<byte>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<double>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<float>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<int>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<long>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<object>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<uint>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.ActionCall<ulong>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<UnityEngine.Bounds>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<UnityEngine.Color>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<UnityEngine.ContactPoint2D>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<UnityEngine.ContactPoint>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<UnityEngine.Keyframe>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<UnityEngine.Quaternion>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<UnityEngine.Ray>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<UnityEngine.RaycastHit2D>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<UnityEngine.RaycastHit>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<UnityEngine.Rect>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<UnityEngine.Vector2>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<UnityEngine.Vector3>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<UnityEngine.Vector4>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<byte>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<double>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<float>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<int>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<long>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<object>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<uint>
    // NodeCanvas.Framework.Internal.ReflectedWrapper.FunctionCall<ulong>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<UnityEngine.Bounds>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<UnityEngine.Color>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<UnityEngine.ContactPoint2D>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<UnityEngine.ContactPoint>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<UnityEngine.Keyframe>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<UnityEngine.Quaternion>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<UnityEngine.Ray>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<UnityEngine.RaycastHit2D>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<UnityEngine.RaycastHit>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<UnityEngine.Rect>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<UnityEngine.Vector2>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<UnityEngine.Vector3>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<UnityEngine.Vector4>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<byte>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<double>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<float>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<int>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<long>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<object>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<uint>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_0<ulong>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<UnityEngine.Bounds>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<UnityEngine.Color>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<UnityEngine.ContactPoint2D>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<UnityEngine.ContactPoint>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<UnityEngine.Keyframe>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<UnityEngine.Quaternion>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<UnityEngine.Ray>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<UnityEngine.RaycastHit2D>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<UnityEngine.RaycastHit>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<UnityEngine.Rect>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<UnityEngine.Vector2>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<UnityEngine.Vector3>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<UnityEngine.Vector4>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<byte>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<double>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<float>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<int>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<long>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<object>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<uint>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_1<ulong>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<UnityEngine.Bounds>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<UnityEngine.Color>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<UnityEngine.ContactPoint2D>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<UnityEngine.ContactPoint>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<UnityEngine.Keyframe>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<UnityEngine.Quaternion>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<UnityEngine.Ray>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<UnityEngine.RaycastHit2D>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<UnityEngine.RaycastHit>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<UnityEngine.Rect>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<UnityEngine.Vector2>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<UnityEngine.Vector3>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<UnityEngine.Vector4>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<byte>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<double>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<float>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<int>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<long>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<object>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<uint>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_2<ulong>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<UnityEngine.Bounds>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<UnityEngine.Color>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<UnityEngine.ContactPoint2D>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<UnityEngine.ContactPoint>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<UnityEngine.Keyframe>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<UnityEngine.Quaternion>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<UnityEngine.Ray>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<UnityEngine.RaycastHit2D>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<UnityEngine.RaycastHit>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<UnityEngine.Rect>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<UnityEngine.Vector2>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<UnityEngine.Vector3>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<UnityEngine.Vector4>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<byte>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<double>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<float>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<int>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<long>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<object>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<uint>
    // NodeCanvas.Framework.Variable.<>c__DisplayClass22_3<ulong>
    // NodeCanvas.Framework.Variable<UnityEngine.Bounds>
    // NodeCanvas.Framework.Variable<UnityEngine.Color>
    // NodeCanvas.Framework.Variable<UnityEngine.ContactPoint2D>
    // NodeCanvas.Framework.Variable<UnityEngine.ContactPoint>
    // NodeCanvas.Framework.Variable<UnityEngine.Keyframe>
    // NodeCanvas.Framework.Variable<UnityEngine.Quaternion>
    // NodeCanvas.Framework.Variable<UnityEngine.Ray>
    // NodeCanvas.Framework.Variable<UnityEngine.RaycastHit2D>
    // NodeCanvas.Framework.Variable<UnityEngine.RaycastHit>
    // NodeCanvas.Framework.Variable<UnityEngine.Rect>
    // NodeCanvas.Framework.Variable<UnityEngine.Vector2>
    // NodeCanvas.Framework.Variable<UnityEngine.Vector3>
    // NodeCanvas.Framework.Variable<UnityEngine.Vector4>
    // NodeCanvas.Framework.Variable<byte>
    // NodeCanvas.Framework.Variable<double>
    // NodeCanvas.Framework.Variable<float>
    // NodeCanvas.Framework.Variable<int>
    // NodeCanvas.Framework.Variable<long>
    // NodeCanvas.Framework.Variable<object>
    // NodeCanvas.Framework.Variable<uint>
    // NodeCanvas.Framework.Variable<ulong>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<UnityEngine.Color>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<UnityEngine.Ray>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<UnityEngine.Rect>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<byte>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<double>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<float>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<int>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<long>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<object>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<uint>
    // NodeCanvas.Tasks.Actions.AddElementToDictionary<ulong>
    // NodeCanvas.Tasks.Actions.AddElementToList<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Actions.AddElementToList<UnityEngine.Color>
    // NodeCanvas.Tasks.Actions.AddElementToList<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Actions.AddElementToList<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Actions.AddElementToList<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Actions.AddElementToList<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Actions.AddElementToList<UnityEngine.Ray>
    // NodeCanvas.Tasks.Actions.AddElementToList<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Actions.AddElementToList<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Actions.AddElementToList<UnityEngine.Rect>
    // NodeCanvas.Tasks.Actions.AddElementToList<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Actions.AddElementToList<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Actions.AddElementToList<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Actions.AddElementToList<byte>
    // NodeCanvas.Tasks.Actions.AddElementToList<double>
    // NodeCanvas.Tasks.Actions.AddElementToList<float>
    // NodeCanvas.Tasks.Actions.AddElementToList<int>
    // NodeCanvas.Tasks.Actions.AddElementToList<long>
    // NodeCanvas.Tasks.Actions.AddElementToList<object>
    // NodeCanvas.Tasks.Actions.AddElementToList<uint>
    // NodeCanvas.Tasks.Actions.AddElementToList<ulong>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<UnityEngine.Color>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<UnityEngine.Ray>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<UnityEngine.Rect>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<byte>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<double>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<float>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<int>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<long>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<object>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<uint>
    // NodeCanvas.Tasks.Actions.GetDictionaryElement<ulong>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<UnityEngine.Color>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<UnityEngine.Ray>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<UnityEngine.Rect>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<byte>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<double>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<float>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<int>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<long>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<object>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<uint>
    // NodeCanvas.Tasks.Actions.GetIndexOfElement<ulong>
    // NodeCanvas.Tasks.Actions.InsertElementToList<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Actions.InsertElementToList<UnityEngine.Color>
    // NodeCanvas.Tasks.Actions.InsertElementToList<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Actions.InsertElementToList<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Actions.InsertElementToList<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Actions.InsertElementToList<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Actions.InsertElementToList<UnityEngine.Ray>
    // NodeCanvas.Tasks.Actions.InsertElementToList<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Actions.InsertElementToList<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Actions.InsertElementToList<UnityEngine.Rect>
    // NodeCanvas.Tasks.Actions.InsertElementToList<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Actions.InsertElementToList<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Actions.InsertElementToList<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Actions.InsertElementToList<byte>
    // NodeCanvas.Tasks.Actions.InsertElementToList<double>
    // NodeCanvas.Tasks.Actions.InsertElementToList<float>
    // NodeCanvas.Tasks.Actions.InsertElementToList<int>
    // NodeCanvas.Tasks.Actions.InsertElementToList<long>
    // NodeCanvas.Tasks.Actions.InsertElementToList<object>
    // NodeCanvas.Tasks.Actions.InsertElementToList<uint>
    // NodeCanvas.Tasks.Actions.InsertElementToList<ulong>
    // NodeCanvas.Tasks.Actions.PickListElement<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Actions.PickListElement<UnityEngine.Color>
    // NodeCanvas.Tasks.Actions.PickListElement<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Actions.PickListElement<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Actions.PickListElement<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Actions.PickListElement<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Actions.PickListElement<UnityEngine.Ray>
    // NodeCanvas.Tasks.Actions.PickListElement<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Actions.PickListElement<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Actions.PickListElement<UnityEngine.Rect>
    // NodeCanvas.Tasks.Actions.PickListElement<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Actions.PickListElement<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Actions.PickListElement<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Actions.PickListElement<byte>
    // NodeCanvas.Tasks.Actions.PickListElement<double>
    // NodeCanvas.Tasks.Actions.PickListElement<float>
    // NodeCanvas.Tasks.Actions.PickListElement<int>
    // NodeCanvas.Tasks.Actions.PickListElement<long>
    // NodeCanvas.Tasks.Actions.PickListElement<object>
    // NodeCanvas.Tasks.Actions.PickListElement<uint>
    // NodeCanvas.Tasks.Actions.PickListElement<ulong>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<UnityEngine.Color>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<UnityEngine.Ray>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<UnityEngine.Rect>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<byte>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<double>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<float>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<int>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<long>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<object>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<uint>
    // NodeCanvas.Tasks.Actions.PickRandomListElement<ulong>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<UnityEngine.Color>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<UnityEngine.Ray>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<UnityEngine.Rect>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<byte>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<double>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<float>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<int>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<long>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<object>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<uint>
    // NodeCanvas.Tasks.Actions.RemoveElementFromList<ulong>
    // NodeCanvas.Tasks.Actions.SendEvent<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Actions.SendEvent<UnityEngine.Color>
    // NodeCanvas.Tasks.Actions.SendEvent<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Actions.SendEvent<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Actions.SendEvent<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Actions.SendEvent<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Actions.SendEvent<UnityEngine.Ray>
    // NodeCanvas.Tasks.Actions.SendEvent<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Actions.SendEvent<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Actions.SendEvent<UnityEngine.Rect>
    // NodeCanvas.Tasks.Actions.SendEvent<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Actions.SendEvent<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Actions.SendEvent<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Actions.SendEvent<byte>
    // NodeCanvas.Tasks.Actions.SendEvent<double>
    // NodeCanvas.Tasks.Actions.SendEvent<float>
    // NodeCanvas.Tasks.Actions.SendEvent<int>
    // NodeCanvas.Tasks.Actions.SendEvent<long>
    // NodeCanvas.Tasks.Actions.SendEvent<object>
    // NodeCanvas.Tasks.Actions.SendEvent<uint>
    // NodeCanvas.Tasks.Actions.SendEvent<ulong>
    // NodeCanvas.Tasks.Actions.SendEventList<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Actions.SendEventList<UnityEngine.Color>
    // NodeCanvas.Tasks.Actions.SendEventList<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Actions.SendEventList<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Actions.SendEventList<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Actions.SendEventList<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Actions.SendEventList<UnityEngine.Ray>
    // NodeCanvas.Tasks.Actions.SendEventList<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Actions.SendEventList<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Actions.SendEventList<UnityEngine.Rect>
    // NodeCanvas.Tasks.Actions.SendEventList<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Actions.SendEventList<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Actions.SendEventList<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Actions.SendEventList<byte>
    // NodeCanvas.Tasks.Actions.SendEventList<double>
    // NodeCanvas.Tasks.Actions.SendEventList<float>
    // NodeCanvas.Tasks.Actions.SendEventList<int>
    // NodeCanvas.Tasks.Actions.SendEventList<long>
    // NodeCanvas.Tasks.Actions.SendEventList<object>
    // NodeCanvas.Tasks.Actions.SendEventList<uint>
    // NodeCanvas.Tasks.Actions.SendEventList<ulong>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<UnityEngine.Color>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<UnityEngine.Ray>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<UnityEngine.Rect>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<byte>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<double>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<float>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<int>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<long>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<object>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<uint>
    // NodeCanvas.Tasks.Actions.SendEventToObjects<ulong>
    // NodeCanvas.Tasks.Actions.SendMessage<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Actions.SendMessage<UnityEngine.Color>
    // NodeCanvas.Tasks.Actions.SendMessage<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Actions.SendMessage<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Actions.SendMessage<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Actions.SendMessage<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Actions.SendMessage<UnityEngine.Ray>
    // NodeCanvas.Tasks.Actions.SendMessage<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Actions.SendMessage<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Actions.SendMessage<UnityEngine.Rect>
    // NodeCanvas.Tasks.Actions.SendMessage<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Actions.SendMessage<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Actions.SendMessage<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Actions.SendMessage<byte>
    // NodeCanvas.Tasks.Actions.SendMessage<double>
    // NodeCanvas.Tasks.Actions.SendMessage<float>
    // NodeCanvas.Tasks.Actions.SendMessage<int>
    // NodeCanvas.Tasks.Actions.SendMessage<long>
    // NodeCanvas.Tasks.Actions.SendMessage<object>
    // NodeCanvas.Tasks.Actions.SendMessage<uint>
    // NodeCanvas.Tasks.Actions.SendMessage<ulong>
    // NodeCanvas.Tasks.Actions.SetListElement<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Actions.SetListElement<UnityEngine.Color>
    // NodeCanvas.Tasks.Actions.SetListElement<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Actions.SetListElement<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Actions.SetListElement<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Actions.SetListElement<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Actions.SetListElement<UnityEngine.Ray>
    // NodeCanvas.Tasks.Actions.SetListElement<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Actions.SetListElement<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Actions.SetListElement<UnityEngine.Rect>
    // NodeCanvas.Tasks.Actions.SetListElement<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Actions.SetListElement<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Actions.SetListElement<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Actions.SetListElement<byte>
    // NodeCanvas.Tasks.Actions.SetListElement<double>
    // NodeCanvas.Tasks.Actions.SetListElement<float>
    // NodeCanvas.Tasks.Actions.SetListElement<int>
    // NodeCanvas.Tasks.Actions.SetListElement<long>
    // NodeCanvas.Tasks.Actions.SetListElement<object>
    // NodeCanvas.Tasks.Actions.SetListElement<uint>
    // NodeCanvas.Tasks.Actions.SetListElement<ulong>
    // NodeCanvas.Tasks.Actions.SetVariable<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Actions.SetVariable<UnityEngine.Color>
    // NodeCanvas.Tasks.Actions.SetVariable<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Actions.SetVariable<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Actions.SetVariable<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Actions.SetVariable<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Actions.SetVariable<UnityEngine.Ray>
    // NodeCanvas.Tasks.Actions.SetVariable<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Actions.SetVariable<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Actions.SetVariable<UnityEngine.Rect>
    // NodeCanvas.Tasks.Actions.SetVariable<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Actions.SetVariable<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Actions.SetVariable<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Actions.SetVariable<byte>
    // NodeCanvas.Tasks.Actions.SetVariable<double>
    // NodeCanvas.Tasks.Actions.SetVariable<float>
    // NodeCanvas.Tasks.Actions.SetVariable<int>
    // NodeCanvas.Tasks.Actions.SetVariable<long>
    // NodeCanvas.Tasks.Actions.SetVariable<object>
    // NodeCanvas.Tasks.Actions.SetVariable<uint>
    // NodeCanvas.Tasks.Actions.SetVariable<ulong>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<UnityEngine.Color>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<UnityEngine.Ray>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<UnityEngine.Rect>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<byte>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<double>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<float>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<int>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<long>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<object>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<uint>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEvent<ulong>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<UnityEngine.Color>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<UnityEngine.Ray>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<UnityEngine.Rect>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<byte>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<double>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<float>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<int>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<long>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<object>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<uint>
    // NodeCanvas.Tasks.Conditions.CheckCSharpEventValue<ulong>
    // NodeCanvas.Tasks.Conditions.CheckEvent<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Conditions.CheckEvent<UnityEngine.Color>
    // NodeCanvas.Tasks.Conditions.CheckEvent<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Conditions.CheckEvent<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Conditions.CheckEvent<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Conditions.CheckEvent<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Conditions.CheckEvent<UnityEngine.Ray>
    // NodeCanvas.Tasks.Conditions.CheckEvent<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Conditions.CheckEvent<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Conditions.CheckEvent<UnityEngine.Rect>
    // NodeCanvas.Tasks.Conditions.CheckEvent<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Conditions.CheckEvent<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Conditions.CheckEvent<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Conditions.CheckEvent<byte>
    // NodeCanvas.Tasks.Conditions.CheckEvent<double>
    // NodeCanvas.Tasks.Conditions.CheckEvent<float>
    // NodeCanvas.Tasks.Conditions.CheckEvent<int>
    // NodeCanvas.Tasks.Conditions.CheckEvent<long>
    // NodeCanvas.Tasks.Conditions.CheckEvent<object>
    // NodeCanvas.Tasks.Conditions.CheckEvent<uint>
    // NodeCanvas.Tasks.Conditions.CheckEvent<ulong>
    // NodeCanvas.Tasks.Conditions.CheckEventList<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Conditions.CheckEventList<UnityEngine.Color>
    // NodeCanvas.Tasks.Conditions.CheckEventList<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Conditions.CheckEventList<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Conditions.CheckEventList<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Conditions.CheckEventList<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Conditions.CheckEventList<UnityEngine.Ray>
    // NodeCanvas.Tasks.Conditions.CheckEventList<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Conditions.CheckEventList<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Conditions.CheckEventList<UnityEngine.Rect>
    // NodeCanvas.Tasks.Conditions.CheckEventList<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Conditions.CheckEventList<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Conditions.CheckEventList<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Conditions.CheckEventList<byte>
    // NodeCanvas.Tasks.Conditions.CheckEventList<double>
    // NodeCanvas.Tasks.Conditions.CheckEventList<float>
    // NodeCanvas.Tasks.Conditions.CheckEventList<int>
    // NodeCanvas.Tasks.Conditions.CheckEventList<long>
    // NodeCanvas.Tasks.Conditions.CheckEventList<object>
    // NodeCanvas.Tasks.Conditions.CheckEventList<uint>
    // NodeCanvas.Tasks.Conditions.CheckEventList<ulong>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<UnityEngine.Color>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<UnityEngine.Ray>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<UnityEngine.Rect>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<byte>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<double>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<float>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<int>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<long>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<object>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<uint>
    // NodeCanvas.Tasks.Conditions.CheckEventValue<ulong>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<UnityEngine.Color>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<UnityEngine.Ray>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<UnityEngine.Rect>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<byte>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<double>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<float>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<int>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<long>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<object>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<uint>
    // NodeCanvas.Tasks.Conditions.CheckStaticCSharpEvent<ulong>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<UnityEngine.Color>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<UnityEngine.Ray>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<UnityEngine.Rect>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<byte>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<double>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<float>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<int>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<long>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<object>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<uint>
    // NodeCanvas.Tasks.Conditions.CheckUnityEvent<ulong>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<UnityEngine.Color>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<UnityEngine.Ray>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<UnityEngine.Rect>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<byte>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<double>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<float>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<int>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<long>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<object>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<uint>
    // NodeCanvas.Tasks.Conditions.CheckUnityEventValue<ulong>
    // NodeCanvas.Tasks.Conditions.CheckVariable<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Conditions.CheckVariable<UnityEngine.Color>
    // NodeCanvas.Tasks.Conditions.CheckVariable<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Conditions.CheckVariable<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Conditions.CheckVariable<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Conditions.CheckVariable<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Conditions.CheckVariable<UnityEngine.Ray>
    // NodeCanvas.Tasks.Conditions.CheckVariable<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Conditions.CheckVariable<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Conditions.CheckVariable<UnityEngine.Rect>
    // NodeCanvas.Tasks.Conditions.CheckVariable<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Conditions.CheckVariable<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Conditions.CheckVariable<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Conditions.CheckVariable<byte>
    // NodeCanvas.Tasks.Conditions.CheckVariable<double>
    // NodeCanvas.Tasks.Conditions.CheckVariable<float>
    // NodeCanvas.Tasks.Conditions.CheckVariable<int>
    // NodeCanvas.Tasks.Conditions.CheckVariable<long>
    // NodeCanvas.Tasks.Conditions.CheckVariable<object>
    // NodeCanvas.Tasks.Conditions.CheckVariable<uint>
    // NodeCanvas.Tasks.Conditions.CheckVariable<ulong>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<UnityEngine.Color>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<UnityEngine.Ray>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<UnityEngine.Rect>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<byte>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<double>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<float>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<int>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<long>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<object>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<uint>
    // NodeCanvas.Tasks.Conditions.ListContainsElement<ulong>
    // NodeCanvas.Tasks.Conditions.TryGetValue<UnityEngine.Bounds>
    // NodeCanvas.Tasks.Conditions.TryGetValue<UnityEngine.Color>
    // NodeCanvas.Tasks.Conditions.TryGetValue<UnityEngine.ContactPoint2D>
    // NodeCanvas.Tasks.Conditions.TryGetValue<UnityEngine.ContactPoint>
    // NodeCanvas.Tasks.Conditions.TryGetValue<UnityEngine.Keyframe>
    // NodeCanvas.Tasks.Conditions.TryGetValue<UnityEngine.Quaternion>
    // NodeCanvas.Tasks.Conditions.TryGetValue<UnityEngine.Ray>
    // NodeCanvas.Tasks.Conditions.TryGetValue<UnityEngine.RaycastHit2D>
    // NodeCanvas.Tasks.Conditions.TryGetValue<UnityEngine.RaycastHit>
    // NodeCanvas.Tasks.Conditions.TryGetValue<UnityEngine.Rect>
    // NodeCanvas.Tasks.Conditions.TryGetValue<UnityEngine.Vector2>
    // NodeCanvas.Tasks.Conditions.TryGetValue<UnityEngine.Vector3>
    // NodeCanvas.Tasks.Conditions.TryGetValue<UnityEngine.Vector4>
    // NodeCanvas.Tasks.Conditions.TryGetValue<byte>
    // NodeCanvas.Tasks.Conditions.TryGetValue<double>
    // NodeCanvas.Tasks.Conditions.TryGetValue<float>
    // NodeCanvas.Tasks.Conditions.TryGetValue<int>
    // NodeCanvas.Tasks.Conditions.TryGetValue<long>
    // NodeCanvas.Tasks.Conditions.TryGetValue<object>
    // NodeCanvas.Tasks.Conditions.TryGetValue<uint>
    // NodeCanvas.Tasks.Conditions.TryGetValue<ulong>
    // ParadoxNotion.EventData<System.Collections.Generic.KeyValuePair<object,object>>
    // ParadoxNotion.EventData<System.ValueTuple<byte,object>>
    // ParadoxNotion.EventData<System.ValueTuple<object,object,int,long>>
    // ParadoxNotion.EventData<UnityEngine.Bounds>
    // ParadoxNotion.EventData<UnityEngine.Color>
    // ParadoxNotion.EventData<UnityEngine.ContactPoint2D>
    // ParadoxNotion.EventData<UnityEngine.ContactPoint>
    // ParadoxNotion.EventData<UnityEngine.Keyframe>
    // ParadoxNotion.EventData<UnityEngine.Quaternion>
    // ParadoxNotion.EventData<UnityEngine.Ray>
    // ParadoxNotion.EventData<UnityEngine.RaycastHit2D>
    // ParadoxNotion.EventData<UnityEngine.RaycastHit>
    // ParadoxNotion.EventData<UnityEngine.Rect>
    // ParadoxNotion.EventData<UnityEngine.Vector2>
    // ParadoxNotion.EventData<UnityEngine.Vector3>
    // ParadoxNotion.EventData<UnityEngine.Vector4>
    // ParadoxNotion.EventData<byte>
    // ParadoxNotion.EventData<double>
    // ParadoxNotion.EventData<float>
    // ParadoxNotion.EventData<int>
    // ParadoxNotion.EventData<long>
    // ParadoxNotion.EventData<object>
    // ParadoxNotion.EventData<uint>
    // ParadoxNotion.EventData<ulong>
    // ParadoxNotion.Services.MessageRouter.MessageData<object>
    // SlotMaker.BlackboardUtils.SerializeToBB<object>
    // SlotMaker.IContextListenable<UnityEngine.Bounds>
    // SlotMaker.IContextListenable<UnityEngine.Color>
    // SlotMaker.IContextListenable<UnityEngine.ContactPoint2D>
    // SlotMaker.IContextListenable<UnityEngine.ContactPoint>
    // SlotMaker.IContextListenable<UnityEngine.Keyframe>
    // SlotMaker.IContextListenable<UnityEngine.Quaternion>
    // SlotMaker.IContextListenable<UnityEngine.Ray>
    // SlotMaker.IContextListenable<UnityEngine.RaycastHit2D>
    // SlotMaker.IContextListenable<UnityEngine.RaycastHit>
    // SlotMaker.IContextListenable<UnityEngine.Rect>
    // SlotMaker.IContextListenable<UnityEngine.Vector2>
    // SlotMaker.IContextListenable<UnityEngine.Vector3>
    // SlotMaker.IContextListenable<UnityEngine.Vector4>
    // SlotMaker.IContextListenable<byte>
    // SlotMaker.IContextListenable<double>
    // SlotMaker.IContextListenable<float>
    // SlotMaker.IContextListenable<int>
    // SlotMaker.IContextListenable<long>
    // SlotMaker.IContextListenable<object>
    // SlotMaker.IContextListenable<uint>
    // SlotMaker.IContextListenable<ulong>
    // SlotMaker.MonoSingleton<object>
    // SlotMaker.MonoWeakSingleton<object>
    // SlotMaker.ScriptableObjectSingleton<object>
    // SlotMaker.SerializedDictionary<object,object>
    // SlotMaker.Singleton<object>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<UnityEngine.Bounds>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<UnityEngine.Color>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<UnityEngine.ContactPoint2D>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<UnityEngine.ContactPoint>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<UnityEngine.Keyframe>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<UnityEngine.Quaternion>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<UnityEngine.Ray>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<UnityEngine.RaycastHit2D>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<UnityEngine.RaycastHit>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<UnityEngine.Rect>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<UnityEngine.Vector2>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<UnityEngine.Vector3>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<UnityEngine.Vector4>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<byte>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<double>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<float>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<int>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<long>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<object>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<uint>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_0<ulong>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<UnityEngine.Bounds>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<UnityEngine.Color>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<UnityEngine.ContactPoint2D>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<UnityEngine.ContactPoint>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<UnityEngine.Keyframe>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<UnityEngine.Quaternion>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<UnityEngine.Ray>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<UnityEngine.RaycastHit2D>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<UnityEngine.RaycastHit>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<UnityEngine.Rect>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<UnityEngine.Vector2>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<UnityEngine.Vector3>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<UnityEngine.Vector4>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<byte>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<double>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<float>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<int>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<long>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<object>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<uint>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable.<>c__DisplayClass6_1<ulong>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<UnityEngine.Bounds>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<UnityEngine.Color>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<UnityEngine.ContactPoint2D>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<UnityEngine.ContactPoint>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<UnityEngine.Keyframe>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<UnityEngine.Quaternion>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<UnityEngine.Ray>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<UnityEngine.RaycastHit2D>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<UnityEngine.RaycastHit>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<UnityEngine.Rect>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<UnityEngine.Vector2>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<UnityEngine.Vector3>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<UnityEngine.Vector4>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<byte>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<double>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<float>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<int>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<long>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<object>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<uint>
    // SlotMaker.Tasks.Actions.SimpleSetContextListenable<ulong>
    // System.Action<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Action<Dreamteck.Splines.SplinePoint>
    // System.Action<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Action<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Action<GameStudio.Slot.FSF.Frame>
    // System.Action<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Action<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Action<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Action<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Action<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Action<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Action<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Action<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Action<System.DateTime>
    // System.Action<System.ValueTuple<object,object>>
    // System.Action<UnityEngine.BoneWeight>
    // System.Action<UnityEngine.Bounds>
    // System.Action<UnityEngine.Color>
    // System.Action<UnityEngine.CombineInstance>
    // System.Action<UnityEngine.ContactPoint2D>
    // System.Action<UnityEngine.ContactPoint>
    // System.Action<UnityEngine.Keyframe>
    // System.Action<UnityEngine.Matrix4x4>
    // System.Action<UnityEngine.Quaternion>
    // System.Action<UnityEngine.Ray>
    // System.Action<UnityEngine.RaycastHit2D>
    // System.Action<UnityEngine.RaycastHit>
    // System.Action<UnityEngine.Rect>
    // System.Action<UnityEngine.UIVertex>
    // System.Action<UnityEngine.Vector2>
    // System.Action<UnityEngine.Vector2Int>
    // System.Action<UnityEngine.Vector3>
    // System.Action<UnityEngine.Vector4>
    // System.Action<byte>
    // System.Action<double>
    // System.Action<float>
    // System.Action<int,int>
    // System.Action<int>
    // System.Action<long,long>
    // System.Action<long>
    // System.Action<object,byte,object>
    // System.Action<object,int>
    // System.Action<object,object>
    // System.Action<object>
    // System.Action<uint>
    // System.Action<ulong>
    // System.ArraySegment.ArraySegmentEnumerator<byte>
    // System.ArraySegment<byte>
    // System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__27<int>
    // System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__27<object>
    // System.Collections.Concurrent.ConcurrentQueue.Segment<int>
    // System.Collections.Concurrent.ConcurrentQueue.Segment<object>
    // System.Collections.Concurrent.ConcurrentQueue<int>
    // System.Collections.Concurrent.ConcurrentQueue<object>
    // System.Collections.Generic.ArraySortHelper<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Collections.Generic.ArraySortHelper<Dreamteck.Splines.SplinePoint>
    // System.Collections.Generic.ArraySortHelper<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Collections.Generic.ArraySortHelper<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Collections.Generic.ArraySortHelper<GameStudio.Slot.FSF.Frame>
    // System.Collections.Generic.ArraySortHelper<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Collections.Generic.ArraySortHelper<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Collections.Generic.ArraySortHelper<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Collections.Generic.ArraySortHelper<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Collections.Generic.ArraySortHelper<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.ArraySortHelper<System.DateTime>
    // System.Collections.Generic.ArraySortHelper<System.ValueTuple<object,object>>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.BoneWeight>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Bounds>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Color>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.CombineInstance>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.ContactPoint2D>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.ContactPoint>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Keyframe>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Matrix4x4>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Quaternion>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Ray>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.RaycastHit2D>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.RaycastHit>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Rect>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.UIVertex>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Vector2>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Vector2Int>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Vector3>
    // System.Collections.Generic.ArraySortHelper<UnityEngine.Vector4>
    // System.Collections.Generic.ArraySortHelper<byte>
    // System.Collections.Generic.ArraySortHelper<double>
    // System.Collections.Generic.ArraySortHelper<float>
    // System.Collections.Generic.ArraySortHelper<int>
    // System.Collections.Generic.ArraySortHelper<long>
    // System.Collections.Generic.ArraySortHelper<object>
    // System.Collections.Generic.ArraySortHelper<uint>
    // System.Collections.Generic.ArraySortHelper<ulong>
    // System.Collections.Generic.Comparer<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Collections.Generic.Comparer<Dreamteck.Splines.SplinePoint>
    // System.Collections.Generic.Comparer<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Collections.Generic.Comparer<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Collections.Generic.Comparer<GameStudio.Slot.FSF.Frame>
    // System.Collections.Generic.Comparer<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Collections.Generic.Comparer<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Collections.Generic.Comparer<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Collections.Generic.Comparer<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Collections.Generic.Comparer<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.Comparer<System.DateTime>
    // System.Collections.Generic.Comparer<System.ValueTuple<object,object>>
    // System.Collections.Generic.Comparer<UnityEngine.BoneWeight>
    // System.Collections.Generic.Comparer<UnityEngine.Bounds>
    // System.Collections.Generic.Comparer<UnityEngine.Color>
    // System.Collections.Generic.Comparer<UnityEngine.CombineInstance>
    // System.Collections.Generic.Comparer<UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Comparer<UnityEngine.ContactPoint>
    // System.Collections.Generic.Comparer<UnityEngine.Keyframe>
    // System.Collections.Generic.Comparer<UnityEngine.Matrix4x4>
    // System.Collections.Generic.Comparer<UnityEngine.Quaternion>
    // System.Collections.Generic.Comparer<UnityEngine.Ray>
    // System.Collections.Generic.Comparer<UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Comparer<UnityEngine.RaycastHit>
    // System.Collections.Generic.Comparer<UnityEngine.Rect>
    // System.Collections.Generic.Comparer<UnityEngine.UIVertex>
    // System.Collections.Generic.Comparer<UnityEngine.Vector2>
    // System.Collections.Generic.Comparer<UnityEngine.Vector2Int>
    // System.Collections.Generic.Comparer<UnityEngine.Vector3>
    // System.Collections.Generic.Comparer<UnityEngine.Vector4>
    // System.Collections.Generic.Comparer<byte>
    // System.Collections.Generic.Comparer<double>
    // System.Collections.Generic.Comparer<float>
    // System.Collections.Generic.Comparer<int>
    // System.Collections.Generic.Comparer<long>
    // System.Collections.Generic.Comparer<object>
    // System.Collections.Generic.Comparer<uint>
    // System.Collections.Generic.Comparer<ulong>
    // System.Collections.Generic.Dictionary.Enumerator<System.ValueTuple<object,int>,object>
    // System.Collections.Generic.Dictionary.Enumerator<UnityEngine.Vector2Int,int>
    // System.Collections.Generic.Dictionary.Enumerator<float,object>
    // System.Collections.Generic.Dictionary.Enumerator<int,System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.Dictionary.Enumerator<int,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary.Enumerator<int,UnityEngine.Color>
    // System.Collections.Generic.Dictionary.Enumerator<int,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary.Enumerator<int,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary.Enumerator<int,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary.Enumerator<int,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary.Enumerator<int,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary.Enumerator<int,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary.Enumerator<int,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary.Enumerator<int,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary.Enumerator<int,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary.Enumerator<int,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary.Enumerator<int,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary.Enumerator<int,byte>
    // System.Collections.Generic.Dictionary.Enumerator<int,double>
    // System.Collections.Generic.Dictionary.Enumerator<int,float>
    // System.Collections.Generic.Dictionary.Enumerator<int,int>
    // System.Collections.Generic.Dictionary.Enumerator<int,long>
    // System.Collections.Generic.Dictionary.Enumerator<int,object>
    // System.Collections.Generic.Dictionary.Enumerator<int,uint>
    // System.Collections.Generic.Dictionary.Enumerator<int,ulong>
    // System.Collections.Generic.Dictionary.Enumerator<long,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary.Enumerator<long,UnityEngine.Color>
    // System.Collections.Generic.Dictionary.Enumerator<long,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary.Enumerator<long,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary.Enumerator<long,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary.Enumerator<long,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary.Enumerator<long,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary.Enumerator<long,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary.Enumerator<long,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary.Enumerator<long,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary.Enumerator<long,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary.Enumerator<long,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary.Enumerator<long,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary.Enumerator<long,byte>
    // System.Collections.Generic.Dictionary.Enumerator<long,double>
    // System.Collections.Generic.Dictionary.Enumerator<long,float>
    // System.Collections.Generic.Dictionary.Enumerator<long,int>
    // System.Collections.Generic.Dictionary.Enumerator<long,long>
    // System.Collections.Generic.Dictionary.Enumerator<long,object>
    // System.Collections.Generic.Dictionary.Enumerator<long,uint>
    // System.Collections.Generic.Dictionary.Enumerator<long,ulong>
    // System.Collections.Generic.Dictionary.Enumerator<object,System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.Dictionary.Enumerator<object,System.ValueTuple<object,object>>
    // System.Collections.Generic.Dictionary.Enumerator<object,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary.Enumerator<object,UnityEngine.Color>
    // System.Collections.Generic.Dictionary.Enumerator<object,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary.Enumerator<object,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary.Enumerator<object,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary.Enumerator<object,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary.Enumerator<object,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary.Enumerator<object,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary.Enumerator<object,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary.Enumerator<object,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary.Enumerator<object,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary.Enumerator<object,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary.Enumerator<object,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary.Enumerator<object,byte>
    // System.Collections.Generic.Dictionary.Enumerator<object,double>
    // System.Collections.Generic.Dictionary.Enumerator<object,float>
    // System.Collections.Generic.Dictionary.Enumerator<object,int>
    // System.Collections.Generic.Dictionary.Enumerator<object,long>
    // System.Collections.Generic.Dictionary.Enumerator<object,object>
    // System.Collections.Generic.Dictionary.Enumerator<object,uint>
    // System.Collections.Generic.Dictionary.Enumerator<object,ulong>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<System.ValueTuple<object,int>,object>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<UnityEngine.Vector2Int,int>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<float,object>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,UnityEngine.Color>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,byte>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,double>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,float>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,int>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,long>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,uint>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,ulong>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,UnityEngine.Color>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,byte>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,double>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,float>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,int>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,long>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,object>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,uint>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,ulong>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,System.ValueTuple<object,object>>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,UnityEngine.Color>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,byte>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,double>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,float>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,long>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,uint>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,ulong>
    // System.Collections.Generic.Dictionary.KeyCollection<System.ValueTuple<object,int>,object>
    // System.Collections.Generic.Dictionary.KeyCollection<UnityEngine.Vector2Int,int>
    // System.Collections.Generic.Dictionary.KeyCollection<float,object>
    // System.Collections.Generic.Dictionary.KeyCollection<int,System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.Dictionary.KeyCollection<int,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary.KeyCollection<int,UnityEngine.Color>
    // System.Collections.Generic.Dictionary.KeyCollection<int,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary.KeyCollection<int,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary.KeyCollection<int,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary.KeyCollection<int,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary.KeyCollection<int,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary.KeyCollection<int,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary.KeyCollection<int,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary.KeyCollection<int,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary.KeyCollection<int,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary.KeyCollection<int,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary.KeyCollection<int,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary.KeyCollection<int,byte>
    // System.Collections.Generic.Dictionary.KeyCollection<int,double>
    // System.Collections.Generic.Dictionary.KeyCollection<int,float>
    // System.Collections.Generic.Dictionary.KeyCollection<int,int>
    // System.Collections.Generic.Dictionary.KeyCollection<int,long>
    // System.Collections.Generic.Dictionary.KeyCollection<int,object>
    // System.Collections.Generic.Dictionary.KeyCollection<int,uint>
    // System.Collections.Generic.Dictionary.KeyCollection<int,ulong>
    // System.Collections.Generic.Dictionary.KeyCollection<long,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary.KeyCollection<long,UnityEngine.Color>
    // System.Collections.Generic.Dictionary.KeyCollection<long,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary.KeyCollection<long,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary.KeyCollection<long,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary.KeyCollection<long,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary.KeyCollection<long,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary.KeyCollection<long,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary.KeyCollection<long,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary.KeyCollection<long,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary.KeyCollection<long,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary.KeyCollection<long,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary.KeyCollection<long,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary.KeyCollection<long,byte>
    // System.Collections.Generic.Dictionary.KeyCollection<long,double>
    // System.Collections.Generic.Dictionary.KeyCollection<long,float>
    // System.Collections.Generic.Dictionary.KeyCollection<long,int>
    // System.Collections.Generic.Dictionary.KeyCollection<long,long>
    // System.Collections.Generic.Dictionary.KeyCollection<long,object>
    // System.Collections.Generic.Dictionary.KeyCollection<long,uint>
    // System.Collections.Generic.Dictionary.KeyCollection<long,ulong>
    // System.Collections.Generic.Dictionary.KeyCollection<object,System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.Dictionary.KeyCollection<object,System.ValueTuple<object,object>>
    // System.Collections.Generic.Dictionary.KeyCollection<object,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary.KeyCollection<object,UnityEngine.Color>
    // System.Collections.Generic.Dictionary.KeyCollection<object,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary.KeyCollection<object,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary.KeyCollection<object,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary.KeyCollection<object,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary.KeyCollection<object,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary.KeyCollection<object,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary.KeyCollection<object,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary.KeyCollection<object,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary.KeyCollection<object,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary.KeyCollection<object,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary.KeyCollection<object,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary.KeyCollection<object,byte>
    // System.Collections.Generic.Dictionary.KeyCollection<object,double>
    // System.Collections.Generic.Dictionary.KeyCollection<object,float>
    // System.Collections.Generic.Dictionary.KeyCollection<object,int>
    // System.Collections.Generic.Dictionary.KeyCollection<object,long>
    // System.Collections.Generic.Dictionary.KeyCollection<object,object>
    // System.Collections.Generic.Dictionary.KeyCollection<object,uint>
    // System.Collections.Generic.Dictionary.KeyCollection<object,ulong>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<System.ValueTuple<object,int>,object>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<UnityEngine.Vector2Int,int>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<float,object>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,UnityEngine.Color>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,byte>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,double>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,float>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,int>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,long>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,uint>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,ulong>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,UnityEngine.Color>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,byte>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,double>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,float>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,int>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,long>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,object>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,uint>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,ulong>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,System.ValueTuple<object,object>>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,UnityEngine.Color>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,byte>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,double>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,float>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,long>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,uint>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,ulong>
    // System.Collections.Generic.Dictionary.ValueCollection<System.ValueTuple<object,int>,object>
    // System.Collections.Generic.Dictionary.ValueCollection<UnityEngine.Vector2Int,int>
    // System.Collections.Generic.Dictionary.ValueCollection<float,object>
    // System.Collections.Generic.Dictionary.ValueCollection<int,System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.Dictionary.ValueCollection<int,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary.ValueCollection<int,UnityEngine.Color>
    // System.Collections.Generic.Dictionary.ValueCollection<int,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary.ValueCollection<int,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary.ValueCollection<int,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary.ValueCollection<int,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary.ValueCollection<int,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary.ValueCollection<int,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary.ValueCollection<int,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary.ValueCollection<int,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary.ValueCollection<int,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary.ValueCollection<int,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary.ValueCollection<int,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary.ValueCollection<int,byte>
    // System.Collections.Generic.Dictionary.ValueCollection<int,double>
    // System.Collections.Generic.Dictionary.ValueCollection<int,float>
    // System.Collections.Generic.Dictionary.ValueCollection<int,int>
    // System.Collections.Generic.Dictionary.ValueCollection<int,long>
    // System.Collections.Generic.Dictionary.ValueCollection<int,object>
    // System.Collections.Generic.Dictionary.ValueCollection<int,uint>
    // System.Collections.Generic.Dictionary.ValueCollection<int,ulong>
    // System.Collections.Generic.Dictionary.ValueCollection<long,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary.ValueCollection<long,UnityEngine.Color>
    // System.Collections.Generic.Dictionary.ValueCollection<long,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary.ValueCollection<long,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary.ValueCollection<long,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary.ValueCollection<long,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary.ValueCollection<long,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary.ValueCollection<long,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary.ValueCollection<long,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary.ValueCollection<long,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary.ValueCollection<long,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary.ValueCollection<long,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary.ValueCollection<long,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary.ValueCollection<long,byte>
    // System.Collections.Generic.Dictionary.ValueCollection<long,double>
    // System.Collections.Generic.Dictionary.ValueCollection<long,float>
    // System.Collections.Generic.Dictionary.ValueCollection<long,int>
    // System.Collections.Generic.Dictionary.ValueCollection<long,long>
    // System.Collections.Generic.Dictionary.ValueCollection<long,object>
    // System.Collections.Generic.Dictionary.ValueCollection<long,uint>
    // System.Collections.Generic.Dictionary.ValueCollection<long,ulong>
    // System.Collections.Generic.Dictionary.ValueCollection<object,System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.Dictionary.ValueCollection<object,System.ValueTuple<object,object>>
    // System.Collections.Generic.Dictionary.ValueCollection<object,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary.ValueCollection<object,UnityEngine.Color>
    // System.Collections.Generic.Dictionary.ValueCollection<object,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary.ValueCollection<object,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary.ValueCollection<object,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary.ValueCollection<object,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary.ValueCollection<object,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary.ValueCollection<object,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary.ValueCollection<object,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary.ValueCollection<object,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary.ValueCollection<object,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary.ValueCollection<object,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary.ValueCollection<object,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary.ValueCollection<object,byte>
    // System.Collections.Generic.Dictionary.ValueCollection<object,double>
    // System.Collections.Generic.Dictionary.ValueCollection<object,float>
    // System.Collections.Generic.Dictionary.ValueCollection<object,int>
    // System.Collections.Generic.Dictionary.ValueCollection<object,long>
    // System.Collections.Generic.Dictionary.ValueCollection<object,object>
    // System.Collections.Generic.Dictionary.ValueCollection<object,uint>
    // System.Collections.Generic.Dictionary.ValueCollection<object,ulong>
    // System.Collections.Generic.Dictionary<System.ValueTuple<object,int>,object>
    // System.Collections.Generic.Dictionary<UnityEngine.Vector2Int,int>
    // System.Collections.Generic.Dictionary<float,object>
    // System.Collections.Generic.Dictionary<int,System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.Dictionary<int,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary<int,UnityEngine.Color>
    // System.Collections.Generic.Dictionary<int,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary<int,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary<int,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary<int,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary<int,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary<int,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary<int,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary<int,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary<int,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary<int,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary<int,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary<int,byte>
    // System.Collections.Generic.Dictionary<int,double>
    // System.Collections.Generic.Dictionary<int,float>
    // System.Collections.Generic.Dictionary<int,int>
    // System.Collections.Generic.Dictionary<int,long>
    // System.Collections.Generic.Dictionary<int,object>
    // System.Collections.Generic.Dictionary<int,uint>
    // System.Collections.Generic.Dictionary<int,ulong>
    // System.Collections.Generic.Dictionary<long,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary<long,UnityEngine.Color>
    // System.Collections.Generic.Dictionary<long,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary<long,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary<long,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary<long,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary<long,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary<long,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary<long,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary<long,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary<long,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary<long,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary<long,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary<long,byte>
    // System.Collections.Generic.Dictionary<long,double>
    // System.Collections.Generic.Dictionary<long,float>
    // System.Collections.Generic.Dictionary<long,int>
    // System.Collections.Generic.Dictionary<long,long>
    // System.Collections.Generic.Dictionary<long,object>
    // System.Collections.Generic.Dictionary<long,uint>
    // System.Collections.Generic.Dictionary<long,ulong>
    // System.Collections.Generic.Dictionary<object,System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.Dictionary<object,System.ValueTuple<object,object>>
    // System.Collections.Generic.Dictionary<object,UnityEngine.Bounds>
    // System.Collections.Generic.Dictionary<object,UnityEngine.Color>
    // System.Collections.Generic.Dictionary<object,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.Dictionary<object,UnityEngine.ContactPoint>
    // System.Collections.Generic.Dictionary<object,UnityEngine.Keyframe>
    // System.Collections.Generic.Dictionary<object,UnityEngine.Quaternion>
    // System.Collections.Generic.Dictionary<object,UnityEngine.Ray>
    // System.Collections.Generic.Dictionary<object,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.Dictionary<object,UnityEngine.RaycastHit>
    // System.Collections.Generic.Dictionary<object,UnityEngine.Rect>
    // System.Collections.Generic.Dictionary<object,UnityEngine.Vector2>
    // System.Collections.Generic.Dictionary<object,UnityEngine.Vector3>
    // System.Collections.Generic.Dictionary<object,UnityEngine.Vector4>
    // System.Collections.Generic.Dictionary<object,byte>
    // System.Collections.Generic.Dictionary<object,double>
    // System.Collections.Generic.Dictionary<object,float>
    // System.Collections.Generic.Dictionary<object,int>
    // System.Collections.Generic.Dictionary<object,long>
    // System.Collections.Generic.Dictionary<object,object>
    // System.Collections.Generic.Dictionary<object,uint>
    // System.Collections.Generic.Dictionary<object,ulong>
    // System.Collections.Generic.EqualityComparer<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Collections.Generic.EqualityComparer<Dreamteck.Splines.SplinePoint>
    // System.Collections.Generic.EqualityComparer<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Collections.Generic.EqualityComparer<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Collections.Generic.EqualityComparer<GameStudio.Slot.FSF.Frame>
    // System.Collections.Generic.EqualityComparer<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Collections.Generic.EqualityComparer<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Collections.Generic.EqualityComparer<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Collections.Generic.EqualityComparer<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Collections.Generic.EqualityComparer<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Collections.Generic.EqualityComparer<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Collections.Generic.EqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.EqualityComparer<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.EqualityComparer<System.DateTime>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<object,int>>
    // System.Collections.Generic.EqualityComparer<System.ValueTuple<object,object>>
    // System.Collections.Generic.EqualityComparer<UnityEngine.BoneWeight>
    // System.Collections.Generic.EqualityComparer<UnityEngine.Bounds>
    // System.Collections.Generic.EqualityComparer<UnityEngine.Color>
    // System.Collections.Generic.EqualityComparer<UnityEngine.CombineInstance>
    // System.Collections.Generic.EqualityComparer<UnityEngine.ContactPoint2D>
    // System.Collections.Generic.EqualityComparer<UnityEngine.ContactPoint>
    // System.Collections.Generic.EqualityComparer<UnityEngine.Keyframe>
    // System.Collections.Generic.EqualityComparer<UnityEngine.Matrix4x4>
    // System.Collections.Generic.EqualityComparer<UnityEngine.Quaternion>
    // System.Collections.Generic.EqualityComparer<UnityEngine.Ray>
    // System.Collections.Generic.EqualityComparer<UnityEngine.RaycastHit2D>
    // System.Collections.Generic.EqualityComparer<UnityEngine.RaycastHit>
    // System.Collections.Generic.EqualityComparer<UnityEngine.Rect>
    // System.Collections.Generic.EqualityComparer<UnityEngine.UIVertex>
    // System.Collections.Generic.EqualityComparer<UnityEngine.Vector2>
    // System.Collections.Generic.EqualityComparer<UnityEngine.Vector2Int>
    // System.Collections.Generic.EqualityComparer<UnityEngine.Vector3>
    // System.Collections.Generic.EqualityComparer<UnityEngine.Vector4>
    // System.Collections.Generic.EqualityComparer<byte>
    // System.Collections.Generic.EqualityComparer<double>
    // System.Collections.Generic.EqualityComparer<float>
    // System.Collections.Generic.EqualityComparer<int>
    // System.Collections.Generic.EqualityComparer<long>
    // System.Collections.Generic.EqualityComparer<object>
    // System.Collections.Generic.EqualityComparer<uint>
    // System.Collections.Generic.EqualityComparer<ulong>
    // System.Collections.Generic.HashSet.Enumerator<int>
    // System.Collections.Generic.HashSet.Enumerator<object>
    // System.Collections.Generic.HashSet<int>
    // System.Collections.Generic.HashSet<object>
    // System.Collections.Generic.HashSetEqualityComparer<int>
    // System.Collections.Generic.HashSetEqualityComparer<object>
    // System.Collections.Generic.ICollection<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Collections.Generic.ICollection<Dreamteck.Splines.SplinePoint>
    // System.Collections.Generic.ICollection<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Collections.Generic.ICollection<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Collections.Generic.ICollection<GameStudio.Slot.FSF.Frame>
    // System.Collections.Generic.ICollection<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Collections.Generic.ICollection<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Collections.Generic.ICollection<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Collections.Generic.ICollection<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Collections.Generic.ICollection<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.ValueTuple<object,int>,object>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<UnityEngine.Vector2Int,int>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<float,object>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,System.Collections.Generic.KeyValuePair<int,object>>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,UnityEngine.Bounds>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,UnityEngine.Color>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,UnityEngine.ContactPoint2D>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,UnityEngine.ContactPoint>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,UnityEngine.Keyframe>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,UnityEngine.Quaternion>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,UnityEngine.Ray>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,UnityEngine.RaycastHit2D>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,UnityEngine.RaycastHit>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,UnityEngine.Rect>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,UnityEngine.Vector2>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,UnityEngine.Vector3>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,UnityEngine.Vector4>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,byte>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,double>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,float>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,long>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,uint>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,ulong>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,UnityEngine.Bounds>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,UnityEngine.Color>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,UnityEngine.ContactPoint2D>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,UnityEngine.ContactPoint>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,UnityEngine.Keyframe>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,UnityEngine.Quaternion>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,UnityEngine.Ray>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,UnityEngine.RaycastHit2D>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,UnityEngine.RaycastHit>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,UnityEngine.Rect>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,UnityEngine.Vector2>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,UnityEngine.Vector3>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,UnityEngine.Vector4>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,byte>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,double>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,float>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,int>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,long>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,object>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,uint>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,ulong>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,System.Collections.Generic.KeyValuePair<object,object>>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<object,object>>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,UnityEngine.Bounds>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,UnityEngine.Color>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,UnityEngine.ContactPoint2D>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,UnityEngine.ContactPoint>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,UnityEngine.Keyframe>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,UnityEngine.Quaternion>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,UnityEngine.Ray>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,UnityEngine.RaycastHit2D>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,UnityEngine.RaycastHit>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,UnityEngine.Rect>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,UnityEngine.Vector2>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,UnityEngine.Vector3>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,UnityEngine.Vector4>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,byte>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,double>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,float>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,long>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,uint>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,ulong>>
    // System.Collections.Generic.ICollection<System.DateTime>
    // System.Collections.Generic.ICollection<System.ValueTuple<object,object>>
    // System.Collections.Generic.ICollection<UnityEngine.BoneWeight>
    // System.Collections.Generic.ICollection<UnityEngine.Bounds>
    // System.Collections.Generic.ICollection<UnityEngine.Color>
    // System.Collections.Generic.ICollection<UnityEngine.CombineInstance>
    // System.Collections.Generic.ICollection<UnityEngine.ContactPoint2D>
    // System.Collections.Generic.ICollection<UnityEngine.ContactPoint>
    // System.Collections.Generic.ICollection<UnityEngine.Keyframe>
    // System.Collections.Generic.ICollection<UnityEngine.Matrix4x4>
    // System.Collections.Generic.ICollection<UnityEngine.Quaternion>
    // System.Collections.Generic.ICollection<UnityEngine.Ray>
    // System.Collections.Generic.ICollection<UnityEngine.RaycastHit2D>
    // System.Collections.Generic.ICollection<UnityEngine.RaycastHit>
    // System.Collections.Generic.ICollection<UnityEngine.Rect>
    // System.Collections.Generic.ICollection<UnityEngine.UIVertex>
    // System.Collections.Generic.ICollection<UnityEngine.Vector2>
    // System.Collections.Generic.ICollection<UnityEngine.Vector2Int>
    // System.Collections.Generic.ICollection<UnityEngine.Vector3>
    // System.Collections.Generic.ICollection<UnityEngine.Vector4>
    // System.Collections.Generic.ICollection<byte>
    // System.Collections.Generic.ICollection<double>
    // System.Collections.Generic.ICollection<float>
    // System.Collections.Generic.ICollection<int>
    // System.Collections.Generic.ICollection<long>
    // System.Collections.Generic.ICollection<object>
    // System.Collections.Generic.ICollection<uint>
    // System.Collections.Generic.ICollection<ulong>
    // System.Collections.Generic.ICollection<ushort>
    // System.Collections.Generic.IComparer<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Collections.Generic.IComparer<Dreamteck.Splines.SplinePoint>
    // System.Collections.Generic.IComparer<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Collections.Generic.IComparer<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Collections.Generic.IComparer<GameStudio.Slot.FSF.Frame>
    // System.Collections.Generic.IComparer<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Collections.Generic.IComparer<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Collections.Generic.IComparer<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Collections.Generic.IComparer<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Collections.Generic.IComparer<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.IComparer<System.DateTime>
    // System.Collections.Generic.IComparer<System.ValueTuple<object,object>>
    // System.Collections.Generic.IComparer<UnityEngine.BoneWeight>
    // System.Collections.Generic.IComparer<UnityEngine.Bounds>
    // System.Collections.Generic.IComparer<UnityEngine.Color>
    // System.Collections.Generic.IComparer<UnityEngine.CombineInstance>
    // System.Collections.Generic.IComparer<UnityEngine.ContactPoint2D>
    // System.Collections.Generic.IComparer<UnityEngine.ContactPoint>
    // System.Collections.Generic.IComparer<UnityEngine.Keyframe>
    // System.Collections.Generic.IComparer<UnityEngine.Matrix4x4>
    // System.Collections.Generic.IComparer<UnityEngine.Quaternion>
    // System.Collections.Generic.IComparer<UnityEngine.Ray>
    // System.Collections.Generic.IComparer<UnityEngine.RaycastHit2D>
    // System.Collections.Generic.IComparer<UnityEngine.RaycastHit>
    // System.Collections.Generic.IComparer<UnityEngine.Rect>
    // System.Collections.Generic.IComparer<UnityEngine.UIVertex>
    // System.Collections.Generic.IComparer<UnityEngine.Vector2>
    // System.Collections.Generic.IComparer<UnityEngine.Vector2Int>
    // System.Collections.Generic.IComparer<UnityEngine.Vector3>
    // System.Collections.Generic.IComparer<UnityEngine.Vector4>
    // System.Collections.Generic.IComparer<byte>
    // System.Collections.Generic.IComparer<double>
    // System.Collections.Generic.IComparer<float>
    // System.Collections.Generic.IComparer<int>
    // System.Collections.Generic.IComparer<long>
    // System.Collections.Generic.IComparer<object>
    // System.Collections.Generic.IComparer<uint>
    // System.Collections.Generic.IComparer<ulong>
    // System.Collections.Generic.IDictionary<object,System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.IDictionary<object,object>
    // System.Collections.Generic.IEnumerable<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Collections.Generic.IEnumerable<Dreamteck.Splines.SplinePoint>
    // System.Collections.Generic.IEnumerable<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Collections.Generic.IEnumerable<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Collections.Generic.IEnumerable<GameStudio.Slot.FSF.Frame>
    // System.Collections.Generic.IEnumerable<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Collections.Generic.IEnumerable<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Collections.Generic.IEnumerable<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Collections.Generic.IEnumerable<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Collections.Generic.IEnumerable<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.ValueTuple<object,int>,object>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<UnityEngine.Vector2Int,int>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<float,object>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,System.Collections.Generic.KeyValuePair<int,object>>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,UnityEngine.Bounds>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,UnityEngine.Color>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,UnityEngine.ContactPoint2D>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,UnityEngine.ContactPoint>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,UnityEngine.Keyframe>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,UnityEngine.Quaternion>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,UnityEngine.Ray>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,UnityEngine.RaycastHit2D>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,UnityEngine.RaycastHit>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,UnityEngine.Rect>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,UnityEngine.Vector2>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,UnityEngine.Vector3>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,UnityEngine.Vector4>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,byte>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,double>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,float>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,long>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,uint>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,ulong>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,UnityEngine.Bounds>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,UnityEngine.Color>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,UnityEngine.ContactPoint2D>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,UnityEngine.ContactPoint>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,UnityEngine.Keyframe>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,UnityEngine.Quaternion>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,UnityEngine.Ray>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,UnityEngine.RaycastHit2D>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,UnityEngine.RaycastHit>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,UnityEngine.Rect>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,UnityEngine.Vector2>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,UnityEngine.Vector3>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,UnityEngine.Vector4>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,byte>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,double>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,float>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,int>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,long>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,object>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,uint>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,ulong>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,System.Collections.Generic.KeyValuePair<object,object>>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<object,object>>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,UnityEngine.Bounds>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,UnityEngine.Color>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,UnityEngine.ContactPoint2D>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,UnityEngine.ContactPoint>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,UnityEngine.Keyframe>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,UnityEngine.Quaternion>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,UnityEngine.Ray>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,UnityEngine.RaycastHit2D>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,UnityEngine.RaycastHit>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,UnityEngine.Rect>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,UnityEngine.Vector2>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,UnityEngine.Vector3>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,UnityEngine.Vector4>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,byte>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,double>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,float>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,long>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,uint>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,ulong>>
    // System.Collections.Generic.IEnumerable<System.DateTime>
    // System.Collections.Generic.IEnumerable<System.ValueTuple<int,object>>
    // System.Collections.Generic.IEnumerable<System.ValueTuple<object,object>>
    // System.Collections.Generic.IEnumerable<UnityEngine.BoneWeight>
    // System.Collections.Generic.IEnumerable<UnityEngine.Bounds>
    // System.Collections.Generic.IEnumerable<UnityEngine.CharacterInfo>
    // System.Collections.Generic.IEnumerable<UnityEngine.Color>
    // System.Collections.Generic.IEnumerable<UnityEngine.CombineInstance>
    // System.Collections.Generic.IEnumerable<UnityEngine.ContactPoint2D>
    // System.Collections.Generic.IEnumerable<UnityEngine.ContactPoint>
    // System.Collections.Generic.IEnumerable<UnityEngine.Keyframe>
    // System.Collections.Generic.IEnumerable<UnityEngine.Matrix4x4>
    // System.Collections.Generic.IEnumerable<UnityEngine.Quaternion>
    // System.Collections.Generic.IEnumerable<UnityEngine.Ray>
    // System.Collections.Generic.IEnumerable<UnityEngine.RaycastHit2D>
    // System.Collections.Generic.IEnumerable<UnityEngine.RaycastHit>
    // System.Collections.Generic.IEnumerable<UnityEngine.Rect>
    // System.Collections.Generic.IEnumerable<UnityEngine.UIVertex>
    // System.Collections.Generic.IEnumerable<UnityEngine.Vector2>
    // System.Collections.Generic.IEnumerable<UnityEngine.Vector2Int>
    // System.Collections.Generic.IEnumerable<UnityEngine.Vector3>
    // System.Collections.Generic.IEnumerable<UnityEngine.Vector4>
    // System.Collections.Generic.IEnumerable<byte>
    // System.Collections.Generic.IEnumerable<double>
    // System.Collections.Generic.IEnumerable<float>
    // System.Collections.Generic.IEnumerable<int>
    // System.Collections.Generic.IEnumerable<long>
    // System.Collections.Generic.IEnumerable<object>
    // System.Collections.Generic.IEnumerable<uint>
    // System.Collections.Generic.IEnumerable<ulong>
    // System.Collections.Generic.IEnumerable<ushort>
    // System.Collections.Generic.IEnumerator<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Collections.Generic.IEnumerator<Dreamteck.Splines.SplinePoint>
    // System.Collections.Generic.IEnumerator<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Collections.Generic.IEnumerator<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Collections.Generic.IEnumerator<GameStudio.Slot.FSF.Frame>
    // System.Collections.Generic.IEnumerator<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Collections.Generic.IEnumerator<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Collections.Generic.IEnumerator<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Collections.Generic.IEnumerator<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Collections.Generic.IEnumerator<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<System.ValueTuple<object,int>,object>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<UnityEngine.Vector2Int,int>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<float,object>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,System.Collections.Generic.KeyValuePair<int,object>>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,UnityEngine.Bounds>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,UnityEngine.Color>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,UnityEngine.ContactPoint2D>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,UnityEngine.ContactPoint>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,UnityEngine.Keyframe>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,UnityEngine.Quaternion>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,UnityEngine.Ray>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,UnityEngine.RaycastHit2D>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,UnityEngine.RaycastHit>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,UnityEngine.Rect>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,UnityEngine.Vector2>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,UnityEngine.Vector3>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,UnityEngine.Vector4>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,byte>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,double>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,float>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,long>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,uint>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,ulong>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,UnityEngine.Bounds>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,UnityEngine.Color>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,UnityEngine.ContactPoint2D>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,UnityEngine.ContactPoint>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,UnityEngine.Keyframe>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,UnityEngine.Quaternion>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,UnityEngine.Ray>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,UnityEngine.RaycastHit2D>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,UnityEngine.RaycastHit>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,UnityEngine.Rect>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,UnityEngine.Vector2>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,UnityEngine.Vector3>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,UnityEngine.Vector4>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,byte>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,double>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,float>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,int>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,long>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,object>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,uint>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,ulong>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,System.Collections.Generic.KeyValuePair<object,object>>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,System.ValueTuple<object,object>>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,UnityEngine.Bounds>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,UnityEngine.Color>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,UnityEngine.ContactPoint2D>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,UnityEngine.ContactPoint>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,UnityEngine.Keyframe>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,UnityEngine.Quaternion>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,UnityEngine.Ray>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,UnityEngine.RaycastHit2D>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,UnityEngine.RaycastHit>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,UnityEngine.Rect>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,UnityEngine.Vector2>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,UnityEngine.Vector3>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,UnityEngine.Vector4>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,byte>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,double>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,float>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,long>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,uint>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,ulong>>
    // System.Collections.Generic.IEnumerator<System.DateTime>
    // System.Collections.Generic.IEnumerator<System.ValueTuple<int,object>>
    // System.Collections.Generic.IEnumerator<System.ValueTuple<object,object>>
    // System.Collections.Generic.IEnumerator<UnityEngine.BoneWeight>
    // System.Collections.Generic.IEnumerator<UnityEngine.Bounds>
    // System.Collections.Generic.IEnumerator<UnityEngine.CharacterInfo>
    // System.Collections.Generic.IEnumerator<UnityEngine.Color>
    // System.Collections.Generic.IEnumerator<UnityEngine.CombineInstance>
    // System.Collections.Generic.IEnumerator<UnityEngine.ContactPoint2D>
    // System.Collections.Generic.IEnumerator<UnityEngine.ContactPoint>
    // System.Collections.Generic.IEnumerator<UnityEngine.Keyframe>
    // System.Collections.Generic.IEnumerator<UnityEngine.Matrix4x4>
    // System.Collections.Generic.IEnumerator<UnityEngine.Quaternion>
    // System.Collections.Generic.IEnumerator<UnityEngine.Ray>
    // System.Collections.Generic.IEnumerator<UnityEngine.RaycastHit2D>
    // System.Collections.Generic.IEnumerator<UnityEngine.RaycastHit>
    // System.Collections.Generic.IEnumerator<UnityEngine.Rect>
    // System.Collections.Generic.IEnumerator<UnityEngine.UIVertex>
    // System.Collections.Generic.IEnumerator<UnityEngine.Vector2>
    // System.Collections.Generic.IEnumerator<UnityEngine.Vector2Int>
    // System.Collections.Generic.IEnumerator<UnityEngine.Vector3>
    // System.Collections.Generic.IEnumerator<UnityEngine.Vector4>
    // System.Collections.Generic.IEnumerator<byte>
    // System.Collections.Generic.IEnumerator<double>
    // System.Collections.Generic.IEnumerator<float>
    // System.Collections.Generic.IEnumerator<int>
    // System.Collections.Generic.IEnumerator<long>
    // System.Collections.Generic.IEnumerator<object>
    // System.Collections.Generic.IEnumerator<uint>
    // System.Collections.Generic.IEnumerator<ulong>
    // System.Collections.Generic.IEnumerator<ushort>
    // System.Collections.Generic.IEqualityComparer<System.ValueTuple<object,int>>
    // System.Collections.Generic.IEqualityComparer<UnityEngine.Vector2Int>
    // System.Collections.Generic.IEqualityComparer<float>
    // System.Collections.Generic.IEqualityComparer<int>
    // System.Collections.Generic.IEqualityComparer<long>
    // System.Collections.Generic.IEqualityComparer<object>
    // System.Collections.Generic.IList<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Collections.Generic.IList<Dreamteck.Splines.SplinePoint>
    // System.Collections.Generic.IList<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Collections.Generic.IList<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Collections.Generic.IList<GameStudio.Slot.FSF.Frame>
    // System.Collections.Generic.IList<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Collections.Generic.IList<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Collections.Generic.IList<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Collections.Generic.IList<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Collections.Generic.IList<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<object,long>>
    // System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.IList<System.DateTime>
    // System.Collections.Generic.IList<System.ValueTuple<object,object>>
    // System.Collections.Generic.IList<UnityEngine.BoneWeight>
    // System.Collections.Generic.IList<UnityEngine.Bounds>
    // System.Collections.Generic.IList<UnityEngine.Color>
    // System.Collections.Generic.IList<UnityEngine.CombineInstance>
    // System.Collections.Generic.IList<UnityEngine.ContactPoint2D>
    // System.Collections.Generic.IList<UnityEngine.ContactPoint>
    // System.Collections.Generic.IList<UnityEngine.Keyframe>
    // System.Collections.Generic.IList<UnityEngine.Matrix4x4>
    // System.Collections.Generic.IList<UnityEngine.Quaternion>
    // System.Collections.Generic.IList<UnityEngine.Ray>
    // System.Collections.Generic.IList<UnityEngine.RaycastHit2D>
    // System.Collections.Generic.IList<UnityEngine.RaycastHit>
    // System.Collections.Generic.IList<UnityEngine.Rect>
    // System.Collections.Generic.IList<UnityEngine.UIVertex>
    // System.Collections.Generic.IList<UnityEngine.Vector2>
    // System.Collections.Generic.IList<UnityEngine.Vector2Int>
    // System.Collections.Generic.IList<UnityEngine.Vector3>
    // System.Collections.Generic.IList<UnityEngine.Vector4>
    // System.Collections.Generic.IList<byte>
    // System.Collections.Generic.IList<double>
    // System.Collections.Generic.IList<float>
    // System.Collections.Generic.IList<int>
    // System.Collections.Generic.IList<long>
    // System.Collections.Generic.IList<object>
    // System.Collections.Generic.IList<uint>
    // System.Collections.Generic.IList<ulong>
    // System.Collections.Generic.IList<ushort>
    // System.Collections.Generic.IReadOnlyDictionary<int,long>
    // System.Collections.Generic.IReadOnlyList<object>
    // System.Collections.Generic.KeyValuePair<System.ValueTuple<object,int>,object>
    // System.Collections.Generic.KeyValuePair<UnityEngine.Vector2Int,int>
    // System.Collections.Generic.KeyValuePair<byte,object>
    // System.Collections.Generic.KeyValuePair<float,object>
    // System.Collections.Generic.KeyValuePair<int,System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.KeyValuePair<int,UnityEngine.Bounds>
    // System.Collections.Generic.KeyValuePair<int,UnityEngine.Color>
    // System.Collections.Generic.KeyValuePair<int,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.KeyValuePair<int,UnityEngine.ContactPoint>
    // System.Collections.Generic.KeyValuePair<int,UnityEngine.Keyframe>
    // System.Collections.Generic.KeyValuePair<int,UnityEngine.Quaternion>
    // System.Collections.Generic.KeyValuePair<int,UnityEngine.Ray>
    // System.Collections.Generic.KeyValuePair<int,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.KeyValuePair<int,UnityEngine.RaycastHit>
    // System.Collections.Generic.KeyValuePair<int,UnityEngine.Rect>
    // System.Collections.Generic.KeyValuePair<int,UnityEngine.Vector2>
    // System.Collections.Generic.KeyValuePair<int,UnityEngine.Vector3>
    // System.Collections.Generic.KeyValuePair<int,UnityEngine.Vector4>
    // System.Collections.Generic.KeyValuePair<int,byte>
    // System.Collections.Generic.KeyValuePair<int,double>
    // System.Collections.Generic.KeyValuePair<int,float>
    // System.Collections.Generic.KeyValuePair<int,int>
    // System.Collections.Generic.KeyValuePair<int,long>
    // System.Collections.Generic.KeyValuePair<int,object>
    // System.Collections.Generic.KeyValuePair<int,uint>
    // System.Collections.Generic.KeyValuePair<int,ulong>
    // System.Collections.Generic.KeyValuePair<long,UnityEngine.Bounds>
    // System.Collections.Generic.KeyValuePair<long,UnityEngine.Color>
    // System.Collections.Generic.KeyValuePair<long,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.KeyValuePair<long,UnityEngine.ContactPoint>
    // System.Collections.Generic.KeyValuePair<long,UnityEngine.Keyframe>
    // System.Collections.Generic.KeyValuePair<long,UnityEngine.Quaternion>
    // System.Collections.Generic.KeyValuePair<long,UnityEngine.Ray>
    // System.Collections.Generic.KeyValuePair<long,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.KeyValuePair<long,UnityEngine.RaycastHit>
    // System.Collections.Generic.KeyValuePair<long,UnityEngine.Rect>
    // System.Collections.Generic.KeyValuePair<long,UnityEngine.Vector2>
    // System.Collections.Generic.KeyValuePair<long,UnityEngine.Vector3>
    // System.Collections.Generic.KeyValuePair<long,UnityEngine.Vector4>
    // System.Collections.Generic.KeyValuePair<long,byte>
    // System.Collections.Generic.KeyValuePair<long,double>
    // System.Collections.Generic.KeyValuePair<long,float>
    // System.Collections.Generic.KeyValuePair<long,int>
    // System.Collections.Generic.KeyValuePair<long,long>
    // System.Collections.Generic.KeyValuePair<long,object>
    // System.Collections.Generic.KeyValuePair<long,uint>
    // System.Collections.Generic.KeyValuePair<long,ulong>
    // System.Collections.Generic.KeyValuePair<object,System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.KeyValuePair<object,System.ValueTuple<object,object>>
    // System.Collections.Generic.KeyValuePair<object,UnityEngine.Bounds>
    // System.Collections.Generic.KeyValuePair<object,UnityEngine.Color>
    // System.Collections.Generic.KeyValuePair<object,UnityEngine.ContactPoint2D>
    // System.Collections.Generic.KeyValuePair<object,UnityEngine.ContactPoint>
    // System.Collections.Generic.KeyValuePair<object,UnityEngine.Keyframe>
    // System.Collections.Generic.KeyValuePair<object,UnityEngine.Quaternion>
    // System.Collections.Generic.KeyValuePair<object,UnityEngine.Ray>
    // System.Collections.Generic.KeyValuePair<object,UnityEngine.RaycastHit2D>
    // System.Collections.Generic.KeyValuePair<object,UnityEngine.RaycastHit>
    // System.Collections.Generic.KeyValuePair<object,UnityEngine.Rect>
    // System.Collections.Generic.KeyValuePair<object,UnityEngine.Vector2>
    // System.Collections.Generic.KeyValuePair<object,UnityEngine.Vector3>
    // System.Collections.Generic.KeyValuePair<object,UnityEngine.Vector4>
    // System.Collections.Generic.KeyValuePair<object,byte>
    // System.Collections.Generic.KeyValuePair<object,double>
    // System.Collections.Generic.KeyValuePair<object,float>
    // System.Collections.Generic.KeyValuePair<object,int>
    // System.Collections.Generic.KeyValuePair<object,long>
    // System.Collections.Generic.KeyValuePair<object,object>
    // System.Collections.Generic.KeyValuePair<object,uint>
    // System.Collections.Generic.KeyValuePair<object,ulong>
    // System.Collections.Generic.LinkedList.Enumerator<object>
    // System.Collections.Generic.LinkedList<object>
    // System.Collections.Generic.LinkedListNode<object>
    // System.Collections.Generic.List.Enumerator<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Collections.Generic.List.Enumerator<Dreamteck.Splines.SplinePoint>
    // System.Collections.Generic.List.Enumerator<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Collections.Generic.List.Enumerator<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Collections.Generic.List.Enumerator<GameStudio.Slot.FSF.Frame>
    // System.Collections.Generic.List.Enumerator<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Collections.Generic.List.Enumerator<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Collections.Generic.List.Enumerator<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Collections.Generic.List.Enumerator<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Collections.Generic.List.Enumerator<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.List.Enumerator<System.DateTime>
    // System.Collections.Generic.List.Enumerator<System.ValueTuple<object,object>>
    // System.Collections.Generic.List.Enumerator<UnityEngine.BoneWeight>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Bounds>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Color>
    // System.Collections.Generic.List.Enumerator<UnityEngine.CombineInstance>
    // System.Collections.Generic.List.Enumerator<UnityEngine.ContactPoint2D>
    // System.Collections.Generic.List.Enumerator<UnityEngine.ContactPoint>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Keyframe>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Matrix4x4>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Quaternion>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Ray>
    // System.Collections.Generic.List.Enumerator<UnityEngine.RaycastHit2D>
    // System.Collections.Generic.List.Enumerator<UnityEngine.RaycastHit>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Rect>
    // System.Collections.Generic.List.Enumerator<UnityEngine.UIVertex>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Vector2>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Vector2Int>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Vector3>
    // System.Collections.Generic.List.Enumerator<UnityEngine.Vector4>
    // System.Collections.Generic.List.Enumerator<byte>
    // System.Collections.Generic.List.Enumerator<double>
    // System.Collections.Generic.List.Enumerator<float>
    // System.Collections.Generic.List.Enumerator<int>
    // System.Collections.Generic.List.Enumerator<long>
    // System.Collections.Generic.List.Enumerator<object>
    // System.Collections.Generic.List.Enumerator<uint>
    // System.Collections.Generic.List.Enumerator<ulong>
    // System.Collections.Generic.List.SynchronizedList<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Collections.Generic.List.SynchronizedList<Dreamteck.Splines.SplinePoint>
    // System.Collections.Generic.List.SynchronizedList<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Collections.Generic.List.SynchronizedList<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Collections.Generic.List.SynchronizedList<GameStudio.Slot.FSF.Frame>
    // System.Collections.Generic.List.SynchronizedList<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Collections.Generic.List.SynchronizedList<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Collections.Generic.List.SynchronizedList<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Collections.Generic.List.SynchronizedList<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Collections.Generic.List.SynchronizedList<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Collections.Generic.List.SynchronizedList<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Collections.Generic.List.SynchronizedList<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.List.SynchronizedList<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.List.SynchronizedList<System.DateTime>
    // System.Collections.Generic.List.SynchronizedList<System.ValueTuple<object,object>>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.BoneWeight>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.Bounds>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.Color>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.CombineInstance>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.ContactPoint2D>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.ContactPoint>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.Keyframe>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.Matrix4x4>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.Quaternion>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.Ray>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.RaycastHit2D>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.RaycastHit>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.Rect>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.UIVertex>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.Vector2>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.Vector2Int>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.Vector3>
    // System.Collections.Generic.List.SynchronizedList<UnityEngine.Vector4>
    // System.Collections.Generic.List.SynchronizedList<byte>
    // System.Collections.Generic.List.SynchronizedList<double>
    // System.Collections.Generic.List.SynchronizedList<float>
    // System.Collections.Generic.List.SynchronizedList<int>
    // System.Collections.Generic.List.SynchronizedList<long>
    // System.Collections.Generic.List.SynchronizedList<object>
    // System.Collections.Generic.List.SynchronizedList<uint>
    // System.Collections.Generic.List.SynchronizedList<ulong>
    // System.Collections.Generic.List<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Collections.Generic.List<Dreamteck.Splines.SplinePoint>
    // System.Collections.Generic.List<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Collections.Generic.List<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Collections.Generic.List<GameStudio.Slot.FSF.Frame>
    // System.Collections.Generic.List<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Collections.Generic.List<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Collections.Generic.List<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Collections.Generic.List<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Collections.Generic.List<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.List<System.DateTime>
    // System.Collections.Generic.List<System.ValueTuple<object,object>>
    // System.Collections.Generic.List<UnityEngine.BoneWeight>
    // System.Collections.Generic.List<UnityEngine.Bounds>
    // System.Collections.Generic.List<UnityEngine.Color>
    // System.Collections.Generic.List<UnityEngine.CombineInstance>
    // System.Collections.Generic.List<UnityEngine.ContactPoint2D>
    // System.Collections.Generic.List<UnityEngine.ContactPoint>
    // System.Collections.Generic.List<UnityEngine.Keyframe>
    // System.Collections.Generic.List<UnityEngine.Matrix4x4>
    // System.Collections.Generic.List<UnityEngine.Quaternion>
    // System.Collections.Generic.List<UnityEngine.Ray>
    // System.Collections.Generic.List<UnityEngine.RaycastHit2D>
    // System.Collections.Generic.List<UnityEngine.RaycastHit>
    // System.Collections.Generic.List<UnityEngine.Rect>
    // System.Collections.Generic.List<UnityEngine.UIVertex>
    // System.Collections.Generic.List<UnityEngine.Vector2>
    // System.Collections.Generic.List<UnityEngine.Vector2Int>
    // System.Collections.Generic.List<UnityEngine.Vector3>
    // System.Collections.Generic.List<UnityEngine.Vector4>
    // System.Collections.Generic.List<byte>
    // System.Collections.Generic.List<double>
    // System.Collections.Generic.List<float>
    // System.Collections.Generic.List<int>
    // System.Collections.Generic.List<long>
    // System.Collections.Generic.List<object>
    // System.Collections.Generic.List<uint>
    // System.Collections.Generic.List<ulong>
    // System.Collections.Generic.ObjectComparer<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Collections.Generic.ObjectComparer<Dreamteck.Splines.SplinePoint>
    // System.Collections.Generic.ObjectComparer<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Collections.Generic.ObjectComparer<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Collections.Generic.ObjectComparer<GameStudio.Slot.FSF.Frame>
    // System.Collections.Generic.ObjectComparer<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Collections.Generic.ObjectComparer<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Collections.Generic.ObjectComparer<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Collections.Generic.ObjectComparer<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Collections.Generic.ObjectComparer<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.ObjectComparer<System.DateTime>
    // System.Collections.Generic.ObjectComparer<System.ValueTuple<object,object>>
    // System.Collections.Generic.ObjectComparer<UnityEngine.BoneWeight>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Bounds>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Color>
    // System.Collections.Generic.ObjectComparer<UnityEngine.CombineInstance>
    // System.Collections.Generic.ObjectComparer<UnityEngine.ContactPoint2D>
    // System.Collections.Generic.ObjectComparer<UnityEngine.ContactPoint>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Keyframe>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Matrix4x4>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Quaternion>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Ray>
    // System.Collections.Generic.ObjectComparer<UnityEngine.RaycastHit2D>
    // System.Collections.Generic.ObjectComparer<UnityEngine.RaycastHit>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Rect>
    // System.Collections.Generic.ObjectComparer<UnityEngine.UIVertex>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Vector2>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Vector2Int>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Vector3>
    // System.Collections.Generic.ObjectComparer<UnityEngine.Vector4>
    // System.Collections.Generic.ObjectComparer<byte>
    // System.Collections.Generic.ObjectComparer<double>
    // System.Collections.Generic.ObjectComparer<float>
    // System.Collections.Generic.ObjectComparer<int>
    // System.Collections.Generic.ObjectComparer<long>
    // System.Collections.Generic.ObjectComparer<object>
    // System.Collections.Generic.ObjectComparer<uint>
    // System.Collections.Generic.ObjectComparer<ulong>
    // System.Collections.Generic.ObjectEqualityComparer<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Collections.Generic.ObjectEqualityComparer<Dreamteck.Splines.SplinePoint>
    // System.Collections.Generic.ObjectEqualityComparer<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Collections.Generic.ObjectEqualityComparer<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Collections.Generic.ObjectEqualityComparer<GameStudio.Slot.FSF.Frame>
    // System.Collections.Generic.ObjectEqualityComparer<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Collections.Generic.ObjectEqualityComparer<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Collections.Generic.ObjectEqualityComparer<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Collections.Generic.ObjectEqualityComparer<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Collections.Generic.ObjectEqualityComparer<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Collections.Generic.ObjectEqualityComparer<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Collections.Generic.ObjectEqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.ObjectEqualityComparer<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.ObjectEqualityComparer<System.DateTime>
    // System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<object,int>>
    // System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<object,object>>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.BoneWeight>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Bounds>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Color>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.CombineInstance>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.ContactPoint2D>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.ContactPoint>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Keyframe>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Matrix4x4>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Quaternion>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Ray>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.RaycastHit2D>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.RaycastHit>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Rect>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.UIVertex>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Vector2>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Vector2Int>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Vector3>
    // System.Collections.Generic.ObjectEqualityComparer<UnityEngine.Vector4>
    // System.Collections.Generic.ObjectEqualityComparer<byte>
    // System.Collections.Generic.ObjectEqualityComparer<double>
    // System.Collections.Generic.ObjectEqualityComparer<float>
    // System.Collections.Generic.ObjectEqualityComparer<int>
    // System.Collections.Generic.ObjectEqualityComparer<long>
    // System.Collections.Generic.ObjectEqualityComparer<object>
    // System.Collections.Generic.ObjectEqualityComparer<uint>
    // System.Collections.Generic.ObjectEqualityComparer<ulong>
    // System.Collections.Generic.Queue.Enumerator<int>
    // System.Collections.Generic.Queue.Enumerator<object>
    // System.Collections.Generic.Queue<int>
    // System.Collections.Generic.Queue<object>
    // System.Collections.Generic.Stack.Enumerator<object>
    // System.Collections.Generic.Stack<object>
    // System.Collections.ObjectModel.ReadOnlyCollection<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Collections.ObjectModel.ReadOnlyCollection<Dreamteck.Splines.SplinePoint>
    // System.Collections.ObjectModel.ReadOnlyCollection<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Collections.ObjectModel.ReadOnlyCollection<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Collections.ObjectModel.ReadOnlyCollection<GameStudio.Slot.FSF.Frame>
    // System.Collections.ObjectModel.ReadOnlyCollection<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Collections.ObjectModel.ReadOnlyCollection<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Collections.ObjectModel.ReadOnlyCollection<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Collections.ObjectModel.ReadOnlyCollection<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Collections.ObjectModel.ReadOnlyCollection<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.ObjectModel.ReadOnlyCollection<System.DateTime>
    // System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<object,object>>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.BoneWeight>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Bounds>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Color>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.CombineInstance>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.ContactPoint2D>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.ContactPoint>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Keyframe>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Matrix4x4>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Quaternion>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Ray>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.RaycastHit2D>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.RaycastHit>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Rect>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.UIVertex>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector2>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector2Int>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector3>
    // System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector4>
    // System.Collections.ObjectModel.ReadOnlyCollection<byte>
    // System.Collections.ObjectModel.ReadOnlyCollection<double>
    // System.Collections.ObjectModel.ReadOnlyCollection<float>
    // System.Collections.ObjectModel.ReadOnlyCollection<int>
    // System.Collections.ObjectModel.ReadOnlyCollection<long>
    // System.Collections.ObjectModel.ReadOnlyCollection<object>
    // System.Collections.ObjectModel.ReadOnlyCollection<uint>
    // System.Collections.ObjectModel.ReadOnlyCollection<ulong>
    // System.Comparison<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Comparison<Dreamteck.Splines.SplinePoint>
    // System.Comparison<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Comparison<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Comparison<GameStudio.Slot.FSF.Frame>
    // System.Comparison<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Comparison<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Comparison<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Comparison<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Comparison<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Comparison<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Comparison<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Comparison<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Comparison<System.DateTime>
    // System.Comparison<System.ValueTuple<object,object>>
    // System.Comparison<UnityEngine.BoneWeight>
    // System.Comparison<UnityEngine.Bounds>
    // System.Comparison<UnityEngine.Color>
    // System.Comparison<UnityEngine.CombineInstance>
    // System.Comparison<UnityEngine.ContactPoint2D>
    // System.Comparison<UnityEngine.ContactPoint>
    // System.Comparison<UnityEngine.Keyframe>
    // System.Comparison<UnityEngine.Matrix4x4>
    // System.Comparison<UnityEngine.Quaternion>
    // System.Comparison<UnityEngine.Ray>
    // System.Comparison<UnityEngine.RaycastHit2D>
    // System.Comparison<UnityEngine.RaycastHit>
    // System.Comparison<UnityEngine.Rect>
    // System.Comparison<UnityEngine.UIVertex>
    // System.Comparison<UnityEngine.Vector2>
    // System.Comparison<UnityEngine.Vector2Int>
    // System.Comparison<UnityEngine.Vector3>
    // System.Comparison<UnityEngine.Vector4>
    // System.Comparison<byte>
    // System.Comparison<double>
    // System.Comparison<float>
    // System.Comparison<int>
    // System.Comparison<long>
    // System.Comparison<object>
    // System.Comparison<uint>
    // System.Comparison<ulong>
    // System.Func<System.Collections.Generic.KeyValuePair<int,object>,object>
    // System.Func<System.Collections.Generic.KeyValuePair<object,byte>,byte>
    // System.Func<System.Collections.Generic.KeyValuePair<object,object>,byte>
    // System.Func<System.Collections.Generic.KeyValuePair<object,object>,object>
    // System.Func<System.Threading.Tasks.VoidTaskResult>
    // System.Func<System.ValueTuple<object,object>,byte>
    // System.Func<UnityEngine.Bounds>
    // System.Func<UnityEngine.CharacterInfo,byte>
    // System.Func<UnityEngine.Color>
    // System.Func<UnityEngine.ContactPoint2D>
    // System.Func<UnityEngine.ContactPoint>
    // System.Func<UnityEngine.Keyframe>
    // System.Func<UnityEngine.Quaternion>
    // System.Func<UnityEngine.Ray>
    // System.Func<UnityEngine.RaycastHit2D>
    // System.Func<UnityEngine.RaycastHit>
    // System.Func<UnityEngine.Rect>
    // System.Func<UnityEngine.Vector2>
    // System.Func<UnityEngine.Vector3,UnityEngine.Vector3,float,UnityEngine.Vector3>
    // System.Func<UnityEngine.Vector3,UnityEngine.Vector3>
    // System.Func<UnityEngine.Vector3,byte>
    // System.Func<UnityEngine.Vector3>
    // System.Func<UnityEngine.Vector4>
    // System.Func<byte,byte>
    // System.Func<byte>
    // System.Func<double,byte>
    // System.Func<double,double>
    // System.Func<double>
    // System.Func<float,byte>
    // System.Func<float>
    // System.Func<int,byte>
    // System.Func<int,int>
    // System.Func<int,object>
    // System.Func<int>
    // System.Func<long,byte>
    // System.Func<long>
    // System.Func<object,System.Threading.Tasks.VoidTaskResult>
    // System.Func<object,UnityEngine.Vector3>
    // System.Func<object,byte>
    // System.Func<object,float>
    // System.Func<object,int>
    // System.Func<object,long>
    // System.Func<object,object,object,object>
    // System.Func<object,object,object>
    // System.Func<object,object>
    // System.Func<object>
    // System.Func<uint>
    // System.Func<ulong>
    // System.IComparable<byte>
    // System.IComparable<double>
    // System.IComparable<int>
    // System.IComparable<long>
    // System.IComparable<object>
    // System.IEquatable<object>
    // System.Linq.Buffer<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Linq.Buffer<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Linq.Buffer<byte>
    // System.Linq.Buffer<int>
    // System.Linq.Buffer<object>
    // System.Linq.Enumerable.<CastIterator>d__99<object>
    // System.Linq.Enumerable.<DistinctIterator>d__68<object>
    // System.Linq.Enumerable.<IntersectIterator>d__74<int>
    // System.Linq.Enumerable.<TakeIterator>d__25<byte>
    // System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Linq.Enumerable.Iterator<UnityEngine.Vector3>
    // System.Linq.Enumerable.Iterator<byte>
    // System.Linq.Enumerable.Iterator<float>
    // System.Linq.Enumerable.Iterator<int>
    // System.Linq.Enumerable.Iterator<long>
    // System.Linq.Enumerable.Iterator<object>
    // System.Linq.Enumerable.WhereArrayIterator<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Linq.Enumerable.WhereArrayIterator<int>
    // System.Linq.Enumerable.WhereArrayIterator<object>
    // System.Linq.Enumerable.WhereEnumerableIterator<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Linq.Enumerable.WhereEnumerableIterator<UnityEngine.Vector3>
    // System.Linq.Enumerable.WhereEnumerableIterator<byte>
    // System.Linq.Enumerable.WhereEnumerableIterator<float>
    // System.Linq.Enumerable.WhereEnumerableIterator<int>
    // System.Linq.Enumerable.WhereEnumerableIterator<long>
    // System.Linq.Enumerable.WhereEnumerableIterator<object>
    // System.Linq.Enumerable.WhereListIterator<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Linq.Enumerable.WhereListIterator<int>
    // System.Linq.Enumerable.WhereListIterator<object>
    // System.Linq.Enumerable.WhereSelectArrayIterator<System.Collections.Generic.KeyValuePair<object,object>,object>
    // System.Linq.Enumerable.WhereSelectArrayIterator<UnityEngine.Vector3,UnityEngine.Vector3>
    // System.Linq.Enumerable.WhereSelectArrayIterator<int,object>
    // System.Linq.Enumerable.WhereSelectArrayIterator<object,UnityEngine.Vector3>
    // System.Linq.Enumerable.WhereSelectArrayIterator<object,byte>
    // System.Linq.Enumerable.WhereSelectArrayIterator<object,float>
    // System.Linq.Enumerable.WhereSelectArrayIterator<object,int>
    // System.Linq.Enumerable.WhereSelectArrayIterator<object,long>
    // System.Linq.Enumerable.WhereSelectArrayIterator<object,object>
    // System.Linq.Enumerable.WhereSelectEnumerableIterator<System.Collections.Generic.KeyValuePair<object,object>,object>
    // System.Linq.Enumerable.WhereSelectEnumerableIterator<UnityEngine.Vector3,UnityEngine.Vector3>
    // System.Linq.Enumerable.WhereSelectEnumerableIterator<int,object>
    // System.Linq.Enumerable.WhereSelectEnumerableIterator<object,UnityEngine.Vector3>
    // System.Linq.Enumerable.WhereSelectEnumerableIterator<object,byte>
    // System.Linq.Enumerable.WhereSelectEnumerableIterator<object,float>
    // System.Linq.Enumerable.WhereSelectEnumerableIterator<object,int>
    // System.Linq.Enumerable.WhereSelectEnumerableIterator<object,long>
    // System.Linq.Enumerable.WhereSelectEnumerableIterator<object,object>
    // System.Linq.Enumerable.WhereSelectListIterator<System.Collections.Generic.KeyValuePair<object,object>,object>
    // System.Linq.Enumerable.WhereSelectListIterator<UnityEngine.Vector3,UnityEngine.Vector3>
    // System.Linq.Enumerable.WhereSelectListIterator<int,object>
    // System.Linq.Enumerable.WhereSelectListIterator<object,UnityEngine.Vector3>
    // System.Linq.Enumerable.WhereSelectListIterator<object,byte>
    // System.Linq.Enumerable.WhereSelectListIterator<object,float>
    // System.Linq.Enumerable.WhereSelectListIterator<object,int>
    // System.Linq.Enumerable.WhereSelectListIterator<object,long>
    // System.Linq.Enumerable.WhereSelectListIterator<object,object>
    // System.Linq.EnumerableSorter<System.Collections.Generic.KeyValuePair<int,object>,object>
    // System.Linq.EnumerableSorter<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Linq.EnumerableSorter<System.Collections.Generic.KeyValuePair<object,object>,object>
    // System.Linq.EnumerableSorter<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Linq.EnumerableSorter<int,int>
    // System.Linq.EnumerableSorter<int>
    // System.Linq.EnumerableSorter<object,int>
    // System.Linq.EnumerableSorter<object,object>
    // System.Linq.EnumerableSorter<object>
    // System.Linq.OrderedEnumerable.<GetEnumerator>d__1<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Linq.OrderedEnumerable.<GetEnumerator>d__1<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Linq.OrderedEnumerable.<GetEnumerator>d__1<int>
    // System.Linq.OrderedEnumerable.<GetEnumerator>d__1<object>
    // System.Linq.OrderedEnumerable<System.Collections.Generic.KeyValuePair<int,object>,object>
    // System.Linq.OrderedEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Linq.OrderedEnumerable<System.Collections.Generic.KeyValuePair<object,object>,object>
    // System.Linq.OrderedEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Linq.OrderedEnumerable<int,int>
    // System.Linq.OrderedEnumerable<int>
    // System.Linq.OrderedEnumerable<object,int>
    // System.Linq.OrderedEnumerable<object,object>
    // System.Linq.OrderedEnumerable<object>
    // System.Linq.Set<int>
    // System.Linq.Set<object>
    // System.Nullable<System.DateTime>
    // System.Nullable<byte>
    // System.Nullable<double>
    // System.Nullable<float>
    // System.Nullable<int>
    // System.Nullable<long>
    // System.Predicate<BagelCode.EligibleBetItem.BetTextInfo>
    // System.Predicate<Dreamteck.Splines.SplinePoint>
    // System.Predicate<GameStudio.Slot.EDM.Feature.EDMBaseGameRandomUpgradeFlyData>
    // System.Predicate<GameStudio.Slot.EDM.Feature.EDMRandomUpgradeFlyData>
    // System.Predicate<GameStudio.Slot.FSF.Frame>
    // System.Predicate<GameStudio.Slot.IIP.Feature.IIPIceFlyingInfo>
    // System.Predicate<GameStudio.Slot.NDS.NDSSuperBonusMultiplierSubSymbolController.SpriteList>
    // System.Predicate<GameStudio.Slot.NDS.NDSWildMultiplierContorller.SpriteList>
    // System.Predicate<SlotMaker.PerformanceAnalyzer.TimeSample>
    // System.Predicate<SlotMaker.ScrollStatusMatchDetailtListCreator.DetailCellInfo>
    // System.Predicate<System.Collections.Generic.KeyValuePair<int,int>>
    // System.Predicate<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Predicate<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Predicate<System.DateTime>
    // System.Predicate<System.ValueTuple<object,object>>
    // System.Predicate<UnityEngine.BoneWeight>
    // System.Predicate<UnityEngine.Bounds>
    // System.Predicate<UnityEngine.Color>
    // System.Predicate<UnityEngine.CombineInstance>
    // System.Predicate<UnityEngine.ContactPoint2D>
    // System.Predicate<UnityEngine.ContactPoint>
    // System.Predicate<UnityEngine.Keyframe>
    // System.Predicate<UnityEngine.Matrix4x4>
    // System.Predicate<UnityEngine.Quaternion>
    // System.Predicate<UnityEngine.Ray>
    // System.Predicate<UnityEngine.RaycastHit2D>
    // System.Predicate<UnityEngine.RaycastHit>
    // System.Predicate<UnityEngine.Rect>
    // System.Predicate<UnityEngine.UIVertex>
    // System.Predicate<UnityEngine.Vector2>
    // System.Predicate<UnityEngine.Vector2Int>
    // System.Predicate<UnityEngine.Vector3>
    // System.Predicate<UnityEngine.Vector4>
    // System.Predicate<byte>
    // System.Predicate<double>
    // System.Predicate<float>
    // System.Predicate<int>
    // System.Predicate<long>
    // System.Predicate<object>
    // System.Predicate<uint>
    // System.Predicate<ulong>
    // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.Threading.Tasks.VoidTaskResult>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.Threading.Tasks.VoidTaskResult>
    // System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
    // System.Runtime.CompilerServices.TaskAwaiter<System.Threading.Tasks.VoidTaskResult>
    // System.Runtime.CompilerServices.TaskAwaiter<object>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<System.Threading.Tasks.VoidTaskResult>
    // System.Threading.Tasks.ContinuationTaskFromResultTask<object>
    // System.Threading.Tasks.Task.<>c<System.Threading.Tasks.VoidTaskResult>
    // System.Threading.Tasks.Task.<>c<object>
    // System.Threading.Tasks.Task<System.Threading.Tasks.VoidTaskResult>
    // System.Threading.Tasks.Task<object>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.Threading.Tasks.VoidTaskResult>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<object>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_1<System.Threading.Tasks.VoidTaskResult>
    // System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_1<object>
    // System.Threading.Tasks.TaskFactory<System.Threading.Tasks.VoidTaskResult>
    // System.Threading.Tasks.TaskFactory<object>
    // System.Tuple<object,object>
    // System.ValueTuple<byte,object>
    // System.ValueTuple<int,object>
    // System.ValueTuple<object,int>
    // System.ValueTuple<object,object,int,long>
    // System.ValueTuple<object,object>
    // UnityEngine.Events.InvokableCall<System.DateTime>
    // UnityEngine.Events.InvokableCall<UnityEngine.Bounds>
    // UnityEngine.Events.InvokableCall<UnityEngine.Color>
    // UnityEngine.Events.InvokableCall<UnityEngine.ContactPoint2D>
    // UnityEngine.Events.InvokableCall<UnityEngine.ContactPoint>
    // UnityEngine.Events.InvokableCall<UnityEngine.Keyframe>
    // UnityEngine.Events.InvokableCall<UnityEngine.Quaternion>
    // UnityEngine.Events.InvokableCall<UnityEngine.Ray>
    // UnityEngine.Events.InvokableCall<UnityEngine.RaycastHit2D>
    // UnityEngine.Events.InvokableCall<UnityEngine.RaycastHit>
    // UnityEngine.Events.InvokableCall<UnityEngine.Rect>
    // UnityEngine.Events.InvokableCall<UnityEngine.Vector2>
    // UnityEngine.Events.InvokableCall<UnityEngine.Vector3>
    // UnityEngine.Events.InvokableCall<UnityEngine.Vector4>
    // UnityEngine.Events.InvokableCall<byte>
    // UnityEngine.Events.InvokableCall<double>
    // UnityEngine.Events.InvokableCall<float,int,int>
    // UnityEngine.Events.InvokableCall<float>
    // UnityEngine.Events.InvokableCall<int>
    // UnityEngine.Events.InvokableCall<long>
    // UnityEngine.Events.InvokableCall<object,object>
    // UnityEngine.Events.InvokableCall<object>
    // UnityEngine.Events.InvokableCall<uint>
    // UnityEngine.Events.InvokableCall<ulong>
    // UnityEngine.Events.UnityAction<System.DateTime>
    // UnityEngine.Events.UnityAction<UnityEngine.Bounds>
    // UnityEngine.Events.UnityAction<UnityEngine.Color>
    // UnityEngine.Events.UnityAction<UnityEngine.ContactPoint2D>
    // UnityEngine.Events.UnityAction<UnityEngine.ContactPoint>
    // UnityEngine.Events.UnityAction<UnityEngine.Keyframe>
    // UnityEngine.Events.UnityAction<UnityEngine.Quaternion>
    // UnityEngine.Events.UnityAction<UnityEngine.Ray>
    // UnityEngine.Events.UnityAction<UnityEngine.RaycastHit2D>
    // UnityEngine.Events.UnityAction<UnityEngine.RaycastHit>
    // UnityEngine.Events.UnityAction<UnityEngine.Rect>
    // UnityEngine.Events.UnityAction<UnityEngine.Vector2>
    // UnityEngine.Events.UnityAction<UnityEngine.Vector3>
    // UnityEngine.Events.UnityAction<UnityEngine.Vector4>
    // UnityEngine.Events.UnityAction<byte>
    // UnityEngine.Events.UnityAction<double>
    // UnityEngine.Events.UnityAction<float,int,int>
    // UnityEngine.Events.UnityAction<float>
    // UnityEngine.Events.UnityAction<int>
    // UnityEngine.Events.UnityAction<long>
    // UnityEngine.Events.UnityAction<object,object>
    // UnityEngine.Events.UnityAction<object>
    // UnityEngine.Events.UnityAction<uint>
    // UnityEngine.Events.UnityAction<ulong>
    // UnityEngine.Events.UnityEvent<System.DateTime>
    // UnityEngine.Events.UnityEvent<UnityEngine.Bounds>
    // UnityEngine.Events.UnityEvent<UnityEngine.Color>
    // UnityEngine.Events.UnityEvent<UnityEngine.ContactPoint2D>
    // UnityEngine.Events.UnityEvent<UnityEngine.ContactPoint>
    // UnityEngine.Events.UnityEvent<UnityEngine.Keyframe>
    // UnityEngine.Events.UnityEvent<UnityEngine.Quaternion>
    // UnityEngine.Events.UnityEvent<UnityEngine.Ray>
    // UnityEngine.Events.UnityEvent<UnityEngine.RaycastHit2D>
    // UnityEngine.Events.UnityEvent<UnityEngine.RaycastHit>
    // UnityEngine.Events.UnityEvent<UnityEngine.Rect>
    // UnityEngine.Events.UnityEvent<UnityEngine.Vector2>
    // UnityEngine.Events.UnityEvent<UnityEngine.Vector3>
    // UnityEngine.Events.UnityEvent<UnityEngine.Vector4>
    // UnityEngine.Events.UnityEvent<byte>
    // UnityEngine.Events.UnityEvent<double>
    // UnityEngine.Events.UnityEvent<float,int,int>
    // UnityEngine.Events.UnityEvent<float>
    // UnityEngine.Events.UnityEvent<int>
    // UnityEngine.Events.UnityEvent<long>
    // UnityEngine.Events.UnityEvent<object,object>
    // UnityEngine.Events.UnityEvent<object>
    // UnityEngine.Events.UnityEvent<uint>
    // UnityEngine.Events.UnityEvent<ulong>
    // ZXing.BarcodeWriterGeneric<object>
    // ZXing.Rendering.IBarcodeRenderer<object>
    // }}

    public void RefMethods()
    {
        // System.Void BagelCode.Internal.BagelCodeHTTP.ChatPoll<object,object>(string,System.Collections.Generic.Dictionary<string,string>,object,System.Func<byte[],object>,System.Action<object>,BagelCode.HTTPErrorCallback)
        // System.Void BagelCode.Internal.BagelCodeHTTP.LongPoll<object,object>(string,object,System.Func<byte[],object>,System.Action<object>,BagelCode.HTTPErrorCallback)
        // System.Void BagelCode.Internal.BagelCodeHTTP.MakeApiCall<object,object>(string,System.Collections.Generic.Dictionary<string,string>,object,System.Func<byte[],object>,System.Action<object>,BagelCode.HTTPErrorCallback,bool)
        // System.Void BagelCode.Internal.BagelCodeHTTP.MakeApiChatCall<object,object>(string,System.Collections.Generic.Dictionary<string,string>,object,System.Func<byte[],object>,System.Action<object>,BagelCode.HTTPErrorCallback,bool)
        // System.Void BagelCode.Internal.BagelCodeHTTP.RequestContainer<object>(BagelCode.Internal.CallRequestContainer,System.Action<object>,System.Func<byte[],object>,BagelCode.HTTPErrorCallback)
        // object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string)
        // object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string,Newtonsoft.Json.JsonSerializerSettings)
        // UnityEngine.Bounds NodeCanvas.Framework.Blackboard.GetValue<UnityEngine.Bounds>(string)
        // UnityEngine.Color NodeCanvas.Framework.Blackboard.GetValue<UnityEngine.Color>(string)
        // UnityEngine.ContactPoint NodeCanvas.Framework.Blackboard.GetValue<UnityEngine.ContactPoint>(string)
        // UnityEngine.ContactPoint2D NodeCanvas.Framework.Blackboard.GetValue<UnityEngine.ContactPoint2D>(string)
        // UnityEngine.Keyframe NodeCanvas.Framework.Blackboard.GetValue<UnityEngine.Keyframe>(string)
        // UnityEngine.Quaternion NodeCanvas.Framework.Blackboard.GetValue<UnityEngine.Quaternion>(string)
        // UnityEngine.Ray NodeCanvas.Framework.Blackboard.GetValue<UnityEngine.Ray>(string)
        // UnityEngine.RaycastHit NodeCanvas.Framework.Blackboard.GetValue<UnityEngine.RaycastHit>(string)
        // UnityEngine.RaycastHit2D NodeCanvas.Framework.Blackboard.GetValue<UnityEngine.RaycastHit2D>(string)
        // UnityEngine.Rect NodeCanvas.Framework.Blackboard.GetValue<UnityEngine.Rect>(string)
        // UnityEngine.Vector2 NodeCanvas.Framework.Blackboard.GetValue<UnityEngine.Vector2>(string)
        // UnityEngine.Vector3 NodeCanvas.Framework.Blackboard.GetValue<UnityEngine.Vector3>(string)
        // UnityEngine.Vector4 NodeCanvas.Framework.Blackboard.GetValue<UnityEngine.Vector4>(string)
        // byte NodeCanvas.Framework.Blackboard.GetValue<byte>(string)
        // double NodeCanvas.Framework.Blackboard.GetValue<double>(string)
        // float NodeCanvas.Framework.Blackboard.GetValue<float>(string)
        // int NodeCanvas.Framework.Blackboard.GetValue<int>(string)
        // long NodeCanvas.Framework.Blackboard.GetValue<long>(string)
        // object NodeCanvas.Framework.Blackboard.GetValue<object>(string)
        // uint NodeCanvas.Framework.Blackboard.GetValue<uint>(string)
        // ulong NodeCanvas.Framework.Blackboard.GetValue<ulong>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Bounds> NodeCanvas.Framework.Blackboard.GetVariable<UnityEngine.Bounds>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Color> NodeCanvas.Framework.Blackboard.GetVariable<UnityEngine.Color>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.ContactPoint2D> NodeCanvas.Framework.Blackboard.GetVariable<UnityEngine.ContactPoint2D>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.ContactPoint> NodeCanvas.Framework.Blackboard.GetVariable<UnityEngine.ContactPoint>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Keyframe> NodeCanvas.Framework.Blackboard.GetVariable<UnityEngine.Keyframe>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Quaternion> NodeCanvas.Framework.Blackboard.GetVariable<UnityEngine.Quaternion>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Ray> NodeCanvas.Framework.Blackboard.GetVariable<UnityEngine.Ray>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.RaycastHit2D> NodeCanvas.Framework.Blackboard.GetVariable<UnityEngine.RaycastHit2D>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.RaycastHit> NodeCanvas.Framework.Blackboard.GetVariable<UnityEngine.RaycastHit>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Rect> NodeCanvas.Framework.Blackboard.GetVariable<UnityEngine.Rect>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Vector2> NodeCanvas.Framework.Blackboard.GetVariable<UnityEngine.Vector2>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Vector3> NodeCanvas.Framework.Blackboard.GetVariable<UnityEngine.Vector3>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Vector4> NodeCanvas.Framework.Blackboard.GetVariable<UnityEngine.Vector4>(string)
        // NodeCanvas.Framework.Variable<byte> NodeCanvas.Framework.Blackboard.GetVariable<byte>(string)
        // NodeCanvas.Framework.Variable<double> NodeCanvas.Framework.Blackboard.GetVariable<double>(string)
        // NodeCanvas.Framework.Variable<float> NodeCanvas.Framework.Blackboard.GetVariable<float>(string)
        // NodeCanvas.Framework.Variable<int> NodeCanvas.Framework.Blackboard.GetVariable<int>(string)
        // NodeCanvas.Framework.Variable<long> NodeCanvas.Framework.Blackboard.GetVariable<long>(string)
        // NodeCanvas.Framework.Variable<object> NodeCanvas.Framework.Blackboard.GetVariable<object>(string)
        // NodeCanvas.Framework.Variable<uint> NodeCanvas.Framework.Blackboard.GetVariable<uint>(string)
        // NodeCanvas.Framework.Variable<ulong> NodeCanvas.Framework.Blackboard.GetVariable<ulong>(string)
        // object NodeCanvas.Framework.Graph.Clone<object>(object,NodeCanvas.Framework.Graph)
        // System.Void NodeCanvas.Framework.GraphOwner.SendEvent<object>(string,object)
        // System.Void NodeCanvas.Framework.GraphOwner.SendGlobalEvent<object>(string,object)
        // UnityEngine.Bounds NodeCanvas.Framework.IBlackboard.GetValue<UnityEngine.Bounds>(string)
        // UnityEngine.Color NodeCanvas.Framework.IBlackboard.GetValue<UnityEngine.Color>(string)
        // UnityEngine.ContactPoint NodeCanvas.Framework.IBlackboard.GetValue<UnityEngine.ContactPoint>(string)
        // UnityEngine.ContactPoint2D NodeCanvas.Framework.IBlackboard.GetValue<UnityEngine.ContactPoint2D>(string)
        // UnityEngine.Keyframe NodeCanvas.Framework.IBlackboard.GetValue<UnityEngine.Keyframe>(string)
        // UnityEngine.Quaternion NodeCanvas.Framework.IBlackboard.GetValue<UnityEngine.Quaternion>(string)
        // UnityEngine.Ray NodeCanvas.Framework.IBlackboard.GetValue<UnityEngine.Ray>(string)
        // UnityEngine.RaycastHit NodeCanvas.Framework.IBlackboard.GetValue<UnityEngine.RaycastHit>(string)
        // UnityEngine.RaycastHit2D NodeCanvas.Framework.IBlackboard.GetValue<UnityEngine.RaycastHit2D>(string)
        // UnityEngine.Rect NodeCanvas.Framework.IBlackboard.GetValue<UnityEngine.Rect>(string)
        // UnityEngine.Vector2 NodeCanvas.Framework.IBlackboard.GetValue<UnityEngine.Vector2>(string)
        // UnityEngine.Vector3 NodeCanvas.Framework.IBlackboard.GetValue<UnityEngine.Vector3>(string)
        // UnityEngine.Vector4 NodeCanvas.Framework.IBlackboard.GetValue<UnityEngine.Vector4>(string)
        // byte NodeCanvas.Framework.IBlackboard.GetValue<byte>(string)
        // double NodeCanvas.Framework.IBlackboard.GetValue<double>(string)
        // float NodeCanvas.Framework.IBlackboard.GetValue<float>(string)
        // int NodeCanvas.Framework.IBlackboard.GetValue<int>(string)
        // long NodeCanvas.Framework.IBlackboard.GetValue<long>(string)
        // object NodeCanvas.Framework.IBlackboard.GetValue<object>(string)
        // uint NodeCanvas.Framework.IBlackboard.GetValue<uint>(string)
        // ulong NodeCanvas.Framework.IBlackboard.GetValue<ulong>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Bounds> NodeCanvas.Framework.IBlackboard.GetVariable<UnityEngine.Bounds>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Color> NodeCanvas.Framework.IBlackboard.GetVariable<UnityEngine.Color>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.ContactPoint2D> NodeCanvas.Framework.IBlackboard.GetVariable<UnityEngine.ContactPoint2D>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.ContactPoint> NodeCanvas.Framework.IBlackboard.GetVariable<UnityEngine.ContactPoint>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Keyframe> NodeCanvas.Framework.IBlackboard.GetVariable<UnityEngine.Keyframe>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Quaternion> NodeCanvas.Framework.IBlackboard.GetVariable<UnityEngine.Quaternion>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Ray> NodeCanvas.Framework.IBlackboard.GetVariable<UnityEngine.Ray>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.RaycastHit2D> NodeCanvas.Framework.IBlackboard.GetVariable<UnityEngine.RaycastHit2D>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.RaycastHit> NodeCanvas.Framework.IBlackboard.GetVariable<UnityEngine.RaycastHit>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Rect> NodeCanvas.Framework.IBlackboard.GetVariable<UnityEngine.Rect>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Vector2> NodeCanvas.Framework.IBlackboard.GetVariable<UnityEngine.Vector2>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Vector3> NodeCanvas.Framework.IBlackboard.GetVariable<UnityEngine.Vector3>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Vector4> NodeCanvas.Framework.IBlackboard.GetVariable<UnityEngine.Vector4>(string)
        // NodeCanvas.Framework.Variable<byte> NodeCanvas.Framework.IBlackboard.GetVariable<byte>(string)
        // NodeCanvas.Framework.Variable<double> NodeCanvas.Framework.IBlackboard.GetVariable<double>(string)
        // NodeCanvas.Framework.Variable<float> NodeCanvas.Framework.IBlackboard.GetVariable<float>(string)
        // NodeCanvas.Framework.Variable<int> NodeCanvas.Framework.IBlackboard.GetVariable<int>(string)
        // NodeCanvas.Framework.Variable<long> NodeCanvas.Framework.IBlackboard.GetVariable<long>(string)
        // NodeCanvas.Framework.Variable<object> NodeCanvas.Framework.IBlackboard.GetVariable<object>(string)
        // NodeCanvas.Framework.Variable<uint> NodeCanvas.Framework.IBlackboard.GetVariable<uint>(string)
        // NodeCanvas.Framework.Variable<ulong> NodeCanvas.Framework.IBlackboard.GetVariable<ulong>(string)
        // UnityEngine.Bounds NodeCanvas.Framework.Internal.BlackboardSource.GetValue<UnityEngine.Bounds>(string)
        // UnityEngine.Color NodeCanvas.Framework.Internal.BlackboardSource.GetValue<UnityEngine.Color>(string)
        // UnityEngine.ContactPoint NodeCanvas.Framework.Internal.BlackboardSource.GetValue<UnityEngine.ContactPoint>(string)
        // UnityEngine.ContactPoint2D NodeCanvas.Framework.Internal.BlackboardSource.GetValue<UnityEngine.ContactPoint2D>(string)
        // UnityEngine.Keyframe NodeCanvas.Framework.Internal.BlackboardSource.GetValue<UnityEngine.Keyframe>(string)
        // UnityEngine.Quaternion NodeCanvas.Framework.Internal.BlackboardSource.GetValue<UnityEngine.Quaternion>(string)
        // UnityEngine.Ray NodeCanvas.Framework.Internal.BlackboardSource.GetValue<UnityEngine.Ray>(string)
        // UnityEngine.RaycastHit NodeCanvas.Framework.Internal.BlackboardSource.GetValue<UnityEngine.RaycastHit>(string)
        // UnityEngine.RaycastHit2D NodeCanvas.Framework.Internal.BlackboardSource.GetValue<UnityEngine.RaycastHit2D>(string)
        // UnityEngine.Rect NodeCanvas.Framework.Internal.BlackboardSource.GetValue<UnityEngine.Rect>(string)
        // UnityEngine.Vector2 NodeCanvas.Framework.Internal.BlackboardSource.GetValue<UnityEngine.Vector2>(string)
        // UnityEngine.Vector3 NodeCanvas.Framework.Internal.BlackboardSource.GetValue<UnityEngine.Vector3>(string)
        // UnityEngine.Vector4 NodeCanvas.Framework.Internal.BlackboardSource.GetValue<UnityEngine.Vector4>(string)
        // byte NodeCanvas.Framework.Internal.BlackboardSource.GetValue<byte>(string)
        // double NodeCanvas.Framework.Internal.BlackboardSource.GetValue<double>(string)
        // float NodeCanvas.Framework.Internal.BlackboardSource.GetValue<float>(string)
        // int NodeCanvas.Framework.Internal.BlackboardSource.GetValue<int>(string)
        // long NodeCanvas.Framework.Internal.BlackboardSource.GetValue<long>(string)
        // object NodeCanvas.Framework.Internal.BlackboardSource.GetValue<object>(string)
        // uint NodeCanvas.Framework.Internal.BlackboardSource.GetValue<uint>(string)
        // ulong NodeCanvas.Framework.Internal.BlackboardSource.GetValue<ulong>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Bounds> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<UnityEngine.Bounds>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Color> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<UnityEngine.Color>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.ContactPoint2D> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<UnityEngine.ContactPoint2D>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.ContactPoint> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<UnityEngine.ContactPoint>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Keyframe> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<UnityEngine.Keyframe>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Quaternion> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<UnityEngine.Quaternion>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Ray> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<UnityEngine.Ray>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.RaycastHit2D> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<UnityEngine.RaycastHit2D>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.RaycastHit> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<UnityEngine.RaycastHit>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Rect> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<UnityEngine.Rect>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Vector2> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<UnityEngine.Vector2>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Vector3> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<UnityEngine.Vector3>(string)
        // NodeCanvas.Framework.Variable<UnityEngine.Vector4> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<UnityEngine.Vector4>(string)
        // NodeCanvas.Framework.Variable<byte> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<byte>(string)
        // NodeCanvas.Framework.Variable<double> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<double>(string)
        // NodeCanvas.Framework.Variable<float> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<float>(string)
        // NodeCanvas.Framework.Variable<int> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<int>(string)
        // NodeCanvas.Framework.Variable<long> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<long>(string)
        // NodeCanvas.Framework.Variable<object> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<object>(string)
        // NodeCanvas.Framework.Variable<uint> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<uint>(string)
        // NodeCanvas.Framework.Variable<ulong> NodeCanvas.Framework.Internal.BlackboardSource.GetVariable<ulong>(string)
        // System.Void NodeCanvas.Framework.Task.SendEvent<int>(string,int)
        // System.Void NodeCanvas.Framework.Task.SendEvent<object>(string,object)
        // object ParadoxNotion.Serialization.JSONSerializer.Deserialize<object>(string,System.Collections.Generic.List<UnityEngine.Object>)
        // bool ParadoxNotion.Services.MessageRouter.Dispatch<object>(string,object,object)
        // System.Void ParadoxNotion.Services.MessageRouter.RegisterCallback<object>(string,System.Action<object>)
        // object SlotMaker.AssetBundleLoadAssetOperation.GetAsset<object>()
        // object SlotMaker.AssetBundleManager.LoadAsset<object>(string,string)
        // SlotMaker.AssetBundleLoadAssetOperation SlotMaker.AssetBundleManager.LoadAssetAsync<object>(string,string)
        // byte SlotMaker.BlackboardUtils.FindValue<byte>(NodeCanvas.Framework.IBlackboard,string)
        // byte SlotMaker.BlackboardUtils.FindValue<byte>(string)
        // double SlotMaker.BlackboardUtils.FindValue<double>(NodeCanvas.Framework.IBlackboard,string)
        // float SlotMaker.BlackboardUtils.FindValue<float>(NodeCanvas.Framework.IBlackboard,string)
        // int SlotMaker.BlackboardUtils.FindValue<int>(NodeCanvas.Framework.IBlackboard,string)
        // int SlotMaker.BlackboardUtils.FindValue<int>(string)
        // long SlotMaker.BlackboardUtils.FindValue<long>(NodeCanvas.Framework.IBlackboard,string)
        // long SlotMaker.BlackboardUtils.FindValue<long>(string)
        // object SlotMaker.BlackboardUtils.FindValue<object>(NodeCanvas.Framework.IBlackboard,string)
        // object SlotMaker.BlackboardUtils.FindValue<object>(string)
        // NodeCanvas.Framework.Variable<byte> SlotMaker.BlackboardUtils.FindVariable<byte>(NodeCanvas.Framework.IBlackboard,string)
        // NodeCanvas.Framework.Variable<byte> SlotMaker.BlackboardUtils.FindVariable<byte>(string)
        // NodeCanvas.Framework.Variable<double> SlotMaker.BlackboardUtils.FindVariable<double>(NodeCanvas.Framework.IBlackboard,string)
        // NodeCanvas.Framework.Variable<int> SlotMaker.BlackboardUtils.FindVariable<int>(NodeCanvas.Framework.IBlackboard,string)
        // NodeCanvas.Framework.Variable<int> SlotMaker.BlackboardUtils.FindVariable<int>(string)
        // NodeCanvas.Framework.Variable<long> SlotMaker.BlackboardUtils.FindVariable<long>(NodeCanvas.Framework.IBlackboard,string)
        // NodeCanvas.Framework.Variable<long> SlotMaker.BlackboardUtils.FindVariable<long>(string)
        // NodeCanvas.Framework.Variable<object> SlotMaker.BlackboardUtils.FindVariable<object>(NodeCanvas.Framework.IBlackboard,string)
        // NodeCanvas.Framework.Variable<object> SlotMaker.BlackboardUtils.FindVariable<object>(string)
        // NodeCanvas.Framework.Variable<uint> SlotMaker.BlackboardUtils.FindVariable<uint>(NodeCanvas.Framework.IBlackboard,string)
        // NodeCanvas.Framework.Variable<byte> SlotMaker.BlackboardUtils.GetOrCreateVariable<byte>(NodeCanvas.Framework.IBlackboard,string,bool)
        // NodeCanvas.Framework.Variable<byte> SlotMaker.BlackboardUtils.GetOrCreateVariable<byte>(string)
        // NodeCanvas.Framework.Variable<double> SlotMaker.BlackboardUtils.GetOrCreateVariable<double>(NodeCanvas.Framework.IBlackboard,string,bool)
        // NodeCanvas.Framework.Variable<float> SlotMaker.BlackboardUtils.GetOrCreateVariable<float>(NodeCanvas.Framework.IBlackboard,string,bool)
        // NodeCanvas.Framework.Variable<int> SlotMaker.BlackboardUtils.GetOrCreateVariable<int>(NodeCanvas.Framework.IBlackboard,string,bool)
        // NodeCanvas.Framework.Variable<int> SlotMaker.BlackboardUtils.GetOrCreateVariable<int>(string)
        // NodeCanvas.Framework.Variable<long> SlotMaker.BlackboardUtils.GetOrCreateVariable<long>(NodeCanvas.Framework.IBlackboard,string,bool)
        // NodeCanvas.Framework.Variable<long> SlotMaker.BlackboardUtils.GetOrCreateVariable<long>(string)
        // NodeCanvas.Framework.Variable<object> SlotMaker.BlackboardUtils.GetOrCreateVariable<object>(NodeCanvas.Framework.IBlackboard,string,bool)
        // NodeCanvas.Framework.Variable<object> SlotMaker.BlackboardUtils.GetOrCreateVariable<object>(string)
        // NodeCanvas.Framework.Variable<ulong> SlotMaker.BlackboardUtils.GetOrCreateVariable<ulong>(NodeCanvas.Framework.IBlackboard,string,bool)
        // System.Void SlotMaker.BlackboardUtils.SetOrCreateList<object>(NodeCanvas.Framework.IBlackboard,string,System.Collections.Generic.List<object>,SlotMaker.BlackboardUtils.SerializeToBB<object>)
        // System.Void SlotMaker.BlackboardUtils.SetOrCreateValue<byte>(NodeCanvas.Framework.IBlackboard,string,byte)
        // System.Void SlotMaker.BlackboardUtils.SetOrCreateValue<double>(NodeCanvas.Framework.IBlackboard,string,double)
        // System.Void SlotMaker.BlackboardUtils.SetOrCreateValue<float>(NodeCanvas.Framework.IBlackboard,string,float)
        // System.Void SlotMaker.BlackboardUtils.SetOrCreateValue<int>(NodeCanvas.Framework.IBlackboard,string,int)
        // System.Void SlotMaker.BlackboardUtils.SetOrCreateValue<long>(NodeCanvas.Framework.IBlackboard,string,long)
        // System.Void SlotMaker.BlackboardUtils.SetOrCreateValue<object>(NodeCanvas.Framework.IBlackboard,string,object)
        // bool SlotMaker.FormatUtility.CompareOperator<byte>(string,byte,byte)
        // bool SlotMaker.FormatUtility.CompareOperator<double>(string,double,double)
        // bool SlotMaker.FormatUtility.CompareOperator<int>(string,int,int)
        // bool SlotMaker.FormatUtility.CompareOperator<long>(string,long,long)
        // bool SlotMaker.FormatUtility.CompareOperator<object>(string,object,object)
        // object SlotMaker.Json.SlotSimpleJson.DeserializeObject<object>(string,SlotMaker.Json.IJsonSerializerStrategy)
        // object System.Activator.CreateInstance<object>()
        // object[] System.Array.Empty<object>()
        // bool System.Enum.TryParse<int>(string,bool,int&)
        // bool System.Enum.TryParse<int>(string,int&)
        // bool System.Enum.TryParse<object>(string,bool,object&)
        // bool System.Enum.TryParse<object>(string,object&)
        // bool System.Linq.Enumerable.All<int>(System.Collections.Generic.IEnumerable<int>,System.Func<int,bool>)
        // bool System.Linq.Enumerable.All<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
        // bool System.Linq.Enumerable.Any<System.Collections.Generic.KeyValuePair<object,byte>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,byte>>,System.Func<System.Collections.Generic.KeyValuePair<object,byte>,bool>)
        // bool System.Linq.Enumerable.Any<System.ValueTuple<object,object>>(System.Collections.Generic.IEnumerable<System.ValueTuple<object,object>>,System.Func<System.ValueTuple<object,object>,bool>)
        // bool System.Linq.Enumerable.Any<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Cast<object>(System.Collections.IEnumerable)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.CastIterator<object>(System.Collections.IEnumerable)
        // bool System.Linq.Enumerable.Contains<int>(System.Collections.Generic.IEnumerable<int>,int)
        // bool System.Linq.Enumerable.Contains<int>(System.Collections.Generic.IEnumerable<int>,int,System.Collections.Generic.IEqualityComparer<int>)
        // bool System.Linq.Enumerable.Contains<object>(System.Collections.Generic.IEnumerable<object>,object)
        // bool System.Linq.Enumerable.Contains<object>(System.Collections.Generic.IEnumerable<object>,object,System.Collections.Generic.IEqualityComparer<object>)
        // int System.Linq.Enumerable.Count<double>(System.Collections.Generic.IEnumerable<double>,System.Func<double,bool>)
        // int System.Linq.Enumerable.Count<int>(System.Collections.Generic.IEnumerable<int>)
        // int System.Linq.Enumerable.Count<object>(System.Collections.Generic.IEnumerable<object>)
        // int System.Linq.Enumerable.Count<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Distinct<object>(System.Collections.Generic.IEnumerable<object>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.DistinctIterator<object>(System.Collections.Generic.IEnumerable<object>,System.Collections.Generic.IEqualityComparer<object>)
        // System.Collections.Generic.KeyValuePair<object,object> System.Linq.Enumerable.ElementAt<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>,int)
        // int System.Linq.Enumerable.ElementAt<int>(System.Collections.Generic.IEnumerable<int>,int)
        // object System.Linq.Enumerable.ElementAt<object>(System.Collections.Generic.IEnumerable<object>,int)
        // System.Collections.Generic.KeyValuePair<object,long> System.Linq.Enumerable.First<System.Collections.Generic.KeyValuePair<object,long>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,long>>)
        // System.Collections.Generic.KeyValuePair<object,object> System.Linq.Enumerable.First<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>)
        // object System.Linq.Enumerable.First<object>(System.Collections.Generic.IEnumerable<object>)
        // object System.Linq.Enumerable.First<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
        // UnityEngine.CharacterInfo System.Linq.Enumerable.FirstOrDefault<UnityEngine.CharacterInfo>(System.Collections.Generic.IEnumerable<UnityEngine.CharacterInfo>,System.Func<UnityEngine.CharacterInfo,bool>)
        // object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>)
        // object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
        // System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Intersect<int>(System.Collections.Generic.IEnumerable<int>,System.Collections.Generic.IEnumerable<int>)
        // System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.IntersectIterator<int>(System.Collections.Generic.IEnumerable<int>,System.Collections.Generic.IEnumerable<int>,System.Collections.Generic.IEqualityComparer<int>)
        // int System.Linq.Enumerable.Last<int>(System.Collections.Generic.IEnumerable<int>)
        // long System.Linq.Enumerable.Last<long>(System.Collections.Generic.IEnumerable<long>)
        // object System.Linq.Enumerable.Last<object>(System.Collections.Generic.IEnumerable<object>)
        // long System.Linq.Enumerable.LastOrDefault<long>(System.Collections.Generic.IEnumerable<long>)
        // long System.Linq.Enumerable.Max<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,long>)
        // long System.Linq.Enumerable.Min<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,long>)
        // System.Linq.IOrderedEnumerable<System.Collections.Generic.KeyValuePair<int,object>> System.Linq.Enumerable.OrderBy<System.Collections.Generic.KeyValuePair<int,object>,object>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>,System.Func<System.Collections.Generic.KeyValuePair<int,object>,object>)
        // System.Linq.IOrderedEnumerable<System.Collections.Generic.KeyValuePair<object,object>> System.Linq.Enumerable.OrderBy<System.Collections.Generic.KeyValuePair<object,object>,object>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>,System.Func<System.Collections.Generic.KeyValuePair<object,object>,object>)
        // System.Linq.IOrderedEnumerable<int> System.Linq.Enumerable.OrderBy<int,int>(System.Collections.Generic.IEnumerable<int>,System.Func<int,int>)
        // System.Linq.IOrderedEnumerable<object> System.Linq.Enumerable.OrderBy<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
        // System.Linq.IOrderedEnumerable<object> System.Linq.Enumerable.OrderBy<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>,System.Collections.Generic.IComparer<object>)
        // System.Linq.IOrderedEnumerable<int> System.Linq.Enumerable.OrderByDescending<int,int>(System.Collections.Generic.IEnumerable<int>,System.Func<int,int>)
        // System.Collections.Generic.IEnumerable<UnityEngine.Vector3> System.Linq.Enumerable.Select<UnityEngine.Vector3,UnityEngine.Vector3>(System.Collections.Generic.IEnumerable<UnityEngine.Vector3>,System.Func<UnityEngine.Vector3,UnityEngine.Vector3>)
        // System.Collections.Generic.IEnumerable<UnityEngine.Vector3> System.Linq.Enumerable.Select<object,UnityEngine.Vector3>(System.Collections.Generic.IEnumerable<object>,System.Func<object,UnityEngine.Vector3>)
        // System.Collections.Generic.IEnumerable<byte> System.Linq.Enumerable.Select<object,byte>(System.Collections.Generic.IEnumerable<object>,System.Func<object,byte>)
        // System.Collections.Generic.IEnumerable<float> System.Linq.Enumerable.Select<object,float>(System.Collections.Generic.IEnumerable<object>,System.Func<object,float>)
        // System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Select<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
        // System.Collections.Generic.IEnumerable<long> System.Linq.Enumerable.Select<object,long>(System.Collections.Generic.IEnumerable<object>,System.Func<object,long>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<System.Collections.Generic.KeyValuePair<object,object>,object>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>,System.Func<System.Collections.Generic.KeyValuePair<object,object>,object>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<int,object>(System.Collections.Generic.IEnumerable<int>,System.Func<int,object>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>)
        // ushort System.Linq.Enumerable.Single<ushort>(System.Collections.Generic.IEnumerable<ushort>)
        // float System.Linq.Enumerable.Sum<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,float>)
        // int System.Linq.Enumerable.Sum<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
        // long System.Linq.Enumerable.Sum<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,long>)
        // System.Collections.Generic.IEnumerable<byte> System.Linq.Enumerable.Take<byte>(System.Collections.Generic.IEnumerable<byte>,int)
        // System.Collections.Generic.IEnumerable<byte> System.Linq.Enumerable.TakeIterator<byte>(System.Collections.Generic.IEnumerable<byte>,int)
        // byte[] System.Linq.Enumerable.ToArray<byte>(System.Collections.Generic.IEnumerable<byte>)
        // object[] System.Linq.Enumerable.ToArray<object>(System.Collections.Generic.IEnumerable<object>)
        // System.Collections.Generic.List<BagelCode.EligibleBetItem.BetTextInfo> System.Linq.Enumerable.ToList<BagelCode.EligibleBetItem.BetTextInfo>(System.Collections.Generic.IEnumerable<BagelCode.EligibleBetItem.BetTextInfo>)
        // System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<object,object>> System.Linq.Enumerable.ToList<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>)
        // System.Collections.Generic.List<UnityEngine.Vector3> System.Linq.Enumerable.ToList<UnityEngine.Vector3>(System.Collections.Generic.IEnumerable<UnityEngine.Vector3>)
        // System.Collections.Generic.List<byte> System.Linq.Enumerable.ToList<byte>(System.Collections.Generic.IEnumerable<byte>)
        // System.Collections.Generic.List<int> System.Linq.Enumerable.ToList<int>(System.Collections.Generic.IEnumerable<int>)
        // System.Collections.Generic.List<long> System.Linq.Enumerable.ToList<long>(System.Collections.Generic.IEnumerable<long>)
        // System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
        // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>> System.Linq.Enumerable.Where<System.Collections.Generic.KeyValuePair<object,object>>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>,System.Func<System.Collections.Generic.KeyValuePair<object,object>,bool>)
        // System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Where<int>(System.Collections.Generic.IEnumerable<int>,System.Func<int,bool>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Where<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
        // System.Collections.Generic.IEnumerable<UnityEngine.Vector3> System.Linq.Enumerable.Iterator<UnityEngine.Vector3>.Select<UnityEngine.Vector3>(System.Func<UnityEngine.Vector3,UnityEngine.Vector3>)
        // System.Collections.Generic.IEnumerable<UnityEngine.Vector3> System.Linq.Enumerable.Iterator<object>.Select<UnityEngine.Vector3>(System.Func<object,UnityEngine.Vector3>)
        // System.Collections.Generic.IEnumerable<byte> System.Linq.Enumerable.Iterator<object>.Select<byte>(System.Func<object,byte>)
        // System.Collections.Generic.IEnumerable<float> System.Linq.Enumerable.Iterator<object>.Select<float>(System.Func<object,float>)
        // System.Collections.Generic.IEnumerable<int> System.Linq.Enumerable.Iterator<object>.Select<int>(System.Func<object,int>)
        // System.Collections.Generic.IEnumerable<long> System.Linq.Enumerable.Iterator<object>.Select<long>(System.Func<object,long>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<System.Collections.Generic.KeyValuePair<object,object>>.Select<object>(System.Func<System.Collections.Generic.KeyValuePair<object,object>,object>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<int>.Select<object>(System.Func<int,object>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<object>.Select<object>(System.Func<object,object>)
        // int System.Nullable.Compare<int>(System.Nullable<int>,System.Nullable<int>)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,SlotMaker.Tasks.Actions.LoadSymbolGraphs.<LoadGraphAsync>d__4>(System.Runtime.CompilerServices.TaskAwaiter&,SlotMaker.Tasks.Actions.LoadSymbolGraphs.<LoadGraphAsync>d__4&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,SlotMaker.Tasks.Actions.LoadSymbolGraphs.<LoadSequenceAsync>d__3>(System.Runtime.CompilerServices.TaskAwaiter&,SlotMaker.Tasks.Actions.LoadSymbolGraphs.<LoadSequenceAsync>d__3&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,SlotMaker.Tasks.Actions.LoadSymbolGraphs.<LoadGraphAsync>d__4>(System.Runtime.CompilerServices.TaskAwaiter&,SlotMaker.Tasks.Actions.LoadSymbolGraphs.<LoadGraphAsync>d__4&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,SlotMaker.Tasks.Actions.LoadSymbolGraphs.<LoadSequenceAsync>d__3>(System.Runtime.CompilerServices.TaskAwaiter&,SlotMaker.Tasks.Actions.LoadSymbolGraphs.<LoadSequenceAsync>d__3&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<SlotMaker.Tasks.Actions.LoadSymbolGraphs.<LoadGraphAsync>d__4>(SlotMaker.Tasks.Actions.LoadSymbolGraphs.<LoadGraphAsync>d__4&)
        // System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<SlotMaker.Tasks.Actions.LoadSymbolGraphs.<LoadSequenceAsync>d__3>(SlotMaker.Tasks.Actions.LoadSymbolGraphs.<LoadSequenceAsync>d__3&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,WebSock.<_Connect>d__15>(System.Runtime.CompilerServices.TaskAwaiter&,WebSock.<_Connect>d__15&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,WebSock.<_Connect>d__15>(System.Runtime.CompilerServices.TaskAwaiter<object>&,WebSock.<_Connect>d__15&)
        // System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<WebSock.<_Connect>d__15>(WebSock.<_Connect>d__15&)
        // byte UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<byte>(System.IntPtr)
        // int UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<int>(System.IntPtr)
        // long UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<long>(System.IntPtr)
        // object UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<object>(System.IntPtr)
        // System.IntPtr UnityEngine.AndroidJNIHelper.GetFieldID<object>(System.IntPtr,string,bool)
        // System.IntPtr UnityEngine.AndroidJNIHelper.GetMethodID<byte>(System.IntPtr,string,object[],bool)
        // System.IntPtr UnityEngine.AndroidJNIHelper.GetMethodID<int>(System.IntPtr,string,object[],bool)
        // System.IntPtr UnityEngine.AndroidJNIHelper.GetMethodID<long>(System.IntPtr,string,object[],bool)
        // System.IntPtr UnityEngine.AndroidJNIHelper.GetMethodID<object>(System.IntPtr,string,object[],bool)
        // byte UnityEngine.AndroidJavaObject.Call<byte>(string,object[])
        // int UnityEngine.AndroidJavaObject.Call<int>(string,object[])
        // object UnityEngine.AndroidJavaObject.Call<object>(string,object[])
        // byte UnityEngine.AndroidJavaObject.CallStatic<byte>(string,object[])
        // long UnityEngine.AndroidJavaObject.CallStatic<long>(string,object[])
        // object UnityEngine.AndroidJavaObject.CallStatic<object>(string,object[])
        // object UnityEngine.AndroidJavaObject.GetStatic<object>(string)
        // System.Void UnityEngine.AndroidJavaObject.Set<object>(string,object)
        // byte UnityEngine.AndroidJavaObject._Call<byte>(string,object[])
        // int UnityEngine.AndroidJavaObject._Call<int>(string,object[])
        // object UnityEngine.AndroidJavaObject._Call<object>(string,object[])
        // byte UnityEngine.AndroidJavaObject._CallStatic<byte>(string,object[])
        // long UnityEngine.AndroidJavaObject._CallStatic<long>(string,object[])
        // object UnityEngine.AndroidJavaObject._CallStatic<object>(string,object[])
        // object UnityEngine.AndroidJavaObject._GetStatic<object>(string)
        // System.Void UnityEngine.AndroidJavaObject._Set<object>(string,object)
        // object UnityEngine.Component.GetComponent<object>()
        // object UnityEngine.Component.GetComponentInChildren<object>()
        // object UnityEngine.Component.GetComponentInChildren<object>(bool)
        // object UnityEngine.Component.GetComponentInParent<object>()
        // System.Void UnityEngine.Component.GetComponents<object>(System.Collections.Generic.List<object>)
        // object[] UnityEngine.Component.GetComponentsInChildren<object>()
        // object[] UnityEngine.Component.GetComponentsInChildren<object>(bool)
        // bool UnityEngine.Component.TryGetComponent<object>(object&)
        // object UnityEngine.GameObject.AddComponent<object>()
        // object UnityEngine.GameObject.GetComponent<object>()
        // object UnityEngine.GameObject.GetComponentInChildren<object>()
        // object UnityEngine.GameObject.GetComponentInChildren<object>(bool)
        // object[] UnityEngine.GameObject.GetComponentsInChildren<object>()
        // object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
        // bool UnityEngine.GameObject.TryGetComponent<object>(object&)
        // AccountLoginView.AccountLoginRespone UnityEngine.JsonUtility.FromJson<AccountLoginView.AccountLoginRespone>(string)
        // AccountLoginViewNew.AccountLoginRegistRespone UnityEngine.JsonUtility.FromJson<AccountLoginViewNew.AccountLoginRegistRespone>(string)
        // AccountLoginViewNew.AccountLoginRespone UnityEngine.JsonUtility.FromJson<AccountLoginViewNew.AccountLoginRespone>(string)
        // AccountLoginViewNew.DeviceAccountRespone UnityEngine.JsonUtility.FromJson<AccountLoginViewNew.DeviceAccountRespone>(string)
        // object UnityEngine.JsonUtility.FromJson<object>(string)
        // object UnityEngine.Object.FindObjectOfType<object>()
        // object[] UnityEngine.Object.FindObjectsOfType<object>()
        // object UnityEngine.Object.Instantiate<object>(object)
        // object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
        // object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
        // object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Vector3,UnityEngine.Quaternion)
        // object[] UnityEngine.Resources.ConvertObjects<object>(UnityEngine.Object[])
        // object UnityEngine.Resources.Load<object>(string)
        // object[] UnityEngine.Resources.LoadAll<object>(string)
        // byte UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<byte>(System.IntPtr)
        // int UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<int>(System.IntPtr)
        // long UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<long>(System.IntPtr)
        // object UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<object>(System.IntPtr)
        // System.IntPtr UnityEngine._AndroidJNIHelper.GetFieldID<object>(System.IntPtr,string,bool)
        // System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<byte>(System.IntPtr,string,object[],bool)
        // System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<int>(System.IntPtr,string,object[],bool)
        // System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<long>(System.IntPtr,string,object[],bool)
        // System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<object>(System.IntPtr,string,object[],bool)
        // string UnityEngine._AndroidJNIHelper.GetSignature<byte>(object[])
        // string UnityEngine._AndroidJNIHelper.GetSignature<int>(object[])
        // string UnityEngine._AndroidJNIHelper.GetSignature<long>(object[])
        // string UnityEngine._AndroidJNIHelper.GetSignature<object>(object[])
        // string string.Join<int>(string,System.Collections.Generic.IEnumerable<int>)
    }
}
