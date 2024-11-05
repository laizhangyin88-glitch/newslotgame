using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ParticleSystemRenderer))]
    public class SplineFollowerParticles : MonoBehaviour
    {
        public BezierSpline spline;
        private ParticleSystem _particleSystem;
        private ParticleSystem.Particle[] _particleArray;
        private int particleCount = 0;
        
        new public ParticleSystem particleSystem 
        {
            get {
                if (_particleSystem == null)
                    _particleSystem = GetComponent<ParticleSystem>();
                
                return _particleSystem;
            }
        }

        public ParticleSystem.Particle[] particleArray
        {
            get {
                if (_particleArray == null)
                    _particleArray = new ParticleSystem.Particle[particleSystem.main.maxParticles];
                
                return _particleArray;
            }
        }

        private void Update()
        {
            UpdateParticle();
        }

        private void UpdateParticle()
        {
            particleCount = particleSystem.GetParticles(particleArray);

            for (int i = 0; i < particleCount; ++i) {
                var particle = particleArray[i];

                var time2Live = particle.startLifetime - particle.remainingLifetime;
                var velocity = particle.velocity.magnitude;
                var progress = time2Live * velocity;

                if (spline.Loop)
                {
                    progress = progress - (int)progress;

                    if (progress < 0f)
                        progress += 1f;
                    else if (progress > 1f)
                        progress -= 1f;
                }
                else
                {
                    progress = Mathf.Clamp01(progress);
                }
                
                var direction = spline.GetDirection(progress);

                Vector2 offset = Vector2.zero;
                
                Vector3 point = spline.GetPoint(progress);
                point.x += (direction.x * offset.x);
                point.y += (direction.y * offset.y);
                particleArray[i].position = point;
            }
            
            particleSystem.SetParticles(particleArray, particleCount);
        }

    }
    
}