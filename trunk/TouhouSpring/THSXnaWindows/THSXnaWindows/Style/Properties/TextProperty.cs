using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TouhouSpring.Style.Values;

namespace TouhouSpring.Style.Properties
{
    class TextProperty : BaseProperty
    {
        public interface IHost : IStyleContainer
        {
            string DefaultText { get; }
            string DefaultFontFamily { get; }
            string DefaultFontSize { get; }
            string DefaultFontStyle { get; }
            string DefaultFontOutlineThickness { get; }
            string DefaultTextColor { get; }
            string DefaultTextOutlineColor { get; }
            void SetText(string text, Font font, Font ansiFont, Color textColor, Color textOutlineColor);
        }

        private string m_text;
        private string m_fontFamily;
        private string m_fontSize;
        private string m_fontStyle;
        private string m_fontOutlineThickness;
        private string m_ansiFontFamily;
        private string m_ansiFontSize;
        private string m_ansiFontStyle;
        private string m_ansiFontOutlineThickness;
        private string m_textColor;
        private string m_textOutlineColor;

        public TextProperty(IStyleContainer parent)
            : base(parent)
        { }

        public override void Initialize()
        {
            if (Parent.Definition == null)
            {
                return;
            }

            var host = Parent as IHost;
            if (host == null)
            {
                throw new ArgumentException(String.Format("'{0}' doesn't implement ImageProperty.IHost.", Parent.GetType().Name));
            }

            var attrText = host.Definition.Attribute("Text");
            var text = attrText != null ? attrText.Value : host.DefaultText;
            if (text == null)
            {
                throw new MissingAttributeException("Text");
            }

            var attrTextColor = host.Definition.Attribute("Color");
            var textColor = attrTextColor != null ? attrTextColor.Value : host.DefaultTextColor;
            if (textColor == null)
            {
                throw new MissingAttributeException("Color");
            }

            var attrTextOutlineColor = host.Definition.Attribute("OutlineColor");
            var textOutlineColor = attrTextOutlineColor != null ? attrTextOutlineColor.Value : host.DefaultTextOutlineColor;
            textOutlineColor = textOutlineColor ?? "!00000000";

            string fontFamily = null;
            string fontSize = null;
            string fontStyle = null;
            string fontOutlineThickness = null;

            var fontAttr = host.Definition.Attribute("Font");
            if (fontAttr != null)
            {
                string[] tokens = fontAttr.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 0)
                {
                    fontFamily = tokens[0].Trim();
                }
                if (tokens.Length > 1)
                {
                    fontSize = tokens[1].Trim();
                }
                if (tokens.Length > 2)
                {
                    fontStyle = tokens[2].Trim();
                }
                if (tokens.Length > 3)
                {
                    fontOutlineThickness = tokens[3].Trim();
                }
                if (tokens.Length > 4)
                {
                    throw new FormatException(string.Format("'{0}' is not a valid value for Font.", fontAttr.Value));
                }
            }
            var fontElement = host.Definition.Element("Font");
            if (fontElement != null)
            {
                var fontFamilyAttr = fontElement.Attribute("Family");
                if (fontFamily != null && fontFamilyAttr != null)
                {
                    throw new DuplicateAttributeException("Font family");
                }
                fontFamily = fontFamily ?? fontFamilyAttr.Value;

                var fontSizeAttr = fontElement.Attribute("Size");
                if (fontSize != null && fontSizeAttr != null)
                {
                    throw new DuplicateAttributeException("Font size");
                }
                fontSize = fontSize ?? fontSizeAttr.Value;

                var fontStyleAttr = fontElement.Attribute("Style");
                if (fontStyle != null && fontStyleAttr != null)
                {
                    throw new DuplicateAttributeException("Font style");
                }
                fontStyle = fontStyle ?? fontStyleAttr.Value;

                var fontOutlineThicknessAttr = fontElement.Attribute("OutlineThickness");
                if (fontOutlineThickness != null && fontOutlineThicknessAttr != null)
                {
                    throw new DuplicateAttributeException("Font outline thickness");
                }
                fontOutlineThickness = fontOutlineThickness ?? fontOutlineThicknessAttr.Value;
            }

            fontFamily = fontFamily ?? host.DefaultFontFamily;
            if (fontFamily == null)
            {
                throw new MissingAttributeException("Font family");
            }

