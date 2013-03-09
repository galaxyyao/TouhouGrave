using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.Graphics
{
    [Services.LifetimeDependency(typeof(Services.ResourceManager))]
    class SwfRenderer : Services.GameService
    {
        private class ResourceManager : V2DRuntime.IResourceManager
        {
            public DDW.V2D.V2DContent LoadV2DContent(string assetName)
            {
                return GameApp.Service<Services.ResourceManager>().Acquire<DDW.V2D.V2DContent>(assetName);
            }
        }

        private SpriteBatch m_spriteBatch;

        public override void Startup()
        {
            V2DRuntime.ResourceManager.Instance = new ResourceManager();
            m_spriteBatch = new SpriteBatch(GameApp.Instance.GraphicsDevice);
        }

        public override void Shutdown()
        {
            m_spriteBatch.Dispose();
            V2DRuntime.ResourceManager.Instance = null;
        }

        public void Render(SwfInstance swfInstance)
        {
            swfInstance.Render(m_spriteBatch);
        }
    }
}
