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
	class LabelStyle : BaseStyleContainer, BoundsProperty.IHost, TextProperty.IHost
	{
		public Label TypedTarget
		{
			get { return (Label)Target; }
		}

		public LabelStyle(IStyleContainer parent, XElement definition)
			: base(parent, definition)
		{ }

		public override void Initialize()
		{
			PreInitialize(() => new Label());

			if (Definition == null)
			{
				return;
			}

			AddChildAndInitialize(new TextProperty(this));
			AddChildAndInitialize(new BoundsProperty(this));
			AddChildAndInitialize(new TransformProperty(this));
		}

		#region BoundsProperty.IHost implementation

		string BoundsProperty.IHost.DefaultWidth { get { return TypedTarget.TextBuffer.TextSize.Width.ToString(); } }
		string BoundsProperty.IHost.DefaultHeight { get { return TypedTarget.TextBuffer.TextSize.Height.ToString(); } }
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

		void TextProperty.IHost.SetText(string text, Font font, Color textColor)
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

			if (TypedTarget.TextBuffer == null
				|| TypedTarget.TextBuffer.Text != text
				|| TypedTarget.TextBuffer.FontFamilyName != font.Family
				|| TypedTarget.TextBuffer.FontSize != font.Size
				|| TypedTarget.TextBuffer.FontStyle != fontStyle)
			{
				using (var sysFont = new SystemFont(font.Family, font.Size.Value, fontStyle))
				{
					TypedTarget.TextBuffer = new Graphics.TextBuffer(text, sysFont, GameApp.Instance.GraphicsDevice);
				}
			}
			TypedTarget.TextColor = new XnaColor(textColor.Red, textColor.Green, textColor.Blue, textColor.Alpha);
		}

		#endregion
	}
}
