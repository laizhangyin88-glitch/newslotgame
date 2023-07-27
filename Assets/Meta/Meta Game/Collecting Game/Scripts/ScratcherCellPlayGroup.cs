using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using static BagelCode.Scratcher.ScratcherCellPlayGroup.PlayOption;

namespace BagelCode.Scratcher
{
    public class ScratcherCellPlayGroup : ScratcherPlayGroup
    {
        public enum PlayOption
        {
            DELAY_EACH_CELL,
            WAIT_BETWEEN,
            ACTIVATE_PARTICLE,
            OPEN_CELL,
            PLAY_HIGHLIGHT,
            DISABLE_HIGHLIGHT,
        }

        public List<ScratcherCellController> cellInstanceList;

        public bool ContainsDelayEachCell => options.Contains(DELAY_EACH_CELL);

        public PlayOption[] options;

        public readonly static PlayOption[] DEFAULT_OPTION_SET = { ACTIVATE_PARTICLE, PLAY_HIGHLIGHT, OPEN_CELL, DELAY_EACH_CELL };
        public readonly static PlayOption[] NON_HIGHLIGHT_OPTION_SET = { ACTIVATE_PARTICLE, OPEN_CELL, DELAY_EACH_CELL };
        public readonly static PlayOption[] OPEN_ALL_OPTION_SET = { ACTIVATE_PARTICLE, PLAY_HIGHLIGHT, OPEN_CELL };
        public readonly static PlayOption[] ONLY_HIGHLIGHT_OPTION_SET = { PLAY_HIGHLIGHT };

        private const float OPEN_DELAY_EACH = 0.1f;

        public ScratcherCellPlayGroup()
        {
            cellInstanceList = new List<ScratcherCellController>();
            options = DEFAULT_OPTION_SET;
        }
        
        public ScratcherCellPlayGroup(PlayOption[] _options)
        {
            cellInstanceList = new List<ScratcherCellController>();
            options = _options;
        }

        public override IEnumerator Play(ScratcherPlayController controller)
        {
            float elapsed = 0f;

            for (int i = 0; i < cellInstanceList.Count; i++)
            {
                ScratcherCellController cell = cellInstanceList[i];

                bool playSound = i == 0 || options.Contains(DELAY_EACH_CELL);

                cell.StartCoroutine(cell.Open(playSound, options));

                // to prevent duplicate highlight in case of the cell's ScratcherSymbolType is BOTH
                if (cell.SymbolType == ClientModels.ScratcherSymbolType.BOTH)
                    cellInstanceList[i].ClearHighlightCells();

                if (options.Contains(DELAY_EACH_CELL))
                {
                    yield return new WaitForSeconds(OPEN_DELAY_EACH);
                    elapsed += OPEN_DELAY_EACH;
                }
            }

            SendEvent(instantOpenDelay + elapsed, controller);
        }

        public void AddCellInstance(ScratcherCellController cell)
        {
            cellInstanceList.Add(cell);
        }

        public void AddCellInstance(List<ScratcherCellController> cells)
        {
            cellInstanceList.AddRange(cells);
        }

        public void ClearCellInstance()
        {
            cellInstanceList.Clear();
        }

        public bool IsCellEmpty()
        {
            return cellInstanceList.Count == 0;
        }

        public bool HasHighlightCells()
        {
            bool exist = false;
            for (int i = 0; i < cellInstanceList.Count; i++)
            {
                if (cellInstanceList[i].HasHighlightCells())
                {
                    exist = true;
                    break;
                }
            }

            return exist;
        }
    }
}