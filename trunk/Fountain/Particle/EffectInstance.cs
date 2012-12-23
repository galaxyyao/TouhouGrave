using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Particle
{
    public class EffectInstance
    {
        private Particle[] m_particles;
        private LocalFrame[] m_particleLocalFrames;
        private int m_liveParticleStart;
        private int m_liveParticleEnd;

        private Particle m_templateParticle;
        private int m_newParticleEnd;

        private Alignment m_oldAlignment;
        private float m_oldEmissionRate;
        private float m_emissionReminder;

        public Effect Effect
        {
            get; private set;
        }

        public ParticleSystem System
        {
            get { return Effect.System; }
        }

        public ParticleSystemInstance SystemInstance
        {
            get; private set;
        }

        public bool IsEmitting
        {
            get; set;
        }

        public int LiveParticleCount
        {
            get
            {
                int d = m_liveParticleEnd - m_liveParticleStart;
                return d >= 0 ? d : Effect.Capacity + d;
            }
        }

#if WINDOWS
        [System.ComponentModel.Browsable(false)]
#endif
        public Particle Template
        {
            get { return m_templateParticle; }
        }

        internal EffectInstance(Effect effect, ParticleSystemInstance instance)
        {
            if (effect == null)
            {
                throw new ArgumentNullException("effect");
            }
            else if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else if (instance.System != effect.System)
            {
                throw new ArgumentException("Effect and Instance is not from the same ParticleSystem.");
            }

            Effect = effect;
            SystemInstance = instance;
            IsEmitting = effect.EmitOnStart;
            m_oldAlignment = effect.Alignment;
        }

        public void BatchProcess(Action<Particle[], int, int> processor)
        {
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }

            if (m_liveParticleEnd >= m_liveParticleStart)
            {
                processor(m_particles, m_liveParticleStart, m_liveParticleEnd);
            }
            else
            {
                processor(m_particles, m_liveParticleStart, m_particles.Length);
                processor(m_particles, 0, m_liveParticleEnd);
            }
        }

        public void BatchProcess(Action<Particle[], LocalFrame[], int, int> processor)
        {
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }

            if (m_liveParticleEnd >= m_liveParticleStart)
            {
                processor(m_particles, m_particleLocalFrames, m_liveParticleStart, m_liveParticleEnd);
            }
            else
            {
                processor(m_particles, m_particleLocalFrames, m_liveParticleStart, m_particles.Length);
                processor(m_particles, m_particleLocalFrames, 0, m_liveParticleEnd);
            }
        }

        public void Emit(Particle particle)
        {
            Debug.Assert(IncrementIndex(m_newParticleEnd) != m_liveParticleStart, "Particle array overflow.");
            m_particles[m_newParticleEnd] = particle;
            if (Effect.Alignment == Alignment.Local)
            {
                if (SystemInstance.LocalFrameProvider != null)
                {
                    m_particleLocalFrames[m_newParticleEnd] = SystemInstance.LocalFrameProvider.LocalFrame;
                }
                else
                {
                    m_particleLocalFrames[m_newParticleEnd].Col0 = Vector4.UnitX;
                    m_particleLocalFrames[m_newParticleEnd].Col1 = Vector4.UnitY;
                    m_particleLocalFrames[m_newParticleEnd].Col2 = Vector4.UnitZ;
                }
            }
            m_newParticleEnd = IncrementIndex(m_newParticleEnd);
        }

        public void EndEmit()
        {
            if (m_newParticleEnd >= m_liveParticleEnd)
            {
                Effect.ModifiersOnEmit.ForEach(mod => mod.InternalProcess(0, this, m_particles, m_liveParticleEnd, m_newParticleEnd));
                Effect.ModifiersOnUpdate.ForEach(mod => mod.InternalProcess(0, this, m_particles, m_liveParticleEnd, m_newParticleEnd));
            }
            else
            {
                Effect.ModifiersOnEmit.ForEach(mod => mod.InternalProcess(0, this, m_particles, m_liveParticleEnd, m_particles.Length));
                Effect.ModifiersOnEmit.ForEach(mod => mod.InternalProcess(0, this, m_particles, 0, m_newParticleEnd));
                Effect.ModifiersOnUpdate.ForEach(mod => mod.InternalProcess(0, this, m_particles, m_liveParticleEnd, m_particles.Length));
                Effect.ModifiersOnUpdate.ForEach(mod => mod.InternalProcess(0, this, m_particles, 0, m_newParticleEnd));
            }

            // trim: find the first born-dead particle
            int trimWrite = m_newParticleEnd;
            for (int i = m_liveParticleEnd; i != m_newParticleEnd; i = IncrementIndex(i))
            {
                if (m_particles[i].m_life <= 0)
                {
                    trimWrite = i;
                    break;
                }
            }

            // trim
            for (int i = trimWrite; i != m_newParticleEnd; i = IncrementIndex(i))
            {
                if (m_particles[i].m_life <= 0)
                {
                    continue;
                }

                m_particles[trimWrite] = m_particles[i];
                if (m_particleLocalFrames != null)
                {
                    m_particleLocalFrames[trimWrite] = m_particleLocalFrames[i];
                }
                trimWrite = IncrementIndex(trimWrite);
            }

            m_liveParticleEnd = trimWrite;
            m_newParticleEnd = m_liveParticleEnd;
        }

        internal void Update(float deltaTime)
        {
            int oldCapacity = m_particles != null ? m_particles.Length : 0;
            if (oldCapacity != Effect.Capacity
                || m_oldAlignment != Effect.Alignment)
            {
                m_particles = Effect.Capacity != 0 ? new Particle[Effect.Capacity] : null;
                m_particleLocalFrames = Effect.Capacity != 0 && Effect.Alignment == Alignment.Local ? new LocalFrame[Effect.Capacity] : null;
                m_liveParticleStart = 0;
                m_liveParticleEnd = 0;
                m_newParticleEnd = 0;
                m_oldAlignment = Effect.Alignment;
            }

            if (m_oldEmissionRate != Effect.EmissionRate)
            {
                m_emissionReminder = 0;
                m_oldEmissionRate = Effect.EmissionRate;
            }

            Debug.Assert(m_newParticleEnd == m_liveParticleEnd, "New particles have been emitted without a call to EndEmit().");

            // update current particles

            int writePos = m_liveParticleEnd;

            for (int i = m_liveParticleStart; i != m_liveParticleEnd; i = IncrementIndex(i))
            {
                Particle p = m_particles[i];
                p.m_age += deltaTime;

                if (p.m_age < p.m_life)
                {
                    p.m_position += p.m_velocity * deltaTime;

                    m_particles[writePos] = p;
                    if (m_particleLocalFrames != null)
                        m_particleLocalFrames[writePos] = m_particleLocalFrames[i];
                    writePos = IncrementIndex(writePos);
                }
            }

            m_liveParticleStart = m_liveParticleEnd;
            m_newParticleEnd = m_liveParticleEnd = writePos;

            BatchProcess((particles, begin, end) =>
            {
                foreach (var mod in Effect.ModifiersOnUpdate)
                {
                    mod.InternalProcess(deltaTime, this, particles, begin, end);
                }
            });

            m_templateParticle = new Particle
            {
                m_life = Effect.DefaultParticleLifetime,
                m_age = 0f,
                m_position = Effect.DefaultParticlePosition,
                m_velocity = Effect.DefaultParticleVelocity,
                m_size = Effect.DefaultParticleSize,
                m_rotation = Effect.DefaultParticleRotation,
                m_color = Color.FromXnaColor(Effect.DefaultParticleColor)
            };

            if (IsEmitting)
            {
                // emit new particles
                m_emissionReminder += deltaTime;
                float interval = 1.0f / Effect.EmissionRate;
                while (m_emissionReminder > interval)
                {
                    Emit(Template);
                    m_emissionReminder -= interval;
                }
                EndEmit();
            }
        }

        private int IncrementIndex(int index)
        {
            ++index;
            Debug.Assert(index >= 0 && index < Effect.Capacity * 2);
            return index >= Effect.Capacity ? index - Effect.Capacity : index;
        }
    }
}
