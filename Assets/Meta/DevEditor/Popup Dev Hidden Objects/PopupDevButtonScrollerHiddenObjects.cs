using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using System.Collections.Generic;

namespace BagelCode.HiddenObjects
{
    public class PopupDevButtonScrollerHiddenObjects : PopupDevButtonScroller
    {
        protected override int CellCount()
        {
            return symbolList.Count * 5;
        }

        protected override string TitleText() => "Select Stage";

        protected override bool IsValidIndex(int i) =>
            HiddenObjects.Utils.IsValidChapterIndex(Chapter(i) + 1, out _);

        protected override void OnClick(int i)
        {
            var symbol = symbolList[Chapter(i)];
            int chapter = HiddenObjects.Utils.ChapterSymbolToNumber(symbol);

            int stage = Stage(i);

            var mainSceneObj = HiddenObjects.Utils.MainScene;
            var mainBB = mainSceneObj.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(mainBB, "targetChapterIndex", chapter);

            var eventData = new EventData<int>(HiddenObjects.Events.ON_CLICK_STAGE, stage);
            EventSender.SendEvent(mainSceneObj, eventData);

            Close();
        }

        protected override string CellText(int i)
        {
            var symbol = symbolList[Chapter(i)];
            return symbol + " " + (Stage(i) + 1);
        }

        private int Chapter(int i) => i / 5;
        private int Stage(int i) => i % 5;

        private List<string> symbolList = null;

        protected override void Start()
        {
            symbolList = HiddenObjects.Utils.HiddenObjectsInfo.GetValue<List<string>>("hiddenUniverseSymbolList");
            if (symbolList == null)
            {
                Destroy(gameObject);
                return;
            }

            base.Start();
        }

        private void Close()
        {
            MetaPopupUtils.ClosePopup(gameObject);
        }
    }
}
