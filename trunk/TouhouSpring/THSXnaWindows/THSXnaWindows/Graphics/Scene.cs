using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.Graphics
{
    [Services.LifetimeDependency(typeof(Services.GameManager))]
    class Scene : Services.GameService
    {
        private bool m_loaded = false;

        private Graphics.TexturedQuad m_backgroundImage;

        // rotating background image
        private Matrix m_backgroundImageTransform;
        private float m_rotationDegree = 0;

        private string m_currentEnvImageUri;
        private int m_currentEnvImageIndex;

        private Graphics.TexturedQuad[] m_envImages = new TexturedQuad[2];

        private Particle.ParticleSystemInstance m_beamMeUp;
        private Matrix m_transform;
        private Matrix m_toScreenSpace;

        private Services.GameEvaluator<string> m_envImageUriEvaluator;

        public override void Startup()
        {
            m_envImageUriEvaluator = GameApp.Service<Services.GameManager>().CreateGameEvaluator(game =>
            {
                foreach (var player in game.Players)
                {
                    var envCard = player.CardsOnBattlefield.FirstOrDefault(card => card.Behaviors.Has<Behaviors.Environment>());
                    if (envCard != null)
                    {
                        return envCard.Behaviors.Get<Behaviors.Environment>().VisualId;
                    }
                }
                return null;
            }, null);
        }

        public override void Update(float deltaTime)
        {
            if (!m_loaded)
            {
                return;
            }

            // switch the environment image
            if (m_envImageUriEvaluator.Value != m_currentEnvImageUri)
            {
                var resourceMgr = GameApp.Service<Services.ResourceManager>();
                int otherIndex = 1 - m_currentEnvImageIndex;
                if (m_envImages[otherIndex] != null && m_envImages[otherIndex] != m_backgroundImage)
                {
                    resourceMgr.Release(m_envImages[otherIndex].Texture);
                }
                m_envImages[otherIndex] = m_envImageUriEvaluator.Value != null
                                          ? new TexturedQuad(resourceMgr.Acquire<VirtualTexture>(m_envImageUriEvaluator.Value))
                                          : m_backgroundImage;
                m_envImages[otherIndex].ColorScale.W = 0f;
                m_currentEnvImageIndex = otherIndex;
                m_currentEnvImageUri = m_envImageUriEvaluator.Value;
            }

            m_envImages[m_currentEnvImageIndex].ColorScale.W += deltaTime / 2f;
            if (m_envImages[m_currentEnvImageIndex].ColorScale.W > 1f)
            {
                m_envImages[m_currentEnvImageIndex].ColorScale.W = 1f;
                int otherIndex = 1 - m_currentEnvImageIndex;
                if (m_envImages[otherIndex] != null && m_envImages[otherIndex] != m_backgroundImage)
                {
                    GameApp.Service<Services.ResourceManager>().Release(m_envImages[otherIndex].Texture);
                }
                m_envImages[otherIndex] = null;
            }

            ////////////////////////////////////////////////////
            // rotating background image

            float vw = GameApp.Instance.GraphicsDevice.Viewport.Width;
            float vh = GameApp.Instance.GraphicsDevice.Viewport.Height;
            float scaleFactor = (float)Math.Sqrt(vw * vw + vh * vh) / Math.Min(vw, vh);

            m_backgroundImageTransform
                = MatrixHelper.Translate(-vw / 2, -vh / 2)
                  * MatrixHelper.RotateZ(MathHelper.ToRadians(m_rotationDegree))
                  * MatrixHelper.Scale(scaleFactor, scaleFactor)
                  * MatrixHelper.Translate(vw / 2, vh / 2)
                  * m_toScreenSpace;

            m_rotationDegree = (m_rotationDegree + 360.0f / (10 * 60.0f) * deltaTime) % 360.0f;

            m_beamMeUp.Update(deltaTime);
            m_transform = /*Matrix.CreateScale(0.01f, 0.01f, 0.01f) **/
                Matrix.CreateLookAt(Vector3.UnitX * -100, Vector3.Zero, Vector3.UnitZ)
                * Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1.3333f, 1f, 1000f);
        }

        public override void Render()
        {
            if (!m_loaded)
            {
                return;
            }

            var renderMgr = GameApp.Service<RenderManager>();
            var vpBounds = new Rectangle(0, 0, renderMgr.Device.Viewport.Width, renderMgr.Device.Viewport.Height);

            int otherIndex = 1 - m_currentEnvImageIndex;
            if (m_envImages[otherIndex] != null)
            {
                m_envImages[otherIndex].BlendState = BlendState.Opaque;
                renderMgr.Draw(m_envImages[otherIndex], vpBounds,
                    m_envImages[otherIndex] == m_backgroundImage ? m_backgroundImageTransform : m_toScreenSpace);
            }
            m_envImages[m_currentEnvImageIndex].BlendState = BlendState.AlphaBlend;
            m_envImages[m_currentEnvImageIndex].ColorScale = new Vector4(m_envImages[m_currentEnvImageIndex].ColorScale.W);
            renderMgr.Draw(m_envImages[m_currentEnvImageIndex], vpBounds,
                    m_envImages[m_currentEnvImageIndex] == m_backgroundImage ? m_backgroundImageTransform : m_toScreenSpace);

            GameApp.Service<ParticleRenderer>().Draw(m_beamMeUp, m_transform, 1.0f, 1.333f);
        }

        internal void GameCreated()
        {
            float vw = GameApp.Instance.GraphicsDevice.Viewport.Width;
            float vh = GameApp.Instance.GraphicsDevice.Viewport.Height;

            m_toScreenSpace = Matrix.Identity;
            m_toScreenSpace.M11 = 2.0f / vw;
            m_toScreenSpace.M22 = -2.0f / vh;
            m_toScreenSpace.M41 = -1.0f;
            m_toScreenSpace.M42 = 1.0f;

            var resourceMgr = GameApp.Service<Services.ResourceManager>();
            m_backgroundImage = new TexturedQuad(resourceMgr.Acquire<VirtualTexture>("Textures/Scene2"));
            m_beamMeUp = new Particle.ParticleSystemInstance(resourceMgr.Acquire<Particle.ParticleSystem>("Particles/BeamMeUp"));
            m_loaded = true;

            m_currentEnvImageIndex = 0;
            m_envImages[m_currentEnvImageIndex] = m_backgroundImage;
        }
    }
}
