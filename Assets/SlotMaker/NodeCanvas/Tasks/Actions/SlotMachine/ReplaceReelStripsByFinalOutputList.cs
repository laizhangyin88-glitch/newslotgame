using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker
{
    [Category("★ SlotMaker/SlotMachine")]
    public class ReplaceReelStripsByFinalOutputList : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<List<int>> reelOutputList;
        public BBParameter<List<Blackboard>> finalOutputList;   // This value is deserialized from 'number[][]' type value of server response.
        public BBParameter<int> column;
        public BBParameter<int> row;

        protected override string info
        {
            get { return string.Format("Replace ReelStrip With Final Output List from server"); }
        }

        protected override void OnExecute()
        {
            var strips = GlobalReelStrips.Instance.GetReelStrips();
            var symbolMask = ContentCustomData.GetSlotData(slotIndex.value).symbolMask;

            for (int colIndex = 0; colIndex < column.value; colIndex++)
            {
                BaseReelStrip reelStrip = strips.GetReelStrip(colIndex);
                var newList = new List<SymbolInfo>();
                for (int rowIndex = 0; rowIndex < row.value; rowIndex++)
                {
                    var symbolIndex = finalOutputList.value[rowIndex].GetValue<List<int>>("value")[colIndex];
                    var newSymbolInfo = new SymbolInfo();
                    newSymbolInfo.symbol = symbolIndex;
                    newSymbolInfo.mask = symbolMask.GetMask(symbolIndex);
                    newList.Add(newSymbolInfo);
                }

                reelStrip.ReplaceRange(reelOutputList.value[colIndex] % reelStrip.stripCount, newList);
            }

            EndAction();
        }
    }
}