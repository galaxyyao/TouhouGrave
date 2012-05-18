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
			string DefaultTextColor { get; }
			void SetText(string text, Font font, Color textColor);
		}

		private string m_text;
		private string m_fontFamily;
		private string m_fontSize;
		private string m_fontStyle;
		private string m_textColor;

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

			var fontAttr = host.Definition.Attribute("Font");
			string fontFamily = null;
			string fontSize = null;
			string fontStyle = null;
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

			m_text = text;
			m_textColor = textColor;
			m_fontFamily = fontFamily;
			m_fontSize = fontSize;
			m_fontStyle = fontStyle ?? host.DefaultFontStyle ?? "regular";
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

			Font font = new Font
			{
				Family = m_fontFamily,
				Size = float.Parse(m_fontSize),
				Style = Font.ParseStyle(m_fontStyle)
			};

			host.SetText(text, font, color);
		}
	}
}
