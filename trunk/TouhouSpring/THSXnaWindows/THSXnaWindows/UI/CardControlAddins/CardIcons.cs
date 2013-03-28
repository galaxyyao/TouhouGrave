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
            public Services.CardDataManager.ICounterData m_counterData;
            public Icon m_ui;

            public bool EqualsTo(Services.CardDataManager.ICounterData counter)
            {
                return m_counterData != null
                       && m_counterData.Name == counter.Name
                       && m_counterData.IconUri == counter.IconUri;
            }
        }

        private object m_lastCounterArray; // use this to see whether the data is refreshed
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
            if (m_lastCounterArray != CardData.Counters)
            {
                IconItem[] newArray = new IconItem[MaxIcons];
                int newIndex = 0;
                for (int i = 0; i < Math.Min(CardData.Counters.Count, MaxIcons); ++i)
                {
                    var counter = CardData.Counters[i];
                    int oldIndex = m_icons.FindIndex(item => item.EqualsTo(counter));
                    if (oldIndex >= 0)
                    {
                        newArray[newIndex] = m_icons[oldIndex];
                        newArray[newIndex].m_counterData = counter;
                        newArray[newIndex].m_ui.CounterData = counter;
                        newArray[newIndex++].m_ui.Dispatcher = null;
                        m_icons[oldIndex].m_counterData = null;
                    }
                    else
                    {
                        newArray[newIndex].m_counterData = counter;
                        newArray[newIndex].m_ui = new Icon
                        {
                            CounterData = counter,
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
                    if (oldItem.m_counterData != null)
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
                m_lastCounterArray = CardData.Counters;
            }

            m_labelPanel.Dispatcher = m_label.FormattedText != null ? GameApp.Service<Services.GameUI>().InGameUIPage.Style.ChildIds["World2D"].Target : null;
            m_label.Dispatcher = m_label.FormattedText != null ? GameApp.Service<Services.GameUI>().InGameUIPage.Style.ChildIds["World2D"].Target : null;
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
