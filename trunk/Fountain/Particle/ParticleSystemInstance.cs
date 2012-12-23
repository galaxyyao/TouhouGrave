using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Particle
{
    public class ParticleSystemInstance
    {
        private List<EffectInstance> m_effectInstances = new List<EffectInstance>();

        public ParticleSystem System
        {
            get; private set;
        }

        public int TotalLiveParticles
        {
            get { return m_effectInstances.Sum(fx => fx.LiveParticleCount); }
        }

        public ILocalFrameProvider LocalFrameProvider
        {
            get; set;
        }

        public IIndexable<EffectInstance> EffectInstances
        {
            get; private set;
        }

        public ParticleSystemInstance(ParticleSystem system)
        {
            if (system == null)
            {
                throw new ArgumentNullException("system");
            }

            System = system;
            RefreshEffectInstances();
        }

        public void Update(float deltaTime)
        {
            RefreshEffectInstances();
            foreach (var effect in m_effectInstances)
            {
                effect.Update(deltaTime);
            }
        }

        private void RefreshEffectInstances()
        {
            if (m_effectInstances.Count != System.Effects.Count)
            {
                var newList = new List<EffectInstance>(System.Effects.Count);
                for (int i = 0; i < System.Effects.Count; ++i)
                {
                    newList.Add(i < m_effectInstances.Count ? m_effectInstances[i] : null);
                }
                m_effectInstances = newList;
                EffectInstances = m_effectInstances.ToIndexable();
            }
            for (int i = 0; i < m_effectInstances.Count; ++i)
            {
                if (m_effectInstances[i] == null || m_effectInstances[i].Effect != System.Effects[i])
                {
                    m_effectInstances[i] = new EffectInstance(System.Effects[i], this);
                }
            }
        }
    }
}
