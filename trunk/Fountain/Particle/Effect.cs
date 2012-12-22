using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaRect = Microsoft.Xna.Framework.Rectangle;

namespace TouhouSpring.Particle
{
    public class Effect
    {
        private ParticleSystem m_system;
        private Alignment m_alignment;
        private Particle[] m_particles;
        private LocalFrame[] m_particleLocalFrames;
        private int m_liveParticleStart;
        private int m_liveParticleEnd;

        private Particle m_templateParticle;
        private int m_newParticleEnd;

        private float m_emissionRate;
        private float m_emissionReminder;

        private string m_uvBoundsName;

        private ModifierList m_modifiersOnEmit;
        private ModifierList m_modifiersOnUpdate;

#if WINDOWS
        [System.ComponentModel.Browsable(false)]
#endif
        public ParticleSystem System
        {
            get { return m_system; }
            internal set
            {
                if (value != m_system)
                {
                    m_system = value;
                    ResolveUVBounds();
                }
            }
        }

        public string Name
        {
            get; set;
        }

        public float DefaultParticleLifetime
        {
            get; set;
        }

        public Vector3 DefaultParticlePosition
        {
            get; set;
        }

        public Vector3 DefaultParticleVelocity
        {
            get; set;
        }

        public Vector2 DefaultParticleSize
        {
            get; set;
        }

        public float DefaultParticleRotation
        {
            get; set;
        }

        public XnaColor DefaultParticleColor
        {
            get; set;
        }

        public int Capacity
        {
            get { return m_particles != null ? m_particles.Length : 0; }
            set
            {
                if (value != Capacity)
                {
                    if (value <= 0)
                    {
                        throw new ArgumentOutOfRangeException("Capacity must be greater than zero.");
                    }

                    m_particles = new Particle[value];
                    m_particleLocalFrames = Alignment == Alignment.Local ? new LocalFrame[value] : null;
                    m_liveParticleStart = 0;
                    m_liveParticleEnd = 0;
                    m_newParticleEnd = 0;
                }
            }
        }

        public int LiveParticleCount
        {
            get
            {
                int d = m_liveParticleEnd - m_liveParticleStart;
                return d >= 0 ? d : Capacity + d;
            }
        }

        public bool IsEmitting
        {
            get; set;
        }

        public float EmissionRate
        {
            get { return m_emissionRate; }
            set
            {
                if (value < 0.0f)
                {
                    throw new ArgumentOutOfRangeException("EmissionRate shall be non-negative.");
                }

                m_emissionRate = value;
                m_emissionReminder = 0;
            }
        }

        public Alignment Alignment
        {
            get { return m_alignment; }
            set
            {
                if (m_alignment != value)
                {
                    m_alignment = value;
                    m_particleLocalFrames = m_alignment == Alignment.Local ? new LocalFrame[Capacity] : null;
                    m_liveParticleStart = 0;
                    m_liveParticleEnd = 0;
                    m_newParticleEnd = 0;
                }
            }
        }

#if WINDOWS
        [System.ComponentModel.TypeConverter(typeof(UVBoundsTypeConverter))]
#endif
        public string UVBoundsName
        {
            get { return m_uvBoundsName; }
            set
            {
                if (value == m_uvBoundsName)
                {
                    return;
                }

                m_uvBoundsName = value;
                ResolveUVBounds();
            }
        }

#if WINDOWS
        [System.ComponentModel.Browsable(false)]
#endif
        public XnaRect UVBounds
        {
            get; private set;
        }

#if WINDOWS
        [System.ComponentModel.Browsable(false)]
#endif
        [Microsoft.Xna.Framework.Content.ContentSerializer(ElementName = "ModifiersOnEmit", CollectionItemName = "Modifier")]
        public ModifierList ModifiersOnEmit
        {
            get { return m_modifiersOnEmit; }
        }

#if WINDOWS
        [System.ComponentModel.Browsable(false)]
#endif
        [Microsoft.Xna.Framework.Content.ContentSerializer(ElementName = "ModifiersOnUpdate", CollectionItemName = "Modifier")]
        public ModifierList ModifiersOnUpdate
        {
            get { return m_modifiersOnUpdate; }
        }

#if WINDOWS
        [System.ComponentModel.Browsable(false)]
#endif
        public Particle Template
        {
            get { return m_templateParticle; }
        }

