using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SlotMaker
{
    [Serializable]
    public class UnityObjectEvent : UnityEvent<object> {}
    [Serializable]
    public class UnityBoolEvent : UnityEvent<bool> {}
    [Serializable]
    public class UnityIntEvent : UnityEvent<int> {}
    [Serializable]
    public class UnityLongEvent : UnityEvent<long> {}
    [Serializable]
    public class UnityFloatEvent : UnityEvent<float> {}
    [Serializable]
    public class UnityDoubleEvent : UnityEvent<double> {}
    [Serializable]
    public class UnityStringEvent : UnityEvent<string> {}

    [Serializable]
    public class UnityRectEvent : UnityEvent<Rect> {}

    [Serializable]
    public class UnityVector2Event : UnityEvent<Vector2> {}

    [Serializable]
    public class UnityVector3Event : UnityEvent<Vector3> {}

    [Serializable]
    public class UnityVector4Event : UnityEvent<Vector4> {}
}
