using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Particle.Modifiers
{
    public class EmitWhen : Modifier
    {
        public string EffectName { get; set; }

        protected override void Process(float deltaTime, EffectInstance instance, Particle[] particles, int begin, int end)
        {
            var effectInstance = instance.SystemInstance.EffectInstances.FirstOrDefault(fx => fx.Effect.Name == EffectName);
            if (effectInstance == null || effectInstance == instance)
            {
                return;
            }

            for (int i = begin; i < end; ++i)
            {
                if (particles[i].m_position.Z < 0)
                {
                    particles[i].m_life = 0; // kill the particle

                    var p = effectInstance.Template;
                    p.m_position = particles[i].m_position;
                    p.m_position.Z = 0;
                    effectInstance.Emit(p);
                }
            }

            effectInstance.EndEmit();
        }
    }
}
