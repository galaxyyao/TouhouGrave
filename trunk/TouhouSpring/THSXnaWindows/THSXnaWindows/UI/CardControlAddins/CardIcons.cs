using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI.CardControlAddins
{
    class CardIcons : CardControl.Addin
    {
        private const int MaxStatusEffects = 4;
        private const int MaxCounters = 4;

        private const int IconSize = 36;
        private const int Interval = 8;
        private const int LabelPanelPadding = 5;

        private struct StatusEffectItem
        {
            public Behaviors.IStatusEffect m_bhv;
            public UI.Button m_ui;
        }

        private StatusEffectItem[] m_statusEffects = new StatusEffectItem[MaxStatusEffects];
        private Graphics.TextRenderer.FormatOptions m_fmtOptions;
        private Label m_label = new Label() { TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) };
        private Panel m_labelPanel = new Panel() { Color = new Color(0.0f, 0.0f, 0.0f, 0.75f) };

        public CardIcons(CardControl control)
            : base(control)
        {
            m_fmtOptions = new Graphics.TextRenderer.FormatOptions(
                new Graphics.TextRenderer.FontDescriptor("DFKai-SB", 12),
                new Graphics.TextRenderer.FontDescriptor("Constantia", 12));
        }

        public override void Update(float deltaTime)
        {
            var statusEffectBhvs = Card.Behaviors.OfType<Behaviors.IStatusEffect>().Take(MaxStatusEffects).ToList();
            StatusEffectItem[] newArray = new StatusEffectItem[MaxStatusEffects];
            for (int i = 0; i < statusEffectBhvs.Count; ++i)
            {
                var bhv = statusEffectBhvs[i];
                int oldIndex = m_statusEffects.FindIndex(item => item.m_bhv == bhv);
                if (oldIndex >= 0)
                {
                    newArray[i] = m_statusEffects[oldIndex];
                    newArray[i].m_ui.Dispatcher = null;
                    m_statusEffects[oldIndex].m_bhv = null;
                }
                else
                {
                    var ui = new UI.Button
                    {
                        NormalFace = new Graphics.TexturedQuad(GameApp.Service<Services.ResourceManager>().Acquire<Graphics.VirtualTexture>(bhv.IconUri)),
                        Region = new Rectangle(0, 0, IconSize, IconSize)
                    };
                    ui.MouseEnter += Icon_MouseEnter;
                    ui.MouseLeave += Icon_MouseLeave;
                    newArray[i].m_bhv = bhv;
                    newArray[i].m_ui = ui;
                }
            }
            
            foreach (var oldItem in m_statusEffects)
            {
                if (oldItem.m_bhv != null)
                {
                    GameApp.Service<Services.ResourceManager>().Release(oldItem.m_ui.NormalFace.Texture);
                    oldItem.m_ui.Dispatcher = null;
                }
            }

            var fromY = (Control.Region.Height - (IconSize + Interval) * MaxStatusEffects + Interval) / 2;
            for (int i = 0; i < newArray.Length; ++i)
            {
                var newItem = newArray[i];
                if (newItem.m_ui != null)
                {
                    newItem.m_ui.Transform = MatrixHelper.Translate(-IconSize / 2, fromY + i * (IconSize + Interval), 0);
                    newItem.m_ui.Dispatcher = Control.BodyContainer;
                }
            }
            m_statusEffects = newArray;

            m_label.Dispatcher = null;
            m_labelPanel.Dispatcher = null;
            if (m_label.FormattedText != null)
            {
                m_labelPanel.Dispatcher = GameApp.Service<Services.GameUI>().InGameUIPage.Style.ChildIds["World2D"].Target;
                m_label.Dispatcher = GameApp.Service<Services.GameUI>().InGameUIPage.Style.ChildIds["World2D"].Target;
            }
        }

        private void Icon_MouseEnter(object sender, MouseEventArgs e)
        {
            var btn = sender as UI.Button;
            var bhv = m_statusEffects.First(item => item.m_ui == btn).m_bhv;
            if (!String.IsNullOrEmpty(bhv.Text))
            {
                m_label.FormattedText = GameApp.Service<Graphics.TextRenderer>().FormatText(bhv.Text, m_fmtOptions);
                var world2D = GameApp.Service<Services.GameUI>().InGameUIPage.Style.ChildIds["World2D"].Target;
                var transform = TransformNode.GetTransformBetween(btn, world2D);
                var pt1 = transform.TransformCoord(new Vector3(0, 0, 0));
                var pt2 = transform.TransformCoord(new Vector3(btn.Region.Width, 0, 0));
                var pt3 = transform.TransformCoord(new Vector3(0, btn.Region.Height, 0));
                var pt4 = transform.TransformCoord(new Vector3(btn.Region.Width, btn.Region.Height, 0));
                float right = Math.Max(Math.Max(pt1.X, pt2.X), Math.Max(pt3.X, pt4.X));
                float top = Math.Min(Math.Min(pt1.Y, pt2.Y), Math.Min(pt3.Y, pt4.Y));
                m_labelPanel.Region = new Rectangle(0, 0,
                    m_label.FormattedText.Size.Width + LabelPanelPadding * 2,
                    m_label.FormattedText.Size.Height + LabelPanelPadding * 2);
                m_labelPanel.Transform = MatrixHelper.Translate(right, top - m_labelPanel.Region.Height, 0);
                m_label.Transform = MatrixHelper.Translate(right + LabelPanelPadding, top - m_labelPanel.Region.Height + LabelPanelPadding, 0);
            }
            else
            {
                m_label.FormattedText = null;
            }
        }

        private void Icon_MouseLeave(object sender, MouseEventArgs e)
        {
            m_label.FormattedText = null;
        }
    }
}
