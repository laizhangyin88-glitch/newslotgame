using System;

namespace SlotMaker.IoC
{
    public enum ValueType
    {
        ByAsset,
        ByValue
    }

    public enum ListType
    {
        SingleList,
        MultiList
    }

    public enum EnableAction
    {
        EnableBehaviour,
        DoNothing
    }

    public enum StartAction
    {
        StartBehaviour,
        DoNothing
    }

    public enum TransformSpace
    {
        Local,
        World
    }

    public enum LoopType
    {
        None,
        Loop,
        Circle,
        PingPong
    }

    public enum TimeControl
    {
        Manual,
        DeltaTime,
        UnscaledDeltaTime
    }

    [Serializable]
    public struct Vector2Mask
    {
        public bool x;
        public bool y;

        public bool Any()
        {
            return x || y;
        }
    }

    [Serializable]
    public struct Vector3Mask
    {
        public bool x;
        public bool y;
        public bool z;

        public bool Any()
        {
            return x || y || z;
        }
    }

    [Serializable]
    public struct Vector4Mask
    {
        public bool x;
        public bool y;
        public bool z;
        public bool w;

        public bool Any()
        {
            return x || y || z || w;
        }
    }

    [Serializable]
    public struct ColorMask
    {
        public bool r;
        public bool g;
        public bool b;
        public bool a;

        public bool Any()
        {
            return r || g || b || a;
        }
    }
}
