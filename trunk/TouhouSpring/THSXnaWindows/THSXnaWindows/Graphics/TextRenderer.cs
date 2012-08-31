using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.Graphics
{
    [Services.LifetimeDependency(typeof(Services.ResourceManager))]
    partial class TextRenderer : Services.GameService
    {
        private const int PageSize = 16;
        private const int CacheTextureSize = 1024;
        private const int MinimalPageLife = 5;

        private int m_timeStamp = MinimalPageLife + 1; // make sure the default page time stamp (0) is old enough to be reused in RequestPage()

        private Effect m_effect;
        private EffectParameter m_paramTexture;

        public override void Startup()
        {
            m_effect = GameApp.Service<Services.ResourceManager>().Acquire<Effect>("Effects/TextRenderer");
            m_paramTexture = m_effect.Parameters["TheTexture"];

            Initialize_Atlas();
            Initialize_DrawText();
        }

        public override void Shutdown()
        {
            Destroy_DrawText();
            Destroy_Atlas();

            GameApp.Service<Services.ResourceManager>().Release(m_effect);
        }

        public override void Update(float deltaTime)
        {
            ++m_timeStamp;
        }

        public void Dump()
        {
            //using (var stream = new System.IO.FileStream("fontatlas.png", System.IO.FileMode.Create))
            //{
            //    m_cacheTextures[0].m_physicalRTTexture.SaveAsPng(stream, m_cacheTextures[0].m_physicalRTTexture.Width, m_cacheTextures[0].m_physicalRTTexture.Height);
            //}
        }
    }
}