        public Effect()
        {
            m_emissionRate = 0f;
            m_emissionReminder = 0f;
            m_modifiersOnEmit = new ModifierList(this);
            m_modifiersOnUpdate = new ModifierList(this);

            Name = "";

            DefaultParticleLifetime = 0f;
            DefaultParticlePosition = Vector3.Zero;
            DefaultParticleVelocity = Vector3.Zero;
            DefaultParticleSize = Vector2.Zero;
            DefaultParticleRotation = 0f;
            DefaultParticleColor = XnaColor.Black;

            Capacity = 0;
            IsEmitting = true;
            Alignment = Alignment.Screen;
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
            if (m_alignment == Alignment.Local)
            {
                if (System.LocalFrameProvider != null)
                {
                    m_particleLocalFrames[m_newParticleEnd] = System.LocalFrameProvider.LocalFrame;
                }
                else
                {
                    m_particleLocalFrames[m_newParticleEnd].XAxis = Vector3.UnitX;
                    m_particleLocalFrames[m_newParticleEnd].YAxis = Vector3.UnitY;
                }
            }
            m_newParticleEnd = IncrementIndex(m_newParticleEnd);
        }

        public void EndEmit()
        {
            if (m_newParticleEnd >= m_liveParticleEnd)
            {
                ModifiersOnEmit.ForEach(mod => mod.InternalProcess(0, m_particles, m_liveParticleEnd, m_newParticleEnd));
                ModifiersOnUpdate.ForEach(mod => mod.InternalProcess(0, m_particles, m_liveParticleEnd, m_newParticleEnd));
            }
            else
            {
                ModifiersOnEmit.ForEach(mod => mod.InternalProcess(0, m_particles, m_liveParticleEnd, m_particles.Length));
                ModifiersOnEmit.ForEach(mod => mod.InternalProcess(0, m_particles, 0, m_newParticleEnd));
                ModifiersOnUpdate.ForEach(mod => mod.InternalProcess(0, m_particles, m_liveParticleEnd, m_particles.Length));
                ModifiersOnUpdate.ForEach(mod => mod.InternalProcess(0, m_particles, 0, m_newParticleEnd));
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

        internal void FillDepthArray(float[] depthArray, int startIndex, Vector3 camDir)
        {
            for (int i = m_liveParticleStart; i != m_liveParticleEnd; i = IncrementIndex(i))
            {
                depthArray[startIndex++] = Vector3.Dot(m_particles[i].m_position, camDir);
            }
        }

        internal void ResolveUVBounds()
        {
            UVBounds = ResourceLoader.Instance.ResolveUVBounds(UVBoundsName, System);
        }

        internal void Update(float deltaTime)
        {
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
                foreach (var mod in ModifiersOnUpdate)
                {
                    mod.InternalProcess(deltaTime, particles, begin, end);
                }
            });

            m_templateParticle = new Particle
            {
                m_life = DefaultParticleLifetime,
                m_age = 0f,
                m_position = DefaultParticlePosition,
                m_velocity = DefaultParticleVelocity,
                m_size = DefaultParticleSize,
                m_rotation = DefaultParticleRotation,
                m_color = Color.FromXnaColor(DefaultParticleColor)
            };

            if (IsEmitting)
            {
                // emit new particles
                m_emissionReminder += deltaTime;
                float interval = 1.0f / m_emissionRate;
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
            Debug.Assert(index >= 0 && index < Capacity * 2);
            return index >= Capacity ? index - Capacity : index;
        }
    }
}
