using UnityEngine;

namespace SlotMaker
{
	public class WorldScaler : MonoBehaviour
    {
		public float zScale;

        private void Start()
        {
            ApplyScale();
            Destroy(this);
        }

        [ContextMenu("ApplyScale")]
        private void ApplyScale()
        {
            transform.localScale = new Vector3(
				transform.localScale.x,
				transform.localScale.y,
				zScale / transform.root.localScale.z
            );
        }
    }
}
