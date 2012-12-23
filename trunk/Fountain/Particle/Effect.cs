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
        private int m_capacity;
        private float m_emissionRate;
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
            get { return m_capacity; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Capacity must be greater than zero.");
                }

                m_capacity = value;
            }
        }

        public bool EmitOnStart
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
            }
        }

        public Alignment Alignment
        {
            get; set;
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

        public Effect()
        {
            m_emissionRate = 0f;
            m_modifiersOnEmit = new ModifierList { Effect = this };
            m_modifiersOnUpdate = new ModifierList { Effect = this };

            Name = "";

            DefaultParticleLifetime = 0f;
            DefaultParticlePosition = Vector3.Zero;
            DefaultParticleVelocity = Vector3.Zero;
            DefaultParticleSize = Vector2.Zero;
            DefaultParticleRotation = 0f;
            DefaultParticleColor = XnaColor.Black;

            Capacity = 0;
            EmitOnStart = true;
            Alignment = Alignment.Screen;
        }

        internal void ResolveUVBounds()
        {
            UVBounds = ResourceLoader.Instance.ResolveUVBounds(UVBoundsName, System);
        }
    }
}
