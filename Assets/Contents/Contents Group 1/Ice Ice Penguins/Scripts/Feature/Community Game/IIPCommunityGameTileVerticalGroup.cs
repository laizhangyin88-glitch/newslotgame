using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameStudio.Slot.IIP.Feature
{
    public class IIPCommunityGameTileVerticalGroup : MonoBehaviour
    {
        public List<IIPCommunityGameTile> allChildTiles = new List<IIPCommunityGameTile>();
        public List<IIPCommunityGameTile> availableChildTiles = new List<IIPCommunityGameTile>();
        public List<int> tileMask = new List<int>();

    }
}