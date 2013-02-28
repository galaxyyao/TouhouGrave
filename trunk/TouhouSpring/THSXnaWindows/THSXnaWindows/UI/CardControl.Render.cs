using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.UI
{
    partial class CardControl : IEventListener<RenderEventArgs>
    {
        private DepthStencilState m_silhouetteWithDepth;
        private DepthStencilState m_silhouetteWithoutDepth;
        private DepthStencilState m_contentDepthStencil;
        private SamplerState m_defaultSamplerState;
        private SamplerState m_mipmapBiasedSamplerState;

        public float Saturate
        {
            get; set;
        }

        public float Brightness
        {
            get; set;
        }

        public bool EnableDepth
        {
            get; set;
        }

        void IEventListener<RenderEventArgs>.RaiseEvent(RenderEventArgs e)
        {
            Graphics.RenderManager.BeginPixEvent(0, "Card.Render:" + Card.Model.Name);

            var transform = BodyContainer.TransformToGlobal;

            CreateDepthStencilStates();

            ////////////////////////////////////////////////////////////
            // render the silhouette to lay down the stencil mask
            e.RenderManager.OverridingDepthStencilState = EnableDepth ? m_silhouetteWithDepth : m_silhouetteWithoutDepth;
            e.RenderManager.PushSimpleTechnique();
            Dispatch(e);
            Addins.ForEach(addin => addin.RenderDepth(transform, e));
            e.RenderManager.PopTechnique();
            e.RenderManager.OverridingDepthStencilState = null;

            ////////////////////////////////////////////////////////////
            // render content with depth off, pass only when stencil value
            // equals to the flagging stencil value
            var oldSamplerState = e.RenderManager.Device.SamplerStates[0];
            e.RenderManager.Device.SamplerStates[0] = m_mipmapBiasedSamplerState;

            e.RenderManager.OverridingDepthStencilState = m_contentDepthStencil;
            bool tone = Saturate < 1f || Brightness != 1f;
            if (tone)
            {
                e.RenderManager.PushToneTechnique(Saturate, Brightness);
            }

            Dispatch(e);
            Addins.ForEach(addin => addin.RenderMain(transform, e));

            if (tone)
            {
                e.RenderManager.PopTechnique();
            }
            e.RenderManager.OverridingDepthStencilState = null;

            e.RenderManager.Device.SamplerStates[0] = oldSamplerState ?? m_defaultSamplerState;

            Addins.ForEach(addin => addin.RenderPostMain(transform, e));

            Graphics.RenderManager.EndPixEvent();
        }

        private void Initialize_Render()
        {
            m_defaultSamplerState = new SamplerState();
            m_mipmapBiasedSamplerState = new SamplerState { MipMapLevelOfDetailBias = -1 };

            Saturate = 1f;
            Brightness = 1f;
            EnableDepth = false;
        }

        private void CreateDepthStencilStates()
        {
            int stencilReference = GameApp.Service<Services.GameUI>().GetRenderIndex(this) + 1;
            if (m_silhouetteWithDepth == null || m_silhouetteWithDepth.ReferenceStencil != stencilReference)
            {
                m_silhouetteWithDepth = new DepthStencilState
                {
                    DepthBufferEnable = true,
                    DepthBufferFunction = CompareFunction.LessEqual,
                    DepthBufferWriteEnable = true,
                    StencilEnable = true,
                    StencilFunction = CompareFunction.Always,
                    StencilPass = StencilOperation.Replace,
                    ReferenceStencil = stencilReference
                };

                m_silhouetteWithoutDepth = new DepthStencilState
                {
                    DepthBufferEnable = false,
                    StencilEnable = true,
                    StencilFunction = CompareFunction.Always,
                    StencilPass = StencilOperation.Replace,
                    ReferenceStencil = stencilReference
                };

                m_contentDepthStencil = new DepthStencilState
                {
                    DepthBufferEnable = false,
                    StencilEnable = true,
                    StencilFunction = CompareFunction.Equal,
                    ReferenceStencil = stencilReference
                };
            }
        }
    }
}
