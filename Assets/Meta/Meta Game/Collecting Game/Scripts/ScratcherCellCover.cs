using NodeCanvas.Framework;
using UnityEngine;

namespace BagelCode.Scratcher
{
    [System.Serializable]
    public class ScratcherCellCover
    {
        public int coverID;

        [System.NonSerialized]
        public Color color;
        [System.NonSerialized]
        public Color textColor;
        [System.NonSerialized]
        public Sprite sprite;
        [System.NonSerialized]
        public int stopFrame;

        // private static ScratcherName scratcherNameForAsset;
        // private static List<List<int>> assetIndexes;

        public ScratcherCellCover(Blackboard symbolBB)//, ScratcherName scratcherName, int index)
        {
            var id = symbolBB.GetVariable<int>("coverId");
            coverID = id == null ? 0 : id.value;

            MakeScratcherCellCover();

            // if(coverID < 0) coverID = 0;
            // if(ScratcherCustomData.Instance.scratcherCellAssets.customCoverAssets.Count <= coverID)
            //     coverID = 0;

            // ScratcherCoverAsset coverAsset = ScratcherCustomData.Instance.scratcherCellAssets.customCoverAssets[coverID];
            // // ScratcherCoverAsset coverAsset = GetScratcherCoverAsset(scratcherName, index);
            // color = coverAsset.color;
            // sprite = coverAsset.sprite;
            // stopFrame = coverAsset.stopFrame;
        }

        public ScratcherCellCover(int makeCoverID)
        {
            coverID = makeCoverID;

            MakeScratcherCellCover();
        }

        private void MakeScratcherCellCover()
        {
            if (coverID < 0) coverID = 0;
            if(ScratcherCustomData.Instance.scratcherCellAssets.customCoverAssets.Count <= coverID)
                coverID = 0;

            ScratcherCoverAsset coverAsset = ScratcherCustomData.Instance.scratcherCellAssets.customCoverAssets[coverID];
            // ScratcherCoverAsset coverAsset = GetScratcherCoverAsset(scratcherName, index);
            color = coverAsset.color;
            textColor = coverAsset.textColor;
            sprite = coverAsset.sprite;
            stopFrame = coverAsset.stopFrame;
        }
        
        // public static ScratcherCoverAsset GetScratcherCoverAsset(ScratcherName scratcherName, int index)
        // {
        //     GenerateIndex(scratcherName);

        //     int coverIndex = -1;
            
        //     coverIndex = GetCoverIndex(assetIndexes, index);
            
        //     if (coverIndex != -1)
        //         return ScratcherCustomData.Instance.scratcherCellAssets.coverAssets[(int)scratcherName - 1].assets[coverIndex];

        //     return null;
        // }

        // private static void GenerateIndex(ScratcherName scratcherName)
        // {
        //     if (assetIndexes == null || scratcherNameForAsset != scratcherName)
        //     {
        //         assetIndexes = new List<List<int>>();
        //         scratcherNameForAsset = scratcherName;
                
        //         switch (scratcherName)
        //         {
        //             case ScratcherName.VEGAS_SEVENS:
        //             case ScratcherName.GRAND_PRIZE_HOTEL:
        //                 assetIndexes.Add(GenerateIndexList(0, 7));
        //                 break;
        //             case ScratcherName.CASINO_NIGHT:
        //             case ScratcherName.MASSIVE_20X:
        //             case ScratcherName.EXTREME_10X:
        //                 assetIndexes.Add(GenerateIndexList(0, 6));
        //                 assetIndexes.Add(GenerateIndexList(6, 18));
        //                 break;
        //             case ScratcherName.VEGAS_NIGHT:
        //                 assetIndexes.Add(GenerateIndexList(0, 3));
        //                 assetIndexes.Add(GenerateIndexList(3, 11));
        //                 assetIndexes.Add(GenerateIndexList(11, 15));
        //                 break;
        //             case ScratcherName.JOKERS_WILD_POKER:
        //             case ScratcherName.JOKERS_POKER_RED:
        //             case ScratcherName.JOKERS_POKER_GREEN:
        //                 assetIndexes.Add(GenerateIndexList(0, 45));
        //                 assetIndexes.Add(GenerateIndexList(45, 53));
        //                 break;
        //             case ScratcherName.JOKERS_WILD_POKER_ADVANCE:
        //             case ScratcherName.JOKERS_10X_PURPLE:
        //             case ScratcherName.JOKERS_10X_GOLD:
        //                 assetIndexes.Add(GenerateIndexList(0, 45));
        //                 assetIndexes.Add(GenerateIndexList(45, 53));
        //                 assetIndexes.Add(GenerateIndexList(53, 54));
        //                 break;
        //             case ScratcherName.RICH_PALACE:
        //                 assetIndexes.Add(GenerateIndexList(0, 12));
        //                 assetIndexes.Add(GenerateIndexList(12, 16));
        //                 break;
        //             case ScratcherName.MATCH_3_TRIPLER:
        //                 assetIndexes.Add(GenerateIndexList(0, 6));
        //                 assetIndexes.Add(GenerateIndexList(6, 7));
        //                 break;
        //             case ScratcherName.CASH_DESERT:
        //                 assetIndexes.Add(GenerateIndexList(0, 3));
        //                 assetIndexes.Add(GenerateIndexList(3, 17));
        //                 assetIndexes.Add(GenerateIndexList(17, 21));
        //                 break;
        //             case ScratcherName.WORLD_TOUR:
        //                 assetIndexes.Add(GenerateIndexList(0, 6));
        //                 assetIndexes.Add(GenerateIndexList(6, 18));
        //                 assetIndexes.Add(GenerateIndexList(18, 19));
        //                 break;
        //             default:
        //                 break;
        //         }
        //     }
        // }
        
        // private static List<int> GenerateIndexList(int start, int end)
        // {
        //     List<int> list = new List<int>();

        //     for (int i = start; i < end; i++)
        //         list.Add(i);

        //     return list;
        // }

        // private static int GetCoverIndex(List<List<int>> assetIndexes, int index)
        // {
        //     for (int i = 0; i < assetIndexes.Count; i++)
        //     {
        //         for (int j = 0; j < assetIndexes[i].Count; j++)
        //         {
        //             if (index == assetIndexes[i][j])
        //                 return i;
        //         }
        //     }

        //     return -1;
        // }
    }
}