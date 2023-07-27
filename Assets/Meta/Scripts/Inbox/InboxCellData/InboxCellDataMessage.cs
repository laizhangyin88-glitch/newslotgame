using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using UnityEngine;

namespace BagelCode
{
    public class InboxCellDataMessage : InboxCellData
    {
        public InboxCellDataMessage(InboxCellController _owner, Blackboard _inboxInfo)
            : base(_owner, _inboxInfo) { }
    }
}