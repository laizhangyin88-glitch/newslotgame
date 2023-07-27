using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class BorderLines : MonoBehaviour
    {
        [Serializable]
        public class BorderLine
        {
            public List<GameObject> lines;
            public List<GameObject> winBorders;
        }
        
    	public List<BorderLine> borderLines;

    	public void Play(int lineIndex, List<Cell> cells)
    	{
            var lineList = borderLines[lineIndex].lines;
            var winBorderList = borderLines[lineIndex].winBorders;
            for (int i = 0; i < lineList.Count; ++i)
            {
                lineList[i].gameObject.SetActive(true);
            }
            if (cells != null)
            {
                for (int i = 0; i < cells.Count; ++i)
                {
                    int colIndex = cells[i].column;
                    lineList[(colIndex * 2)].gameObject.SetActive(false);
                    winBorderList[colIndex].gameObject.SetActive(true);
                }
            }
    	}

    	public void Stop()
    	{
    		for (int i = 0; i < borderLines.Count; ++i)
    		{
                var lineList = borderLines[i].lines;
                for (int j = 0; j < lineList.Count; ++j)
                {
                    lineList[j].gameObject.SetActive(false);
                }
                var winBorderList = borderLines[i].winBorders;
                for (int j = 0; j < winBorderList.Count; ++j)
                {
                    winBorderList[j].gameObject.SetActive(false);
                }
    		}
    	}
    }
}
