using SlotMaker;

namespace BagelCode
{
    public class EligibleBetClubLeaguePoint : EligibleBetItem
    {
        public override bool Init()
        {
            base.Init();

            SetDefaultStringTableKey(
                    "CLUB_LEAGUE_POINT_ELIGIBLE_BET_TITLE",
                    "CLUB_LEAGUE_POINT_ELIGIBLE_BET_LESS_TEXT",
                    "CLUB_LEAGUE_POINT_ELIGIBLE_BET_TEXT");

            return true;
        }

        protected override bool IsAvailableInternal()
        {
            // always available
            return ClubUtils.IsClubber();
        }

        public override long GetThreshold()
        {
            return BlackboardUtils.FindValue<long>("./lpEligibleBet");
        }
    }
}
