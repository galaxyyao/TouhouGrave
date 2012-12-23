using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Particle.Modifiers
{
    public class Emit : Modifier
    {
        private int m_amount = 0;

        public string EffectName { get; set; }

        public int Amount
        {
            get { return m_amount; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Amount should be non-negative.");
                }

                m_amount = value;
            }
        }

        protected override void Process(float deltaTime, EffectInstance instance, Particle[] particles, int begin, int end)
        {
            var effectInstance = instance.SystemInstance.EffectInstances.FirstOrDefault(fx => fx.Effect.Name == EffectName);
            if (effectInstance == null || effectInstance == instance)
            {
                return;
            }

            for (int i = 0; i < Amount * Math.Max(end - begin, 0); ++i)
            {
                effectInstance.Emit(effectInstance.Template);
            }
            effectInstance.EndEmit();
        }
    }
}
