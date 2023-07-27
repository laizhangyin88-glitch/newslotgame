

namespace BagelCode
{
    public class KudoController : EventMonoBehaviour
    {
        private void OnDestroy()
        {
            KudoEventManager.OnDestroyKudoPrefab(this);
        }
    }
}