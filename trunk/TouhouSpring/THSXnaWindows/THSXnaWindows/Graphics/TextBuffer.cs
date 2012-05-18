using System;
using System.Collections.Generic;
using System.Drawing; // not on WP7
using System.Drawing.Imaging; // not on WP7
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.Graphics
{
	public class TextBuffer : IDisposable
	{
		private static SolidBrush s_whiteBrush;
		private static System.Drawing.Graphics s_graphics;

		public string Text
		{
			get; private set;
		}

		public Size TextSize
		{
			get; private set;
		}

		public VirtualTexture Texture
		{
			get; private set;
		}

		public string FontFamilyName
		{
			get; private set;
		}

		public float FontSize
		{
			get; private set;
		}

		public FontStyle FontStyle
		{
			get; private set;
		}

		public TextBuffer(string text, Font font, GraphicsDevice device)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			else if (font == null)
			{
				throw new ArgumentNullException("font");
			}

			Text = text;
			FontFamilyName = font.OriginalFontName;
			FontSize = font.Size;
			FontStyle = font.Style;
			var textSize = s_graphics.MeasureString(text, font);
			TextSize = new Size(textSize.Width, textSize.Height);

			int width = Math.Max((int)Math.Ceiling(TextSize.Width), 1);
			int height = Math.Max((int)Math.Ceiling(TextSize.Height), 1);
			using (Bitmap bmp = new Bitmap(width, height))
			{
				System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
				g.DrawString(text, font, s_whiteBrush, new PointF(0, 0));
				using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
				{
					bmp.Save(mem, ImageFormat.Png);
					var texture = Texture2D.FromStream(device, mem);
					Texture = new VirtualTexture(texture, texture.Bounds);
				}
			}
		}

		#region IDisposable implementation

		private bool m_disposed = false;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~TextBuffer()
		{
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (!m_disposed)
			{
				if (disposing)
				{
					Texture.XnaTexture.Dispose();
				}

				Texture = null;
				m_disposed = true;
			}
		}

		#endregion

		static TextBuffer()
		{
			s_whiteBrush = new SolidBrush(Color.White);
			using (var empty = new Bitmap(1, 1))
			{
				s_graphics = System.Drawing.Graphics.FromImage(empty);
			}
		}
	}
}
