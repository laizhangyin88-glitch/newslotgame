using UnityEngine;
using SlotMaker;
using System.Collections.Generic;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsGurusRankingRewardController : EventMonoBehaviour
    {
        private ContextElement root;
        private Animator anim;

        private bool isInit = false;

        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            // Play Anim
            anim.SetBool("Active", true);
            anim.SetBool("IsLoading", false);

            // Init Elements
            InitContents();

            isInit = true;
        }

        private void InitContents()
        {
            var rank = VegasDreams.Utils.GurusRank;
            var percentile = VegasDreams.Utils.GurusPercentile;

            var rankRewardList = VegasDreams.Utils.GurusFinalRewardRankList;
            var percentileRewardList = VegasDreams.Utils.GurusFinalRewardPercentileList;

            // Profile
            MetaContextElementUtils.SimpleSetIntProperty(root, "Title Area/Current Club Ranking/Profile Picture Normal", MetaSystem.GetTierGroup(BlackboardQueryUtils.GetMyProfile().tier), FULL);
            MetaContextElementUtils.SimpleSetWebImage(root, "Title Area/Current Club Ranking/Profile Picture Normal/Image", BlackboardQueryUtils.GetMyProfile().profileUrl, null, null, FULL);
            MetaContextElementUtils.SimpleSetText(root, "Title Area/Current Club Ranking/Text User Name", BlackboardQueryUtils.GetMyProfile().name, FULL);

            // Current Rank
            if (rank == 0)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Current Club Ranking/Title Text", "VEGAS_DREAMS_GURUS_RANKING_REWARD_CURRENT_RANK_DISABLE", FULL);
            }
            else
            {
                MetaContextElementUtils.SimpleSetTextGlobal(root, "Title Area/Current Club Ranking/Title Text", "VEGAS_DREAMS_GURUS_RANKING_REWARD_CURRENT_RANK", FULL, rank, percentile);
            }


            // Rank Prize
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Reward 1st/Text Reward", "TEXT_GEM_BONUS", FULL, rankRewardList[0].GetValue<long>("gem"));
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Reward 2nd/Text Reward", "TEXT_GEM_BONUS", FULL, rankRewardList[1].GetValue<long>("gem"));
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Reward 3rd/Text Reward", "TEXT_GEM_BONUS", FULL, rankRewardList[2].GetValue<long>("gem"));
            
            for (int i = 1; i <= 4; i++)
            {
                MetaContextElementUtils.SimpleSetText(root, $"Reward Group/Rank Cell {i}/Text Rank",
                    $"{rankRewardList[i+1].GetValue<int>("rank") + 1}th-{rankRewardList[i+2].GetValue<int>("rank")}th", FULL);
                    
                MetaContextElementUtils.SimpleSetTextGlobal(root, $"Reward Group/Rank Cell {i}/Text Reward",
                    "TEXT_GEM_BONUS", FULL, rankRewardList[i+2].GetValue<long>("gem"));
                    //Reward Group/Rank Cell 1/Text Reward
            }

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Reward Group/Text Top Percentile Text 1",
                "VEGAS_DREAMS_GURUS_RANKING_REWARD_PERCENT_1", FULL, 
                1, percentileRewardList[0].GetValue<int>("percentile"),
                percentileRewardList[0].GetValue<int>("percentile") + 1, percentileRewardList[1].GetValue<int>("percentile"),
                percentileRewardList[1].GetValue<int>("percentile") + 1, percentileRewardList[2].GetValue<int>("percentile"),
                percentileRewardList[2].GetValue<int>("percentile") + 1, percentileRewardList[3].GetValue<int>("percentile"),
                percentileRewardList[3].GetValue<int>("percentile") + 1, percentileRewardList[4].GetValue<int>("percentile")
            );
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Reward Group/Text Top Percentile Text 2",
                "VEGAS_DREAMS_GURUS_RANKING_REWARD_PERCENT_2", FULL, 
                percentileRewardList[4].GetValue<int>("percentile") + 1, percentileRewardList[5].GetValue<int>("percentile"),
                percentileRewardList[5].GetValue<int>("percentile") + 1, percentileRewardList[6].GetValue<int>("percentile"),
                percentileRewardList[6].GetValue<int>("percentile") + 1, percentileRewardList[7].GetValue<int>("percentile"),
                percentileRewardList[7].GetValue<int>("percentile") + 1, percentileRewardList[8].GetValue<int>("percentile"),
                percentileRewardList[8].GetValue<int>("percentile") + 1, percentileRewardList[9].GetValue<int>("percentile")
            );

            MetaContextElementUtils.SimpleSetTextGlobal(root, "Reward Group/Text Top Percentile Gem 1",
                "VEGAS_DREAMS_GURUS_RANKING_REWARD_GEM", FULL, 
                percentileRewardList[0].GetValue<long>("gem"),
                percentileRewardList[1].GetValue<long>("gem"),
                percentileRewardList[2].GetValue<long>("gem"),
                percentileRewardList[3].GetValue<long>("gem"),
                percentileRewardList[4].GetValue<long>("gem")
            );
            MetaContextElementUtils.SimpleSetTextGlobal(root, "Reward Group/Text Top Percentile Gem 2",
                "VEGAS_DREAMS_GURUS_RANKING_REWARD_GEM", FULL, 
                percentileRewardList[5].GetValue<long>("gem"),
                percentileRewardList[6].GetValue<long>("gem"),
                percentileRewardList[7].GetValue<long>("gem"),
                percentileRewardList[8].GetValue<long>("gem"),
                percentileRewardList[9].GetValue<long>("gem")
            );
            
            // Highlight Expected Reward
            var prevRank = 0;
            bool highlight = false;

            for (int i = 0; i < rankRewardList.Count; i++)
            {
                var requireRank = rankRewardList[i].GetValue<int>("rank");
                if (prevRank < rank && requireRank >= rank)
                {
                    MetaContextElementUtils.SimpleSetActive(root, $"My Rank Position Light/Rank Cell {i}", true, FULL);
                    highlight = true;
                    break;
                }
                prevRank = requireRank;
            }

            if (!highlight)
            {
                var prevPercentile = 0;
                for (int i = 0; i < percentileRewardList.Count; i++)
                {
                    var requirePercentile = percentileRewardList[i].GetValue<int>("percentile");
                    if (prevPercentile < percentile && requirePercentile >= percentile)
                    {
                        MetaContextElementUtils.SimpleSetActive(root, $"My Rank Position Light/Per Cell {i+1:00}", true, FULL);
                        break;
                    }
                    prevPercentile = requirePercentile;
                }
            }

            // Close
            var closeButtonElement = ContextUtils.FindElement(root, "Button Close", CHILDREN);
            MetaContextElementUtils.SetClickable(closeButtonElement,
                () => EventSender.SendEvent(gameObject, VegasDreams.Events.ON_CLOSE));
        }
    }
}
