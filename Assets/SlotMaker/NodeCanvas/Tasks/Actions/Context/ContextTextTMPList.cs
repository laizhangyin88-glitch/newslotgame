
using System.Collections.Generic;
using TMPro;

namespace SlotMaker
{
    public class ContextTextTMPList : ContextCompositor, IContextText
    {
        public List<TMP_Text> textTMPList = new List<TMP_Text>();

        public void SetText(string text)
        {
            for (int i = 0; i < textTMPList.Count; ++i)
            {
                textTMPList[i].text = text;
            }
        }

        public string GetText()
        {
            return textTMPList[0].text;
        }
    }

}