            fontSize = fontSize ?? host.DefaultFontSize;
            if (fontSize == null)
            {
                throw new MissingAttributeException("Font size");
            }

            string ansiFontFamily = null;
            string ansiFontSize = null;
            string ansiFontStyle = null;
            string ansiFontOutlineThickness = null;

            var ansiFontAttr = host.Definition.Attribute("AnsiFont");
            if (ansiFontAttr != null)
            {
                string[] tokens = ansiFontAttr.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 0)
                {
                    ansiFontFamily = tokens[0].Trim();
                }
                if (tokens.Length > 1)
                {
                    ansiFontSize = tokens[1].Trim();
                }
                if (tokens.Length > 2)
                {
                    ansiFontStyle = tokens[2].Trim();
                }
                if (tokens.Length > 3)
                {
                    ansiFontOutlineThickness = tokens[3].Trim();
                }
                if (tokens.Length > 4)
                {
                    throw new FormatException(string.Format("'{0}' is not a valid value for AnsiFont.", ansiFontAttr.Value));
                }
            }
            var ansiFontElement = host.Definition.Element("AnsiFont");
            if (ansiFontElement != null)
            {
                var fontFamilyAttr = ansiFontElement.Attribute("Family");
                if (ansiFontFamily != null && fontFamilyAttr != null)
                {
                    throw new DuplicateAttributeException("Font family");
                }
                ansiFontFamily = ansiFontFamily ?? fontFamilyAttr.Value;

                var fontSizeAttr = ansiFontElement.Attribute("Size");
                if (ansiFontSize != null && fontSizeAttr != null)
                {
                    throw new DuplicateAttributeException("Font size");
                }
                ansiFontSize = ansiFontSize ?? fontSizeAttr.Value;

                var fontStyleAttr = ansiFontElement.Attribute("Style");
                if (ansiFontStyle != null && fontStyleAttr != null)
                {
                    throw new DuplicateAttributeException("Font style");
                }
                ansiFontStyle = ansiFontStyle ?? fontStyleAttr.Value;

                var fontOutlineThicknessAttr = ansiFontElement.Attribute("OutlineThickness");
                if (ansiFontOutlineThickness != null && fontOutlineThicknessAttr != null)
                {
                    throw new DuplicateAttributeException("Font outline thickness");
                }
                ansiFontOutlineThickness = ansiFontOutlineThickness ?? fontOutlineThicknessAttr.Value;
            }

            m_text = text;
            m_textColor = textColor;
            m_textOutlineColor = textOutlineColor;
            m_fontFamily = fontFamily;
            m_fontSize = fontSize;
            m_fontStyle = fontStyle ?? host.DefaultFontStyle ?? "regular";
            m_fontOutlineThickness = fontOutlineThickness ?? host.DefaultFontOutlineThickness ?? "0";
            m_ansiFontFamily = ansiFontFamily ?? m_fontFamily;
            m_ansiFontSize = ansiFontSize ?? m_fontSize;
            m_ansiFontStyle = ansiFontStyle ?? m_fontStyle;
            m_ansiFontOutlineThickness = ansiFontOutlineThickness ?? m_fontOutlineThickness;
        }

        public override void Apply()
        {
            if (Parent.Definition == null)
            {
                return;
            }

            var host = Parent as IHost;
            if (host == null)
            {
                throw new ArgumentException(String.Format("'{0}' doesn't implement ImageProperty.IHost.", Parent.GetType().Name));
            }

            var text = BindValuesFor(m_text);
            var color = Color.Parse(BindValuesFor(m_textColor));
            var outlineColor = Color.Parse(BindValuesFor(m_textOutlineColor));

            Font font = new Font
            {
                Family = m_fontFamily,
                Size = float.Parse(m_fontSize),
                Style = Font.ParseStyle(m_fontStyle),
                OutlineThickness = float.Parse(m_fontOutlineThickness)
            };

            Font ansiFont = new Font
            {
                Family = m_ansiFontFamily,
                Size = float.Parse(m_ansiFontSize),
                Style = Font.ParseStyle(m_ansiFontStyle),
                OutlineThickness = float.Parse(m_ansiFontOutlineThickness)
            };

            host.SetText(text, font, ansiFont, color, outlineColor);
        }
    }
}
