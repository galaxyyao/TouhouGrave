using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Particle
{
    public class ParticleSorter
    {
        private int[] m_sortedIndices;
        private float[] m_depth;

        public int[] SortedIndices
        {
            get { return m_sortedIndices; }
        }

        public ParticleSorter()
        {
            m_sortedIndices = new int[1];
            m_depth = new float[1];
        }

        public void Sort(ParticleSystemInstance instance, Vector3 camDir)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("system");
            }

            Debug.Assert(m_sortedIndices.Length == m_depth.Length);

            int totalParticles = instance.TotalLiveParticles;

            int destLength = m_sortedIndices.Length;
            while (destLength < totalParticles)
            {
                destLength *= 2;
            }

            if (destLength > m_sortedIndices.Length)
            {
                m_sortedIndices = new int[destLength];
                m_depth = new float[destLength];
            }

            // fill the indices
            for (int i = 0; i < totalParticles; ++i)
            {
                m_sortedIndices[i] = i;
            }

            // sort the indices by depth
            if (instance.System.BlendMode == BlendMode.Alpha)
            {
                int j = 0;
                foreach (var effect in instance.EffectInstances)
                {
                    effect.BatchProcess((particles, begin, end) =>
                    {
                        for (int i = begin; i < end; ++i)
                        {
                            var p = particles[i];
                            m_depth[j++] = Vector3.Dot(p.m_position, camDir);
                        }
                    });
                }

                Array.Sort(m_depth, m_sortedIndices, 0, totalParticles);
            }
        }
    }
}
