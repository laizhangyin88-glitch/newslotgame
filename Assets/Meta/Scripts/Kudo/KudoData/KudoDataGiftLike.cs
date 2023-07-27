

namespace BagelCode
{
    public class KudoDataGiftLike : KudoDataLike
    {
        protected override string GetKudoSceneName()
        {
            return "Kudo Gift Like Scene";
        }

        protected override void InitProperty()
        {
            base.InitProperty();

            likeTextKey = "FEED_RECEIVE_GIFT_LIKE_TEXT_{0}";
        }
    }
}