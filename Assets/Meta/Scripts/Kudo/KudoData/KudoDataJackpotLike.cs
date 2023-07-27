

namespace BagelCode
{
    public class KudoDataJackpotLike : KudoDataLike
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Jackpot Like Scene";
        }

        protected override void InitProperty()
        {
            base.InitProperty();

            likeTextKey = "FEED_RECEIVE_TEXT_{0}";
        }
    }
}