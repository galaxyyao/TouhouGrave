using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI.CardControlAddins
{
    class SpellButtons : CardControl.Addin
    {
        private List<Button> m_spellButtons = new List<Button>();
        private TransformNode m_spellButtonContainer = new UI.TransformNode();

        private Graphics.VirtualTexture m_buttonFaceTexture;

        private Animation.Track m_fadeInOutTrack = new Animation.LinearTrack(0.15f);
        private bool m_lastShown = false;

        public IEnumerable<Button> Buttons
        {
            get { return m_spellButtons; }
        }

        public SpellButtons(CardControl control) : base(control)
        {
            // TODO: spell button shall be stylized from cardcontrol's style
            const int IntervalV = 10;

            m_buttonFaceTexture = GameApp.Service<Services.ResourceManager>().Acquire<Graphics.VirtualTexture>("Textures/Button");
            var buttonFace = new Graphics.TexturedQuad(m_buttonFaceTexture);
            var font = new Graphics.TextRenderer.FontDescriptor("Segoe UI", 16);

            int y = 0;
            foreach (var spell in Card.GetSpells())
            {
                y -= m_buttonFaceTexture.Height + IntervalV;

                var btn = new Button
                {
                    NormalFace = buttonFace,
                    ButtonText = GameApp.Service<Graphics.TextRenderer>().FormatText(spell.Model.Name, new Graphics.TextRenderer.FormatOptions(font))
                };
                btn.Transform = MatrixHelper.Translate(-m_buttonFaceTexture.Width / 2, y);
                btn.MouseButton1Up += new EventHandler<MouseEventArgs>(SpellButton_MouseButton1Up);
                btn.Dispatcher = m_spellButtonContainer;
                m_spellButtons.Add(btn);
            }
 
            m_fadeInOutTrack.Elapsed += w =>
            {
                foreach (var button in Buttons)
                {
                    var clr = new Color(1.0f, 1.0f, 1.0f, w);
                    button.NormalFace.ColorToModulate = clr;
                    button.TextColor = clr;
                }
                if (w == 0.0f)
                {
                    m_spellButtonContainer.Dispatcher = null;
                }
            };
        }

        public override void Dispose()
        {
            m_spellButtonContainer.Dispatcher = null;
            GameApp.Service<Services.ResourceManager>().Release(m_buttonFaceTexture);
        }

        public override void Update(float deltaTime)
        {
            var gameui = GameApp.Service<Services.GameUI>();
            bool needShow = gameui.TacticalPhase_CardToCastSpell == Card && gameui.ZoomedInCard != Control;

            if (needShow)
            {
                var world2D = GameApp.Service<Services.GameUI>().InGameUIPage.Style.ChildIds["World2D"].Target;
                var transform = TransformNode.GetTransformBetween(Control, world2D);

                var pt1 = transform.TransformCoord(new Vector3(0, 0, 0));
                //var pt2 = transform.TransformCoord(new Vector3(Control.Region.Width, 0, 0));
                //var pt3 = transform.TransformCoord(new Vector3(0, Control.Region.Height, 0));
                //var pt4 = transform.TransformCoord(new Vector3(Control.Region.Width, Control.Region.Height, 0));
                var pt2 = transform.TransformCoord(new Vector3(1f, 0, 0));
                var pt3 = transform.TransformCoord(new Vector3(0, -1.44f, 0));
                var pt4 = transform.TransformCoord(new Vector3(1f, -1.44f, 0));


                var minY = Math.Min(Math.Min(pt1.Y, pt2.Y), Math.Min(pt3.Y, pt4.Y));
                var minX = Math.Min(Math.Min(pt1.X, pt2.X), Math.Min(pt3.X, pt4.X));
                var maxX = Math.Max(Math.Max(pt1.X, pt2.X), Math.Max(pt3.X, pt4.X));

                var x = (minX + maxX) / 2;
                var y = minY;

                m_spellButtonContainer.Transform = MatrixHelper.Translate(x, y);
                m_spellButtonContainer.Dispatcher = world2D;
            }

            if (needShow != m_lastShown)
            {
                if (needShow)
                {
                    m_fadeInOutTrack.TimeFactor = 1.0f;
                    if (!m_fadeInOutTrack.IsPlaying)
                    {
                        m_fadeInOutTrack.Play();
                    }
                }
                else
                {
                    m_fadeInOutTrack.TimeFactor = -1.0f;
                    if (!m_fadeInOutTrack.IsPlaying)
                    {
                        m_fadeInOutTrack.PlayFrom(m_fadeInOutTrack.Time);
                    }
                }

                m_lastShown = !m_lastShown;
            }

            if (m_fadeInOutTrack.IsPlaying)
            {
                m_fadeInOutTrack.Elapse(deltaTime);
            }
        }

        private void SpellButton_MouseButton1Up(object sender, MouseEventArgs e)
        {
            var index = m_spellButtons.IndexOf((Button)sender);
            var spell = Card.GetSpells().Skip(index).First();
            GameApp.Service<Services.GameUI>().OnSpellClicked(Control, spell);
        }
    }
}
