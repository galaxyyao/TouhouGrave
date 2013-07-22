using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI.CardControlAddins
{
    class DamageIndicator : CardControl.Addin
    {
        [Services.LifetimeDependency(typeof(Services.ResourceManager))]
        private class Resources : Services.GameService
        {
            public Graphics.TextRenderer.FormatOptions FormatOptions
            {
                get; private set;
            }

            public Graphics.TextRenderer.DrawOptions DrawOptions
            {
                get; private set;
            }

            public Curve MoveCurve
            {
                get; private set;
            }

            public Curve FadeCurve
            {
                get; private set;
            }

            public override void Startup()
            {
                var font = new Graphics.TextRenderer.FontDescriptor("Constantia", 20, System.Drawing.FontStyle.Bold);
                FormatOptions = new Graphics.TextRenderer.FormatOptions(font) { Alignment = Graphics.TextRenderer.Alignment.CenterMiddle };
                var drawOptions = Graphics.TextRenderer.DrawOptions.Default;
                drawOptions.ColorScaling = Vector4.One;
                DrawOptions = drawOptions;
                MoveCurve = GameApp.Service<Services.ResourceManager>().Acquire<Curve>("Curves/DamageIndicatorFloat");
                FadeCurve = GameApp.Service<Services.ResourceManager>().Acquire<Curve>("Curves/DamageIndicatorFade");
            }
        }

        private int m_lastLife = Int32.MinValue;
        private Vector2 m_offset = new Vector2(20, -80);

        public DamageIndicator(CardControl control) : base(control)
        {
        }

        public override void Update(float deltaTime)
        {
            var life = CardData.IsWarrior && !Control.IsCardDead
                       ? CardData.LifeAndInitialLife.Item1 : 0;
            if (CardData.IsWarrior && m_lastLife != life && m_lastLife != Int32.MinValue)
            {
                // get the center position of the card in screen space
                var pt = new Vector3(Control.Region.Width / 2, Control.Region.Height / 2, 0);
                pt = Control.BodyContainer.TransformToGlobal.TransformCoord(pt);
                var halfVpWidth = GameApp.Instance.GraphicsDevice.Viewport.Width * 0.5f;
                var halfVpHeight = GameApp.Instance.GraphicsDevice.Viewport.Height * 0.5f;
                var screenPt = new Vector2(pt.X * halfVpWidth + halfVpWidth, halfVpHeight - pt.Y * halfVpHeight);

                var resources = GameApp.Service<Resources>();
                string textStr;
                if (m_lastLife > life)
                {
                    textStr = "[color:Red]-" + (m_lastLife - life).ToString() + "[/color]";
                }
                else
                {
                    textStr = "[color:Green]+" + (life - m_lastLife).ToString() + "[/color]";
                }

                var text = GameApp.Service<Graphics.TextRenderer>().FormatText(textStr, resources.FormatOptions);
                GameApp.Service<Graphics.FloatingText>().Register(text, resources.DrawOptions,
                    screenPt, screenPt + m_offset,
                    new Animation.CurveTrack(resources.MoveCurve), new Animation.CurveTrack(resources.FadeCurve));
            }

            m_lastLife = CardData.IsWarrior ? life : Int32.MinValue;
        }
    }
}
