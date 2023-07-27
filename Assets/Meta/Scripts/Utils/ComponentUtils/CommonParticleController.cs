using UnityEngine;

namespace BagelCode
{
    public class CommonParticleController : MonoBehaviour
    {
        // Settings
        public bool destroyOnFinish;
        //

        private new ParticleSystem particleSystem;
        private float remaining;

        private void Start()
        {
            particleSystem = gameObject.GetComponent<ParticleSystem>();

            remaining = particleSystem.main.duration + particleSystem.main.startLifetime.constant;
        }

        private void Update()
        {
            remaining -= Time.deltaTime;

            if (destroyOnFinish && remaining < 0f)
                Destroy(gameObject);
        }
    }
}
