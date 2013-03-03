using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI.CardControlAddins
{
    partial class CardIcons : CardControl.Addin
    {
        private const int MaxIcons = 4;
        private const int IconSize = 42;
        private const int CounterNumberPadding = 4;
        private const int Interval = 8;
        private const int LabelPanelPadding = 5;

        private struct IconItem
        {
            public Behaviors.IStatusEffect m_statusEffect;
            public Behaviors.ICounter m_counter;
            public Icon m_ui;
        }

        private IconItem[] m_icons = new IconItem[MaxIcons];
        private Graphics.TextRenderer.FormatOptions m_counterFmtOptions;
        private Graphics.TextRenderer.FormatOptions m_textFmtOptions;
        private Label m_label = new Label() { TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) };
        private Panel m_labelPanel = new Panel() { Color = new Color(0.0f, 0.0f, 0.0f, 0.75f) };

        public CardIcons(CardControl control)
            : base(control)
        {
            m_counterFmtOptions = new Graphics.TextRenderer.FormatOptions(
                new Graphics.TextRenderer.FontDescriptor("Helvetica", 26, System.Drawing.FontStyle.Bold));
            m_textFmtOptions = new Graphics.TextRenderer.FormatOptions(
                new Graphics.TextRenderer.FontDescriptor("Microsoft YaHei", 12),
                new Graphics.TextRenderer.FontDescriptor("Constantia", 12));
        }

        public override void Update(float deltaTime)
        {
            var counters = Card.Counters.Take(MaxIcons).ToList();
            var statusEffectBhvs = Card.Behaviors.OfType<Behaviors.IStatusEffect>().Take(MaxIcons - counters.Count).ToList();

            IconItem[] newArray = new IconItem[MaxIcons];
            int newIndex = 0;
            for (int i = 0; i < counters.Count; ++i)
            {
                var counter = counters[i];
                int oldIndex = m_icons.FindIndex(item => item.m_counter == counter);
                if (oldIndex >= 0)
                {
                    newArray[newIndex] = m_icons[oldIndex];
                    newArray[newIndex++].m_ui.Dispatcher = null;
                    m_icons[oldIndex].m_counter = null;
                }
                else
                {
                    newArray[newIndex].m_counter = counter;
                    newArray[newIndex].m_ui = new Icon
                    {
                        Counter = counter,
                        Card = Card,
                        TextLabel = m_label,
                        TextPanel = m_labelPanel,
                        CounterNumberColor = Color.White,
                        CounterNumberFormatOptions = m_counterFmtOptions,
                        LabelTextFormatOptions = m_textFmtOptions
                    };
                    newArray[newIndex++].m_ui.Initialize();
                }
            }
            for (int i = 0; i < statusEffectBhvs.Count; ++i)
            {
                var bhv = statusEffectBhvs[i];
                int oldIndex = m_icons.FindIndex(item => item.m_statusEffect == bhv);
                if (oldIndex >= 0)
                {
                    newArray[newIndex] = m_icons[oldIndex];
                    newArray[newIndex++].m_ui.Dispatcher = null;
                    m_icons[oldIndex].m_statusEffect = null;
                }
                else
                {
                    newArray[newIndex].m_statusEffect = bhv;
                    newArray[newIndex].m_ui = new Icon
                    {
                        StatusEffect = bhv,
                        Card = (bhv as Behaviors.IBehavior).Host,
                        TextLabel = m_label,
                        TextPanel = m_labelPanel,
                        CounterNumberColor = Color.White,
                        CounterNumberFormatOptions = m_counterFmtOptions,
                        LabelTextFormatOptions = m_textFmtOptions
                    };
                    newArray[newIndex++].m_ui.Initialize();
                }
            }
            
            foreach (var oldItem in m_icons)
            {
                if (oldItem.m_counter != null || oldItem.m_statusEffect != null)
                {
                    oldItem.m_ui.ReleaseTexture();
                    oldItem.m_ui.Dispatcher = null;
                }
            }

            var fromY = (Control.Region.Height - (IconSize + Interval) * MaxIcons + Interval) / 2;
            for (int i = 0; i < newArray.Length; ++i)
            {
                var newItem = newArray[i];
                if (newItem.m_ui != null)
                {
                    newItem.m_ui.Transform = MatrixHelper.Translate(0, fromY + i * (IconSize + Interval), 0);
                    newItem.m_ui.Dispatcher = Control.BodyContainer;
                    newItem.m_ui.UpdateCounter();
                }
            }
            m_icons = newArray;

            m_label.Dispatcher = null;
            m_labelPanel.Dispatcher = null;
            if (m_label.FormattedText != null)
            {
                m_labelPanel.Dispatcher = GameApp.Service<Services.GameUI>().InGameUIPage.Style.ChildIds["World2D"].Target;
                m_label.Dispatcher = GameApp.Service<Services.GameUI>().InGameUIPage.Style.ChildIds["World2D"].Target;
            }
        }

        public override void RenderPostMain(Matrix transform, RenderEventArgs e)
        {
            foreach (var item in m_icons)
            {
                if (item.m_ui != null)
                {
                    item.m_ui.Draw();
                }
            }
        }
    }
}
