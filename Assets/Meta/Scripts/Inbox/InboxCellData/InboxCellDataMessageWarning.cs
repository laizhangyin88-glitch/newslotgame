using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using UnityEngine;

namespace BagelCode
{
    public class InboxCellDataMessageWarning : InboxCellData
    {
        public InboxCellDataMessageWarning(InboxCellController _owner, Blackboard _inboxInfo)
            : base(_owner, _inboxInfo)
        {
            useWarningColor = true;
        }
    }
}