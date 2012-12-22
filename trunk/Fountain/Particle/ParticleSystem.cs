using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaRect = Microsoft.Xna.Framework.Rectangle;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;

namespace TouhouSpring.Particle
{
    public class ParticleSystem
    {
        private string m_textureName;
        private EffectList m_effects;

#if WINDOWS
        [System.ComponentModel.TypeConverter(typeof(TextureNameTypeConverter))]
#endif
        public string TextureName
        {
            get { return m_textureName; }
            set
            {
                if (value == m_textureName)
                {
                    return;
                }

                ResourceLoader.Instance.Unload(TextureObject);
                TextureObject = ResourceLoader.Instance.LoadTexture(value);
                m_textureName = value;

                Effects.ForEach(fx => fx.ResolveUVBounds());
            }
        }

        public BlendMode BlendMode
        {
            get; set;
        }

#if WINDOWS
        [System.ComponentModel.Browsable(false)]
#endif
        [Microsoft.Xna.Framework.Content.ContentSerializer(ElementName = "Effects", CollectionItemName = "Effect")]
        public EffectList Effects
        {
            get { return m_effects; }
        }

        public int TotalLiveParticles
        {
            get { return Effects.Sum(fx => fx.LiveParticleCount); }
        }

#if WINDOWS
        [System.ComponentModel.Browsable(false)]
#endif
        public Texture2D TextureObject
        {
            get; private set;
        }

#if WINDOWS
        [System.ComponentModel.Browsable(false)]
#endif
        [Microsoft.Xna.Framework.Content.ContentSerializerIgnore]
        public ILocalFrameProvider LocalFrameProvider
        {
            get; set;
        }

        public ParticleSystem()
        {
            m_effects = new EffectList(this);
            BlendMode = BlendMode.Additive;
        }

        public void Update(float deltaTime)
        {
            foreach (var effect in Effects)
            {
                effect.Update(deltaTime);
            }
        }
    }
}
