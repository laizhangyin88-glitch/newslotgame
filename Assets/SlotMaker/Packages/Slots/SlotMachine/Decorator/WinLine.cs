using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public abstract class WinLine : MonoBehaviour
    {
        [PropertyOrder(-100)]
        public int lineIndex;

        [PropertyOrder(100)]
        public UnityEvent onActive;
        
        [PropertyOrder(101)]
        public UnityEvent onInActive;
        
        [PropertyOrder(102)]
        public UnityEvent onWin;
        
        public void ActiveLine()
        {
            onActive.Invoke();
        }

        public void InActiveLine()
        {
            onInActive.Invoke();
        }

        public abstract void Win(SpinLineWin win);

        public virtual void SetLineIndex(int lineIndex_) { lineIndex = lineIndex_; }
        public abstract void SetLine(List<int> line, int maxRow, float offset);
        public abstract void SetColor(Color color);
        public abstract void SetGradient(Gradient gradient);
    }
}