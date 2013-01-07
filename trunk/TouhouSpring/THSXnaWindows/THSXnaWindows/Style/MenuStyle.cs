using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TouhouSpring.Style.Properties;
using TouhouSpring.Style.Values;
using TouhouSpring.UI;
using SystemFont = System.Drawing.Font;
using SystemFontStyle = System.Drawing.FontStyle;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace TouhouSpring.Style
{
    class MenuStyle : BaseStyleContainer, BoundsProperty.IHost, TextProperty.IHost
    {
        public override Rectangle Bounds
        {
            get { return TypedTarget.Region; }
            protected set { TypedTarget.Region = value; }
        }

        public MenuItem TypedTarget
        {
            get { return (MenuItem)Target; }
        }

        public MenuStyle(IStyleContainer parent, XElement definition)
            : base(parent, definition)
        { }

        public override void Initialize()
        {
            PreInitialize(() => new MenuItem());

            if (Definition == null)
            {
                return;
            }

            AddChildAndInitialize(new TextProperty(this));
            AddChildAndInitialize(new BoundsProperty(this));
            AddChildAndInitialize(new TransformProperty(this));
        }

        #region BoundsProperty.IHost implementation

        string BoundsProperty.IHost.DefaultWidth { get { return TypedTarget.Label.FormattedText.Size.Width.ToString(); } }
        string BoundsProperty.IHost.DefaultHeight { get { return TypedTarget.Label.FormattedText.Size.Height.ToString(); } }
        string BoundsProperty.IHost.DefaultHorizontalAlignment { get { return null; } }
        string BoundsProperty.IHost.DefaultVerticalAlignment { get { return null; } }
        void BoundsProperty.IHost.SetBounds(Rectangle value)
        {
            UpdateBounds(value);
        }

        #endregion

        #region TextProperty.IHost implementation

        string TextProperty.IHost.DefaultText { get { return ""; } }
        string TextProperty.IHost.DefaultFontFamily { get { return "Segoe UI"; } }
        string TextProperty.IHost.DefaultFontSize { get { return "11"; } }
        string TextProperty.IHost.DefaultFontStyle { get { return "regular"; } }
        string TextProperty.IHost.DefaultTextColor { get { return "Black"; } }

        void TextProperty.IHost.SetText(string text, Font font, Font ansiFont, Color textColor)
        {
            SystemFontStyle fontStyle;
            switch (font.Style)
            {
                default:
                case FontStyle.Regular:
                    fontStyle = SystemFontStyle.Regular;
                    break;
                case FontStyle.Bold:
                    fontStyle = SystemFontStyle.Bold;
                    break;
                case FontStyle.Italic:
                    fontStyle = SystemFontStyle.Italic;
                    break;
            }

            SystemFontStyle ansiFontStyle;
            switch (ansiFont.Style)
            {
                default:
                case FontStyle.Regular:
                    ansiFontStyle = SystemFontStyle.Regular;
                    break;
                case FontStyle.Bold:
                    ansiFontStyle = SystemFontStyle.Bold;
                    break;
                case FontStyle.Italic:
                    ansiFontStyle = SystemFontStyle.Italic;
                    break;
            }

            if (TypedTarget.Label.FormattedText == null
                || TypedTarget.Label.FormattedText.Text != text
                || TypedTarget.Label.FormattedText.FormatOptions.Font.FamilyName != font.Family
                || TypedTarget.Label.FormattedText.FormatOptions.Font.Size != font.Size
                || TypedTarget.Label.FormattedText.FormatOptions.Font.Style != fontStyle
                || TypedTarget.Label.FormattedText.FormatOptions.AnsiFont.FamilyName != ansiFont.Family
                || TypedTarget.Label.FormattedText.FormatOptions.AnsiFont.Size != ansiFont.Size
                || TypedTarget.Label.FormattedText.FormatOptions.AnsiFont.Style != ansiFontStyle)
            {
                var fd = new Graphics.TextRenderer.FontDescriptor(font.Family, font.Size.Value, fontStyle);
                var ansiFd = new Graphics.TextRenderer.FontDescriptor(ansiFont.Family, ansiFont.Size.Value, ansiFontStyle);
                TypedTarget.Label.FormattedText = GameApp.Service<Graphics.TextRenderer>().FormatText(text, new Graphics.TextRenderer.FormatOptions(fd, ansiFd));
            }
            TypedTarget.Label.TextColor = new XnaColor(textColor.Red, textColor.Green, textColor.Blue, textColor.Alpha);
        }

        #endregion
    }
}
