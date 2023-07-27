
namespace BagelCode
{
    public abstract class GestureHandler
    {
        public bool Enabled { get; set; }
    }

    public abstract class GestureMouseHandler : GestureHandler
    {
        public abstract GestureManager.GestureType CheckGesture(MouseMotion motion, out GestureData gestureData);
    }

    public abstract class GestureTouchHandler : GestureHandler
    {
        public abstract GestureManager.GestureType CheckGesture(TouchMotion[] motions, out GestureData gestureData);
    }
}
