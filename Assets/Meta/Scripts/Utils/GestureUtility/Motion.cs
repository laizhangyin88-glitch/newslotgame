using UnityEngine;

namespace BagelCode
{
    public enum CursorState
    {
        NONE = 0,
        TAB,
        HOLD,
        UN_TAB,

        // Mouse Only
        WHEEL_UP,
        WHEEL_DOWN,
    }

    public class Motion
    {
        public CursorState State => state;
        protected CursorState state;

        public Vector2 BeganPos => beganPos;
        protected Vector2 beganPos;
        public Vector2 PrevPos => prevPos;
        protected Vector2 prevPos;
        public Vector2 DeltaPos => deltaPos;
        protected Vector2 deltaPos;
        public Vector2 CurrentPos => currentPos;
        protected Vector2 currentPos;

        //

        public void InitState()
        {
            state = CursorState.NONE;
        }

        public void Tab(Vector2 pos)
        {
            state = CursorState.TAB;

            deltaPos = Vector2.zero;
            beganPos = pos;
            prevPos = pos;
            currentPos = pos;
        }

        public void Update(Vector2 pos)
        {
            state = CursorState.HOLD;

            prevPos = currentPos;
            currentPos = pos;
            deltaPos = currentPos - prevPos;
        }

        public void UnTab()
        {
            state = CursorState.UN_TAB;
        }
    }
}
