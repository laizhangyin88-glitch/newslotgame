using System.Collections.Generic;

namespace SlotMaker
{
    public struct ExpandableWheelData
    {
        public List<int> SegmentValuesList;
        public List<int> JackpotIndexesList;
        public int StopSegmentIndex;
        public int JackpotOffsetPosition;
    }
}