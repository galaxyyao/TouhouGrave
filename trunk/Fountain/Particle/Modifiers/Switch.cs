using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Particle.Modifiers
{
    public class Switch : Modifier
    {
        public string EffectName { get; set; }
        public float Duration { get; set; }

        protected override void Process(float deltaTime, EffectInstance instance, Particle[] particles, int begin, int end)
        {
            var effectInstance = instance.SystemInstance.EffectInstances.FirstOrDefault(fx => fx.Effect.Name == EffectName);
            if (effectInstance == null || effectInstance == instance)
            {
                return;
            }

            if (end > begin)
            {
                effectInstance.IsEmitting = particles[begin].m_age <= Duration;
            }
        }
    }
}
