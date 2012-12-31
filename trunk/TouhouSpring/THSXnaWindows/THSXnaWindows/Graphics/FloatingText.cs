using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Graphics
{
    [Services.RenderDependency(typeof(Services.UIManager))]
    class FloatingText : Services.GameService
    {
        private class Entry
        {
            public TextRenderer.IFormattedText m_text;
            public TextRenderer.DrawOptions m_drawOptions;
            public Vector2 m_2dPosStart;
            public Vector2 m_2dPosEnd;
            public Animation.Track m_positionTrack;
            public Animation.Track m_alphaTrack;
        }

        private List<Entry> m_entries = new List<Entry>();
        private Matrix m_transform;

        public void Register(TextRenderer.IFormattedText text, TextRenderer.DrawOptions drawOptions, Vector2 screenVelocity, Vector2 screenPos, float duration)
        {
            Register(text, drawOptions, screenVelocity, screenPos, duration, false);
        }

        public void Register(TextRenderer.IFormattedText text, TextRenderer.DrawOptions drawOptions, Vector2 screenVelocity, Vector2 screenPos, float duration, bool fadeOut)
        {
            var posEnd = screenPos + duration * screenVelocity;
            var positionTrack = new Animation.LinearTrack(duration);
            var alphaTrack = fadeOut ? new Animation.ReverseLinearTrack(duration) : null;
            Register(text, drawOptions, screenPos, posEnd, positionTrack, alphaTrack);
        }

        public void Register(TextRenderer.IFormattedText text, TextRenderer.DrawOptions drawOptions,
            Vector2 screenPosStart, Vector2 screenPosEnd,
            Animation.Track positionTrack, Animation.Track alphaTrack)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            else if (positionTrack == null)
            {
                throw new ArgumentNullException("positionTrack");
            }

            positionTrack.Play();
            if (alphaTrack != null)
            {
                alphaTrack.Play();
            }

            m_entries.Add(new Entry
            {
                m_text = text,
                m_drawOptions = drawOptions,
                m_2dPosStart = screenPosStart,
                m_2dPosEnd = screenPosEnd,
                m_positionTrack = positionTrack,
                m_alphaTrack = alphaTrack
            });
        }

        public override void Startup()
        {
            var device = GameApp.Instance.GraphicsDevice;
            m_transform = Matrix.CreateScale(2.0f / device.Viewport.Width, -2.0f / device.Viewport.Height, 1.0f)
                          * Matrix.CreateTranslation(-1.0f, 1.0f, 0.0f);
        }

        public override void Update(float deltaTime)
        {
            for (int i = 0; i < m_entries.Count; ++i)
            {
                var entry = m_entries[i];
                entry.m_positionTrack.Elapse(deltaTime);
                if (entry.m_alphaTrack != null)
                {
                    entry.m_alphaTrack.Elapse(deltaTime);
                }

                if (!entry.m_positionTrack.IsPlaying)
                {
                    m_entries.RemoveAt(i);
                    --i;
                }
            }
        }

        public override void Render()
        {
            var textRenderer = GameApp.Service<TextRenderer>();
            m_entries.ForEach(entry => RenderEntry(textRenderer, entry));
        }

        private void RenderEntry(TextRenderer renderer, Entry entry)
        {
            var pos = Vector2.Lerp(entry.m_2dPosStart, entry.m_2dPosEnd, entry.m_positionTrack.CurrentValue);
            var transform = Matrix.CreateTranslation(pos.X, pos.Y, 0.0f) * m_transform;
            entry.m_drawOptions.ColorScaling.W = entry.m_alphaTrack != null ? entry.m_alphaTrack.CurrentValue : 1.0f;
            renderer.DrawText(entry.m_text, transform, entry.m_drawOptions);
        }
    }
}
