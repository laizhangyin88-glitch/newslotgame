using UnityEngine;

namespace BagelCode
{
    public abstract class GestureData { }

    public class GestureScrollData : GestureData
    {
        public Vector2 movement;
    }

    public class GestureSwipeData : GestureData
    {
        public float swipeScale;
    }

    public class GestureZoomInOutData : GestureData
    {
        public Vector2 zoomPoint;
        public float zoomScale;
    }

    public class GestureCorrectClickData : GestureData
    {
        public Vector2 clickPos;
        public bool isPointerOverGameObject;
        public GameObject clickedGameObject;
    }
}
