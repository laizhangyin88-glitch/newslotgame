using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public abstract class MementoReelStrip : IMemento
    {
        protected ReelStrip strip;

        public MementoReelStrip(ReelStrip strip)
        {
            this.strip = strip;
        }

        public abstract void Do();

        public abstract void UnDo();
    }

    public class InsertReelStrip : MementoReelStrip
    {
        protected int beginIndex;
        protected List<SymbolInfo> insertList;

        public InsertReelStrip(ReelStrip strip, int beginIndex, List<SymbolInfo> insertList) : base(strip)
        {
            this.beginIndex = beginIndex;
            this.insertList = insertList;
        }

        public override void Do()
        {
            strip.strip.InsertRange(beginIndex, insertList);
        }

        public override void UnDo()
        {
            strip.strip.RemoveRange(beginIndex, insertList.Count);
        }
    }

    public class ReplaceReelStrip : MementoReelStrip
    {
        protected int beginIndex;
        protected List<SymbolInfo> restoreList;
        protected List<SymbolInfo> replaceList;

        public ReplaceReelStrip(ReelStrip strip, int beginIndex, List<SymbolInfo> replaceList) : base(strip)
        {
            this.beginIndex = beginIndex;
            this.replaceList = replaceList;
        }

        public override void Do()
        {
            restoreList = new List<SymbolInfo>();
            int stripIndex;
            for (int i = 0; i < replaceList.Count; ++i)
            {
                stripIndex = (beginIndex + i) % strip.stripCount;
                restoreList.Add(strip.strip[stripIndex]);
                strip.strip[stripIndex] = replaceList[i];
            }
        }

        public override void UnDo()
        {
            int stripIndex;
            for (int i = 0; i < restoreList.Count; ++i)
            {
                stripIndex = (beginIndex + i) % strip.stripCount;
                strip.strip[stripIndex] = restoreList[i];
            }
        }
    }

    public class RemoveReelStrip : MementoReelStrip
    {
        protected int beginIndex;
        protected int count;
        protected List<SymbolInfo> restoreList;

        public RemoveReelStrip(ReelStrip strip, int beginIndex, int count) : base(strip)
        {
            this.beginIndex = beginIndex;
            this.count = count;
        }

        public override void Do()
        {
            restoreList = new List<SymbolInfo>();
            restoreList.InsertRange(0, strip.strip.GetRange(beginIndex, count));

            strip.strip.RemoveRange(beginIndex, count);
        }

        public override void UnDo()
        {
            strip.strip.InsertRange(beginIndex, restoreList);
        }
    }

    public class ReelStrip : BaseReelStrip
    {
        public int _stripIndex;

        public override int stripIndex
        { get { return _stripIndex; } set { _stripIndex = value; } }

        public override int stripCount
        { get { return strip.Count; } }

        protected int _stripSubSymbolOffset = 0;

        public override int stripSubSymbolOffset
        { get { return _stripSubSymbolOffset; } set { _stripSubSymbolOffset = value; } }

        public List<SymbolInfo> strip;

        protected List<MementoReelStrip> history = new List<MementoReelStrip>();
        protected int historyIndex = -1;

        public override int GetRandomIndex()
        {
            return UnityEngine.Random.Range(0, strip.Count);
        }

        /// <summary>
        /// TODO：修改这里，返回具体的值，就可以显示正确的牌面值
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override int CalcIndex(int idx)
        {
            if (idx < 0)
                idx += strip.Count;
            else if (idx > (strip.Count - 1))
                idx %= strip.Count;

            Debug.LogError("最终的 index......." + idx);
            return idx;
        }

        public override SymbolInfo GetSymbol(int index)
        {
            return strip[index];
        }

        public override void InsertRange(int beginIndex, List<SymbolInfo> insertStrip)
        {
            DoAndAddMemento(new InsertReelStrip(this, beginIndex, insertStrip));
        }

        public override void ReplaceRange(int beginIndex, List<SymbolInfo> replaceList)
        {
            DoAndAddMemento(new ReplaceReelStrip(this, beginIndex, replaceList));
        }

        public override void RemoveRange(int index, int count)
        {
            DoAndAddMemento(new RemoveReelStrip(this, index, count));
        }

        public override bool UnDo()
        {
            if (historyIndex < 0)
                return false;

            history[historyIndex--].UnDo();
            return true;
        }

        public override bool ReDo()
        {
            if (historyIndex > history.Count - 1)
                return false;

            history[++historyIndex].Do();
            return true;
        }

        public override void ClearHistory()
        {
            history.Clear();
            historyIndex = -1;
        }

        protected void DoAndAddMemento(MementoReelStrip memento)
        {
            if (historyIndex < history.Count - 1)
                history.RemoveRange(historyIndex + 1, history.Count - historyIndex - 1);

            memento.Do();
            history.Add(memento);
            historyIndex = history.Count - 1;
        }

        // This is copied shallowly
        public override void CopyFrom(IReelStrip value)
        {
            ReelStrip reelStrip = (ReelStrip)value;

            this.stripIndex = reelStrip.stripIndex;
            this.stripSubSymbolOffset = reelStrip.stripSubSymbolOffset;
            this.strip = reelStrip.strip;
            this.history = reelStrip.history;
            this.historyIndex = reelStrip.historyIndex;
        }
    }
}
